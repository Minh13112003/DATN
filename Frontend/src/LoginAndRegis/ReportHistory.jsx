import React, { useState, useEffect, useContext } from 'react';
import { Container, Table, Button, Modal, Badge, Spinner, Pagination, Form } from 'react-bootstrap';
import { FaEye, FaCheckCircle, FaSpinner, FaTimesCircle, FaExclamationCircle } from 'react-icons/fa';
import { toast } from 'react-toastify';
import { FaArrowLeft } from 'react-icons/fa';
import { useNavigate } from 'react-router-dom';
import Navbar from '../Dashboard/Navbar';
import { DataContext } from '../ContextAPI/ContextNavbar';
import { SelfReport } from '../apis/reportAPI';

const ReportHistory = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(true);
    const [reportHistory, setReportHistory] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage] = useState(10);
    const { categories, movieTypes, nations, statuses, statusMap } = useContext(DataContext);

    const handleGoBack = () => {
        navigate(-1);
    };

    useEffect(() => {
        fetchReportHistory();
    }, []);

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
    const currentItems = reportHistory.slice(indexOfFirstItem, indexOfLastItem);
    const totalPages = Math.ceil(reportHistory.length / itemsPerPage);

    const fetchReportHistory = async () => {
        try {
            setLoading(true);
            const response = await SelfReport();
            if (response && response.data && Array.isArray(response.data)) {
                setReportHistory(response.data);
            } else {
                console.error('Error:', response);
                toast.error('Định dạng dữ liệu không hợp lệ');
                setReportHistory([]);
            }
        } catch (error) {
            console.error('Error:', error);
            toast.error('Không thể tải lịch sử thanh toán');
            setReportHistory([]);
        } finally {
            setLoading(false);
        }
    };

    if (loading) {
        return (
            <Container className="py-5 text-center">
                <Spinner animation="border" variant="primary" />
                <p className="mt-3">Đang tải dữ liệu...</p>
            </Container>
        );
    }

    const getStatusBadge = (status) => {
        switch (status) {
            case 'Đã phản hồi':
                return <Badge bg="success"><FaCheckCircle /> {status}</Badge>;
            case 'Đang xử lý':
                return <Badge bg="warning"><FaSpinner className="spinning" /> {status}</Badge>;
            case 'Chưa xử lý':
                return <Badge bg='danger'><FaExclamationCircle className="spinning" /> {status}</Badge>;
            default:
                return <Badge bg="error"><FaSpinner className="spinning" /> {status}</Badge>;
        }
    };

    return (
        <div>
        <Navbar categories={categories} movieTypes={movieTypes} nations={nations} statuses={statuses} statusMap={statusMap}/>    
        <Container className="py-4">
            <div className="mb-4">
                <Button variant="outline-secondary" onClick={handleGoBack}>
                    <FaArrowLeft className="me-2" />
                    Quay lại
                </Button>
            </div>

            <h2 className="text-center mb-4" style={{color: 'white'}}>Lịch sử báo cáo</h2>
            
            <div className="table-responsive">
                <Table className="payment-table">
                    <thead>
                        <tr>
                            <th>Thời gian báo cáo</th>
                            <th>Thời gian phản hồi</th>
                            <th>Nội dung báo cáo</th>
                            <th>Nội dung phản hồi</th>
                            <th>Trạng thái</th>
                        </tr>
                    </thead>
                    <tbody>
                        {currentItems && reportHistory.length > 0 ? (
                            currentItems.map((item) => (
                                <tr key={item.idReport}>
                                    <td>{item.timeReport}</td>
                                    <td>{item.timeResponse ? item.timeResponse : 'Hiện chưa phản hồi'}</td>
                                    <td>{item.content}</td>
                                    <td>{item.response ? item.response : 'Hiện chưa phản hồi'}</td>
                                    <td>{getStatusBadge(item.statusText)}</td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="5" className="text-center">
                                    Không có dữ liệu
                                </td>
                            </tr>
                        )}
                    </tbody>
                </Table>
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
        </Container>
        </div>
    );
};

export default ReportHistory; 