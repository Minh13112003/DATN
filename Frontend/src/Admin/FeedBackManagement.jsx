import React, { useState, useEffect } from 'react';
import { Container, Table, Badge, Button, Modal, Form, Row, Col } from 'react-bootstrap';
import { FaTrash, FaEye, FaAngleLeft, FaAngleRight, FaFilm, FaList, FaUsers, FaReply, FaCheck } from 'react-icons/fa';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import axios from 'axios';
import { DataContext } from '../ContextAPI/ContextNavbar';
import Navbar from '../Dashboard/Navbar';
import Footer from '../Dashboard/Footer';
import './AdminStyles.css';
import { slidebarMenus } from './slidebar';

const FeedbackManagement = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const { categories, movieTypes, nations, statuses, statusMap } = React.useContext(DataContext);
    const [feedbacks, setFeedbacks] = useState([]);
    const [isSidebarOpen, setIsSidebarOpen] = useState(true);
    const [loading, setLoading] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [selectedFeedback, setSelectedFeedback] = useState(null);
    const [replyText, setReplyText] = useState('');
    const [filter, setFilter] = useState('all'); // all, pending, responded

    // Thêm định nghĩa sidebarMenus
    useEffect(() => {
        fetchFeedbacks();
    }, [filter]);

    const fetchFeedbacks = async () => {
        try {
            setLoading(true);
            let url = 'http://localhost:5285/api/feedback/admin';
            if (filter === 'pending') {
                url = 'http://localhost:5285/api/feedback/admin/pending';
            } else if (filter === 'responded') {
                url = 'http://localhost:5285/api/feedback/admin/responded';
            }
            
            const token = localStorage.getItem('userToken');
            const response = await axios.get(url, {
                headers: { Authorization: `Bearer ${token}` }
            });
            
            setFeedbacks(response.data);
            setLoading(false);
        } catch (error) {
            console.error('Error fetching feedbacks:', error);
            setLoading(false);
        }
    };

    const handleViewFeedback = (feedback) => {
        setSelectedFeedback(feedback);
        setShowModal(true);
    };

    const handleDeleteFeedback = async (id) => {
        if (window.confirm('Bạn có chắc muốn xóa góp ý này?')) {
            try {
                const token = localStorage.getItem('userToken');
                await axios.delete(`http://localhost:5285/api/feedback/admin/${id}`, {
                    headers: { Authorization: `Bearer ${token}` }
                });
                fetchFeedbacks();
            } catch (error) {
                console.error('Error deleting feedback:', error);
            }
        }
    };

    const handleReply = async () => {
        if (!replyText.trim() || !selectedFeedback) return;

        try {
            const token = localStorage.getItem('userToken');
            await axios.post(`http://localhost:5285/api/feedback/admin/${selectedFeedback.id}/reply`, {
                responseMessage: replyText
            }, {
                headers: { Authorization: `Bearer ${token}` }
            });
            
            setReplyText('');
            setShowModal(false);
            fetchFeedbacks();
        } catch (error) {
            console.error('Error replying to feedback:', error);
        }
    };

    const handleMarkAsResolved = async (id) => {
        try {
            const token = localStorage.getItem('userToken');
            await axios.post(`http://localhost:5285/api/feedback/admin/${id}/resolve`, {}, {
                headers: { Authorization: `Bearer ${token}` }
            });
            fetchFeedbacks();
            if (selectedFeedback && selectedFeedback.id === id) {
                setShowModal(false);
            }
        } catch (error) {
            console.error('Error marking feedback as resolved:', error);
        }
    };

    const formatDate = (dateString) => {
        const date = new Date(dateString);
        return date.toLocaleDateString('vi-VN', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit'
        });
    };

    const getFeedbackTypeLabel = (type) => {
        switch (type) {
            case 'suggestion': return 'Góp ý cải thiện';
            case 'bug': return 'Báo cáo lỗi';
            case 'content': return 'Yêu cầu nội dung';
            case 'other': return 'Khác';
            default: return type;
        }
    };

    const getFeedbackTypeBadge = (type) => {
        switch (type) {
            case 'suggestion': return 'primary';
            case 'bug': return 'danger';
            case 'content': return 'warning';
            case 'other': return 'secondary';
            default: return 'info';
        }
    };

    return (
        <div>
            <Navbar 
                categories={categories} 
                movieTypes={movieTypes} 
                nations={nations} 
                statuses={statuses} 
                statusMap={statusMap} 
            />

            <div className="admin-layout">
                {/* Sidebar */}
                <div className={`admin-sidebar ${isSidebarOpen ? 'open' : 'closed'}`}>
                    <div className="sidebar-header">
                        <Button 
                            variant="link" 
                            className="text-white text-decoration-none mb-3"
                            onClick={() => navigate('/quan-ly')}
                        >
                            <FaAngleLeft className="me-2" />
                            Quay lại Dashboard
                        </Button>
                    </div>
                    {slidebarMenus.map((menu, index) => (
                        <div key={index} className="sidebar-menu-item">
                            <div className="sidebar-menu-header">
                                {menu.icon}
                                <span>{menu.title}</span>
                            </div>
                            <div className="sidebar-submenu">
                                {menu.items.map((item, idx) => (
                                    <Link 
                                        key={idx}
                                        to={item.link}
                                        className={`sidebar-submenu-item ${location.pathname === item.link ? 'active' : ''}`}
                                    >
                                        {item.title}
                                    </Link>
                                ))}
                            </div>
                        </div>
                    ))}
                </div>

                {/* Toggle Button */}
                <Button
                    variant="dark"
                    className="sidebar-toggle-fixed"
                    onClick={() => setIsSidebarOpen(!isSidebarOpen)}
                >
                    {isSidebarOpen ? <FaAngleLeft /> : <FaAngleRight />}
                </Button>

                {/* Main Content */}
                <div className={`admin-content ${isSidebarOpen ? '' : 'expanded'}`}>
                    <Container fluid>
                        <h2 className="mb-4">Quản lý Góp ý</h2>
                        
                        <div className="d-flex justify-content-between align-items-center mb-4">
                            <div className="btn-group">
                                <Button 
                                    variant={filter === 'all' ? 'primary' : 'outline-primary'}
                                    onClick={() => setFilter('all')}
                                >
                                    Tất cả
                                </Button>
                                <Button 
                                    variant={filter === 'pending' ? 'primary' : 'outline-primary'}
                                    onClick={() => setFilter('pending')}
                                >
                                    Chưa phản hồi
                                </Button>
                                <Button 
                                    variant={filter === 'responded' ? 'primary' : 'outline-primary'}
                                    onClick={() => setFilter('responded')}
                                >
                                    Đã phản hồi
                                </Button>
                            </div>
                            
                            <Button 
                                variant="success" 
                                onClick={() => fetchFeedbacks()}
                            >
                                Làm mới
                            </Button>
                        </div>

                        {loading ? (
                            <div className="text-center py-5">
                                <div className="spinner-border" role="status">
                                    <span className="visually-hidden">Đang tải...</span>
                                </div>
                            </div>
                        ) : (
                            <Table striped bordered hover responsive>
                                <thead>
                                    <tr>
                                        <th>ID</th>
                                        <th>Người gửi</th>
                                        <th>Email</th>
                                        <th>Tiêu đề</th>
                                        <th>Loại</th>
                                        <th>Thời gian</th>
                                        <th>Trạng thái</th>
                                        <th>Thao tác</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {feedbacks.length > 0 ? (
                                        feedbacks.map(feedback => (
                                            <tr key={feedback.id}>
                                                <td>{feedback.id}</td>
                                                <td>{feedback.name}</td>
                                                <td>{feedback.email}</td>
                                                <td>{feedback.subject}</td>
                                                <td>
                                                    <Badge bg={getFeedbackTypeBadge(feedback.type)}>
                                                        {getFeedbackTypeLabel(feedback.type)}
                                                    </Badge>
                                                </td>
                                                <td>{formatDate(feedback.createdAt)}</td>
                                                <td>
                                                    {feedback.isResolved ? (
                                                        <Badge bg="success">Đã giải quyết</Badge>
                                                    ) : feedback.response ? (
                                                        <Badge bg="info">Đã phản hồi</Badge>
                                                    ) : (
                                                        <Badge bg="warning">Chưa phản hồi</Badge>
                                                    )}
                                                </td>
                                                <td>
                                                    <Button 
                                                        variant="info" 
                                                        size="sm" 
                                                        className="me-2"
                                                        onClick={() => handleViewFeedback(feedback)}
                                                    >
                                                        <FaEye />
                                                    </Button>
                                                    
                                                    {!feedback.isResolved && (
                                                        <Button 
                                                            variant="success" 
                                                            size="sm"
                                                            className="me-2"
                                                            onClick={() => handleMarkAsResolved(feedback.id)}
                                                        >
                                                            <FaCheck />
                                                        </Button>
                                                    )}
                                                    
                                                    <Button 
                                                        variant="danger" 
                                                        size="sm"
                                                        onClick={() => handleDeleteFeedback(feedback.id)}
                                                    >
                                                        <FaTrash />
                                                    </Button>
                                                </td>
                                            </tr>
                                        ))
                                    ) : (
                                        <tr>
                                            <td colSpan="8" className="text-center">Không có góp ý nào</td>
                                        </tr>
                                    )}
                                </tbody>
                            </Table>
                        )}
                    </Container>
                </div>
            </div>

            {/* Modal Chi tiết góp ý */}
            <Modal show={showModal} onHide={() => setShowModal(false)} size="lg">
                <Modal.Header closeButton>
                    <Modal.Title>Chi tiết góp ý</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {selectedFeedback && (
                        <>
                            <Row className="mb-3">
                                <Col md={6}>
                                    <p><strong>Người gửi:</strong> {selectedFeedback.name}</p>
                                    <p><strong>Email:</strong> {selectedFeedback.email}</p>
                                </Col>
                                <Col md={6}>
                                    <p>
                                        <strong>Loại góp ý:</strong>{' '}
                                        <Badge bg={getFeedbackTypeBadge(selectedFeedback.type)}>
                                            {getFeedbackTypeLabel(selectedFeedback.type)}
                                        </Badge>
                                    </p>
                                    <p><strong>Thời gian:</strong> {formatDate(selectedFeedback.createdAt)}</p>
                                </Col>
                            </Row>

                            <div className="mb-4">
                                <h6>Tiêu đề:</h6>
                                <p className="fw-bold">{selectedFeedback.subject}</p>
                            </div>

                            <div className="p-3 bg-light rounded mb-4">
                                <h6>Nội dung góp ý:</h6>
                                <p className="mb-0">{selectedFeedback.message}</p>
                            </div>

                            {selectedFeedback.response && (
                                <div className="p-3 bg-info bg-opacity-10 rounded mb-4">
                                    <h6>Phản hồi của admin:</h6>
                                    <p className="mb-0">{selectedFeedback.response}</p>
                                    <small className="text-muted">
                                        Phản hồi lúc: {formatDate(selectedFeedback.respondedAt)}
                                    </small>
                                </div>
                            )}

                            {!selectedFeedback.isResolved && (
                                <Form>
                                    <Form.Group className="mb-3">
                                        <Form.Label>Phản hồi góp ý:</Form.Label>
                                        <Form.Control
                                            as="textarea"
                                            rows={3}
                                            value={replyText}
                                            onChange={(e) => setReplyText(e.target.value)}
                                            placeholder="Nhập nội dung phản hồi..."
                                        />
                                    </Form.Group>
                                </Form>
                            )}
                        </>
                    )}
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowModal(false)}>
                        Đóng
                    </Button>
                    {selectedFeedback && !selectedFeedback.isResolved && (
                        <>
                            <Button 
                                variant="primary" 
                                onClick={handleReply} 
                                disabled={!replyText.trim()}
                            >
                                <FaReply className="me-1" /> Gửi phản hồi
                            </Button>
                            <Button 
                                variant="success" 
                                onClick={() => handleMarkAsResolved(selectedFeedback.id)}
                            >
                                <FaCheck className="me-1" /> Đánh dấu đã giải quyết
                            </Button>
                        </>
                    )}
                    <Button 
                        variant="danger" 
                        onClick={() => {
                            handleDeleteFeedback(selectedFeedback.id);
                            setShowModal(false);
                        }}
                    >
                        <FaTrash className="me-1" /> Xóa góp ý
                    </Button>
                </Modal.Footer>
            </Modal>

            
        </div>
    );
};

export default FeedbackManagement;