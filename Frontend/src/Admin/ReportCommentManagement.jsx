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
import { ExecuteCommentReport, ResponseCommentReport, GetReportComment, ReceiveReport, ResponseReport } from '../apis/reportAPI';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCheckToSlot, faPaperPlane } from '@fortawesome/free-solid-svg-icons';

const ReportCommentManagement = ({searchType, searchTerm}) => {
    const [comments, setComments] = useState([]);
    const [loading, setLoading] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage] = useState(10);
    const [filteredComment, setFilteredComment] = useState([]);
    const [contentReport, setContentReport] = useState('');
    const [contentReported, setContentReported] = useState('');
    const [iDReport, setIDReport] = useState('');
    const [contentRespone, setContentResponse] = useState('');

    useEffect(() => {
        fetchComments();
    }, []);

    useEffect(() => {
        const debounceSearch = setTimeout(() => {
          if (searchTerm.trim() === "") {
            setFilteredComment(comments);
            return;
          }
          console.log(">>> ", searchTerm)
          const filtered = comments.filter((comment) => {
            
            const searchValue = searchTerm.toLowerCase();
            const normalizedSearchValue = searchValue.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
            switch (searchType) {
                case "Id":
                    return comment.idReport.toLowerCase().includes(searchValue)
                    || comment.idComment.toLowerCase().includes(searchValue);
                case "Người dùng":
                    return (comment.userNameReporter).normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().includes(normalizedSearchValue)
                    || (comment.nameOfUserReported).normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().includes(normalizedSearchValue);;
                case "Nội dung":
                    return (comment.content).normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().includes(normalizedSearchValue)
                    || (comment.contentCommentReported).normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().includes(normalizedSearchValue);
                case "Thời gian":
                    return (comment.timeReport).normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().includes(normalizedSearchValue);
                case "Trạng thái":
                    return (comment.statusText).normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().includes(normalizedSearchValue);
                case "Tất cả":
                    return (
                        (comment.idReport && comment.idReport.toLowerCase().includes(searchValue)) ||
                        (comment.idComment && comment.idComment.toLowerCase().includes(searchValue)) ||
                        (comment.userNameReporter &&
                        comment.userNameReporter
                            .normalize("NFD")
                            .replace(/[\u0300-\u036f]/g, "")
                            .toLowerCase()
                            .includes(normalizedSearchValue)) ||
                        (comment.nameOfUserReported &&
                        comment.nameOfUserReported
                            .normalize("NFD")
                            .replace(/[\u0300-\u036f]/g, "")
                            .toLowerCase()
                            .includes(normalizedSearchValue)) ||
                        (comment.statusText &&
                        comment.statusText
                            .normalize("NFD")
                            .replace(/[\u0300-\u036f]/g, "")
                            .toLowerCase()
                            .includes(normalizedSearchValue)) ||
                        (comment.content &&
                        comment.content
                            .normalize("NFD")
                            .replace(/[\u0300-\u036f]/g, "")
                            .toLowerCase()
                            .includes(normalizedSearchValue)) ||
                        (comment.contentCommentReported &&
                        comment.contentCommentReported
                            .normalize("NFD")
                            .replace(/[\u0300-\u036f]/g, "")
                            .toLowerCase()
                            .includes(normalizedSearchValue)) ||
                        (comment.timeReport &&
                        comment.timeReport
                            .normalize("NFD")
                            .replace(/[\u0300-\u036f]/g, "")
                            .toLowerCase()
                            .includes(normalizedSearchValue))
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
            const response = await GetReportComment();
            setComments(response.data);
            setFilteredComment(response.data);
            setLoading(false);
        } catch (error) {
            toast.error('Không thể tải danh sách comment');
            setLoading(false);
        }
    };

    const handleReceiveReport = async (idReport) => {
        try {
            await ReceiveReport(idReport);
            toast.success('Tiếp nhận báo cáo thành công');
            fetchComments();
        } catch(error) {
            console.error('Lỗi:', error);
            toast.error('Không thể tiếp nhận báo cáo');
        }
    }

    const handleOpenModalRespone = async (idReport, txt_report, txt_reported) => {
        setShowModal(true);
        setIDReport(idReport);
        setContentReport(txt_report);
        setContentReported(txt_reported);
    }

    const handleCloseModal = () => {
        setShowModal(false);
        setIDReport('');
        setContentResponse('');
        setContentReported('');
    };

    const handleResponseChange = (e) => {
        setContentResponse(e.target.value);
    }

    const handleResponeReport = async () => {
        try {
            await ResponseReport(iDReport, contentRespone);
            toast.success('Phản hồi báo cáo thành công');
            handleCloseModal();
            fetchComments();
        } catch(error) {
            console.error('Lỗi:', error);
            toast.error('Đã có lỗi xảy ra trong quá trình phản hồi');
        }
    }

    return (
        
        <div>
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
                                <th>ID Báo cáo</th>
                                {/* <th>ID Bình luận</th> */}
                                <th>Người báo cáo</th>
                                <th>Người bị báo cáo</th>
                                <th>Nội dung báo cáo</th>
                                <th>Nội dung bị cáo cáo</th>
                                <th>Thời Gian</th>
                                <th>Trạng Thái</th>
                                <th>Thao Tác</th>
                            </tr>
                        </thead>
                        <tbody>
                            {currentItems.map((comment) => (
                                <tr key={comment.idReport}>
                                    <td style={{
                                    maxWidth: '120px',
                                    overflow: 'hidden',
                                    textOverflow: 'ellipsis',
                                    whiteSpace: 'nowrap',
                                    }}>{comment.idReport}</td>
                                    {/* <td style={{
                                    maxWidth: '120px',
                                    overflow: 'hidden',
                                    textOverflow: 'ellipsis',
                                    whiteSpace: 'nowrap',
                                    }}>{comment.idComment}</td> */}
                                    <td>{comment.userNameReporter}</td>
                                    <td>{comment.nameOfUserReported}</td>
                                    <td>{comment.content}</td>
                                    <td>{comment.contentCommentReported}</td>
                                    <td>{comment.timeReport}</td>
                                    <td>
                                        <span className={`badge ${comment.statusText === 'Chưa xử lý' ? 'bg-danger' : 'bg-warning'}`}>
                                            {comment.statusText}
                                        </span>
                                    </td>
                                    <td>
                                        <Button
                                            variant="success"
                                            size="sm"
                                            className="me-2"
                                            disabled={comment.statusText !== 'Chưa xử lý'}
                                            onClick={() => handleReceiveReport(comment.idReport)}
                                        >
                                            <FontAwesomeIcon icon={faCheckToSlot} className='me-1' /> Tiếp nhận
                                        </Button>
                                        <Button
                                            variant="info"
                                            size="sm"
                                            disabled={comment.statusText === 'Chưa xử lý'}
                                            onClick={() => handleOpenModalRespone(comment.idReport, comment.content, comment.contentCommentReported)}
                                        >
                                            <FontAwesomeIcon icon={faPaperPlane} className='me-1' /> Phản hồi
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
                    <Modal.Title>Phản hồi báo cáo người dùng</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form.Group style={{display: 'flex', flexDirection: 'column'}}>
                        <Form.Label style={{fontSize: '18px'}}>Nội dung báo cáo</Form.Label>
                        <p className='mb-5' style={{fontStyle: 'italic'}}>" {contentReport} "</p>
                        <Form.Label style={{fontSize: '18px'}}>Nội dung bị báo cáo</Form.Label>
                        <p className='mb-5' style={{fontStyle: 'italic'}}>" {contentReported} "</p>
                        <Form.Control
                            as="textarea"
                            rows={8}
                            onChange={(e) => handleResponseChange(e)}
                            placeholder="Nhập phản hồi cho báo cáo này..."
                        />
                    </Form.Group>
                </Modal.Body>
                <Modal.Footer>
                    <Button 
                        variant="secondary" 
                        onClick={handleCloseModal}
                        style={{
                            width: '100px',
                            marginRight: '20px'
                        }}
                    >
                        Hủy
                    </Button>
                    <Button
                        variant="primary"
                        onClick={handleResponeReport}
                        style={{
                            width: '100px'
                        }}
                    >
                        Gửi
                    </Button>
                </Modal.Footer>
            </Modal>
        </div>
    );
};

export default ReportCommentManagement;