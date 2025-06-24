import 'bootstrap/dist/css/bootstrap.min.css';
import { useState, useEffect, useRef } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Modal, Button, Form, Spinner } from 'react-bootstrap';
import slugify from '../Helper/Slugify';
import { useData } from '../ContextAPI/ContextNavbar';
import { UpReport } from '../apis/reportAPI'; // Import hàm UpReport
import './Navbar.css';
import Cookies from 'js-cookie';
import { FaCrown } from 'react-icons/fa';
import avatar_default from '../assets/images/avatar_default.png'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faClockRotateLeft } from '@fortawesome/free-solid-svg-icons';

const Navbar = ({ categories, movieTypes, nations, statuses, statusMap }) => {
    const navigate = useNavigate();
    const [dropdownOpen, setDropdownOpen] = useState({
        categories: false,
        movieTypes: false,
        nations: false,
        statuses: false,
        userMenu: false,
        admin: false,
    });

    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [showUserMenu, setShowUserMenu] = useState(false);
    const userMenuRef = useRef(null);
    const [userRole, setUserRole] = useState(null);
    const [isSearching, setIsSearching] = useState(false);
    const {searchMovies, keyword, setKeyword } = useData();
    const [userName, setUserName] = useState();
    const avatar_img = Cookies.get("avatar");
    const image = avatar_img !== 'undefined' ? avatar_img : avatar_default;

    // States cho modal báo cáo
    const [showFeedbackModal, setShowFeedbackModal] = useState(false);
    const [feedbackContent, setFeedbackContent] = useState('');
    const [feedbackLoading, setFeedbackLoading] = useState(false);

    useEffect(() => {
        const token = Cookies.get('accessToken');
        const userData = JSON.parse(localStorage.getItem('userData'));
        const {Username} = Cookies.get("username")? Cookies.get("username") : "AppUser";    
        setUserName(Username);
        setIsLoggedIn(!!token);
        if (userData) {
            setUserRole(userData.roles);
        }

        const handleClickOutside = (event) => {
            if (userMenuRef.current && !userMenuRef.current.contains(event.target)) {
                setShowUserMenu(false);
            }
        };

        document.addEventListener('mousedown', handleClickOutside);
        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, []);

    const handleMouseEnter = (menu) => {
        setDropdownOpen(prev => ({ ...prev, [menu]: true }));
    };

    const handleMouseLeave = (menu) => {
        setDropdownOpen(prev => ({ ...prev, [menu]: false }));
    };

    const handleLogout = () => {
        Cookies.remove('accessToken');
        Cookies.remove('username');
        Cookies.remove('refreshToken');
        Cookies.remove('avatar');
        localStorage.removeItem('userData');
        setIsLoggedIn(false);
        setShowUserMenu(false);
        navigate('/tai-khoan/auth');
    };

    const handleViewProfile = () => {
        navigate('/tai-khoan/thong-tin-ca-nhan');
    };

    const handleSearch = () => {
        if (keyword.trim()) {
            setIsSearching(false);
            navigate(`/tim-kiem?keyword=${encodeURIComponent(keyword.trim())}&page=1`);
        }
    };

    const handleKeyPress = (e) => {
        if (e.key === 'Enter' && keyword.trim()) {
            handleSearch();
        }
    };

    // Hàm xử lý gửi báo cáo
    const handleFeedbackSubmit = async () => {
        if (!feedbackContent.trim()) {
            alert('Vui lòng nhập nội dung báo cáo');
            return;
        }

        setFeedbackLoading(true);
        try {
            const payload = {
                content: feedbackContent.trim()
            };
            
            await UpReport(payload);
            alert('Gửi báo cáo thành công!');
            setFeedbackContent('');
            setShowFeedbackModal(false);
        } catch (error) {
            console.error('Lỗi khi gửi báo cáo:', error);
            alert('Có lỗi xảy ra khi gửi báo cáo. Vui lòng thử lại!');
        } finally {
            setFeedbackLoading(false);
        }
    };

    // Hàm mở modal báo cáo
    const handleOpenFeedbackModal = () => {
        setShowFeedbackModal(true);
    };

    const UserMenu = () => (
        <div className="position-relative" ref={userMenuRef}>
            <div 
                className="user-avatar-container"
                onClick={() => setShowUserMenu(!showUserMenu)}
            >
                <img
                    src={image}
                    alt= {userName}
                    className="user-avatar"
                />
            </div>
            {showUserMenu && (
                <div className="user-dropdown-menu">
                    <Link 
                        to="/tai-khoan/thong-tin-ca-nhan" 
                        className="dropdown-item"
                        onClick={() => setShowUserMenu(false)}
                    >
                        <i className="fas fa-user me-2"></i>
                        Xem thông tin tài khoản
                    </Link>
                    <Link 
                        to="/danh-sach-thanh-toan" 
                        className="dropdown-item"
                        onClick={() => setShowUserMenu(false)}
                    >
                        <i className="fas fa-receipt me-2"></i>
                        Danh sách thanh toán
                    </Link>
                    {userRole !== 'Admin' && (
                        <Link 
                            to="/lich-su-bao-cao" 
                            className="dropdown-item"
                            onClick={() => setShowUserMenu(false)}
                        >
                            <FontAwesomeIcon icon={faClockRotateLeft} className='me-3'/>
                            Lịch sử báo cáo
                        </Link>                        
                    )}
                    <div className="dropdown-divider"></div>
                    <button 
                        onClick={handleLogout}
                        className="dropdown-item text-danger"
                    >
                        <i className="fas fa-sign-out-alt me-2"></i>
                        Đăng xuất
                    </button>
                </div>
            )}
        </div>
    );

    return (
        <div className='row'>
            <nav className="navbar navbar-expand-lg navbar-dark bg-dark fixed-top mb-2">
                <div className="container-fluid">
                {userRole !== 'Admin' && (<a className="navbar-brand bg-danger text-white p-2 rounded" href="/">Movie</a>)}
                    <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
                        <span className="navbar-toggler-icon"></span>
                    </button>
                    <div className="collapse navbar-collapse" id="navbarSupportedContent">
                        <ul className="navbar-nav me-auto mb-2 mb-lg-0">
                            <li className={`nav-item dropdown ${dropdownOpen.categories ? "show" : ""}`}
                                onMouseEnter={() => handleMouseEnter("categories")}
                                onMouseLeave={() => handleMouseLeave("categories")}>
                                    {userRole !== 'Admin' && (<a className="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown" aria-expanded={dropdownOpen.categories}>
                                    Thể loại
                                </a>)}
                                
                                <ul className={`dropdown-menu ${dropdownOpen.categories ? "show" : ""}`}>
                                    {categories.length > 0 ? (
                                        categories.map(category => (
                                            <li key={category.idCategories}>
                                                <Link className="dropdown-item" to={`/the-loai/${category.slugNameCategories}`}>{category.nameCategories}</Link>
                                            </li>
                                        ))
                                    ) : (
                                        <li className="dropdown-item">Không có dữ liệu</li>
                                    )}
                                </ul>
                            </li>

                            <li className={`nav-item dropdown ${dropdownOpen.movieTypes ? "show" : ""}`}
                                onMouseEnter={() => handleMouseEnter("movieTypes")}
                                onMouseLeave={() => handleMouseLeave("movieTypes")}>
                                    {userRole !== 'Admin' && (<a className="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown" aria-expanded={dropdownOpen.movieTypes}>
                                    Loại phim
                                </a>)}
                                
                                <ul className={`dropdown-menu ${dropdownOpen.movieTypes ? "show" : ""}`}>
                                    {movieTypes.length > 0 ? (
                                        movieTypes.map((type, index) => (
                                            <li key={index}>
                                                <Link className="dropdown-item" to={`/loai-phim/${type.slugNameMovieType}`}>{type.nameMovieType}</Link>
                                            </li>
                                        ))
                                    ) : (
                                        <li className="dropdown-item">Không có dữ liệu</li>
                                    )}
                                </ul>
                            </li>

                            <li className={`nav-item dropdown ${dropdownOpen.nations ? "show" : ""}`}
                                onMouseEnter={() => handleMouseEnter("nations")}
                                onMouseLeave={() => handleMouseLeave("nations")}>
                                    {userRole !== 'Admin' && (<a className="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown" aria-expanded={dropdownOpen.nations}>
                                    Quốc gia
                                </a>)}
                                
                                <ul className={`dropdown-menu ${dropdownOpen.nations ? "show" : ""}`}>
                                    {nations.length > 0 ? (
                                        nations.map((nation, index) => (
                                            <li key={index}>    
                                                <Link className="dropdown-item" to={`/quoc-gia/${nation.slugNation}`}>{nation.nation}</Link>
                                            </li>
                                        ))
                                    ) : (
                                        <li className="dropdown-item">Không có dữ liệu</li>
                                    )}
                                </ul>
                            </li>

                            <li className={`nav-item dropdown ${dropdownOpen.statuses ? "show" : ""}`}
                                onMouseEnter={() => handleMouseEnter("statuses")}
                                onMouseLeave={() => handleMouseLeave("statuses")}>
                                    {userRole !== 'Admin' && (<a className="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown" aria-expanded={dropdownOpen.statuses}>
                                    Trạng thái phim
                                </a>)}
                                
                                <ul className={`dropdown-menu ${dropdownOpen.statuses ? "show" : ""}`}>
                                    {statuses.length > 0 ? (
                                        statuses.map(status => (
                                            <li key={status}>
                                                <Link className="dropdown-item" to={`/trang-thai/${slugify(statusMap[status])}`}>{statusMap[status] || "Không xác định"}</Link>
                                            </li>
                                        ))
                                    ) : (
                                        <li className="dropdown-item">Không có dữ liệu</li>
                                    )}
                                </ul>
                            </li>

                            {isLoggedIn && userRole !== 'Admin' && (
                                <li className="nav-item">
                                    <Link 
                                        to="/yeu-thich" 
                                        className="nav-link d-flex align-items-center"
                                    >
                                        Yêu thích
                                    </Link>
                                </li>
                            )}
                            {isLoggedIn && userRole !== 'Admin' && (
                                <li className="nav-item">
                                    <Link 
                                        to="/lich-su-xem" 
                                        className="nav-link d-flex align-items-center"
                                    >
                                        Lịch sử 
                                    </Link>
                                </li>
                            )}
                            {isLoggedIn && userRole !== 'Admin' && (
                                <li className="nav-item">
                                    <Link 
                                        to="/thanh-toan" 
                                        className="nav-link d-flex align-items-center"
                                    >
                                        <FaCrown style={{ color: 'gold', marginRight: '5px' }} />
                                        Nạp Vip
                                    </Link>
                                </li>
                            )}

                            {isLoggedIn && userRole !== 'Admin' && (
                                <li className="nav-item">
                                    <button 
                                        className="nav-link"
                                        onClick={handleOpenFeedbackModal}
                                        style={{ 
                                            border: 'none', 
                                            background: 'none', 
                                            color: 'rgba(255,255,255,.75)',
                                            padding: '0.5rem 1rem',
                                            display: 'block',
                                            textDecoration: 'none'
                                        }}
                                    >
                                        Báo cáo
                                    </button>
                                </li>
                            )}


                            
                        </ul>
                        {userRole !== 'Admin' && (
                        <form 
                            className="d-flex align-items-center gap-2 position-relative" 
                            role="search"
                            onSubmit={(e) => {
                                e.preventDefault();
                                handleSearch();
                            }}
                        >
                            <div className="search-container">
                                <input 
                                    className="form-control search-input" 
                                    type="search" 
                                    placeholder="Tìm kiếm theo: tên phim, thể loại, quốc gia,..." 
                                    aria-label="Search" 
                                    style={{ height: "50px", borderRadius: "8px", width: "400px", paddingRight: "40px" }}
                                    value={keyword}
                                    onChange={(e) => setKeyword(e.target.value)}
                                    onFocus={() => setIsSearching(true)}
                                    onBlur={() => {
                                        setTimeout(() => setIsSearching(false), 200);
                                    }}
                                    onKeyPress={handleKeyPress}
                                />
                                <span 
                                    className="search-icon"
                                    onClick={handleSearch}
                                >
                                    <i className="fas fa-search"></i>
                                </span>
                            </div>
                            {isSearching && keyword && searchMovies.length > 0 && (
                                <ul
                                    style={{
                                        position: "absolute",
                                        top: "55px",
                                        width: "400px",
                                        background: "white",
                                        border: "1px solid #ccc",
                                        borderRadius: "8px",
                                        listStyleType: "none",
                                        padding: "10px",
                                        margin: "0",
                                        zIndex: 1000,
                                        maxHeight: "400px",
                                        overflowY: "auto",
                                    }}
                                >
                                    {searchMovies.map((movieItem, index) => (
                                        <li
                                            key={movieItem.id || `movie-${index}`}
                                            style={{
                                                display: "flex",
                                                alignItems: "center",
                                                padding: "10px",
                                                cursor: "pointer",
                                                borderBottom: "1px solid #ddd",
                                            }}
                                            onClick={() => {
                                                setIsSearching(false);
                                                setKeyword('');
                                                navigate(`/chi-tiet-phim/${movieItem.id}__${slugify(movieItem.title)}`);
                                            }}
                                        >
                                            {movieItem.image ? (
                                                <img
                                                    src={movieItem.image}
                                                    alt={movieItem.title}
                                                    style={{ 
                                                        width: "80px", 
                                                        height: "120px", 
                                                        borderRadius: "8px",
                                                        objectFit: "cover"
                                                    }}
                                                    onError={(e) => {
                                                        e.target.style.display = 'none';
                                                        const div = document.createElement('div');
                                                        div.style.cssText = `
                                                            width: 80px;
                                                            height: 120px;
                                                            background-color: #eee;
                                                            border-radius: 8px;
                                                            display: flex;
                                                            align-items: center;
                                                            justify-content: center;
                                                        `;
                                                        div.innerText = 'No Image';
                                                        e.target.parentNode.appendChild(div);
                                                    }}
                                                />
                                            ) : (
                                                <div 
                                                    style={{ 
                                                        width: "80px", 
                                                        height: "120px", 
                                                        backgroundColor: "#eee", 
                                                        borderRadius: "8px",
                                                        display: "flex",
                                                        alignItems: "center",
                                                        justifyContent: "center"
                                                    }}
                                                >
                                                    No Image
                                                </div>
                                            )}
                                            <div style={{ marginLeft: "10px", flex: 1 }}>
                                                <h4 style={{ 
                                                    margin: "0", 
                                                    fontSize: "16px",
                                                    color: "#000"
                                                }}>
                                                    {movieItem.title}
                                                </h4>
                                                <p style={{ 
                                                    margin: "5px 0", 
                                                    fontSize: "14px", 
                                                    color: "gray" 
                                                }}>
                                                    {movieItem.nameCategories}
                                                </p>
                                            </div>
                                        </li>
                                    ))}
                                </ul>
                            )}
                        
                            {isLoggedIn ? (
                                <UserMenu />
                            ) : (
                                <Link 
                                    to="/tai-khoan/auth"
                                    className="btn btn-outline-danger" 
                                    style={{ 
                                        height: "50px", 
                                        padding: "10px 30px", 
                                        borderRadius: "8px", 
                                        fontWeight: "bold", 
                                        fontSize: "14px", 
                                        width: "auto", 
                                        whiteSpace: "nowrap"
                                    }}
                                >
                                    Đăng nhập/Đăng ký
                                </Link>
                            )}
                        </form>
                        )}
                        {userRole === 'Admin' && (
                            <div className="d-flex align-items-center">
                                {isLoggedIn ? (
                                    <UserMenu />
                                ) : (
                                    <Link 
                                        to="/tai-khoan/auth"
                                        className="btn btn-outline-danger" 
                                        style={{ 
                                            height: "50px", 
                                            padding: "10px 30px", 
                                            borderRadius: "8px", 
                                            fontWeight: "bold", 
                                            fontSize: "14px", 
                                            width: "auto", 
                                            whiteSpace: "nowrap"
                                        }}
                                    >
                                        Đăng nhập/Đăng ký
                                    </Link>
                                )}
                            </div>
                        )}
                    </div>
                </div>
            </nav>

            {/* Modal Báo cáo */}
            <Modal show={showFeedbackModal} onHide={() => setShowFeedbackModal(false)} centered>
                <Modal.Header closeButton>
                    <Modal.Title>Góp ý/Báo cáo</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form>
                        <Form.Group className="mb-3">
                            <Form.Label>Nội dung góp ý/báo cáo</Form.Label>
                            <Form.Control
                                as="textarea"
                                rows={4}
                                value={feedbackContent}
                                onChange={(e) => setFeedbackContent(e.target.value)}
                                placeholder="Nhập nội dung góp ý hoặc báo cáo của bạn..."
                            />
                        </Form.Group>
                    </Form>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowFeedbackModal(false)}>
                        Hủy
                    </Button>
                    <Button 
                        variant="primary" 
                        onClick={handleFeedbackSubmit}
                        disabled={feedbackLoading}
                    >
                        {feedbackLoading ? (
                            <>
                                <Spinner animation="border" size="sm" className="me-2" />
                                Đang gửi...
                            </>
                        ) : (
                            'Gửi'
                        )}
                    </Button>
                </Modal.Footer>
            </Modal>
        </div>
    );
};

export default Navbar;