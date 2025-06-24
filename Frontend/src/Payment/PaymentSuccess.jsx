import React, { useEffect } from 'react';
import { Container, Card, Button } from 'react-bootstrap';
import { FaCheckCircle } from 'react-icons/fa';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import './Payment.css';

const PaymentSuccess = () => {
    const navigate = useNavigate();

    useEffect(() => {
        toast.success('Thanh toán thành công! Tài khoản của bạn đã được nâng cấp VIP.');
    }, []);

    return (
        <Container className="py-5">
            <Card className="text-center p-5">
                <FaCheckCircle className="text-success mb-4" style={{ fontSize: '5rem' }} />
                <h2 className="mb-4">Thanh toán thành công!</h2>
                <p className="lead mb-4">
                    Cảm ơn bạn đã nâng cấp tài khoản VIP. Bạn có thể bắt đầu thưởng thức các tính năng VIP ngay bây giờ.
                </p>
                <div className="d-flex justify-content-center gap-3">
                    <Button 
                        variant="primary" 
                        size="lg"
                        onClick={() => navigate('/')}
                    >
                        Về trang chủ
                    </Button>
                    <Button 
                        variant="outline-primary" 
                        size="lg"
                        onClick={() => navigate('/tai-khoan')}
                    >
                        Xem tài khoản
                    </Button>
                </div>
            </Card>
        </Container>
    );
};

export default PaymentSuccess; 