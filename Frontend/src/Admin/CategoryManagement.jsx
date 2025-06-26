import React, { useState, useEffect, useContext } from 'react';
import { Container, Table, Button, Modal, Form, Row, Col, Pagination } from 'react-bootstrap';
import { FaEdit, FaTrash, FaPlus, FaArrowLeft, FaFilm, FaList, FaUsers, FaBars, FaTimes, FaAngleLeft, FaAngleRight, FaCheck, FaTimes as FaX } from 'react-icons/fa';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import axios from 'axios';
import { DataContext } from '../ContextAPI/ContextNavbar';
import Navbar from '../Dashboard/Navbar';
import Footer from '../Dashboard/Footer';
import './AdminStyles.css';
import { toast } from 'react-toastify';
import { slidebarMenus } from './slidebar';
import { GetAll, AddCategory, DeleteCategory, UpdateCategory } from '../apis/categoryAPI';

const CategoryManagement = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const { categories, movieTypes, nations, statuses, statusMap } = useContext(DataContext);
    const [categoryList, setCategoryList] = useState([]);
    const [showModal, setShowModal] = useState(false);
    const [selectedCategory, setSelectedCategory] = useState(null);
    const [isSidebarOpen, setIsSidebarOpen] = useState(true);
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage] = useState(10);

    const [formData, setFormData] = useState({
        idCategories: '',
        nameCategories: '',
        slugNameCategories: ''
    });

    // Thêm định nghĩa sidebarMenus

    useEffect(() => {
        fetchCategories();
    }, []);

    const fetchCategories = async () => {
        try {
            // const response = await axios.get('http://localhost:5285/api/category/getall');
            const response = await GetAll();
            setCategoryList(response.data);
        } catch (error) {
            console.error('Error fetching categories:', error);
            toast.error('Không thể tải danh sách thể loại');
        }
    };

    const handleShowModal = (category = null) => {
        if (category) {
            setSelectedCategory(category);
            setFormData({
                idCategories: category.idCategories,
                nameCategories: category.nameCategories,
                slugNameCategories: category.slugNameCategories
            });
        } else {
            setSelectedCategory(null);
            setFormData({
                idCategories: '',
                nameCategories: '',
                slugNameCategories: ''
            });
        }
        setShowModal(true);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (selectedCategory) {
                // Cập nhật thể loại
                // await axios.put('http://localhost:5285/api/category/update', formData, {
                //     headers: {
                //         Authorization: `Bearer ${token}`
                //     }
                // });
                await UpdateCategory(formData);
                toast.success('Cập nhật thể loại thành công!');
            } else {
                // Thêm thể loại mới
                // await axios.post('http://localhost:5285/api/category/addCategory', formData, {
                //     headers: {
                //         Authorization: `Bearer ${token}`
                //     }
                // });
                await AddCategory(formData);
                toast.success('Thêm thể loại mới thành công!');
            }
            fetchCategories();
            setShowModal(false);
        } catch (error) {
            console.error('Error saving category:', error);
            toast.error(selectedCategory ? 'Cập nhật thể loại thất bại!' : 'Thêm thể loại mới thất bại!');
        }
    };

    const handleDelete = async (idCategories) => {
        if (window.confirm('Bạn có chắc muốn xóa thể loại này?')) {
            try {
                
                // await axios.delete(`http://localhost:5285/api/category/deleteCategory/${idCategories}`, {
                //     headers: {
                //         Authorization: `Bearer ${token}`
                //     }
                // });
                await DeleteCategory(idCategories);
                fetchCategories();
                toast.success('Xóa thể loại thành công!');
            } catch (error) {
                console.error('Error deleting category:', error);
                toast.error('Xóa thể loại thất bại!');
            }
        }
    };

    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;
    const currentItems = categoryList.slice(indexOfFirstItem, indexOfLastItem);
    const totalPages = Math.ceil(categoryList.length / itemsPerPage);

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
                {/* Sidebar */}
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

                {/* Fixed toggle button */}
                <Button
                    variant="dark"
                    className="sidebar-toggle-fixed"
                    onClick={() => setIsSidebarOpen(!isSidebarOpen)}
                >
                    {isSidebarOpen ? <FaAngleLeft /> : <FaAngleRight />}
                </Button>

                {/* Content area */}
                <div className={`admin-content ${isSidebarOpen ? '' : 'expanded'}`}>
                    <Container fluid>
                        <div className="content-header d-flex justify-content-between align-items-center mb-4">
                            <h2>Quản lý Thể loại</h2>
                            <Button variant="primary" onClick={() => handleShowModal()}>
                                <FaPlus className="me-2" />
                                Thêm Thể loại Mới
                            </Button>
                        </div>

                        <Table striped bordered hover responsive>
                            <thead>
                                <tr>                                    
                                    <th>Tên thể loại</th>                                    
                                    <th style={{ width: '150px' }}>Thao tác</th>
                                </tr>
                            </thead>
                            <tbody>
                                {currentItems.map(category => (
                                    <tr key={category.idCategories}>
                                        
                                        <td>{category.nameCategories}</td>
                                        
                                        <td>
                                            <Button 
                                                variant="warning" 
                                                size="sm" 
                                                className="me-2"
                                                onClick={() => handleShowModal(category)}
                                            >
                                                <FaEdit />
                                            </Button>
                                            <Button 
                                                variant="danger" 
                                                size="sm"
                                                onClick={() => handleDelete(category.idCategories)}
                                            >
                                                <FaTrash />
                                            </Button>
                                        </td>
                                    </tr>
                                ))}
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

                        <Modal show={showModal} onHide={() => setShowModal(false)}>
                            <Modal.Header closeButton>
                                <Modal.Title>
                                    {selectedCategory ? 'Chỉnh sửa Thể loại' : 'Thêm Thể loại Mới'}
                                </Modal.Title>
                            </Modal.Header>
                            <Modal.Body>
                                <Form onSubmit={handleSubmit}>
                                    <Form.Group className="mb-3">
                                        <Form.Label>Tên thể loại</Form.Label>
                                        <Form.Control
                                            type="text"
                                            value={formData.nameCategories}
                                            onChange={(e) => setFormData({...formData, nameCategories: e.target.value})}
                                            required
                                        />
                                    </Form.Group>
                                    <Form.Group className="mb-3">
                                        <Form.Label>Slug</Form.Label>
                                        <Form.Control
                                            type="text"
                                            value={formData.slugNameCategories}
                                            onChange={(e) => setFormData({...formData, slugNameCategories: e.target.value})}
                                            required
                                        />
                                    </Form.Group>
                                    <div className="text-end">
                                        <Button variant="secondary" className="me-2" onClick={() => setShowModal(false)}>
                                            Hủy
                                        </Button>
                                        <Button variant="primary" type="submit">
                                            {selectedCategory ? 'Cập nhật' : 'Thêm mới'}
                                        </Button>
                                    </div>
                                </Form>
                            </Modal.Body>
                        </Modal>
                    </Container>
                </div>
            </div>
           
        </div>
    );
};

export default CategoryManagement; 