import React from 'react';
import { Container, Row, Col, Button } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import Navbar from './Dashboard/Navbar';
import Footer from './Dashboard/Footer';
import { DataContext } from "./ContextAPI/ContextNavbar";
import { useContext } from 'react';

const ErrorPage = () => {
    const navigate = useNavigate();
    const { categories, movieTypes, nations, statuses, statusMap } = useContext(DataContext);

    return (
        <div>
            <Navbar categories={categories} movieTypes={movieTypes} nations={nations} statuses={statuses} statusMap={statusMap} />
            <Container className="py-5">
                <Row className="justify-content-center text-center">
                    <Col md={8}>
                        <div className="error-page">
                            <h1 className="display-1 text-danger mb-4">404</h1>
                            <h2 className="mb-4">Oops! Có lỗi xảy ra</h2>
                            <p className="lead mb-5">
                                Trang bạn đang tìm kiếm không tồn tại hoặc đã bị xóa.
                            </p>
                            <Button 
                                variant="primary" 
                                size="lg"
                                onClick={() => navigate('/')}
                                className="px-4 py-2"
                            >
                                Quay về trang chủ
                            </Button>
                        </div>
                    </Col>
                </Row>
            </Container>
            <Footer />
        </div>
    );
};

export default ErrorPage; 