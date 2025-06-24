from langchain_text_splitters import RecursiveCharacterTextSplitter
from langchain_chroma import Chroma
from langchain_google_genai import GoogleGenerativeAIEmbeddings, ChatGoogleGenerativeAI
from langchain_core.prompts import ChatPromptTemplate
from langchain_core.output_parsers import StrOutputParser
from langchain_core.runnables import RunnableLambda
from langchain.schema import Document
from langchain.memory import ConversationBufferWindowMemory
from langchain.schema import HumanMessage
from dotenv import load_dotenv
from sqlalchemy import text
import os
import asyncio
from typing import List
import re
import cohere
from typing import Dict
from sqlalchemy.orm import Session
from ..database import get_db, SessionLocal
from fastapi import Depends
from datetime import datetime, timedelta
import logging

dotenv_path = os.path.join(os.path.dirname(__file__), "..", "..", ".env")
load_dotenv(dotenv_path=dotenv_path)
# Load Google API key
os.environ["GOOGLE_API_KEY"] = os.getenv("GOOGLE_API_KEY")
COHERE_API_KEY = os.getenv("COHERE_API_KEY")
co = cohere.Client(COHERE_API_KEY)

logging.basicConfig(level=logging.INFO)

def clean_value(value):
    if value is None or (isinstance(value, str) and value.strip() == ""):
        return "null"
    return str(value).lower() if isinstance(value, str) else str(value)

def convert_from_postgres(db: Session = Depends(get_db)) -> list[Document]:
    sql = text("""
        SELECT 
            m."Title", 
            m."Description", 
            m."Nation", 
            m."TypeMovie", 
            CASE 
                WHEN CAST(m."Status" AS INTEGER) = 0 THEN 'Chưa có lịch'
                WHEN CAST(m."Status" AS INTEGER) = 1 THEN 'Sắp chiếu'
                WHEN CAST(m."Status" AS INTEGER) = 2 THEN 'Đang cập nhật'
                WHEN CAST(m."Status" AS INTEGER) = 3 THEN 'Đang chiếu'
                WHEN CAST(m."Status" AS INTEGER) = 4 THEN 'Đã kết thúc'
                WHEN CAST(m."Status" AS INTEGER) = 5 THEN 'Đã hoàn thành'
                ELSE 'Không xác định'
            END AS "Status", 
            m."NumberOfMovie", 
            m."Duration", 
            m."Quality", 
            m."Language", 
            m."View", 
            m."IsVip",
            m."NameDirector",
            m."Point",
            COALESCE(STRING_AGG(DISTINCT c."NameCategories", ', '), '') AS "NameCategories",
            COALESCE(STRING_AGG(DISTINCT a."ActorName", ', '), '') AS "NameActors",
            lm."Episode",
            TO_CHAR(lm."CreatedAt", 'DD/MM/YYYY HH24:MI:SS') AS "CreatedAt"
        FROM "Movie" m
        LEFT JOIN "SubCategories" sc ON m."Id" = sc."IdMovie"
        LEFT JOIN "Category" c ON sc."IdCategory" = c."IdCategories"
        LEFT JOIN (
            SELECT DISTINCT ON ("IdMovie") "IdMovie", "Episode", "CreatedAt"
            FROM "LinkMovie"
            ORDER BY "IdMovie", "CreatedAt" DESC
        ) lm ON m."Id" = lm."IdMovie"
        LEFT JOIN "SubActor" sa ON m."Id" = sa."IdMovie"
        LEFT JOIN "Actor" a ON sa."IdActor" = a."IdActor"
        WHERE m."Block" = false
        GROUP BY m."Id", lm."Episode", lm."CreatedAt";
    """)
    result = db.execute(sql)

    documents = []
    for row in result.fetchall():
        title = clean_value(row.Title)
        description = clean_value(row.Description)
        nation = clean_value(row.Nation)
        type_movie = clean_value(row.TypeMovie)
        status = clean_value(row.Status)
        number_of_episodes = clean_value(row.NumberOfMovie)
        duration = clean_value(row.Duration)
        quality = clean_value(row.Quality)
        language = clean_value(row.Language)
        number_of_views = clean_value(row.View)
        is_vip = clean_value(row.IsVip)
        director_name = clean_value(row.NameDirector)
        rating = clean_value(row.Point)
        categories_name = clean_value(row.NameCategories)
        actors_name = clean_value(row.NameActors)
        latest_episode = clean_value(row.Episode)
        created_at = clean_value(row.CreatedAt)

        metadata = {
            "title": title,
            "nation": nation,
            "type_movie": type_movie,
            "status": status,
            "number_of_episodes": number_of_episodes,
            "duration": duration,
            "quality": quality,
            "language": language,
            "number_of_views": number_of_views,
            "is_vip": is_vip,
            "director_name": director_name,
            "rating": rating,
            "categories_name": categories_name, 
            "actors_name": actors_name,
            "latest_episode": latest_episode,
            "created_at": created_at 
        }
        print("metadata:", metadata)

        # doc = Document(page_content=description, metadata=metadata)
        doc = Document(
            page_content=f"Mô tả: {description}. Tên phim: {title}. Quốc gia: {nation}. Loại phim: {type_movie}. Trạng thái: {status}. Số tập: {number_of_episodes}. Thời lượng: {duration}. Chất lượng: {quality}. Ngôn ngữ: {language}. Số views/lượt xem: {number_of_views}. Rating: {rating}. Thuộc VIP: {is_vip}. Thể loại: {categories_name}. Đạo diễn: {director_name}. Diễn viên: {actors_name}. Ngày tạo: {created_at}",
            metadata=metadata
        )
        documents.append(doc)
        # print(f"Title: {title}")
    return documents

def create_vector_store(chunks: List[Document], db_path: str, batch_size: int = 5000) -> Chroma:
    # Google's embeddings
    embedding_model = GoogleGenerativeAIEmbeddings(
        model="models/embedding-001"  # Google's embedding model
    )

    if os.path.exists(db_path) and os.listdir(db_path):
        db = Chroma(persist_directory=db_path, embedding_function=embedding_model)
        print("Loaded existing vector store.")

        db.add_documents(chunks)
        print("Added documents to existing vector store.")
    else:
        db = Chroma.from_documents(documents=chunks, embedding=embedding_model, persist_directory=db_path)
        print("Created new vector store.")

    return db

def is_within_time_window(paper_time: str, current_time: datetime,time_window: timedelta) -> bool:
    if not paper_time or paper_time == "null":
        return True
    try:
        paper_time = datetime.strptime(paper_time, "%d/%m/%Y %H:%M:%S")
        time_dfference = current_time - paper_time
        return time_dfference <= time_window
    except ValueError:
        print(f"Invalid time format: {paper_time}")
        return True  # Bỏ qua lọc nếu định dạng không hợp lệ

def retrieve_context(db: Chroma, query: str, time_window: timedelta=timedelta(weeks=1)) -> List[Document]:
    retriever = db.as_retriever(search_type="similarity", search_kwargs={"k": 10})
    print("Relevant chunks are retrieved...\n")
    relevant_chunks = retriever.invoke(query)
    print(f"Before filtering: {len(relevant_chunks)} chunks")
    for chunk in relevant_chunks:
        print(f"Chunk: {chunk.page_content}, Created_at: {chunk.metadata.get('created_at')}")
    
    # specific_date = extract_date_from_query(query)
    # now = datetime.now()
    # if specific_date:
    #     filtered_chunks = [
    #         chunk for chunk in relevant_chunks if is_same_day(chunk.metadata["created_at"], specific_date)
    #     ]
    # else:
    #     filtered_chunks = [
    #         chunk for chunk in relevant_chunks if is_within_time_window(chunk.metadata["created_at"], now, time_window)
    #     ]
    # print(f"After filtering: {len(filtered_chunks)} chunks")
    return relevant_chunks

def is_same_day(paper_time: str, specific_date: datetime) -> bool:
    if not paper_time or paper_time == "null":
        return True
    try:
        paper_time = datetime.strptime(paper_time, "%d/%m/%Y %H:%M:%S")
        return paper_time.date() == specific_date.date()
    except ValueError:
        print(f"Invalid time format: {paper_time}")
        return True

def extract_date_from_query(query: str) -> str:
    date_pattern = r"(\d{1,2})[-/](\d{1,2})[-/](\d{4})"
    match = re.search(date_pattern, query)
    if match:
        day = int(match.group(1))
        month = int(match.group(2))
        year = int(match.group(3))
        return datetime(year, month, day)
    date_pattern_alt = r"Ngày (\d{1,2}) tháng (\d{1,2}) năm (\d{4})"
    match_alt = re.search(date_pattern_alt, query)
    if match_alt:
        day = int(match_alt.group(1))
        month = int(match_alt.group(2))
        year = int(match_alt.group(3))
        return datetime(year, month, day)
    return None

def data_chunks(text: List[Document]) -> List[Document]:
    print("Data file text is chunked...")
    text_splitter = RecursiveCharacterTextSplitter(chunk_size=1000, chunk_overlap=50)
    chunks = text_splitter.split_documents(text)
 
    return chunks

def build_context(relevant_chunks: List[Document]) -> List[Document]:
    print("Context is built from relevant chunks")
    context = "\n\n".join([chunk.page_content for chunk in relevant_chunks])
    print(context)
    return context

def embedding_pipeline():
    logging.info("Starting embedding process from PostgreSQL...")
    db_path = "vector-store"
    with SessionLocal() as db:
        docs = convert_from_postgres(db)
        logging.info(f"Loaded {len(docs)} documents from PostgreSQL.")
        chunks = data_chunks(docs)
        logging.info(f"Split into {len(chunks)} text chunks.")
        create_vector_store(chunks, db_path)
        logging.info(f"Vector store created and saved successfully at: {db_path}")
    logging.info("Embedding process completed.")

def get_context(inputs: Dict[str, str]) -> Dict[str, str]:
    query, db_path = inputs["query"], inputs["db_path"]
    print("Loading the existing vector store\n")
    # Google's embeddings
    embedding_model = GoogleGenerativeAIEmbeddings(
        model="models/embedding-001"  # Google's embedding model
    )
    db = Chroma(persist_directory=db_path, embedding_function=embedding_model)
    relevant_chunks = retrieve_context(db, query)
    print("=====", len(relevant_chunks))
    documents_text = [doc.page_content for doc in relevant_chunks]
    rerank_response = co.rerank(model='rerank-v3.5', query=query, documents=documents_text)
    reranked_indices = [item.index for item in rerank_response.results]
    top_10_indices = reranked_indices[:10]
    top_10_docs = [relevant_chunks[i] for i in top_10_indices]
    for chunk in top_10_docs:
        # Lấy thời gian từ metadata của chunk
        time_info = chunk.metadata.get("created_at", "Không có thông tin thời gian")
        
        # In ra thông tin thời gian của chunk
        print(f"Chunk time: {time_info}")
    context = build_context(top_10_docs)
    print("context:", context)
    if "url" in query or "đường dẫn" in query or "link" in query:
        urls = []
        for chunk in top_10_docs:
            url = chunk.metadata.get("url", None)
            if url:
                urls.append(url)
        
        return {"context": "\n".join(urls), "query": query}
    print("query:", query)
    return {"context": context, "query": query}

# Hàm định dạng lịch sử hội thoại
def format_chat_history(chat_messages):
    result = "\n".join(
        f"Người dùng: {msg.content}" if isinstance(msg, HumanMessage)
        else f"Trợ lý: {msg.content}"
        for msg in chat_messages
    )
    print(result)
    return result


# Khởi tạo bộ nhớ ngoài hàm để giữ trạng thái liên tục
memory = ConversationBufferWindowMemory(
    k=5,
    memory_key="chat_history",
    return_messages=True
)


async def clarify_question(question: str, chat_history_text: str) -> str:
    vague_words = ["vậy","phim nào","cái gì","khi nào","ở đâu","có những","ở trên","trước đó","trên","họ","này","lúc này","làm gì"]

    if any(w in question.lower() for w in vague_words):
        prompt = f"""Dựa trên lịch sử hội thoại dưới đây, hãy biến câu hỏi ngắn sau thành câu hỏi đầy đủ rõ nghĩa:
        Lịch sử hội thoại:
        {chat_history_text}

        Câu hỏi ngắn: {question}

        Câu hỏi đầy đủ:"""

        llm = ChatGoogleGenerativeAI(
            model="gemini-2.0-flash",  # Use Gemini model
            temperature=0.0,
            convert_system_message_to_human=True  # Google Gemini handles system messages differently
        )
        full_question_response = await llm.agenerate([[HumanMessage(content=prompt)]])
        return full_question_response.generations[0][0].text.strip()

    return question


async def rag_chat(question: str):
    db_path = "vector-store"
    chat_messages = memory.load_memory_variables({})["chat_history"]

    # 1. Format lịch sử hội thoại
    chat_history_text = format_chat_history(chat_messages)

    # 2. Chuẩn hóa câu hỏi dựa vào lịch sử (nếu câu hỏi quá ngắn, mơ hồ)
    normalized_question = await clarify_question(question, chat_history_text)
    print("normalized_question:", normalized_question)

    # 3. Truy vấn vector store với câu hỏi đã chuẩn hóa
    context_data = get_context({"query": normalized_question, "db_path": db_path})
    context = context_data.get("context", "").strip()
    if not context:
        yield "Xin lỗi, mình không có đủ thông tin để trả lời câu hỏi này."
        return
    
    # - Bắt đầu câu trả lời bằng một câu chào ngắn (như "Chào bạn," hoặc "Cảm ơn vì câu hỏi của bạn.") và kết thúc bằng một câu nhẹ nhàng (như "Hy vọng thông tin này hữu ích!" hoặc "Nếu cần thêm thông tin, hãy cho tôi biết nhé.").
    # 4. Tạo prompt với lịch sử + ngữ cảnh + câu hỏi chuẩn hóa
    prompt_template = """
    Bạn là một trợ lý AI thông minh. Dựa vào 'Ngữ cảnh' và 'Lịch sử hội thoại' dưới đây, hãy trả lời câu hỏi một cách chính xác, chi tiết nhưng không quá dài dòng và chỉ sử dụng thông tin trong ngữ cảnh.

    Hướng dẫn trả lời:
    - Nếu không có đủ thông tin trong ngữ cảnh, hãy trả lời: "Tôi không có đủ thông tin để trả lời câu hỏi này. Bạn có thể cung cấp thêm chi tiết không?"
    - Không phỏng đoán hoặc đưa ra thông tin không có trong ngữ cảnh.
    - Sử dụng ngôn ngữ lịch sự, chuyên nghiệp và dễ hiểu.
    - Tránh lặp lại toàn bộ câu hỏi trong câu trả lời. 

    Lịch sử hội thoại:
    {chat_history}

    Ngữ cảnh:
    {context}

    Câu hỏi:
    {query}
    """

    rag_prompt = ChatPromptTemplate.from_template(prompt_template)
    formatted_prompt = rag_prompt.format_prompt(
        query=normalized_question,
        context=context,
        chat_history=chat_history_text
    )
    messages = formatted_prompt.to_messages()

    llm = ChatGoogleGenerativeAI(
        model="gemini-2.0-flash",  # Use Gemini model
        temperature=0.0,
        convert_system_message_to_human=True  # Google Gemini handles system messages differently
    )

    # Stream response
    stream = llm.astream(messages)

    response_text = ""
    async for chunk in stream:
        if isinstance(chunk.content, str):
            response_text += chunk.content
            yield chunk.content
    
    memory.save_context({"input": question}, {"output": response_text})


def get_documents(query: str):
    db_path = "vector-store"
    embedding_model = GoogleGenerativeAIEmbeddings(
        model="models/embedding-001"  # Google's embedding model
    )
    db = Chroma(persist_directory=db_path, embedding_function=embedding_model)
    docs = db.search(query, search_type="similarity")
    result = [
        {
            "document": doc.page_content,
            "metadata": doc.metadata
        } for doc in docs
    ]
    return result