import React, { useEffect, useRef, useState } from "react"
import Lottie from "lottie-react"
import "./ChatbotAssistant.css"

import logo from "../src/assets/images/client.png"
import client from "../src/assets/images/client.png"
import logo_chatbot from "../src/assets/images/client.png"
import wave from "../src/assets/animations/wave.json"
import wave_voice from "../src/assets/animations/wave_voice.json"
import { URL_Chat } from "./apis/ConstantAPI"

import { FontAwesomeIcon } from "@fortawesome/react-fontawesome"
import { faPaperPlane, faUpRightAndDownLeftFromCenter, faXmark, faMicrophone } from "@fortawesome/free-solid-svg-icons"

const ChatbotAssistant = () => {
    const [isShowChatbot, setIsShowChatbot] = useState(false)
    const [isExpanded, setIsExpanded] = useState(false)
    const textareaRef = useRef(null)
    const chatbotRef = useRef(null)
    const messagesRef = useRef(null)
    const [transcript, setTranscript] = useState("")
    const [isRecording, setIsRecording] = useState(false)
    const recognitionRef = useRef(null)
    const [isProcessing, setIsProcessing] = useState(false)
    const [messageClient, setMessageClient] = useState("")
    const [messages, setMessages] = useState([
        {
            role: "assistant",
            content: "🤖 Xin chào! Tôi là Movina_Bot, sẵn sàng trò chuyện với bạn!",
        },
    ])

    //Chỗ này format lại response của Gemini -> Markdown
    const parseMarkdown = (text) => {
        let parsed = text
        parsed = parsed.replace(/\*\*(.*?)\*\*/g, "<strong>$1</strong>")
        parsed = parsed.replace(/\*([^\n*]+)(?:\n|$)/g, "<li>- $1</li>")
        if (parsed.includes("<li>")) {
            parsed = `<ul>${parsed}</ul>`
        }
        return parsed
    }

    useEffect(() => {
        if (messagesRef.current) {
            messagesRef.current.scrollTop = messagesRef.current.scrollHeight
        }
    }, [messages])

    // Chỗ này gọi API streaming từ backend
    const customApiCall = async (message) => {
        try {
            const response = await fetch(URL_Chat, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "ngrok-skip-browser-warning" : "true"
                },
                body: JSON.stringify({ question: message }),
            })

            if (!response.ok) {
                const errorData = await response.json()
                throw new Error(errorData.error || "API request failed")
            }

            // Handle streaming response
            const reader = response.body.getReader()
            const decoder = new TextDecoder("utf-8")

            return {
                async *[Symbol.asyncIterator]() {
                    while (true) {
                        const { done, value } = await reader.read()
                        if (done) break
                        const chunk = decoder.decode(value, { stream: true })
                        yield chunk // Yield each chunk for real-time updates
                    }
                },
            }
        } catch (error) {
            console.error("Error calling API:", error)
            return `🤖 Có lỗi xảy ra: ${error.message}`
        }
    }

    // Chỗ này xử lý response streaming
    const handleBotResponse = async (stream) => {
        setIsProcessing(true)

        // Initialize streaming message with a unique ID
        const streamingMessageId = Date.now().toString()
        setMessages((prevMessages) => [
            ...prevMessages,
            { role: "assistant", content: "🤖 ● ● ● ", id: streamingMessageId }, // Initial "typing" indicator
        ])

        let accumulatedResponse = ""

        // Process stream chunks
        for await (const chunk of stream) {
            accumulatedResponse += chunk
            setMessages((prevMessages) => {
                return prevMessages.map((msg) => (msg.id === streamingMessageId ? { ...msg, content: accumulatedResponse } : msg))
            })
        }

        // Finalize response with Markdown
        const parsedResponse = parseMarkdown(accumulatedResponse)
        setMessages((prevMessages) => {
            return prevMessages.map((msg) => (msg.id === streamingMessageId ? { ...msg, content: parsedResponse } : msg))
        })

        setIsProcessing(false)
    }

    // Send message and handle streaming
    const handleSendMessage = async () => {
        textareaRef.current.value = ""
        if (messageClient.trim()) {
            setMessages((prevMessages) => [...prevMessages, { role: "user", content: messageClient }])

            const stream = await customApiCall(messageClient)
            if (typeof stream === "string") {
                setMessages((prevMessages) => [...prevMessages, { role: "assistant", content: stream }])
                setIsProcessing(false)
            } else {
                await handleBotResponse(stream)
            }
            setMessageClient("")
            if (textareaRef.current) {
                textareaRef.current.value = ""
            }
        }
    }

    // Chỗ này thì bắt sự kiện bàn phím
    const handleKeyDownMessage = (event) => {
        if (event.key === "Enter") {
            if (event.shiftKey) {
                textareaRef.current.value += ""
            } else {
                event.preventDefault()
                if (messageClient) {
                    textareaRef.current.value = ""
                    handleSendMessage()
                }
            }
        }
    }

    // Chỗ này thì hàm sự kiện thông thường
    const handleChangeMessage = (event) => {
        const message_client = event.target.value
        setMessageClient(message_client)
    }

    // Chỗ này thay đổi kích cỡ - theo chiều cao của textarea
    useEffect(() => {
        const textarea = textareaRef.current
        if (textarea) {
            const adjustHeight = () => {
                textarea.style.height = "45px"
                const minHeight = 45
                const maxHeight = 100
                const scrollHeight = textarea.scrollHeight
                const newHeight = Math.max(minHeight, Math.min(scrollHeight, maxHeight))
                textarea.style.height = `${newHeight}px`
                // console.log("scrollHeight:", scrollHeight, "newHeight:", newHeight, "computedHeight:", textarea.offsetHeight)
            }

            textarea.style.height = "45px"
            adjustHeight()

            textarea.addEventListener("input", adjustHeight)

            return () => {
                textarea.removeEventListener("input", adjustHeight)
            }
        }
    }, [messageClient])

    // Chỗ này tạo sự kiện tắt popup chatbot khi click ra ngoài màn hình
    useEffect(() => {
        const handleClickOutside = (event) => {
            if (chatbotRef.current && !chatbotRef.current.contains(event.target)) {
                setIsShowChatbot(false)
                setIsExpanded(false)
            }
        }

        if (isShowChatbot) {
            document.addEventListener("mousedown", handleClickOutside)
        }

        return () => {
            document.removeEventListener("mousedown", handleClickOutside)
        }
    }, [isShowChatbot])

    // Chỗ này sự kiện mở chatbot
    const handleClickShowChatbot = () => {
        setIsShowChatbot(!isShowChatbot)
        // console.log(isShowChatbot)
    }

    // Chỗ này sự kiện mở rộng khung chatbot
    const handleClickExpand = () => {
        setIsExpanded(!isExpanded)
        const chatbot = chatbotRef.current
        const offset = 85

        if (!isExpanded) {
            const expandedWidth = window.innerWidth - offset * 2
            const expandedHeight = window.innerHeight - offset
            chatbot.style.width = `${expandedWidth}px`
            chatbot.style.height = `${expandedHeight}px`
        } else {
            chatbot.style.width = "350px"
            chatbot.style.height = "550px"
        }
    }

    // Chỗ này thì hàm sự kiện thông thường
    const handleClickClose = () => {
        setIsShowChatbot(false)
    }

    // Chỗ này thì mở micro để bắt đầu thu âm
    const handleClickMicro = () => {
        setIsRecording(true)
        if (!recognitionRef.current) {
            alert("Trình duyệt của bạn không hỗ trợ Speech Recognition. Hãy sử dụng Chrome!")
            return
        }
        try {
            recognitionRef.current.start()
        } catch (error) {
            console.error("Lỗi khi bật micro:", error)
            alert("Không thể bật micro. Vui lòng kiểm tra quyền hoặc thử lại!")
            setIsRecording(false)
        }
    }

    // Chỗ này thì đóng micro
    const handleCloseMicro = () => {
        recognitionRef.current.stop()
        setIsRecording(false)
    }

    // Chỗ này thì sự kiện mở thu âm ở trình duyệt
    useEffect(() => {
        if (typeof window !== "undefined") {
            const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition
            if (SpeechRecognition) {
                recognitionRef.current = new SpeechRecognition()
                recognitionRef.current.lang = "vi-VN"
                recognitionRef.current.continuous = false
                recognitionRef.current.interimResults = false

                recognitionRef.current.onresult = (event) => {
                    const recordedTranscript = event.results[0][0].transcript
                    setTranscript(recordedTranscript)
                    setIsRecording(false)
                    if (recordedTranscript && textareaRef.current) {
                        textareaRef.current.value = recordedTranscript
                        setMessageClient(recordedTranscript)
                    }
                }

                recognitionRef.current.onerror = (event) => {
                    console.error("Error:", event.error)
                    setIsRecording(false)
                }

                recognitionRef.current.onend = () => {
                    setIsRecording(false)
                }
            }
        }
    }, [])

    return (
        <div className={"chatbot_ui"}>
            <div className={"chatbot_ui_container"}>
                <div className={"icon_open_chatbot"} onClick={handleClickShowChatbot}>
                    <div className={"spiner"}></div>
                    <img className={"logo_icon"} src={logo} alt="logo Movina" />
                </div>

                {isShowChatbot && (
                    <div className={"chatbot"} ref={chatbotRef}>
                        <div className={"chatbot_container"}>
                            <div className={"header_box"}>
                                <div className={"intro"}>
                                    <img className={"icon_header_box"} src={logo} alt="Movina" />
                                    <span className={"name_assistant"}>Movina Bot</span>
                                    {isRecording && (
                                        <div className={"animation "} onClick={handleCloseMicro}>
                                            <Lottie animationData={wave_voice} loop={true} autoplay={true} />
                                        </div>
                                    )}
                                </div>

                                <div className={"float_func"}>
                                    <FontAwesomeIcon className={"ico_zoom"} icon={faUpRightAndDownLeftFromCenter} onClick={handleClickExpand} />
                                    <FontAwesomeIcon className={"ico_close"} icon={faXmark} onClick={handleClickClose} />
                                </div>
                            </div>
                            <hr />
                            <div className={"chatbot_message_container"}>
                                <div className={"box_message"} ref={messagesRef}>
                                    {messages.map((item, index) =>
                                        item?.role === "assistant" ? (
                                            <div key={index} className={"message_box message_box_assistant"}>
                                                <div className={"message_container"}>
                                                    <img src={logo_chatbot} alt="Movina" className={"logo logo_assistant"} />
                                                    <div className={"message_content"}>
                                                        <p
                                                            dangerouslySetInnerHTML={{
                                                                __html: item?.content,
                                                            }}
                                                        />
                                                    </div>
                                                </div>
                                            </div>
                                        ) : (
                                            <div key={index} className={"message_box message_box_client"}>
                                                <div className={"message_container"}>
                                                    <img src={client} alt="Movina" className={"logo logo_client"} />
                                                    <div className={"message_content"}>
                                                        <p>{item?.content}</p>
                                                    </div>
                                                </div>
                                            </div>
                                        )
                                    )}
                                </div>
                            </div>
                            <div className={"footer_box"}>
                                <div className={"input_message_box"}>
                                    <textarea
                                        ref={textareaRef}
                                        placeholder="Chat ngay để nhận trợ giúp!"
                                        type="text"
                                        className={"input_message"}
                                        onChange={handleChangeMessage}
                                        onKeyDown={handleKeyDownMessage}
                                    />
                                    <div className={"float_box_func"}>
                                        {/* <FontAwesomeIcon className={ico_attached} icon={faPaperclip} /> */}
                                        {isRecording ? (
                                            <div className={"animation "} onClick={handleCloseMicro}>
                                                <Lottie animationData={wave} loop={true} autoplay={true} />
                                            </div>
                                        ) : (
                                            <FontAwesomeIcon icon={faMicrophone} className={"ico_micro"} onClick={handleClickMicro} />
                                        )}
                                        <FontAwesomeIcon icon={faPaperPlane} className={"ico_send"} onClick={handleSendMessage} />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                )}
            </div>
        </div>
    )
}

export default ChatbotAssistant
