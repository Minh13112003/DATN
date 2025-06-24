import React, { useContext, useEffect, useState } from 'react';  
import './ListFilm.css';
import { DataContext } from '../ContextAPI/ContextNavbar';
import Navbar from '../Dashboard/Navbar';
import { useParams, useLocation, data, Link, useNavigate } from 'react-router-dom';
import axios from 'axios';
import Footer from '../Dashboard/Footer';
import slugify from '../Helper/Slugify';
import Cookies from 'js-cookie';
import { SearchMovieByType } from '../apis/movieAPI';
import { Pagination } from 'react-bootstrap';
const ListTypeFilm = () => {
    const { categories, movieTypes, nations, statuses, statusMap } = useContext(DataContext);
    const [movies, setMovies] = useState([]);
    const {movieTypeSlug } = useParams();
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage] = useState(36);
    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;
    const [currentMovies, setCurrentMovies] = useState(null);
    const [totalPages, setTotalPages] = useState(0);
    const navigate = useNavigate();
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
            // Lấy token từ localStorage
            // const response = await axios.get('http://localhost:5285/api/movie/FavoriteMovies', {
            //     headers: {
            //         'Authorization': `Bearer ${token}`
            //     }
            // });
            const response = await SearchMovieByType(movieTypeSlug, page, itemsPerPage);
            console.log(response.data);
            setMovies(response.data);
            // setCurrentMovies(response.data.movies?.slice(indexOfFirstItem, indexOfLastItem));
            setCurrentMovies(response.data.movies);
            setTotalPages(Math.ceil(response.data.totalRecords / itemsPerPage));
            if (response.data.movies?.length === 0) {
                navigate("/*");
            }
        } catch (error) {
            console.error('Error fetching favorite movies:', error);
            if (error.response?.status === 401) {
                toast.error('Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại');
                Cookies.remove('accessToken');
                Cookies.remove('refreshToken');
                Cookies.remove('username');
                localStorage.removeItem('userData');
                navigate('/tai-khoan/auth');
            } else {
                toast.error('Không thể tải danh sách phim yêu thích');
            }
        }
    };
    useEffect(() => {
        
        const params = new URLSearchParams(location.search);
        let page = params.get("page");
        setCurrentPage(parseInt(page));
        console.log(">>>", page);
        let test = page ? page : 1;
        console.log("test:",test);
        window.scrollTo({ top: 0, behavior: 'smooth' });
        fetchMovies(test);
    }, [navigate]);
    return (
        <div>
            <Navbar categories={categories} movieTypes={movieTypes} nations={nations} statuses={statuses} statusMap={statusMap} />
            <div className='container'>
                <div className='row'>
                    <div className="d-flex justify-content-between align-items-center">
                        <h4 className="ms-2"><b>Danh sách {currentMovies?.length > 0 ? currentMovies[0].typeMovie : 'Phim'}</b></h4>
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
            </div>
            <Footer/>
        </div>
    )
}

export default ListTypeFilm;
