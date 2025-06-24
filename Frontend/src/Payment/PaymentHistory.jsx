import React, { useState, useEffect, useContext } from 'react';
import { Container, Table, Button, Modal, Badge, Spinner, Pagination, Form } from 'react-bootstrap';
import { FaEye, FaCheckCircle, FaSpinner, FaTimesCircle } from 'react-icons/fa';
import { GetDetailPayment, GetPaymentOrders } from '../apis/paymentAPI';
import { toast } from 'react-toastify';
import './PaymentHistory.css';
import { FaArrowLeft } from 'react-icons/fa';
import { useNavigate } from 'react-router-dom';
import { DataContext } from '../ContextAPI/ContextNavbar';
import Navbar from '../Dashboard/Navbar';

const PaymentHistory = () => {
    const [showDetailModal, setShowDetailModal] = useState(false);
    const [selectedOrder, setSelectedOrder] = useState(null);
    const [paymentHistory, setPaymentHistory] = useState([]);
    const [orderDetails, setOrderDetails] = useState(null);
    const [loading, setLoading] = useState(true);
    const [detailLoading, setDetailLoading] = useState(false);
    const { categories, movieTypes, nations, statuses, statusMap } = useContext(DataContext);
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage] = useState(10);
    const [searchType, setSearchType] = useState("Tất cả");
    const [searchTerm, setSearchTerm] = useState("");
    const [filteredPayment, setFilteredPayment] = useState([]);
    const navigate = useNavigate();
    const handleGoBack = () => {
        navigate(-1);
    };
    useEffect(() => {
        fetchPaymentHistory();
    }, []);
    useEffect(() => {
        const debounceSearch = setTimeout(() => {
          if (searchTerm.trim() === "") {
            setFilteredPayment(paymentHistory); // Reset to all movies if search term is empty
            return;
          }
          const filtered = paymentHistory.filter((payment) => {
            
            const searchValue = searchTerm.toLowerCase();
            const normalizedSearchValue = searchValue.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
            switch (searchType) {
              case "Mã đơn hàng":
                return (payment.orderCode || "").toString().includes(normalizedSearchValue);
              case "Tên gói dịch vụ":
                return (payment.item).normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().includes(normalizedSearchValue);
              case "Trạng thái":
                return (payment.status).normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().includes(normalizedSearchValue);
              case "Tất cả":
                return (payment.orderCode || "").toString().includes(searchValue) || (payment.item).normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().includes(normalizedSearchValue)
                || (payment.status).normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().includes(normalizedSearchValue);
              default:
                return true;
            }
          });
    
          setFilteredPayment(filtered);
          setCurrentPage(1); // Reset to first page on search
        }, 1000); // 1-second delay
        return () => clearTimeout(debounceSearch); // Cleanup timeout
  }, [searchTerm, searchType, paymentHistory]); // Trigger on searchTerm or searchType change

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
    const currentItems = filteredPayment.slice(indexOfFirstItem, indexOfLastItem);
    const totalPages = Math.ceil(filteredPayment.length / itemsPerPage);

    const fetchPaymentHistory = async () => {
        try {
            setLoading(true);
            const response = await GetPaymentOrders();
            if (response && response.data && Array.isArray(response.data.data)) {
                setPaymentHistory(response.data.data);
                setFilteredPayment(response.data.data);
            } else {
                console.error('Invalid payment history data format:', response);
                toast.error('Định dạng dữ liệu không hợp lệ');
                setPaymentHistory([]);
            }
        } catch (error) {
            console.error('Error fetching payment history:', error);
            toast.error('Không thể tải lịch sử thanh toán');
            setPaymentHistory([]);
        } finally {
            setLoading(false);
        }
    };

    const handleViewDetails = async (order) => {
        try {
            setDetailLoading(true);
            setSelectedOrder(order);
            const response = await GetDetailPayment(order.orderCode);
            if (response && response.data) {
                setOrderDetails(response.data);
                setShowDetailModal(true);
            } else {
                toast.error('Không thể lấy thông tin chi tiết đơn hàng');
            }
        } catch (error) {
            console.error('Error fetching payment details:', error);
            toast.error('Không thể tải chi tiết đơn hàng');
        } finally {
            setDetailLoading(false);
        }
    };

    const getStatusBadge = (status) => {
        switch (status) {
            case 'Thành công':
                return <Badge bg="success"><FaCheckCircle /> {status}</Badge>;
            case 'Đang xử lý':
                return <Badge bg="warning"><FaSpinner className="spinning" /> {status}</Badge>;
            default:
                return <Badge bg="danger"><FaTimesCircle /> {status}</Badge>;
        }
    };

    const formatDate = (dateString) => {
        const date = new Date(dateString);
        return date.toLocaleString('vi-VN', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit'
        });
    };

    if (loading) {
        return (
            <Container className="py-5 text-center">
                <Spinner animation="border" variant="primary" />
                <p className="mt-3">Đang tải dữ liệu...</p>
            </Container>
        );
    }

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
            <div className="search-bar d-flex align-items-center me-3">
                                <Form.Select
                                className="me-2"
                                style={{ width: "200px" }}
                                value={searchType}
                                onChange={(e) => {
                                    e.preventDefault();
                                    setSearchType(e.target.value);
                                }}
                                >
                                <option value="Tất cả">Tất cả</option>
                                <option value="Mã đơn hàng">Mã đơn hàng</option>
                                <option value="Tên gói dịch vụ">Tên gói dịch vụ</option>
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
            <h2 className="text-center mb-4" style={{color: 'white'}}>Lịch sử thanh toán</h2>
            
            <div className="table-responsive">
                <Table className="payment-table">
                    <thead>
                        <tr>
                            <th>Mã đơn hàng</th>
                            <th>Gói dịch vụ</th>
                            <th>Ngày tạo</th>
                            <th>Trạng thái</th>
                            <th>Thao tác</th>
                        </tr>
                    </thead>
                    <tbody>
                        {currentItems && paymentHistory.length > 0 ? (
                            currentItems.map((order) => (
                                <tr key={order.idPaymentOrder}>
                                    <td>{order.orderCode}</td>
                                    <td>{order.item}</td>
                                    <td>{formatDate(order.createdAt)}</td>
                                    <td>{getStatusBadge(order.status)}</td>
                                    <td>
                                        <Button
                                            variant="outline-primary"
                                            size="sm"
                                            onClick={() => handleViewDetails(order)}
                                        >
                                            <FaEye /> Chi tiết
                                        </Button>
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="5" className="text-center">
                                    Không có dữ liệu thanh toán
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

            {/* Modal Chi tiết đơn hàng */}
            <Modal
                show={showDetailModal}
                onHide={() => setShowDetailModal(false)}
                size="lg"
                centered
            >
                <Modal.Header closeButton>
                    <Modal.Title>Chi tiết đơn hàng</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {detailLoading ? (
                        <div className="text-center py-4">
                            <Spinner animation="border" variant="primary" />
                            <p className="mt-3">Đang tải chi tiết đơn hàng...</p>
                        </div>
                    ) : orderDetails && (
                        <div className="order-details">
                            <div className="detail-section">
                                <h5>Thông tin đơn hàng</h5>
                                <div className="detail-grid">
                                    <div className="detail-item">
                                        <span className="label">Mã đơn hàng:</span>
                                        <span className="value">{orderDetails.orderCode}</span>
                                    </div>
                                    <div className="detail-item">
                                        <span className="label">Số tiền:</span>
                                        <span className="value">{orderDetails.amount.toLocaleString('vi-VN')} VND</span>
                                    </div>
                                    <div className="detail-item">
                                        <span className="label">Đã thanh toán:</span>
                                        <span className="value">{orderDetails.amountPaid.toLocaleString('vi-VN')} VND</span>
                                    </div>
                                    <div className="detail-item">
                                        <span className="label">Còn lại:</span>
                                        <span className="value">{orderDetails.amountRemaining.toLocaleString('vi-VN')} VND</span>
                                    </div>
                                    <div className="detail-item">
                                        <span className="label">Trạng thái:</span>
                                        <span className="value">{getStatusBadge(orderDetails.status === 'PAID' ? 'Thành công' : 'Đang xử lý')}</span>
                                    </div>
                                    <div className="detail-item">
                                        <span className="label">Ngày tạo:</span>
                                        <span className="value">{formatDate(orderDetails.createdAt)}</span>
                                    </div>
                                </div>
                            </div>

                            <div className="detail-section mt-4">
                                <h5>Thông tin giao dịch</h5>
                                <Table className="transaction-table">
                                    <thead>
                                        <tr>
                                            <th>Mã giao dịch</th>
                                            <th>Số tiền</th>
                                            <th>Số tài khoản</th>
                                            <th>Mô tả</th>
                                            <th>Thời gian</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {orderDetails.transactions && orderDetails.transactions.length > 0 ? (
                                            orderDetails.transactions.map((transaction) => (
                                                <tr key={transaction.reference}>
                                                    <td>{transaction.reference}</td>
                                                    <td>{transaction.amount.toLocaleString('vi-VN')} VND</td>
                                                    <td>{transaction.accountNumber}</td>
                                                    <td>{transaction.description}</td>
                                                    <td>{formatDate(transaction.transactionDateTime)}</td>
                                                </tr>
                                            ))
                                        ) : (
                                            <tr>
                                                <td colSpan="5" className="text-center">
                                                    Không có thông tin giao dịch
                                                </td>
                                            </tr>
                                        )}
                                    </tbody>
                                </Table>
                            </div>
                        </div>
                    )}
                </Modal.Body>
            </Modal>
        </Container>
        </div>
    );
};

export default PaymentHistory; 