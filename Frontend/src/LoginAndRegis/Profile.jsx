import React, { useState, useEffect, useContext } from 'react';
import axios from 'axios';
import { Container, Row, Col, Card, Form, Spinner, Alert, Button, Image } from 'react-bootstrap';
import { FaUser, FaEnvelope, FaPhone, FaBirthdayCake, FaArrowLeft, FaCamera, FaEye, FaEyeSlash } from 'react-icons/fa';
import { DataContext } from '../ContextAPI/ContextNavbar';
import Navbar from '../Dashboard/Navbar';
import Footer from '../Dashboard/Footer';
import 'bootstrap/dist/css/bootstrap.min.css';
import Cookies from 'js-cookie';
import { ChangeInfor, ProfileInfor, SendOTP } from '../apis/authAPI';
import { Link } from 'react-router-dom';
import avatar_default from '../assets/images/avatar_default.png';
import { UploadAvatar } from '../apis/imageAPI';
import { toast } from 'react-toastify';

const Profile = () => {
    const { categories, movieTypes, nations, statuses, statusMap } = useContext(DataContext);
    const [userInfo, setUserInfo] = useState({
        userName: '',
        nickname: '',
        email: '',
        phoneNumber: '',
        age: '',
        image: ''
    });
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [isEditing, setIsEditing] = useState(false);
    const [formData, setFormData] = useState({
        nickname: '',
        password: '',
        phoneNumber: '',
        email: '',
        otp: '',
        verifyCode: ''
    });
    const [imagePreview, setImagePreview] = useState('');
    const [imageFile, setImageFile] = useState(null);
    const [showPassword, setShowPassword] = useState(false);
    const [passwordError, setPasswordError] = useState('');
    const [phoneError, setPhoneError] = useState('');
    const [emailError, setEmailError] = useState('');
    const [otpSent, setOtpSent] = useState(false);
    const [countdown, setCountdown] = useState(0);

    useEffect(() => {
        const fetchUserInfo = async () => {
            try {
                const username = Cookies.get('username');
                const payload = { userName: username };
                const response = await ProfileInfor(payload);

                setUserInfo(response.data);
                setFormData({
                    nickname: response.data.nickname || '',
                    password: '',
                    phoneNumber: response.data.phoneNumber || '',
                    email: response.data.email || '',
                    otp: '',
                    verifyCode: ''
                });
                setImagePreview(response.data.image || '');
                setLoading(false);
            } catch (error) {
                console.error('Error fetching user info:', error);
                setError('Không thể tải thông tin người dùng. Vui lòng thử lại sau.');
                setLoading(false);
            }
        };

        fetchUserInfo();
    }, []);

    useEffect(() => {
        let timer;
        if (countdown > 0) {
            timer = setInterval(() => {
                setCountdown((prev) => prev - 1);
            }, 1000);
        } else if (countdown === 0 && otpSent) {
            setOtpSent(false);
        }
        return () => clearInterval(timer);
    }, [countdown, otpSent]);

    const validatePassword = (password) => {
        if (!password) return '';
        const minLength = 8;
        const hasUpperCase = /[A-Z]/.test(password);
        const hasLowerCase = /[a-z]/.test(password);
        const hasNumber = /[0-9]/.test(password);
        const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(password);

        if (password.length < minLength) {
            return `Mật khẩu phải có ít nhất ${minLength} ký tự`;
        }
        if (!hasUpperCase) {
            return 'Mật khẩu phải chứa ít nhất 1 chữ hoa';
        }
        if (!hasLowerCase) {
            return 'Mật khẩu phải chứa ít nhất 1 chữ thường';
        }
        if (!hasNumber) {
            return 'Mật khẩu phải chứa ít nhất 1 số';
        }
        if (!hasSpecialChar) {
            return 'Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt';
        }
        return '';
    };

    const validatePhoneNumber = (phone) => {
        const phoneRegex = /^\+?[0-9]{10,15}$/;
        if (!phone) {
            return '';
        }
        if (!phoneRegex.test(phone)) {
            return 'Số điện thoại không hợp lệ (10-15 số, chỉ chứa số và có thể bắt đầu bằng +)';
        }
        return '';
    };

    const validateEmail = (email) => {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!email) {
            return 'Vui lòng nhập email';
        }
        if (!emailRegex.test(email)) {
            return 'Email không hợp lệ';
        }
        return '';
    };

    const handleEditToggle = () => {
        setIsEditing(!isEditing);
        if (!isEditing) {
            setFormData({
                nickname: userInfo.nickname || '',
                password: '',
                phoneNumber: userInfo.phoneNumber || '',
                email: '', // Xóa giá trị Email hiện tại khi bấm chỉnh sửa
                otp: '',
                verifyCode: ''
            });
            setImagePreview(userInfo.image || '');
            setImageFile(null);
            setPasswordError('');
            setPhoneError('');
            setEmailError('');
            setOtpSent(false);
            setCountdown(0);
        }
    };

    const handleCancel = () => {
        setIsEditing(false);
        setImageFile(null);
        setImagePreview(userInfo.image || '');
        setFormData({
            nickname: userInfo.nickname || '',
            password: '',
            phoneNumber: userInfo.phoneNumber || '',
            email: userInfo.email || '',
            otp: '',
            verifyCode: ''
        });
        setPasswordError('');
        setPhoneError('');
        setEmailError('');
        setOtpSent(false);
        setCountdown(0);
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        if (name === 'phoneNumber') {
            const filteredValue = value.replace(/[^0-9+]/g, '');
            setFormData({ ...formData, [name]: filteredValue });
            setPhoneError(validatePhoneNumber(filteredValue));
        } else if (name === 'email') {
            setFormData({ ...formData, [name]: value });
            setEmailError(validateEmail(value));
        } else {
            setFormData({ ...formData, [name]: value });
            if (name === 'password') {
                setPasswordError(validatePassword(value));
                if (!value) {
                    setOtpSent(false);
                    setCountdown(0);
                }
            }
        }
    };

    const handleKeyPress = (e) => {
        if (e.target.name === 'phoneNumber') {
            if (!/[0-9+]/.test(e.key)) {
                e.preventDefault();
            }
        }
    };

    const handleImageChange = (event) => {
        const file = event.target.files[0];
        if (file) {
            setImageFile(file);
            setImagePreview(URL.createObjectURL(file));
        }
    };

    const togglePasswordVisibility = () => {
        setShowPassword(!showPassword);
    };

    const handleSendOTP = async () => {
        if (passwordError || !formData.password) {
            setError('Vui lòng nhập mật khẩu hợp lệ trước khi gửi OTP');
            return;
        }
        try {
            await SendOTP(userInfo.userName, 2);
            setOtpSent(true);
            setCountdown(60);
            toast.success(`Chúng tôi đã gửi mã OTP đến Email: ${userInfo.email || 'Không xác định'} xin mời bạn kiểm tra`);
            setError('');
        } catch (error) {
            setError('Không thể gửi OTP. Vui lòng thử lại.');
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (formData.phoneNumber && phoneError) {
                setError('Vui lòng nhập số điện thoại hợp lệ');
                return;
            }
            if (formData.email && emailError) {
                setError('Vui lòng nhập email hợp lệ');
                return;
            }
            if (formData.password && (passwordError || !otpSent || !formData.otp)) {
                setError('Vui lòng nhập mật khẩu hợp lệ, gửi OTP và nhập mã OTP');
                return;
            }
            if (formData.email && !emailError && !formData.verifyCode) {
                setError('Vui lòng nhập mã xác thực Email');
                return;
            }

            let updatedImage = null;
            if (imageFile) {
                const imageFormData = new FormData();
                imageFormData.append('File', imageFile);
                const response = await UploadAvatar(imageFormData);
                if (response.status === 200) {
                    updatedImage = response.data.fileUrl;
                }
            }

            const payload = {
                nickName: formData.nickname,
                password: formData.password || "",
                phoneNumber: formData.phoneNumber,
                email: formData.email,
                image: updatedImage ?? userInfo.image,
                otp: formData.password ? formData.otp : undefined,
                verifyCode: formData.email ? formData.verifyCode : undefined
            };

            const response = await ChangeInfor(payload);

            setUserInfo({
                ...userInfo,
                nickname: formData.nickname,
                phoneNumber: formData.phoneNumber,
                email: formData.email,
                image: updatedImage ?? userInfo.image
            });
            setImagePreview(updatedImage ?? userInfo.image);
            setIsEditing(false);
            setError('');
            toast.success("Sửa thông tin thành công");
        } catch (error) {
            console.error('Error updating profile:', error);
            toast.error(error.response.data.message);
        }
    };

    if (loading) {
        return (
            <Container className="d-flex justify-content-center align-items-center" style={{ minHeight: '80vh' }}>
                <Spinner animation="border" role="status" variant="primary">
                    <span className="visually-hidden">Đang tải...</span>
                </Spinner>
            </Container>
        );
    }

    if (error) {
        return (
            <Container className="py-5">
                <Alert variant="danger">{error}</Alert>
            </Container>
        );
    }

    return (
        <div style={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
            <Navbar categories={categories} movieTypes={movieTypes} nations={nations} statuses={statuses} statusMap={statusMap} />
            <Container className="py-5 flex-grow-1" style={{ maxWidth: '960px' }}>
                <Row className="justify-content-center mb-3">
                    <Col md={10} lg={8}>
                        <div className="mb-3">
                            <Link to="/" className="btn btn-outline-primary d-flex align-items-center">
                                <FaArrowLeft className="me-2" />
                                Quay lại trang chủ
                            </Link>
                        </div>
                    </Col>
                </Row>
                <Row className="justify-content-center">
                    <Col md={10} lg={8}>
                        <Card className="shadow">
                            <Card.Header className="bg-primary text-white text-center py-3">
                                <h3 className="mb-0">Thông tin tài khoản</h3>
                            </Card.Header>
                            <Card.Body className="p-4">
                                <Row>
                                    <Col xs={12} md={4} className="text-center mb-4 mb-md-0 d-flex flex-column align-items-center">
                                        <Image
                                            src={imagePreview || avatar_default}
                                            roundedCircle
                                            style={{ 
                                                width: '150px', 
                                                height: '150px', 
                                                objectFit: 'cover',
                                                border: '2px solid #dee2e6'
                                            }}
                                            alt="Avatar"
                                        />
                                        {isEditing && (
                                            <Form.Group className="mt-3">
                                                <Form.Label
                                                    htmlFor="imageUpload"
                                                    className="btn btn-outline-secondary d-flex align-items-center justify-content-center"
                                                    style={{ cursor: 'pointer' }}
                                                >
                                                    <FaCamera className="me-2" />
                                                    Chọn ảnh
                                                </Form.Label>
                                                <Form.Control
                                                    type="file"
                                                    id="imageUpload"
                                                    accept="image/jpeg,image/png,image/gif"
                                                    onChange={handleImageChange}
                                                    style={{ display: 'none' }}
                                                />
                                            </Form.Group>
                                        )}
                                    </Col>
                                    <Col xs={12} md={8} className="ps-md-4">
                                        <Form onSubmit={handleSubmit}>
                                            <Form.Group className="mb-4">
                                                <Form.Label className="d-flex align-items-center">
                                                    <FaUser className="me-2" />
                                                    Tài khoản
                                                </Form.Label>
                                                <Form.Control
                                                    type="text"
                                                    value={userInfo.userName || ''}
                                                    readOnly
                                                    className="bg-light"
                                                />
                                            </Form.Group>

                                            <Form.Group className="mb-4">
                                                <Form.Label className="d-flex align-items-center">
                                                    <FaUser className="me-2" />
                                                    Tên người dùng
                                                </Form.Label>
                                                <Form.Control
                                                    type="text"
                                                    name="nickname"
                                                    value={isEditing ? formData.nickname : userInfo.nickname || ''}
                                                    onChange={handleInputChange}
                                                    readOnly={!isEditing}
                                                    className="bg-light"
                                                />
                                            </Form.Group>

                                            {isEditing && (
                                                <Form.Group className="mb-4">
                                                    <Form.Label className="d-flex align-items-center">
                                                        <FaUser className="me-2" />
                                                        Mật khẩu
                                                    </Form.Label>
                                                    <div className="input-group">
                                                        <Form.Control
                                                            type={showPassword ? "text" : "password"}
                                                            name="password"
                                                            value={formData.password}
                                                            onChange={handleInputChange}    
                                                            placeholder="Nhập mật khẩu mới (nếu muốn thay đổi)"
                                                            className={`bg-light ${passwordError ? 'is-invalid' : ''}`}
                                                            autoComplete="new-password"
                                                            style={{ fontSize: '0.8rem' }}
                                                        />
                                                        <Button
                                                            variant="outline-secondary"
                                                            onClick={togglePasswordVisibility}
                                                            className="border-start-0"
                                                        >
                                                            {showPassword ? <FaEyeSlash /> : <FaEye />}
                                                        </Button>
                                                        {passwordError && (
                                                            <div className="invalid-feedback">{passwordError}</div>
                                                        )}
                                                    </div>
                                                    <Form.Text className="text-muted">
                                                        Mật khẩu ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt
                                                    </Form.Text>
                                                </Form.Group>
                                            )}

                                            {isEditing && formData.password && !passwordError && (
                                                <Form.Group className="mb-4">
                                                    <Form.Label className="d-flex align-items-center">
                                                        <FaUser className="me-2" />
                                                        Mã OTP
                                                    </Form.Label>
                                                    <div className="d-flex align-items-center">
                                                        <Form.Control
                                                            type="text"
                                                            name="otp"
                                                            value={formData.otp}
                                                            onChange={handleInputChange}
                                                            placeholder="Nhập mã OTP"
                                                            className="bg-light me-2"
                                                            style={{ flex: 1 }}
                                                        />
                                                        <Button
                                                            variant="secondary"
                                                            onClick={handleSendOTP}
                                                            disabled={otpSent && countdown > 0}
                                                        >
                                                            {otpSent && countdown > 0 ? `Gửi lại (${countdown}s)` : 'Gửi mã'}
                                                        </Button>
                                                    </div>
                                                </Form.Group>
                                            )}

                                            <Form.Group className="mb-4">
                                                <Form.Label className="d-flex align-items-center">
                                                    <FaEnvelope className="me-2" />
                                                    Email
                                                </Form.Label>
                                                <Form.Control
                                                    type="email"
                                                    name="email"
                                                    placeholder="Nhập Email mới (nếu muốn thay đổi)"
                                                    value={isEditing ? formData.email : userInfo.email || ''}
                                                    onChange={handleInputChange}
                                                    readOnly={!isEditing}
                                                    className={`bg-light ${emailError ? 'is-invalid' : ''}`}
                                                    
                                                />
                                                {emailError && (
                                                    <div className="invalid-feedback">{emailError}</div>
                                                )}
                                            </Form.Group>

                                            {isEditing && formData.email && !emailError && (
                                                <Form.Group className="mb-4">
                                                    <Form.Label className="d-flex align-items-center">
                                                        <FaEnvelope className="me-2" />
                                                        Mã xác thực Email
                                                    </Form.Label>
                                                    <Form.Control
                                                        type="text"
                                                        name="verifyCode"
                                                        value={formData.verifyCode}
                                                        onChange={handleInputChange}
                                                        placeholder="Nhập mã xác thực Email"
                                                        className="bg-light"
                                                    />
                                                </Form.Group>
                                            )}

                                            <Form.Group className="mb-4">
                                                <Form.Label className="d-flex align-items-center">
                                                    <FaPhone className="me-2" />
                                                    Số điện thoại
                                                </Form.Label>
                                                <Form.Control
                                                    type="text"
                                                    name="phoneNumber"
                                                    value={isEditing ? formData.phoneNumber : userInfo.phoneNumber || ''}
                                                    onChange={handleInputChange}
                                                    onKeyPress={handleKeyPress}
                                                    readOnly={!isEditing}
                                                    className={`bg-light ${phoneError ? 'is-invalid' : ''}`}
                                                />
                                                {phoneError && (
                                                    <div className="invalid-feedback">{phoneError}</div>
                                                )}
                                            </Form.Group>

                                            <Form.Group className="mb-4">
                                                <Form.Label className="d-flex align-items-center">
                                                    <FaBirthdayCake className="me-2" />
                                                    Tuổi
                                                </Form.Label>
                                                <Form.Control
                                                    type="number"
                                                    value={userInfo.age || ''}
                                                    readOnly
                                                    className="bg-light"
                                                />
                                            </Form.Group>

                                            <div className="text-center">
                                                <Button
                                                    variant={isEditing ? "success" : "primary"}
                                                    onClick={isEditing ? handleSubmit : handleEditToggle}
                                                    className="me-2"
                                                    disabled={isEditing && (
                                                        (formData.password && (!otpSent || !formData.otp || passwordError)) ||
                                                        phoneError ||
                                                        (formData.email && emailError) ||
                                                        (formData.email && !emailError && !formData.verifyCode)
                                                    )}
                                                >
                                                    {isEditing ? "OK" : "Chỉnh sửa"}
                                                </Button>
                                                {isEditing && (
                                                    <Button
                                                        variant="danger"
                                                        onClick={handleCancel}
                                                    >
                                                        Hủy
                                                    </Button>
                                                )}
                                            </div>
                                        </Form>
                                    </Col>
                                </Row>
                            </Card.Body>
                            <Card.Footer className="text-center py-3 bg-light">
                            </Card.Footer>
                        </Card>
                    </Col>
                </Row>
            </Container>
            <Footer />
        </div>
    );
};

export default Profile;