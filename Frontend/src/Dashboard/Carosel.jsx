import React from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import "./Carosel.css";
import { Link } from 'react-router-dom';

const Carousel = ({ movie }) => {
    return (
        <div className="carousel-wrapper">
            <div id="carouselExampleInterval" className="carousel slide" data-bs-ride="carousel">
                <div className="carousel-indicators">
                    {movie && movie.slice(0, 5).map((_, index) => (
                        <button
                            key={index}
                            type="button"
                            data-bs-target="#carouselExampleInterval"
                            data-bs-slide-to={index}
                            className={index === 0 ? "active" : ""}
                            aria-current={index === 0 ? "true" : "false"}
                            aria-label={`Slide ${index + 1}`}
                        />
                    ))}
                </div>

                <div className="carousel-inner">
                    {movie && movie.slice(0, 5).map((item, index) => (
                        <div key={index} className={`carousel-item ${index === 0 ? "active" : ""}`} data-bs-interval="5000">
                            <div className="carousel-image-container">
                                <img 
                                    src={item.backgroundImage ? item.backgroundImage.replace(/"/g, "") : item.thumbnail} 
                                    className="d-block w-100 carousel-image" 
                                    alt={item.title}
                                />
                                <div className="carousel-overlay">
                                    <div className="container">
                                        <div className="row">
                                            <div className="col-md-8">
                                                <h2 className="carousel-title">{item.title}</h2>
                                                <p className="carousel-description">{item.description}</p>
                                                <div className="movie-info mb-3">
                                                    <span className="badge bg-primary me-2">{item.nation}</span>
                                                    <span className="badge bg-success me-2">{item.nameCategories}</span>
                                                </div>
                                                <Link 
                                                    to={`/chi-tiet-phim/${item.idmovie}__${item.slugTitle}`}
                                                    className="btn btn-danger btn-lg"
                                                >
                                                    Xem Ngay
                                                </Link>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>

                <button className="carousel-control-prev" type="button" data-bs-target="#carouselExampleInterval" data-bs-slide="prev">
                    <span className="carousel-control-prev-icon" aria-hidden="true"></span>
                    <span className="visually-hidden">Previous</span>
                </button>
                <button className="carousel-control-next" type="button" data-bs-target="#carouselExampleInterval" data-bs-slide="next">
                    <span className="carousel-control-next-icon" aria-hidden="true"></span>
                    <span className="visually-hidden">Next</span>
                </button>
            </div>

            {/* <style jsx>{`
                .carousel-wrapper {
                    position: relative;
                    overflow: hidden;
                }

                .carousel-image-container {
                    position: relative;
                    height: 600px;
                }

                .carousel-image {
                    height: 100%;
                    object-fit: cover;
                    filter: brightness(0.7);
                }

                .carousel-overlay {
                    position: absolute;
                    top: 0;
                    left: 0;
                    right: 0;
                    bottom: 0;
                    background: linear-gradient(to right, rgba(0,0,0,0.8) 0%, rgba(0,0,0,0.4) 50%, rgba(0,0,0,0) 100%);
                    display: flex;
                    align-items: center;
                }

                .carousel-title {
                    color: white;
                    font-size: 3rem;
                    font-weight: bold;
                    margin-bottom: 1rem;
                    text-shadow: 2px 2px 4px rgba(0,0,0,0.5);
                }

                .carousel-description {
                    color: #f8f9fa;
                    font-size: 1.2rem;
                    margin-bottom: 2rem;
                    text-shadow: 1px 1px 2px rgba(0,0,0,0.5);
                }

                .movie-info {
                    margin-bottom: 1.5rem;
                }

                .movie-info .badge {
                    font-size: 0.9rem;
                    padding: 0.5rem 1rem;
                }

                .carousel-control-prev,
                .carousel-control-next {
                    width: 5%;
                }

                .carousel-indicators {
                    margin-bottom: 2rem;
                }

                .carousel-indicators button {
                    width: 12px;
                    height: 12px;
                    border-radius: 50%;
                    margin: 0 5px;
                }

                @media (max-width: 768px) {
                    .carousel-image-container {
                        height: 400px;
                    }

                    .carousel-title {
                        font-size: 2rem;
                    }

                    .carousel-description {
                        font-size: 1rem;
                    }
                }
            `}</style> */}
        </div>
    );
};

export default Carousel;