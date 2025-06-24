import React, { useContext, useEffect, useState } from 'react';  
import './ListFilm.css';
import { DataContext } from '../ContextAPI/ContextNavbar';
import Navbar from '../Dashboard/Navbar';
import { useLocation, Link, useNavigate, useParams } from 'react-router-dom';
import Footer from '../Dashboard/Footer';
import slugify from '../Helper/Slugify';
import { SearchMovie } from '../apis/movieAPI';
import { Pagination } from 'react-bootstrap';

const ListSearchFilm = () => {
    const { categories, movieTypes, nations, statuses, statusMap } = useContext(DataContext);
    const [keyword, setKeyword] = useState();
    const [movies, setMovies] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage] = useState(36);
    const [currentMovies, setCurrentMovies] = useState([]);
    const [totalPages, setTotalPages] = useState(0);

    const location = useLocation();
    const navigate = useNavigate();

    const handlePageChange = (pageNumber) => {
        setCurrentPage(pageNumber);
        const params = new URLSearchParams(location.search);
        params.set('page', `${pageNumber}`);
        window.scrollTo({ top: 0, behavior: 'smooth' });
        navigate(`${location.pathname}?${params.toString()}`, { replace: true });
    };

    const fetchMovies = async (keyword, page) => {   
        try {
            const response = await SearchMovie(keyword, page, itemsPerPage);
            setMovies(response.data);
            setCurrentMovies(response.data.movies);
            setTotalPages(Math.ceil(response.data.totalRecords / itemsPerPage));

            if (response.data.movies?.length === 0) {
                navigate("/*");
            }
        } catch (error) {
            console.error('Error fetching movies:', error);
            // Xử lý lỗi nếu cần thiết
        }
    };

    useEffect(() => {
        const params = new URLSearchParams(location.search);
        const page = parseInt(params.get("page")) || 1;
        const keyword = params.get("keyword") || '';
        setKeyword(keyword);
        setCurrentPage(page);    
        window.scrollTo({ top: 0, behavior: 'smooth' });
        fetchMovies(keyword,page);
    }, [location.search, navigate]);

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
            <div className='container'>
                <div className='row'>
                    <div className="d-flex justify-content-between align-items-center">
                        <h4 className="ms-2"><b>Danh sách Phim có từ khóa ""{keyword}""</b></h4>
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
            <Footer />
        </div>
    )
};

export default ListSearchFilm;
