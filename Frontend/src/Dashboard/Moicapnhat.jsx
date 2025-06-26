import React from 'react';
import './Moicapnhat.css'; 
import { Link } from 'react-router-dom';

const MoiCapNhat = ({ movies, type, links }) => {
    return (
        <div>
            <div className='row'>
                <div className="d-flex justify-content-between align-items-center">
                    <h4 className="ms-2"><b>{type}</b></h4>
                    <a href={links} className="text-decoration-none text-danger">Xem Tất Cả →</a>
                </div>
            </div>
            <div className='row mt-3'>
                {movies.slice(0, 12).map((movie, index) => (
                    <div key={index} className='col-lg-2 col-md-4 col-sm-6 mb-4'>
                        <div className="movie-card">
                            <div className="image-container">
                                <img 
                                    src={movie.image} 
                                    className="card-img-top" 
                                    alt={movie.title} 
                                    loading="lazy" // Thêm thuộc tính lazy loading
                                />
                                <div className="overlay">
                                    <div className="categories">
                                        {movie.nameCategories.split(', ').map((category, idx) => (
                                            <span key={idx} className="badge bg-secondary m-1">{category}</span>
                                        ))}
                                    </div>
                                    <Link to={`/chi-tiet-phim/${movie.id}__${movie.slugTitle}`} className="overlay-button">Xem ngay</Link>
                                </div>
                            </div>
                            <div className="card-body">
                                <h6 className="card-title text-center">{movie.title}</h6>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default MoiCapNhat;