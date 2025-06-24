from fastapi import FastAPI, HTTPException, Depends
from fastapi.middleware.cors import CORSMiddleware
from sqlalchemy.orm import Session
from .database import get_db
from sqlalchemy import text
from .schemas import ChatRequest, ChatResponse
from .rag.rag_chain import rag_chat, embedding_pipeline, get_documents
from dotenv import load_dotenv
import os
# from .embedding_task import start_scheduler
# from contextlib import asynccontextmanager
from fastapi.responses import StreamingResponse

load_dotenv()

# @asynccontextmanager
# async def lifespan(app: FastAPI):
#     start_scheduler()
#     yield
app = FastAPI()

allowed_origins = os.getenv("ALLOWED_CORS_ORIGINS").split(",")
print("cors", allowed_origins)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


@app.get("/movies")
def get_movies(db: Session = Depends(get_db)):
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
    movies = result.mappings().all() 
    if not movies:
        raise HTTPException(status_code=404, detail="Movie table is empty")
    return movies


@app.post("/chat", response_model=ChatResponse)
async def chat(req: ChatRequest):
    # Giả sử rag_chat trả về một async_generator
    answer = rag_chat(req.question) 

    return StreamingResponse(answer, media_type="text/plain")

@app.get("/run-embedding")
async def run_embedding():
    try:
        print(f"Received request")
        embedding_pipeline()
        return {
            "status": "success",
            "message": "Embedding pipeline completed successfully!",
        }
    except Exception as e:
        print(f"Error: {e}")
        raise HTTPException(status_code=500, detail=f"An error occurred: {e}")


@app.post("/documents")
async def get_all_documents(req: ChatRequest):
    return get_documents(req.question)
