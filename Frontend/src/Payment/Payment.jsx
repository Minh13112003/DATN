import React, { useState } from 'react';
import { Container, Row, Col, Card, Button } from 'react-bootstrap';
import { FaCrown, FaCheck, FaLock } from 'react-icons/fa';
import { DataContext } from '../ContextAPI/ContextNavbar';
import Navbar from '../Dashboard/Navbar';
import './Payment.css';
import { useContext } from 'react';
import { FaArrowLeft } from 'react-icons/fa';
import { useNavigate } from 'react-router-dom';
import { CreatePaymentURL } from '../apis/paymentAPI';
import { toast } from 'react-toastify';

const Payment = () => {
    const [selectedPlan, setSelectedPlan] = useState(null);
    const { categories, movieTypes, nations, statuses, statusMap } = useContext(DataContext);
    
    const navigate = useNavigate();

    const handleGoBack = () => {
        navigate(-1);
    };
    

    const plans = [
        {
            id: 1,
            name: 'Gói 3 ngày',
            price: '45.000đ',
            duration: '3 ngày',
            features: [
                'Xem phim không giới hạn',
                'Không quảng cáo',
                'Chất lượng Full HD',
                'Hỗ trợ 24/7'
            ],
            description: 'Gói dùng thử phù hợp cho những ai muốn trải nghiệm dịch vụ của chúng tôi trong thời gian ngắn.'
        },
        {
            id: 2,
            name: 'Gói 1 tuần',
            price: '80.000đ',
            duration: '7 ngày',
            features: [
                'Xem phim không giới hạn',
                'Không quảng cáo',
                'Chất lượng Full HD',
                'Hỗ trợ 24/7',
                'Tiết kiệm 10% chi phí '
            ],
            description: 'Gói phổ biến nhất, phù hợp cho người dùng muốn thưởng thức phim trong tuần nghỉ dưỡng.'
        },
        {
            id: 3,
            name: 'Gói 1 tháng',
            price: '280.000đ',
            duration: '30 ngày',
            features: [
                'Xem phim không giới hạn',
                'Không quảng cáo',
                'Chất lượng 4K',
                'Hỗ trợ 24/7',
                'Tiết kiệm 20% chi phí'
            ],
            description: 'Gói tiết kiệm nhất, phù hợp cho người dùng thường xuyên xem phim và muốn trải nghiệm đầy đủ tính năng cao cấp.'
        }
    ];

    const handleSelectPlan = (planId) => {
        setSelectedPlan(planId);
    };

    const handlePayment = async() => {
        if (!selectedPlan) {
            toast.error('Vui lòng chọn gói thanh toán');
            return;
        }
        try {
            const response = await CreatePaymentURL(selectedPlan - 1);
            if (response.data && response.data.checkoutUrl) {
                window.location.href = response.data.checkoutUrl;
            } else {
                toast.error('Không thể tạo link thanh toán');
            }
        } catch (error) {
            console.error('Error creating payment:', error);
            toast.error('Có lỗi xảy ra khi tạo thanh toán');
        }
    };

    return (
        <>
        <Navbar categories={categories} movieTypes={movieTypes} nations={nations} statuses={statuses} statusMap={statusMap}/>
        <Container className="py-5">
            <div className="mb-4">
                <Button variant="outline-secondary" onClick={handleGoBack}>
                    <FaArrowLeft className="me-2" />
                    Quay lại
                </Button>
            </div>
            <div className="text-center mb-5">
                <h1 className="display-4 mb-3">Nâng cấp tài khoản VIP</h1>
                <p className="lead text-muted">Chọn gói phù hợp với nhu cầu của bạn</p>
            </div>

            <Row className="justify-content-center">
                {plans.map((plan) => (
                    <Col key={plan.id} md={4} className="mb-4">
                        <Card className={`h-100 plan-card ${selectedPlan === plan.id ? 'selected' : ''}`}>
                            <Card.Body className="d-flex flex-column">
                                <div className="text-center mb-4">
                                    <FaCrown className="plan-icon mb-3" />
                                    <h3 className="plan-name">{plan.name}</h3>
                                    <div className="plan-price">
                                        <span className="price">{plan.price}</span>
                                        <span className="duration">/{plan.duration}</span>
                                    </div>
                                </div>

                                <ul className="feature-list mb-4">
                                    {plan.features.map((feature, index) => (
                                        <li key={index}>
                                            <FaCheck className="me-2 text-success" />
                                            {feature}
                                        </li>
                                    ))}
                                </ul>

                                <div className="plan-description mb-4">
                                    <p>{plan.description}</p>
                                </div>

                                <Button
                                    variant={selectedPlan === plan.id ? "primary" : "outline-primary"}
                                    className="mt-auto d-flex justify-content-center align-items-center"
                                    onClick={() => handleSelectPlan(plan.id)
                                    }
                                >
                                    {selectedPlan === plan.id ? 'Đã chọn' : 'Chọn gói'}
                                </Button>
                            </Card.Body>
                        </Card>
                    </Col>
                ))}
            </Row>

            <div className="text-center mt-4">
                <Button
                    variant="success"
                    size="lg"
                    className="payment-button"
                    onClick={handlePayment}
                    disabled={!selectedPlan}
                >
                    <FaLock className="me-2" />
                    Thanh toán ngay
                </Button>
            </div>
        </Container>
        </>
    );
};

export default Payment; 