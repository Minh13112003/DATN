import React, { useContext, useEffect, useState } from 'react';  
import './ListFilm.css';
import { DataContext } from '../ContextAPI/ContextNavbar';
import Navbar from '../Dashboard/Navbar';
import { useParams, useLocation, Link, useNavigate } from 'react-router-dom';
import Footer from '../Dashboard/Footer';
import slugify from '../Helper/Slugify';
import { GetMovieByActor } from '../apis/movieAPI';
import { Pagination } from 'react-bootstrap';
import { toast } from 'react-toastify';
import Cookies from 'js-cookie';

const ListActorFilm = () => {
    const { categories, movieTypes, nations, statuses, statusMap } = useContext(DataContext);
    const [movies, setMovies] = useState([]);
    const { slugActor } = useParams();
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage] = useState(36);
    const [currentMovies, setCurrentMovies] = useState(null);
    const [totalPages, setTotalPages] = useState(0);
    const [actorName, setActorName] = useState('');
    const navigate = useNavigate();
    const location = useLocation();

    const handlePageChange = (pageNumber) => {
        setCurrentPage(pageNumber);
        const params = new URLSearchParams(location.search);
        params.set('page', `${pageNumber}`);
        navigate(`${location.pathname}?${params.toString()}`, { replace: true });
        window.scrollTo({ top: 0, behavior: 'smooth' });
        fetchMovies(pageNumber);
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

    const fetchMovies = async (page) => {   
        try {
            let decodedActorName = decodeURIComponent(slugActor);
        
        // Sanitize tên diễn viên
        decodedActorName = decodedActorName
            .trim()
            .replace(/[<>'"]/g, '') // Loại bỏ ký tự đặc biệt
            .replace(/\s+/g, ' '); // Chuẩn hóa khoảng trắng
        
        if (!decodedActorName || decodedActorName.length < 2) {
            throw new Error('Tên diễn viên không hợp lệ');
        }
        
        setActorName(decodedActorName);
        
        const response = await GetMovieByActor(decodedActorName, page, itemsPerPage);
            
            if (!response.data || !response.data.movies) {
                throw new Error('Dữ liệu trả về không hợp lệ');
            }
            
            setMovies(response.data);
            setCurrentMovies(response.data.movies);
            setTotalPages(Math.ceil(response.data.totalRecords / itemsPerPage));
            
            if (response.data.movies.length === 0) {
                toast.warning(`Không tìm thấy phim nào của ${location.pathname.includes('/dien-vien/') ? 'diễn viên' : 'đạo diễn'}: ${decodedActorName}`);
                // Không navigate về 404, để user thấy trang trống
            }
        } catch (error) {
            console.error('Error fetching movies by actor:', error);
            
            if (error.response?.status === 404) {
                toast.error('Không tìm thấy thông tin diễn viên/đạo diễn');
            } else if (error.response?.status === 500) {
                toast.error('Lỗi server, vui lòng thử lại sau');
            } else {
                toast.error(error.message || 'Không thể tải danh sách phim');
            }
            
            navigate("/*");
        }
    };
    
    

    useEffect(() => {
        const params = new URLSearchParams(location.search);
        let page = params.get("page");
        setCurrentPage(parseInt(page) || 1);
        let test = page ? page : 1;
        window.scrollTo({ top: 0, behavior: 'smooth' });
        fetchMovies(test);
    }, [slugActor, location.search]);
   
    return (
        <div>
            <Navbar categories={categories} movieTypes={movieTypes} nations={nations} statuses={statuses} statusMap={statusMap} />
            <div className='container'>
                <div className='row'>
                    <div className="d-flex justify-content-between align-items-center">
                        <h4 className="ms-2">
                            <b>Danh sách Phim có {location.pathname.includes('/dien-vien/') ? 'diễn viên' : 'đạo diễn'}: {actorName}</b>
                        </h4>
                    </div>
                </div>
                <div className='row mt-5'>
                    {currentMovies?.map((movie, index) => (
                        <div key={index} className='col-lg-2 col-md-4 col-sm-6 mb-4'>
                            <div className="movie-card">
                                <div className="image-container">
                                    <img src={movie.image} className="card-img-top" alt={movie.title} />
                                    <div className="overlay">
                                        <div className="categories">
                                            {movie.nameCategories.split(', ').map((category, idx) => (
                                                <span key={idx} className="badge bg-secondary m-1">{category}</span>
                                            ))}
                                        </div>
                                        <Link to={`/chi-tiet-phim/${movie.id}__${slugify(movie.title)}`} className="overlay-button">Xem ngay</Link>
                                    </div>
                                </div>
                                <div className="card-body">
                                    <h6 className="card-title text-center">{movie.title}</h6>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
                {totalPages > 1 && (
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
                )}
            </div>
            <Footer/>
        </div>
    )
}

export default ListActorFilm;