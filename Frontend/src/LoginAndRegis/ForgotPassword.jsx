import React, { useState, useEffect } from 'react';
import { Container, Form, Button, Alert, InputGroup } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import Navbar from '../Dashboard/Navbar';
import { DataContext } from '../ContextAPI/ContextNavbar';
import { useContext } from 'react';
import { FaEye, FaEyeSlash,FaArrowLeft } from 'react-icons/fa';
import './ForgotPassword.css';
import { SendOTP, VerifyOTP, ResetPassword } from '../apis/authAPI';
import Lottie from "lottie-react";
import loadingAnimation from "../assets/loading.json";
const ForgotPassword = () => {
    const navigate = useNavigate();
    const { categories, movieTypes, nations, statuses, statusMap } = useContext(DataContext);
    const [step, setStep] = useState(1); // 1: Nhập username, 2: Nhập OTP, 3: Đổi mật khẩu
    const [username, setUsername] = useState('');
    const [otp, setOtp] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');
    const [passwordError, setPasswordError] = useState('');
    const [loading, setLoading] = useState(false);
    const [showNewPassword, setShowNewPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);

    // Kiểm tra mật khẩu mỗi khi newPassword hoặc confirmPassword thay đổi
    useEffect(() => {
        if (step === 3) {
            if (newPassword && confirmPassword) {
                // Regex kiểm tra mật khẩu
                const hasUpperCase = /[A-Z]/.test(newPassword);
                const hasLowerCase = /[a-z]/.test(newPassword);
                const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(newPassword);
                const isLengthValid = newPassword.length > 8;
    
                if (newPassword !== confirmPassword) {
                    setPasswordError('Mật khẩu xác nhận không khớp');
                } else if (!isLengthValid) {
                    setPasswordError('Mật khẩu phải trên 8 ký tự');
                } else if (!hasUpperCase) {
                    setPasswordError('Mật khẩu phải chứa ít nhất 1 ký tự hoa');
                } else if (!hasLowerCase) {
                    setPasswordError('Mật khẩu phải chứa ít nhất 1 ký tự thường');
                } else if (!hasSpecialChar) {
                    setPasswordError('Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt');
                } else {
                    setPasswordError('');
                }
            } else {
                setPasswordError('');
            }
        }
    }, [newPassword, confirmPassword, step]);
    const handleFormAction = async (e) => {
        if (e.type === 'keydown' && e.key !== 'Enter') return;
    
        e.preventDefault();
        setError('');
        setSuccess('');
        setLoading(true);
        try {
            if (step === 1) {
                const response = await SendOTP(username, 2);
                setSuccess(response.data.message);
                setStep(2);
            } else if (step === 2) {
                const response = await VerifyOTP(username, otp);
                setSuccess(response.data.message);
                setStep(3);
            } else if (step === 3) {
                if (passwordError) {
                    setError('Vui lòng kiểm tra lại thông tin mật khẩu');
                    return;
                }
                if (newPassword !== confirmPassword) {
                    setError('Mật khẩu xác nhận không khớp');
                    return;
                }
                console.log(step);
                const payload = {
                    userName : username,
                    password : newPassword
                };
                const response = await ResetPassword(payload);
                setSuccess(response.data.message);
                setTimeout(() => navigate('/tai-khoan/auth'), 1000);
            }
        } catch (error) {
            setError('Bạn đã nhập sai OTP hoặc để trống thông tin, xin vui lòng thử lại!');
        } finally {
            setLoading(false); // <-- Quan trọng
        }
    };
    

    const handleSendOTP = async () => {
        try {
            setLoading(true);
            const response = await SendOTP(username, 2);
            setSuccess("Mã OTP đã được gửi lại đến email của bạn");
            setError('');
        } catch (error) {
            setError('Không thể gửi OTP. Vui lòng thử lại sau.');
            setSuccess('');
        } finally {
            setLoading(false);
        }
    };

    const handleVerifyOTP = async (e) => {
        try {
            e.preventDefault();
            const response = await VerifyOTP(username, otp);
            setSuccess(response.data.message);
            setStep(3);
        } catch (error) {
            setError('OTP không đúng. Vui lòng thử lại.');
        }
    };

    const handleResetPassword = async (e) => {
        e.preventDefault();
        if (passwordError) {
            setError('Vui lòng kiểm tra lại thông tin mật khẩu');
            return;
        }
        try {
            if(newPassword === confirmPassword){
             const response = await ResetPassword(username, newPassword);
            setSuccess(response.data.message);
            setTimeout(() => {
                navigate('/tai-khoan/auth');
            }, 1000);
        }else {
            setError('Mật khẩu xác nhận không khớp');

        }
        } catch (error) {
            setError('Không thể đổi mật khẩu. Vui lòng thử lại sau.');
        }
    };

    const handleBack = () => {
        setStep(step - 1);
        setError('');
        setSuccess('');
        setPasswordError('');
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
            <Container className="mt-5 pt-5">
                <div className="auth-container">
                    <h2 className="text-center mb-4">Quên mật khẩu</h2>
                    {error && <Alert variant="danger">{error}</Alert>}
                    {success && <Alert variant="success">{success}</Alert>}
                    
                    
                    <Form onSubmit={handleFormAction} onKeyDown={handleFormAction}> 
                        <Form.Group className="mb-3">
                            <Form.Label>Tên đăng nhập</Form.Label>
                            <Form.Control
                                type="text"
                                value={username}
                                onChange={(e) => setUsername(e.target.value)}
                                disabled={step > 1}
                                required
                            />
                        </Form.Group>

                        {step === 2 && (
                            <>
                                <Form.Group className="mb-3">
                                    <Form.Label>Mã OTP</Form.Label>
                                    <Form.Control
                                        type="text"
                                        value={otp}
                                        onChange={(e) => setOtp(e.target.value)}
                                        required
                                    />
                                </Form.Group>

                                {/* Đây là chỗ bạn đặt nút "Gửi lại OTP" */}
                                <div className="mb-3 text-end">
                                    <Button 
                                        variant="link" 
                                        onClick={handleSendOTP} 
                                        disabled={loading}
                                    >
                                        Gửi lại OTP
                                    </Button>
                                </div>
                            </>
                        )}

                        {step === 3 && (
                        <>
                            <Form.Group className="mb-3">
                            <Form.Label>Mật khẩu mới</Form.Label>
                            <br/>
                            <p className="text-muted">
                                Mật khẩu phải trên 8 ký tự, chứa ít nhất 1 ký tự hoa, 1 ký tự thường và 1 ký tự đặc biệt
                            </p>
                            <InputGroup>
                                <Form.Control
                                type={showNewPassword ? "text" : "password"}
                                value={newPassword}
                                onChange={(e) => setNewPassword(e.target.value)}
                                required
                                isInvalid={!!passwordError}
                                />
                                <InputGroup.Text
                                style={{ cursor: 'pointer' }}
                                onClick={() => setShowNewPassword(!showNewPassword)}
                                aria-label={showNewPassword ? "Ẩn mật khẩu" : "Hiển thị mật khẩu"}
                                >
                                {showNewPassword ? <FaEyeSlash /> : <FaEye />}
                                </InputGroup.Text>
                            </InputGroup>
                            </Form.Group>

                            <Form.Group className="mb-3">
                            <Form.Label>Xác nhận mật khẩu mới</Form.Label>
                            <InputGroup>
                                <Form.Control
                                type={showConfirmPassword ? "text" : "password"}
                                value={confirmPassword}
                                onChange={(e) => setConfirmPassword(e.target.value)}
                                required
                                isInvalid={!!passwordError}
                                />
                                <InputGroup.Text
                                style={{ cursor: 'pointer' }}
                                onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                                aria-label={showConfirmPassword ? "Ẩn mật khẩu" : "Hiển thị mật khẩu"}
                                >
                                {showConfirmPassword ? <FaEyeSlash /> : <FaEye />}
                                </InputGroup.Text>
                            </InputGroup>
                            {passwordError && (
                                <Form.Control.Feedback type="invalid">
                                {passwordError}
                                </Form.Control.Feedback>
                            )}
                            </Form.Group>
                        </>
                        )}

                        <div className="d-flex gap-2">
                            {step > 1 && (
                                <Button 
                                    variant="outline-secondary" 
                                    onClick={handleBack}
                                    className="d-flex align-items-center gap-2"
                                >
                                    <FaArrowLeft /> Quay lại
                                </Button>
                            )}
                            <Button 
                                variant="danger" 
                                type="submit" 
                                className="flex-grow-1 d-flex align-items-center justify-content-center gap-2"
                                disabled={step === 3 && !!passwordError || loading}
                            >
                                {loading ? (
                                    <Lottie 
                                        animationData={loadingAnimation} 
                                        style={{ width: 30, height: 30 }} 
                                    />
                                ) : (
                                    <>
                                        {step === 1 ? 'Gửi OTP' : step === 2 ? 'Xác nhận OTP' : 'Đổi mật khẩu'}
                                    </>
                                )}
                            </Button>
                        </div>
                    </Form>
                </div>
            </Container>
        </div>
    );
};

export default ForgotPassword; 