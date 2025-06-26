import React, { useState, useEffect, useContext } from 'react';
import { GetAllComment, DeleteComment } from '../apis/commentAPI';
import { slidebarMenus } from './slidebar';
import { toast } from 'react-toastify';
import 'bootstrap/dist/css/bootstrap.min.css';
import { Container, Table, Button, Modal, Form, Row, Col, Pagination, Card } from 'react-bootstrap';
import { FaEdit, FaTrash, FaPlus, FaArrowLeft, FaFilm, FaList, FaUsers, FaBars, FaTimes, FaAngleLeft, FaAngleRight, FaCheck, FaTimes as FaX } from 'react-icons/fa';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { DataContext } from '../ContextAPI/ContextNavbar';
import Navbar from '../Dashboard/Navbar';
import Footer from '../Dashboard/Footer';
import { ExecuteCommentReport, ResponseCommentReport, GetCommentReport } from '../apis/reportAPI';

const CommentManagement = () => {
    const [comments, setComments] = useState([]);
    const { categories, movieTypes, nations, statuses, statusMap } = useContext(DataContext);
    const [loading, setLoading] = useState(true);
    const [isSidebarOpen, setIsSidebarOpen] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [selectedComment, setSelectedComment] = useState(null);
    const [reports, setReports] = useState([]);
    const [reportResponses, setReportResponses] = useState({}); // Lưu phản hồi cho từng báo cáo
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage] = useState(10);
    const [searchType, setSearchType] = useState("Tất cả");
    const [searchTerm, setSearchTerm] = useState("");
    const [filteredComment, setFilteredComment] = useState([]);
    const navigate = useNavigate();
    const location = useLocation();

    useEffect(() => {
        fetchComments();
    }, []);
    useEffect(() => {
        const debounceSearch = setTimeout(() => {
          if (searchTerm.trim() === "") {
            setFilteredComment(comments); // Reset to all movies if search term is empty
            return;
          }
          const filtered = comments.filter((comment) => {
            
            const searchValue = searchTerm.toLowerCase();
            const normalizedSearchValue = searchValue.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
            switch (searchType) {
              case "Id":
                return comment.idComment.toLowerCase().includes(searchValue);
              case "Người dùng":
                return (comment.idUserName).normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().includes(normalizedSearchValue);
              case "Phim":
                return (comment.title).normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().includes(normalizedSearchValue);
              case "Nội dung":
                return (comment.content).normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().includes(normalizedSearchValue);
              case "Trạng thái":
                const normalizedStatus = normalizedSearchValue;

  // Chuyển trạng thái từ input thành boolean tương ứng
                const searchIsReported =
                    normalizedStatus.includes("binh thuong") ? false :
                    normalizedStatus.includes("bi bao cao") ? true :
                    null;

                if (searchIsReported === null) {
                    // Trường hợp nhập linh tinh (ví dụ: "xyz") thì fallback so sánh chuỗi
                    return comment.isReported.toString().toLowerCase().includes(normalizedStatus);
                } else {
                    return comment.isReported === searchIsReported;
                }
              case "Tất cả":
                return (
                    comment.idComment.toLowerCase().includes(searchValue) ||
                    comment.idUserName
                      .normalize("NFD")
                      .replace(/[\u0300-\u036f]/g, "")
                      .toLowerCase()
                      .includes(normalizedSearchValue) ||
                    comment.title
                      .normalize("NFD")
                      .replace(/[\u0300-\u036f]/g, "")
                      .toLowerCase()
                      .includes(normalizedSearchValue) ||
                    comment.content
                      .normalize("NFD")
                      .replace(/[\u0300-\u036f]/g, "")
                      .toLowerCase()
                      .includes(normalizedSearchValue) ||
                    comment.isReported.toString().toLowerCase().includes(normalizedSearchValue)
                  );
              default:
                return true;
            }
          });
    
          setFilteredComment(filtered);
          setCurrentPage(1); // Reset to first page on search
        }, 1000); // 1-second delay
        return () => clearTimeout(debounceSearch); // Cleanup timeout
  }, [searchTerm, searchType, comments]);

    const renderPagination = () => {
        let items = [];
        for (let number = 1; number <= totalPages; number++) {
            items.push(
                <Pagination.Item 
                    key={number} 
                    active={number === currentPage}
                    onClick={() => handlePageChange(number)}
                >
                    {number}
                </Pagination.Item>
            );
        }
        return items;
    };
    const handlePageChange = (pageNumber) => {
        setCurrentPage(pageNumber);
    };
    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;
    const currentItems = filteredComment.slice(indexOfFirstItem, indexOfLastItem);
    const totalPages = Math.ceil(filteredComment.length / itemsPerPage);
    const fetchComments = async () => {
        try {
            const response = await GetAllComment();
            setComments(response.data);
            setFilteredComment(response.data);
            setLoading(false);
        } catch (error) {
            toast.error('Không thể tải danh sách comment');
            setLoading(false);
        }
    };

    const handleDeleteComment = async (idComment) => {
        if (window.confirm('Bạn có chắc chắn muốn xóa comment này?')) {
            try {
                await DeleteComment(idComment);
                toast.success('Xóa comment thành công');
                fetchComments();
            } catch (error) {
                toast.error('Không thể xóa comment');
            }
        }
    };

    const handleEditComment = async (comment) => {
        try {
            const response = await GetCommentReport(comment.idComment);
            const reports = response.data || [];
            const canRespond = reports.length > 0 && reports.every(report => report.status === 1);
            setSelectedComment({ ...comment, canRespond });
            setReports(reports);
            // Khởi tạo reportResponses với giá trị rỗng cho mỗi báo cáo
            setReportResponses(reports.reduce((acc, report) => ({
                ...acc,
                [report.idReport]: report.response || ''
            }), {}));
            setShowModal(true);
        } catch (error) {
            console.error('Lỗi khi lấy danh sách báo cáo:', error);
            toast.error('Không thể tải danh sách báo cáo');
        }
    };

    const handleAcceptReport = async () => {
        if (!selectedComment) return;
        try {
            await ExecuteCommentReport(selectedComment.idComment);
            toast.success('Tiếp nhận báo cáo thành công');
            const response = await GetCommentReport(selectedComment.idComment);
            const reports = response.data || [];
            const canRespond = reports.length > 0 && reports.every(report => report.status === 1);
            setSelectedComment({ ...selectedComment, canRespond });
            setReports(reports);
            setReportResponses(reports.reduce((acc, report) => ({
                ...acc,
                [report.idReport]: report.response || ''
            }), {}));
            fetchComments();
        } catch (error) {
            console.error('Lỗi khi tiếp nhận báo cáo:', error);
            toast.error('Không thể tiếp nhận báo cáo');
        }
    };

    const handleRespondReport = async () => {
        if (!selectedComment) return;
        const responses = Object.entries(reportResponses)
        .map(([idReport, response]) => ({
            idReport,
            response
        }));
        try {
            await ResponseCommentReport(responses);
            toast.success('Phản hồi báo cáo thành công');
            setShowModal(false);
            fetchComments();
        } catch (error) {
            console.error('Lỗi khi phản hồi báo cáo:', error);
            toast.error('Không thể phản hồi báo cáo');
        }
    };

    const handleResponseChange = (idReport, value) => {
        setReportResponses(prev => ({
            ...prev,
            [idReport]: value
        }));
    };

    const handleCloseModal = () => {
        setShowModal(false);
        setSelectedComment(null);
        setReports([]);
        setReportResponses({});
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
            <div className={`admin-sidebar ${isSidebarOpen ? 'open' : 'closed'}`}>
                <div className="sidebar-header">
                    <Button 
                        variant="link" 
                        className="text-white text-decoration-none mb-3"
                        onClick={() => navigate('/quan-ly')}
                    >
                        <FaArrowLeft className="me-2" />
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
            <Button
                variant="dark"
                className="sidebar-toggle-fixed"
                onClick={() => setIsSidebarOpen(!isSidebarOpen)}
            >
                {isSidebarOpen ? <FaAngleLeft /> : <FaAngleRight />}
            </Button>
            <div className={`admin-content ${isSidebarOpen ? '' : 'expanded'}`}>
            <Container fluid>
                <h2 className="mb-4">Quản Lý Bình Luận</h2>
                <h2 className="mb-4">Quản Lý Bình Luận</h2>
                <div className="search-bar d-flex align-items-center me-3">
                                <Form.Select
                                className="me-2"
                                style={{ width: "100px" }}
                                value={searchType}
                                onChange={(e) => {
                                    e.preventDefault();
                                    setSearchType(e.target.value);
                                }}
                                >
                                <option value="Tất cả">Tất cả</option>
                                <option value="Id">Id</option>
                                <option value="Người dùng">Người dùng</option>
                                <option value="Phim">Phim</option>
                                <option value="Nội dung">Nội dung</option>
                                <option value="Trạng thái">Trạng thái</option>
                                </Form.Select>
                                <Form.Control
                                type="text"
                                placeholder="Nhập thông tin tìm kiếm..."
                                className="me-4"
                                value={searchTerm}
                                onChange={(e) => {
                                    e.preventDefault();
                                    setSearchTerm(e.target.value);
                                }}
                                />
            </div>
                {loading ? (
                    <div className="text-center">
                        <div className="spinner-border" role="status">
                            <span className="visually-hidden">Loading...</span>
                        </div>
                    </div>
                ) : (
                    <div className="table-responsive">
                        <table className="table table-striped table-hover">
                            <thead className="table-dark">
                                <tr>
                                    <th>ID</th>
                                    <th>Người Dùng</th>
                                    <th>Phim</th>
                                    <th>Nội Dung</th>
                                    <th>Thời Gian</th>
                                    <th>Trạng Thái</th>
                                    <th>Thao Tác</th>
                                </tr>
                            </thead>
                            <tbody>
                                {currentItems.map((comment) => (
                                    <tr key={comment.idComment}>
                                        <td>{comment.idComment}</td>
                                        <td>{comment.idUserName}</td>
                                        <td>{comment.title}</td>
                                        <td>{comment.content}</td>
                                        <td>{comment.timeComment}</td>
                                        <td>
                                            <span className={`badge ${comment.isReported ? 'bg-danger' : 'bg-success'}`}>
                                                {comment.isReported ? 'Bị báo cáo' : 'Bình Thường'}
                                            </span>
                                        </td>
                                        <td>
                                            <Button
                                                variant="warning"
                                                size="sm"
                                                className="me-2"
                                                onClick={() => handleEditComment(comment)}
                                            >
                                                <FaEdit /> Chi tiết
                                            </Button>
                                            <Button
                                                variant="danger"
                                                size="sm"
                                                onClick={() => handleDeleteComment(comment.idComment)}
                                            >
                                                <FaTrash /> Xóa
                                            </Button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                        <div className="d-flex justify-content-center mt-4">
                                <Pagination>
                                    <Pagination.First 
                                        onClick={() => handlePageChange(1)}
                                        disabled={currentPage === 1}
                                    />
                                    <Pagination.Prev 
                                        onClick={() => handlePageChange(currentPage - 1)}
                                        disabled={currentPage === 1}
                                    />
                                    {renderPagination()}
                                    <Pagination.Next 
                                        onClick={() => handlePageChange(currentPage + 1)}
                                        disabled={currentPage === totalPages}
                                    />
                                    <Pagination.Last 
                                        onClick={() => handlePageChange(totalPages)}
                                        disabled={currentPage === totalPages}
                                    />
                                </Pagination>
                            </div>
                        
                    </div>
                )}

                <Modal show={showModal} onHide={handleCloseModal} size="lg">
                    <Modal.Header closeButton>
                        <Modal.Title>Quản Lý Báo Cáo Bình Luận</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        {selectedComment && (
                            <>
                                <h5>Bình luận: {selectedComment.content}</h5>
                                <p><strong>Phim:</strong> {selectedComment.title}</p>
                                <p><strong>Người dùng:</strong> {selectedComment.idUserName}</p>
                                <h6 className="mt-4">Danh sách báo cáo</h6>
                                {reports.length > 0 ? (
                                    <div>
                                        {reports.map((report) => (
                                            <Card key={report.idReport} className="mb-3">
                                                <Card.Body>
                                                    <Row>
                                                        <Col md={6}>
                                                            <p><strong>ID Báo Cáo:</strong> {report.idReport}</p>
                                                            <p><strong>Người Báo Cáo:</strong> {report.userNameReporter}</p>
                                                            <p><strong>Nội Dung:</strong> {report.content}</p>
                                                            <p><strong>Thời Gian Báo Cáo:</strong> {report.timeReport}</p>
                                                        </Col>
                                                        <Col md={6}>
                                                            <p><strong>Trạng Thái:</strong> 
                                                                <span className={`badge ms-2 ${report.status === 0 ? 'bg-danger' : 'bg-warning'}`}>
                                                                    {report.status === 0 ? 'Chưa xử lý' : 'Đang xử lý'}
                                                                </span>
                                                            </p>
                                                            <p><strong>Admin Xử Lý:</strong> {report.userNameAdminFix || 'Chưa có'}</p>
                                                            <p><strong>Thời Gian Phản Hồi:</strong> {report.timeResponse || 'Chưa có'}</p>
                                                        </Col>
                                                    </Row>
                                                    <Form.Group className="mt-3">
                                                        <Form.Label>Phản hồi báo cáo</Form.Label>
                                                        <Form.Control
                                                            as="textarea"
                                                            rows={3}
                                                            value={reportResponses[report.idReport] || ''}
                                                            onChange={(e) => handleResponseChange(report.idReport, e.target.value)}
                                                            placeholder="Nhập phản hồi cho báo cáo này..."
                                                            disabled={!selectedComment?.canRespond}
                                                        />
                                                    </Form.Group>
                                                </Card.Body>
                                            </Card>
                                        ))}
                                    </div>
                                ) : (
                                    <p>Không có báo cáo nào cho bình luận này.</p>
                                )}
                                
                            </>     
                        )}
                        
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="secondary" onClick={handleCloseModal}>
                            Hủy
                        </Button>
                        <Button
                            variant="success"
                            onClick={handleAcceptReport}
                            disabled={selectedComment?.canRespond}
                        >
                            Tiếp nhận
                        </Button>
                        <Button
                            variant="primary"
                            onClick={handleRespondReport}
                            disabled={!selectedComment?.canRespond}
                        >
                            Phản hồi
                        </Button>
                    </Modal.Footer>
                </Modal>
                </Container>
            </div>
        </div>
    );
};

export default CommentManagement;