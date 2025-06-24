import React, { useState, useEffect, useContext } from 'react';
import { slidebarMenus } from './slidebar';
import { toast } from 'react-toastify';
import 'bootstrap/dist/css/bootstrap.min.css';
import { Container, Table, Button, Modal, Form, Row, Col, Pagination, Card } from 'react-bootstrap';
import { FaEdit, FaTrash, FaPlus, FaArrowLeft, FaFilm, FaList, FaUsers, FaBars, FaTimes, FaAngleLeft, FaAngleRight, FaCheck, FaTimes as FaX } from 'react-icons/fa';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { DataContext } from '../ContextAPI/ContextNavbar';
import Navbar from '../Dashboard/Navbar';
import ReportCommentManagement from './ReportCommentManagement';
import ReportMovieManagement from './ReportMovieManagement';
import ReportSystemManagement from './ReportSystemManagement';

const ReportManagement = () => {
    const { categories, movieTypes, nations, statuses, statusMap } = useContext(DataContext);
    const [isSidebarOpen, setIsSidebarOpen] = useState(true);
    const [searchType, setSearchType] = useState("Tất cả");
    const [searchTerm, setSearchTerm] = useState("");
    const [typeReport, setTypeReport] = useState("Hệ thống");
    const navigate = useNavigate();
    const location = useLocation();

    const RenderTypeReport = (type) => {
        switch (type) {
            case 'Hệ thống':
                return <ReportSystemManagement searchType={searchType} searchTerm={searchTerm.trim()}/>;
            case 'Bình luận':
                return <ReportCommentManagement searchType={searchType} searchTerm={searchTerm.trim()} />
            case 'Phim':
                return <ReportMovieManagement searchType={searchType} searchTerm={searchTerm.trim()} />
            default:
                return <ReportSystemManagement searchType={searchType} searchTerm={searchTerm.trim()} />;
        }     
    }

    return (
        
        <div>
            <Navbar 
                categories={categories} 
                movieTypes={movieTypes} 
                nations={nations} 
                statuses={statuses} 
                statusMap={statusMap} 
            />
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
                    <h2 className="mb-4" style={{marginTop: '60px'}}>Quản lý báo cáo của người dùng</h2>
                    
                    <div className="search-bar d-flex align-items-center me-3 mb-3 w-100 gap-4">
                        <Form.Select
                            style={{ width: "200px" }}
                            value={typeReport}
                            onChange={(e) => {
                                e.preventDefault();
                                setTypeReport(e.target.value);
                            }}
                        >
                            <option selected disabled hidden>Loại báo cáo</option> {/* Nhãn mặc định */}
                            <option value="Hệ thống">Hệ thống</option>
                            <option value="Bình luận">Bình luận</option>
                            <option value="Phim">Phim</option>
                        </Form.Select>


                        <Form.Select
                            className=""
                            style={{ width: "200px" }}
                            value={searchType}
                            onChange={(e) => {
                                e.preventDefault();
                                setSearchType(e.target.value);
                            }}
                        >
                            <option value="Tất cả">Tất cả</option>
                            <option value="Id">Id</option>
                            <option value="Người dùng">Người dùng</option>
                            <option value="Thời gian">Thời gian</option>
                            <option value="Nội dung">Nội dung</option>
                            <option value="Trạng thái">Trạng thái</option>
                        </Form.Select>

                        <Form.Control
                            type="text"
                            placeholder="Nhập thông tin tìm kiếm..."
                            // className="me-4"
                            value={searchTerm}
                            onChange={(e) => {
                                e.preventDefault();
                                setSearchTerm(e.target.value);
                            }}
                        />
                </div>
                    {RenderTypeReport(typeReport)}
                </Container>
            </div>
        </div>
    );
};

export default ReportManagement;