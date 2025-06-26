import React, { useState, useEffect, useContext } from 'react';
import { Container, Table, Button, Modal, Form, Row, Col, Pagination, Image } from 'react-bootstrap';
import { FaEdit, FaTrash, FaPlus, FaArrowLeft, FaCheck, FaTimes as FaX, FaAngleRight, FaAngleLeft, FaEye, FaEyeSlash} from 'react-icons/fa';
import { useNavigate, Link } from 'react-router-dom';
import { DataContext } from '../ContextAPI/ContextNavbar';
import Navbar from '../Dashboard/Navbar';
import Footer from '../Dashboard/Footer';
import './AdminStyles.css';
import { toast } from 'react-toastify';
import { GetUser, DeleteUser, ChangeInfor } from '../apis/authAPI';
import { slidebarMenus } from './slidebar';
import avatar_default from '../assets/images/avatar_default.png'
import DatePicker from 'react-datepicker';

const AccountManagement = () => {
    const navigate = useNavigate();
    const { categories, movieTypes, nations, statuses, statusMap } = useContext(DataContext);
    const [users, setUsers] = useState([]);
    const [errors, setErrors] = useState({});
    const [showModal, setShowModal] = useState(false);
    const [selectedUser, setSelectedUser] = useState(null);
    const [isSidebarOpen, setIsSidebarOpen] = useState(true);
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage] = useState(10);
    const [searchTerm, setSearchTerm] = useState("");
    const [filteredUsers, setFilteredUsers] = useState([]);
    const [showPassword, setShowPassword] = useState(false);
    const [searchType, setSearchType] = useState("Tất cả");
    const [formData, setFormData] = useState({
        userName: '',
        nickName: '',
        password: '',
        email: '',
        phoneNumber: '',
        isVip: false,
        birthday: ''
    });

    useEffect(() => {
        fetchUsers();
    }, []);

    useEffect(() => {
        const debounceSearch = setTimeout(() => {
            if (searchTerm.trim() === "") {
                setFilteredUsers(users);
                setCurrentPage(1);
                return;
            }

            // const filtered = users.filter(user => 
            //     user.userName.toLowerCase().includes(searchTerm.toLowerCase()) ||
            //     user.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
            //     user.phoneNumber.includes(searchTerm)
            // );
            const filtered = users.filter((user) => {
                const searchValue = searchTerm.toLowerCase();
                switch(searchType) {
                    case "Tài khoản":
                        return (user.userName).normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().includes(searchValue);
                    case "Tên người dùng":
                        return (user.nickname ? user.nickname:user.userName).normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().includes(searchValue);
                    case "Email" :
                        return (user.email).normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().includes(searchValue);
                    case "Số điện thoại" :
                        return (user.phoneNumber).normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().includes(searchValue);
                    case "Tất cả" :
                        return (user.nickname ? user.nickname:user.userName).normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().includes(searchValue) ||
                        (user.email).normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().includes(searchValue)||(user.phoneNumber).normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().includes(searchValue);
                    default:
                        return true;
                }
                 // Trường hợp mặc định (không lọc)
            });
            setFilteredUsers(filtered);
            setCurrentPage(1);
        }, 300);

        return () => clearTimeout(debounceSearch);
    }, [searchTerm,searchType, users]);

    const fetchUsers = async () => {
        try {
            const response = await GetUser();
            setUsers(response.data);
            setFilteredUsers(response.data);
        } catch (error) {
            console.error('Error fetching users:', error);
            toast.error('Lỗi khi tải danh sách người dùng');
        }
    };
    const togglePasswordVisibility = () => {
        setShowPassword(!showPassword);
    };

    const handleShowModal = (user = null) => {
        if (user) {
            setSelectedUser(user);
            setFormData({
                userName: user.userName,
                nickName : user.nickname ? user.nickname : user.userName,
                email: user.email,
                password: '',
                birthday: '',
                phoneNumber: user.phoneNumber,
                isVip: user.isVip,
                
            });
        } else {
            setSelectedUser(null);
            setFormData({
                userName: '',
                nickName: '',
                email: '',
                birthday: '',
                phoneNumber: '',
                isVip: false,
                
            });
        }
        setShowModal(true);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try { 
            const editData = { ...formData
             };
            delete editData.birthday;
            delete editData.nickName; 
            console.log(editData);                
            // const response = await fetch('http://localhost:5285/api/User/ChangeInfor', {
            //     method: 'PUT',
            //     headers: {
            //         'Content-Type': 'application/json',
            //     },
            //     body: JSON.stringify(formData)
            // });
            const response = await ChangeInfor(editData);

            if (response.status === 200) {
                toast.success('Cập nhật thông tin admin thành công');
                setShowModal(false);
                fetchUsers();
            } else {
                toast.error('Lỗi khi cập nhật thông tin người dùng');
            }
        } catch (error) {
            console.error('Error updating user:', error);
            toast.error('Lỗi khi cập nhật thông tin người dùng');
        }
    };

    const handleDelete = async (userName) => {
        if (window.confirm('Bạn có chắc muốn xóa người dùng này?')) {
            try {
                const response = await fetch(`http://localhost:5285/api/User/DeleteUser/${userName}`, {
                    method: 'DELETE'
                });

                if (response.ok) {
                    toast.success('Xóa người dùng thành công');
                    fetchUsers();
                } else {
                    toast.error('Lỗi khi xóa người dùng');
                }
            } catch (error) {
                console.error('Error deleting user:', error);
                toast.error('Lỗi khi xóa người dùng');
            }
        }
    };

    const handleRegisterAdmin = async () => {
        try {
            const response = await fetch('http://localhost:5285/api/User/RegisterAdmin', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(formData)
            });

            if (response.ok) {
                toast.success('Đăng ký tài khoản admin thành công');
                setShowModal(false);
                fetchUsers();
            } else {
                toast.error('Lỗi khi đăng ký tài khoản admin');
            }
        } catch (error) {
            console.error('Error registering admin:', error);
            toast.error('Lỗi khi đăng ký tài khoản admin');
        }
    };

    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;
    const currentItems = filteredUsers.slice(indexOfFirstItem, indexOfLastItem);
    const totalPages = Math.ceil(filteredUsers.length / itemsPerPage);

    const handlePageChange = (pageNumber) => {
        setCurrentPage(pageNumber);
    };

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

    return (
        <div>
            <Navbar 
                categories={categories} 
                movieTypes={movieTypes} 
                nations={nations} 
                statuses={statuses} 
                statusMap={statusMap} 
            />
            <div className="admin-layout">
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
                        <div className="d-flex justify-content-between align-items-center mb-4">
                            <h2>Quản lý tài khoản</h2>
                            
                            <Button 
                                variant="primary" 
                                onClick={() => handleShowModal()}
                            >
                                <FaPlus className="me-2" />
                                Thêm tài khoản Admin
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
                                <option value="Tài khoản">Tài khoản</option>
                                <option value="Tên người dùng">Tên người dùng</option>
                                <option value="Email">Email</option>
                                <option value="Số điện thoại">Số điện thoại</option>
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

                        

                        <div style={{ 
                            overflowX: 'auto', 
                            // width: '100%',
                            maxWidth: '1450px',
                            // width:'calc(2200px - 1000px)'
                            
                        }}
                        className={isSidebarOpen ? 'change_size_table' : ''}
                        >
                        <Table striped bordered hover responsive style={{ 
                                // width: 'max-content',
                                width: '2200px',
                                // minWidth: '2200px'
                            }}>
                                <thead>
                                    <tr>
                                        <th>Avatar</th>
                                        <th>Tài khoản</th>
                                        <th>Tên người dùng</th>
                                        <th>Email</th>
                                        <th>Số điện thoại</th>
                                        <th>VIP</th>
                                        <th>Thao tác</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {currentItems.map((user) => (
                                        <tr key={user.userName}>
                                            <td> <Image
                                            src={user.image || avatar_default}
                                            alt={user.nickname || user.userName}
                                            roundedCircle
                                            style={{
                                                width: '40px',
                                                height: '40px',
                                                objectFit: 'cover',
                                                border: '1px solid #dee2e6'
                                            }}
                                                />
                                           </td>
                                            <td>{user.userName}</td>
                                            <td>{user.nickname ? user.nickname : user.userName}</td>                                            
                                            <td>{user.email}</td>
                                            <td>{user.phoneNumber}</td>
                                            <td>
                                                {user.isVip ? (
                                                    <FaCheck style={{ color: 'green', fontSize: '20px' }} />
                                                ) : (
                                                    <FaX style={{ color: 'red', fontSize: '20px' }} />
                                                )}
                                            </td>
                                            <td>
                                                <Button 
                                                    variant="warning" 
                                                    size="sm" 
                                                    className="me-2"
                                                    onClick={() => handleShowModal(user)}
                                                >
                                                    <FaEdit />
                                                </Button>
                                                <Button 
                                                    variant="danger" 
                                                    size="sm"
                                                    onClick={() => handleDelete(user.userName)}
                                                >
                                                    <FaTrash />
                                                </Button>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </Table>
                        </div>

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
                    </Container>
                </div>
            </div>

            <Modal show={showModal} onHide={() => setShowModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>
                        {selectedUser ? 'Chỉnh sửa thông tin người dùng' : 'Thêm tài khoản Admin'}
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form onSubmit={selectedUser ? handleSubmit : handleRegisterAdmin}>
                        <Form.Group className="mb-3">
                            <Form.Label>Tên người dùng</Form.Label>
                            <Form.Control
                                type="text"
                                value={formData.userName}
                                onChange={(e) => setFormData({...formData, userName: e.target.value})}
                                required
                            />
                        </Form.Group>

                        <Form.Group className="mb-3">
                            <Form.Label>Email</Form.Label>
                            <Form.Control
                                type="email"
                                value={formData.email}
                                onChange={(e) => setFormData({...formData, email: e.target.value})}
                                required
                            />
                        </Form.Group>

                        <Form.Group className="mb-3">
                            <Form.Label>Số điện thoại</Form.Label>
                            <Form.Control
                                type="text"
                                value={formData.phoneNumber}
                                onChange={(e) => setFormData({...formData, phoneNumber: e.target.value})}
                                required
                            />
                        </Form.Group>

                        {selectedUser && (
                            <Form.Group className="mb-3">
                                <Form.Check
                                    type="checkbox"
                                    label="Tài khoản VIP"
                                    checked={formData.isVip}
                                    onChange={(e) => setFormData({...formData, isVip: e.target.checked})}
                                />
                            </Form.Group>
                        )}

                        {!selectedUser && (
                            <Form.Group className="mb-3">
                                <Form.Label>Mật khẩu</Form.Label>
                                <div style={{ position: 'relative' }}>
                                    <Form.Control
                                        type={showPassword ? "text" : "password"}
                                        value={formData.password}
                                        placeholder="Nhập mật khẩu mới"
                                        onChange={(e) => setFormData({...formData, password: e.target.value})}
                                        required
                                    />
                                    <span
                                        style={{
                                            position: 'absolute',
                                            top: '50%',
                                            right: '10px',
                                            transform: 'translateY(-50%)',
                                            cursor: 'pointer'
                                        }}
                                        onClick={togglePasswordVisibility}
                                    >
                                        {showPassword ? <FaEyeSlash /> : <FaEye />}
                                    </span>
                                </div>
                            </Form.Group>
                            
                        )}
                        {!selectedUser && (
                        <Form.Group className="mb-3">
                            <div className="mb-3 row align-items-center">
                            <Form.Label>Mật khẩu</Form.Label>
                                <div className="col-sm-9">
                                    <DatePicker
                                    selected={formData.birthday}
                                    value={formData.birthday}
                                    onChange={(date) => setFormData({...formData, birthday: date})}
                                    dateFormat="yyyy-MM-dd"
                                    className={`form-control ${errors.birthday ? 'is-invalid' : ''}`}
                                    showYearDropdown
                                    scrollableYearDropdown
                                    yearDropdownItemNumber={100}
                                    maxDate={new Date()}
                                    placeholderText="Chọn ngày sinh"
                                    />
                                    
                                </div>
                            </div>
                        </Form.Group>
                        )}


                        <div className="d-flex justify-content-end">
                            <Button variant="secondary" className="me-2" onClick={() => setShowModal(false)}>
                                Hủy
                            </Button>
                            <Button variant="primary" type="submit">
                                {selectedUser ? 'Cập nhật' : 'Đăng ký'}
                            </Button>
                        </div>
                    </Form>
                </Modal.Body>
            </Modal>

            
        </div>
    );
};

export default AccountManagement; 