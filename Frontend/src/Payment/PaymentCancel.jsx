import React, { useEffect, useContext, useState } from 'react';
import { Container, Card, Button } from 'react-bootstrap';
import { FaTimesCircle } from 'react-icons/fa';
import { useNavigate, useLocation } from 'react-router-dom';
import { toast } from 'react-toastify';
import './Payment.css';
import { CancelPayment } from '../apis/paymentAPI';
import Navbar from '../Dashboard/Navbar';
import { DataContext } from '../ContextAPI/ContextNavbar';
import Footer from '../Dashboard/Footer';

const PaymentCancel = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const [cancelSuccess, setCancelSuccess] = useState(false);
    const [errorShown, setErrorShown] = useState(false); // kiểm soát toast

    // Lấy orderCode từ query parameters
    const queryParams = new URLSearchParams(location.search);
    const orderCode = queryParams.get('orderCode');
    const code = queryParams.get('code');
    const status = queryParams.get('status');
    const { categories, movieTypes, nations, statuses, statusMap } = useContext(DataContext);

    useEffect(() => {
        if (!orderCode || !code || !status) {
            navigate("/*");
            return;
        }

        const handleCancelPayment = async () => {
            const result = await CancelPayment(orderCode);
            if (result.status === 200) {
                setCancelSuccess(true); // chỉ cập nhật state
            } else {
                navigate("/*");
            }
        };

        handleCancelPayment();
    }, [orderCode, code, status]);

    // Effect hiển thị toast (chỉ 1 lần)
    useEffect(() => {
        if (cancelSuccess && !errorShown) {
            toast.error(`Thanh toán đã bị hủy. Mã đơn hàng: ${orderCode || 'Không xác định'}`);
            setErrorShown(true);
        }
    }, [cancelSuccess, errorShown, orderCode]);

    return (
        <div>
            <Navbar 
                categories={categories} 
                movieTypes={movieTypes} 
                nations={nations} 
                statuses={statuses} 
                statusMap={statusMap} 
            />
            <Container className="py-5">
                <Card className="text-center p-5">
                    <FaTimesCircle className="text-danger mb-4" style={{ fontSize: '5rem' }} />
                    <h2 className="mb-4">Thanh toán đã bị hủy</h2>
                    <p className="lead mb-4">
                        Bạn đã hủy quá trình thanh toán. Nếu bạn gặp vấn đề trong quá trình thanh toán, vui lòng liên hệ với chúng tôi để được hỗ trợ.
                    </p>
                    <div className="d-flex justify-content-center gap-3">
                        <Button 
                            variant="primary" 
                            size="lg"
                            onClick={() => navigate('/thanh-toan')}
                        >
                            Thử lại
                        </Button>
                        <Button 
                            variant="outline-secondary" 
                            size="lg"
                            onClick={() => navigate('/')}
                        >
                            Về trang chủ
                        </Button>
                    </div>
                </Card>
            </Container>
            <Footer />
        </div>
    );
};

export default PaymentCancel;
