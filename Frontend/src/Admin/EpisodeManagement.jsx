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
import Cookies from 'js-cookie';
import Select from 'react-select';
import {GetAllLinkMovie, DeleteLinkMovie, AddLinkMovie, GetLinkMovieByIdMovie, UpdateLinkMovie } from '../apis/linkmovieAPI';
import { GetAllMovie } from '../apis/movieAPI';
import { slidebarMenus } from './slidebar';

const EpisodeManagement = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const { categories, movieTypes, nations, statuses, statusMap, movieTitle } = useContext(DataContext);
    const [episodeList, setEpisodeList] = useState([]);
    const [movieList, setMovieList] = useState([]);
    const [showModal, setShowModal] = useState(false);
    const [selectedEpisode, setSelectedEpisode] = useState(null);
    const [isSidebarOpen, setIsSidebarOpen] = useState(true);
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage] = useState(10);
    const [showImageModal, setShowImageModal] = useState(false);
    const [showBackgroundModal, setShowBackgroundModal] = useState(false);
    const [selectedImage, setSelectedImage] = useState('');
    const [selectedBackground, setSelectedBackground] = useState('');
    const [searchType, setSearchType] = useState("Tên Phim");
    const [searchTerm, setSearchTerm] = useState("");
    const [filteredEpisodes, setFilteredEpisodes] = useState([])
    const [formData, setFormData] = useState({
        idLinkMovie: '',
        idMovie: '',
        episode: '',
        urlMovie: ''
    });

    // Thêm định nghĩa sidebarMenus   

    useEffect(() => {
        fetchEpisodes();
        fetchMovies();
    }, []);
    useEffect(() => {
        const debounceSearch = setTimeout(() => {
          // Nếu không có từ khóa, hiển thị toàn bộ danh sách
          if (searchTerm.trim() === "") {
            setFilteredEpisodes(episodeList);
            console.log('No search term, showing all episodes:', episodeList);
            setCurrentPage(1);
            return;
          }
      
          // Chuẩn hóa từ khóa tìm kiếm
          const searchValue = searchTerm.toLowerCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "");
      
          // Lọc danh sách dựa trên searchType
          const filtered = episodeList.filter((episode) => {
            // Xử lý trường hợp dữ liệu không hợp lệ
            if (!episode || typeof episode !== 'object') return false;
      
            const episodeTitle = (episode.title || '').normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase();
      
            if (searchType === "Tên Phim") {
              return episodeTitle.includes(searchValue);
            }
            return true; // Mặc định không lọc nếu searchType khác
          });
      
          console.log('Filtered episodes:', filtered); // Debug
          setFilteredEpisodes(filtered);
          setCurrentPage(1); // Reset về trang đầu khi lọc
        }, 1000); // Trì hoãn 1 giây
      
        // Dọn dẹp timeout khi component unmount hoặc dependencies thay đổi
        return () => clearTimeout(debounceSearch);
      }, [searchTerm, searchType, episodeList]);

    const fetchEpisodes = async () => {
        try {
            // const response = await axios.get('http://localhost:5285/api/linkmovie/GetAllLinkMovie');
            const response = await GetAllLinkMovie();
            // Sắp xếp dữ liệu
            const sortedData = response.data.sort((a, b) => {
                // So sánh tên phim trước
                if (a.title !== b.title) {
                    return a.title.localeCompare(b.title);
                }
                // Nếu cùng phim thì sắp xếp theo số tập
                return a.episode - b.episode;
            });
            setFilteredEpisodes(response.data);
            setEpisodeList(sortedData);
        } catch (error) {
            console.error('Error fetching episodes:', error);
            toast.error('Không thể tải danh sách tập phim');
        }
    };

    const fetchMovies = async () => {
        try {
            // const token = Cookies.get('accessToken');
            // const response = await axios.get('http://localhost:5285/api/movie/ShowAllMovieCategory', {
            //     headers: {
            //         Authorization: `Bearer ${token}`
            //     }
            // });
            const response = await GetAllMovie(currentPage, itemsPerPage);
            const listmovie = await GetAllMovie(1,100000000);
            setMovieList(listmovie.data.movies);
        } catch (error) {
            console.error('Error fetching movies:', error);
            toast.error('Không thể tải danh sách phim');
        }
    };

    const handleShowModal = (episode = null) => {
        if (episode) {
            setSelectedEpisode(episode);
            setFormData({
                idLinkMovie: episode.idLinkMovie,
                idMovie: episode.idMovie,
                episode: episode.episode,
                urlMovie: episode.urlMovie
            });
        } else {
            setSelectedEpisode(null);
            setFormData({
                idLinkMovie: '',
                idMovie: '',
                episode: '',
                urlMovie: ''
            });
        }
        setShowModal(true);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            // const token = Cookies.get('accessToken');
            if (selectedEpisode) {
                // Cập nhật tập phim
                // await axios.put('http://localhost:5285/api/linkmovie/UpdateLinkMovie', formData, {
                //     headers: {
                //         Authorization: `Bearer ${token}`
                //     }
                // });
                await UpdateLinkMovie(formData);
                toast.success('Cập nhật tập phim thành công!');
            } else {
                // Thêm tập phim mới
                // await axios.post('http://localhost:5285/api/linkmovie/AddLinkMovie', formData, {
                //     headers: {
                //         Authorization: `Bearer ${token}`
                //     }
                // });
                await AddLinkMovie(formData);
                toast.success('Thêm tập phim mới thành công!');
            }
            fetchEpisodes();
            setShowModal(false);
        } catch (error) {
            console.error('Error saving episode:', error);
            toast.error(selectedEpisode ? 'Cập nhật tập phim thất bại!' : 'Thêm tập phim mới thất bại!');
        }
    };

    const handleDelete = async (idLinkMovie) => {
        if (window.confirm('Bạn có chắc muốn xóa tập phim này?')) {
            try {
                // const token = localStorage.getItem('userToken');
                // await axios.delete(`http://localhost:5285/api/linkmovie/DeleteLinkMovie/${idLinkMovie}`, {
                //     headers: {
                //         Authorization: `Bearer ${token}`
                //     }
                // });
                await DeleteLinkMovie(idLinkMovie);
                fetchEpisodes();
                toast.success('Xóa tập phim thành công!');
            } catch (error) {
                console.error('Error deleting episode:', error);
                toast.error('Xóa tập phim thất bại!');
            }
        }
    };

    const getMovieTitle = (idMovie) => {
        const movie = movieList.find(m => m.id === idMovie);
        return movie ? movie.title : 'Không xác định';
    };

    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;
    const currentItems = filteredEpisodes.slice(indexOfFirstItem, indexOfLastItem); // Sử dụng filteredEpisodes
    const totalPages = Math.ceil(filteredEpisodes.length / itemsPerPage);

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

    const handleImageClick = (image) => {
        setSelectedImage(image);
        setShowImageModal(true);
    };

    const handleBackgroundClick = (background) => {
        setSelectedBackground(background);
        setShowBackgroundModal(true);
    };
    const options = movieList.map(movie => ({
        value: movie.id,
        label: movie.title
      }));

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
                            <h2>Quản lý Tập phim</h2>
                            <div className="search-bar d-flex align-items-center me-3">
                            {/*
                                <Form.Select
                                className="me-2"
                                style={{ width: "200px" }}
                                value={searchType}
                                onChange={(e) => {
                                    e.preventDefault();
                                    setSearchType(e.target.value);
                                }}
                                >
                                <option value="Tên Phim">Tên Phim</option>
                                </Form.Select>
                                */}
                                <Form.Control
                                type="text"
                                placeholder="Nhập tên phim cần tìm kiếm..."
                                style={{width : '500px'}}
                                value={searchTerm}
                                onChange={(e) => {
                                    e.preventDefault();
                                    setSearchTerm(e.target.value);
                                }}
                                />
                           
                            </div>
                            <Button variant="primary" onClick={() => handleShowModal()}>
                                <FaPlus className="me-2" />
                                Thêm Tập phim Mới
                            </Button>
                        </div>

                        <Table striped bordered hover responsive>
                            <thead>
                                <tr>
                                    <th>Ảnh</th>
                                    <th>Ảnh nền</th>
                                    <th>Tên phim</th>
                                    <th>Tập</th>
                                    <th>Link phim</th>
                                    <th>Ngày tạo</th>
                                    
                                    <th style={{ width: '150px' }}>Thao tác</th>
                                </tr>
                            </thead>
                            <tbody>
                                {currentItems.map(episode => (
                                    <tr key={episode.idLinkMovie}>
                                        <td>
                                            <img 
                                                src={episode.image} 
                                                alt={`${episode.title} - Tập ${episode.episode}`}
                                                style={{ width: '50px', height: '50px', cursor: 'pointer' }}
                                                onClick={() => handleImageClick(episode.image)}
                                            />
                                        </td>
                                        <td>
                                            <img 
                                                src={episode.backgroundImage} 
                                                alt={`${episode.title} - Tập ${episode.episode} - Background`}
                                                style={{ width: '50px', height: '50px', cursor: 'pointer' }}
                                                onClick={() => handleBackgroundClick(episode.backgroundImage)}
                                            />
                                        </td>
                                        <td>{episode.title}</td>
                                        <td>{episode.episode}</td>
                                        <td>
                                            <a href={episode.urlMovie} target="_blank" rel="noopener noreferrer">
                                                {episode.urlMovie}
                                            </a>
                                        </td>
                                        <td>{episode.createdAtString}</td>
                                        
                                        <td>
                                            <Button 
                                                variant="warning" 
                                                size="sm" 
                                                className="me-2"
                                                onClick={() => handleShowModal(episode)}
                                            >
                                                <FaEdit />
                                            </Button>
                                            <Button 
                                                variant="danger" 
                                                size="sm"
                                                onClick={() => handleDelete(episode.idLinkMovie)}
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
                                    {selectedEpisode ? 'Chỉnh sửa Tập phim' : 'Thêm Tập phim Mới'}
                                </Modal.Title>
                            </Modal.Header>
                            <Modal.Body>
                                <Form onSubmit={handleSubmit}>
                                    <Form.Group className="mb-3">
                                        <Form.Label>Phim</Form.Label>
                                        {/* <Form.Select
                                            value={formData.idMovie}
                                            onChange={(e) => setFormData({...formData, idMovie: e.target.value})}
                                            required
                                        >
                                            <option value="">Chọn phim</option>
                                            {movieList?.map(movie => (
                                                <option key={movie.id} value={movie.id}>
                                                    {movie.title}
                                                </option>
                                            ))}
                                        </Form.Select> */}
                                        <Select
                                            options={options}
                                            value={options.find(option => option.value === formData.idMovie) || null}
                                            onChange={(selected) => setFormData({...formData, idMovie: selected?.value || ''})}
                                            placeholder="Chọn phim..."
                                            isClearable
                                            isDisabled={!!selectedEpisode}
                                        />                                        
                                    </Form.Group>
                                    <Form.Group className="mb-3">
                                        <Form.Label>Tập</Form.Label>
                                        <Form.Control
                                            type="number"
                                            value={formData.episode}
                                            onChange={(e) => setFormData({...formData, episode: e.target.value})}
                                            required
                                        />
                                    </Form.Group>
                                    <Form.Group className="mb-3">
                                        <Form.Label>Link phim</Form.Label>
                                        <Form.Control
                                            type="text"
                                            value={formData.urlMovie}
                                            onChange={(e) => setFormData({...formData, urlMovie: e.target.value})}
                                            required
                                        />
                                    </Form.Group>
                                    <div className="text-end">
                                        <Button variant="secondary" className="me-2" onClick={() => setShowModal(false)}>
                                            Hủy
                                        </Button>
                                        <Button variant="primary" type="submit">
                                            {selectedEpisode ? 'Cập nhật' : 'Thêm mới'}
                                        </Button>
                                    </div>
                                </Form>
                            </Modal.Body>
                        </Modal>

                        {/* Modal xem ảnh */}
                        <Modal show={showImageModal} onHide={() => setShowImageModal(false)} size="lg">
                            <Modal.Header closeButton>
                                <Modal.Title>Xem ảnh</Modal.Title>
                            </Modal.Header>
                            <Modal.Body className="text-center">
                                <img 
                                    src={selectedImage} 
                                    alt="Ảnh phim" 
                                    style={{ maxWidth: '100%', maxHeight: '80vh' }}
                                />
                            </Modal.Body>
                        </Modal>

                        {/* Modal xem ảnh nền */}
                        <Modal show={showBackgroundModal} onHide={() => setShowBackgroundModal(false)} size="lg">
                            <Modal.Header closeButton>
                                <Modal.Title>Xem ảnh nền</Modal.Title>
                            </Modal.Header>
                            <Modal.Body className="text-center">
                                <img 
                                    src={selectedBackground} 
                                    alt="Ảnh nền phim" 
                                    style={{ maxWidth: '100%', maxHeight: '80vh' }}
                                />
                            </Modal.Body>
                        </Modal>
                    </Container>
                </div>
            </div>
            <Footer />
        </div>
    );
};

export default EpisodeManagement; 