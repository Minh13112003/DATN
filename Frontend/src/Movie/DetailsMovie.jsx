import React, { useState, useEffect, useContext } from 'react';
import { Container, Row, Col, Card, Button, Badge, Spinner, Modal, Form } from 'react-bootstrap';
import { FaUserTie, FaHeart, FaEye, FaPlay, FaCalendar, FaTheaterMasks, FaClock, FaLanguage, FaRocket, FaComment, FaStar, FaArrowLeft } from 'react-icons/fa';
import { MdHighQuality } from 'react-icons/md';
import { Link, useParams, useNavigate } from 'react-router-dom';
import slugify from '../Helper/Slugify';
import Navbar from '../Dashboard/Navbar';
import Footer from '../Dashboard/Footer';
import { DataContext } from '../ContextAPI/ContextNavbar';
import './DetailsMovie.css';
import { toast } from 'react-toastify';
import CommentSection from './CommentSection';
import { GetMovieByTitle, FavoriteMovies, ToggleFavoriteMovie, AddHistory } from '../apis/movieAPI';
import Cookies from 'js-cookie';
import { GetVip } from '../apis/authAPI';
import { UpReport } from '../apis/reportAPI';
import { AddRating, GetRating } from '../apis/ratingAPI';

const DetailsMovie = () => {
    const { idAndSlug } = useParams();
    const [id, slugTitle] = idAndSlug.split('__');
    const navigate = useNavigate();
    const { categories, movieTypes, nations, statuses, statusMap } = useContext(DataContext);
    const [movie, setMovie] = useState(null);
    const [isFavorite, setIsFavorite] = useState(false);
    const [loadingFavorite, setLoadingFavorite] = useState(false);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const [showFeedbackModal, setShowFeedbackModal] = useState(false);
    const [feedbackContent, setFeedbackContent] = useState('');
    const [feedbackLoading, setFeedbackLoading] = useState(false);
    const [showRatingModal, setShowRatingModal] = useState(false);
    const [rating, setRating] = useState(0);
    const [hoverRating, setHoverRating] = useState(0);
    const [ratingLoading, setRatingLoading] = useState(false);
    const [userRating,setUserRating] = useState(0);
    const [newRating, setNewRating] = useState(0);
    const [hasRated, setHasRated] = useState(false);

    const isAuthenticated = () => {
        return !!Cookies.get('accessToken');
    };
    const handleGoBack = () => {
        navigate(-1);
    };

    useEffect(() => {
        const fetchMovie = async () => {
            try {
                // Lấy thông tin phim
                const movieResponse = await GetMovieByTitle(slugTitle);
                setMovie(movieResponse.data[0]);

                // Lấy đánh giá của người dùng nếu đã đăng nhập
                if (isAuthenticated()) {
                    const ratingResponse = await GetRating(id);
                    if (ratingResponse.data && ratingResponse.data.rating >= 0) {
                        console.log(ratingResponse.data.rating);
                        setUserRating(ratingResponse.data.rating);
                        setRating(ratingResponse.data.rating);
                        setHasRated(true);
                    } else {
                        setUserRating(null);
                        setHasRated(false);
                    }
                }
            } catch (error) {
                console.error('Error fetching movie or rating:', error);
                toast.error('Không thể tải thông tin phim hoặc đánh giá');
            };
        };
        fetchMovie();
    }, [id, slugTitle]);

    const checkFavorite = async () => {
        if (!isAuthenticated() || !movie) return;

        try {
            const response = await FavoriteMovies(1,50000000);
            const favorites = response.data.movies || [];
            const favoriteSlugs = favorites.map(fav => slugify(fav.title) || '');
            setIsFavorite(favoriteSlugs.includes(slugTitle)); // So sánh với slugTitle từ URL
        } catch (error) {
            console.error('Error checking favorite:', error);
            if (error.response?.status === 401) {
                Cookies.remove('accessToken');
                Cookies.remove('username');
                localStorage.removeItem('userData');
                navigate('/tai-khoan/auth');
            }
        }
    };

    useEffect(() => {
        checkFavorite();
    }, [movie, id]);

    const handleFavoriteClick = async () => {
        if (!isAuthenticated()) {
            toast.info('Vui lòng đăng nhập để yêu thích phim');
            navigate('/tai-khoan/auth');
            return;
        }

        try {
            setLoadingFavorite(true);
            const response = await ToggleFavoriteMovie(slugTitle);
            setIsFavorite(response.data.isFavorite); // Sử dụng trạng thái từ API
            toast.success(response.data.message);
            
        } catch (error) {
            console.error('Error toggling favorite:', error);
            if (error.response?.status === 401) {
                toast.error('Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại');
                Cookies.remove('accessToken');
                Cookies.remove('refreshToken');
                localStorage.removeItem('userData');
                navigate('/tai-khoan/auth');
            } else {
                toast.error('Có lỗi xảy ra, vui lòng thử lại sau');
            }
        } finally {
            setLoadingFavorite(false);
        }
    };

    const HandleSeeMovieButton = async () => {
        const url = movie.numberOfMovie > 1 
            ? `/xem-phim/${movie.id}__${slugify(movie.title)}__Tap_1`
            : `/xem-phim/${movie.id}__${slugify(movie.title)}`;

        if (!isAuthenticated()) {
            if(movie.isVip === true){
                toast.error("Phim này thuộc phim Vip, xin vui lòng đăng nhập và nâng cấp Vip");
                return;
            }
            navigate(url);
            return;
        }

        if (movie.statusText === "Chưa có lịch" ){
            toast.error('Phim chưa có tập, xin vui lòng quay lại sau');
            return;
        }
        if (movie.statusText === "Sắp chiếu" ){
            toast.error('Phim chưa có lịch chiếu, xin vui lòng quay lại sau ');
            return;
        }

        try {
            const isVip = await GetVip();
            if(isVip.data === false && movie.isVip === true){
                toast.error("Phim này thuộc phim Vip, xin vui lòng nâng cấp Vip để thưởng thức");
                return;
            }

            setLoading(true);
            setError(null);
            
            const param = {idMovie : movie.id};
            const response = await AddHistory(param);
            if (response.status != 200) {
                throw new Error('Failed to add history');
            }
            
            navigate(url);
        } catch (err) {
            setError(err.message);
            console.error('Error:', err);
            toast.error('Có lỗi xảy ra khi thêm lịch sử xem phim');
        } finally {
            setLoading(false);
        }
    };

    const handleFeedbackSubmit = async () => {
        if (!feedbackContent.trim()) {
            toast.error('Vui lòng nhập nội dung góp ý/báo cáo');
            return;
        }

        setFeedbackLoading(true);
        try {
            const payload = {
                slugMovie : slugTitle,
                content: feedbackContent
            }
            const response = await UpReport(payload);
            if (response.status !== 200) {
                setShowFeedbackModal(false);
                setFeedbackContent('');
                toast.error('Đã có lỗi');
                return;
            }

            toast.success('Cảm ơn bạn đã góp ý/báo cáo!');
            setShowFeedbackModal(false);
            setFeedbackContent('');
        } catch (error) {
            console.error('Error sending feedback:', error);
            toast.error('Có lỗi xảy ra khi gửi góp ý/báo cáo');
        } finally {
            setFeedbackLoading(false);
        }
    };
    const handleRatingClick = () => {
        if (!isAuthenticated()) {
            toast.info('Vui lòng đăng nhập để đánh giá phim');
            navigate('/tai_khoan/auth');
            return;
        }

        setShowRatingModal(true);
        if (userRating !== null) {
            setRating(userRating);
        } else {
            setNewRating(0);
        }
        setHoverRating(0);
    };

    const handleStarHover = (star) => {
        if (!hasRated) {
            setHoverRating(star);
        }
    };

    const handleStarClick = (star) => {
        if (!hasRated) {
            setNewRating(star);
            console.log("Vô đây r ku");
        }
    };

    const handleRatingSubmit = async () => {
        if (hasRated) {
            toast.info('Bạn đã đánh giá phim này!');
            return;
        }

        if (newRating === 0) {
            toast.warning('Vui lòng chọn để đánh giá!');
            return;
        }

        if (!isAuthenticated()) {
            toast.error('Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại');
            navigate('/tai_khoan/auth');
            return;
        }
        console.log(hasRated + "+" + ratingLoading + "+" + newRating);
        setRatingLoading(true);
        try {
            const payload = {
                idMovie: id,
                point: newRating
            };
            await AddRating(payload);
            toast.success('Đánh giá của bạn đã được gửi thành công!');
            setShowRatingModal(false);
            setHasRated(true);
            setUserRating(newRating);
        } catch (err) {
            console.error('Error sending rating:', err);
            if (err?.response?.status === 401) {
                toast.error('Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại');
                Cookies.remove('LoginToken');
                navigate('/tai_khoan/auth');
            } else {
                toast.error(err.response.data.message || 'Có lỗi xảy ra khi gửi rating');
            }
        } finally {
            setRatingLoading(false);
        }
    };
    const handleStarLeave = () => {
        if (!hasRated) {
          setHoverRating(0); // Khi rời chuột, trở về 0 để hiển thị newRating hoặc 0
        }
      };


    if (!movie) return (
        <div className="d-flex justify-content-center align-items-center" style={{ height: '100vh' }}>
            <Spinner animation="border" variant="primary" />
        </div>
    );

    return (
        <div>
            <title>Chi tiết phim</title>
            <Navbar categories={categories} movieTypes={movieTypes} nations={nations} statuses={statuses} statusMap={statusMap} />
            <div className="movie-details py-5" style={{
                backgroundImage: `linear-gradient(rgba(0, 0, 0, 0.8), rgba(0, 0, 0, 0.8)), url(${movie.backgroundImage})`,
                backgroundSize: 'cover',
                backgroundPosition: 'center',
                minHeight: '50vh'
            }}>
                <Container>
                <div className="mb-4">
                    <Button variant="outline-secondary" onClick={handleGoBack}>
                        <FaArrowLeft className="me-2" />
                        Quay lại
                    </Button>
                </div>
                    <Row className="g-4">
                        <Col md={4}>
                            <Card className="border-0 h-100">
                                <Card.Img
                                    variant="top"
                                    src={movie.image}
                                    alt={movie.title}
                                    className="rounded-3 shadow"
                                    style={{
                                        width: '100%',
                                        height: '600px',
                                        objectFit: 'contain',
                                        backgroundColor: 'rgba(0, 0, 0, 0.5)'
                                    }}
                                />
                                {movie.isVip && (
                                    <div className="vip-label">VIP</div>
                                )}
                            </Card>
                        </Col>
                        <Col md={8}>
                            <div className="text-white">
                                <h1 className="mb-3">{movie.title}</h1>
                                <div className="mb-4">
                                    <Badge bg="danger" className="me-2">{movie.statusText}</Badge>
                                    <Badge bg="primary" className="me-2">{movie.typeMovie}</Badge>
                                    <Badge bg="success">{movie.nation}</Badge>
                                </div>
                                <Row className="mb-4 g-3">
                                    <Col sm={6} md={4}>
                                        <div className="d-flex align-items-center">
                                            <FaEye className="text-warning me-2" />
                                            <span>{movie.view} lượt xem</span>
                                        </div>
                                    </Col>
                                    <Col sm={6} md={4}>
                                        <div className="d-flex align-items-center">
                                            <FaClock className="text-warning me-2" />
                                            <span>{movie.duration} phút</span>
                                        </div>
                                    </Col>
                                    <Col sm={6} md={4}>
                                        <div className="d-flex align-items-center">
                                            <MdHighQuality className="text-warning me-2" />
                                            <span>{movie.quality}</span>
                                        </div>
                                    </Col>
                                    <Col sm={6} md={4}>
                                        <div className="d-flex align-items-center">
                                            <FaLanguage className="text-warning me-2" />
                                            <span>{movie.language}</span>
                                        </div>
                                    </Col>
                                    <Col sm={6} md={4}>
                                        <div className="d-flex align-items-center">
                                            <FaRocket className="text-warning me-2" />
                                            <span>{movie.createdAtString} - Tập {movie.episode}</span>
                                        </div>
                                    </Col>
                                    <Col sm={6} md={4}>
    <div className="d-flex align-items-center">
        <FaUserTie className="text-warning me-2" />
        <Link 
            to={`/dao-dien/${encodeURIComponent(movie.nameDirector)}`}
            className="text-decoration-none text-primary"
            style={{ cursor: 'pointer' }}
        >
            {movie.nameDirector}
        </Link>
    </div>
</Col>
                                </Row>
                                <Row>
                                <Col sm={12} md={4}>
    <div className="d-flex align-items-center">
        <FaTheaterMasks className="text-warning me-2" />
        <span>
            {Array.isArray(movie.nameActors)
                ? movie.nameActors.map((actor, index) => (
                    <span key={index}>
                        <Link 
                            to={`/dien-vien/${encodeURIComponent(actor.trim())}`}
                            className="text-decoration-none text-primary"
                            style={{ cursor: 'pointer' }}
                        >
                            {actor.trim()}
                        </Link>
                        {index < movie.nameActors.length - 1 && ', '}
                    </span>
                ))
                : typeof movie.nameActors === 'string'
                ? movie.nameActors.split(', ').map((actor, index, array) => (
                    <span key={index}>
                        <Link 
                            to={`/dien-vien/${encodeURIComponent(actor.trim())}`}
                            className="text-decoration-none text-primary"
                            style={{ cursor: 'pointer' }}
                        >
                            {actor.trim()}
                        </Link>
                        {index < array.length - 1 && ', '}
                    </span>
                ))
                : 'Không rõ'}
        </span>
    </div>
</Col>
                                    <Col sm={6} md={4}>
                                        <div className="d-flex align-items-center">
                                            <FaStar className="text-warning me-2" />
                                            <span>{movie.point}/5</span>
                                        </div>
                                    </Col>
                                </Row>
                                <div className="mb-4">
                                    <h5 className="text-warning mb-3">Thể loại:</h5>
                                    <div>
                                        {movie.nameCategories.split(', ').map((category, index) => (
                                            <Badge bg="secondary" className="me-2 mb-2" key={index}>
                                                {category}
                                            </Badge>
                                        ))}
                                    </div>
                                </div>
                                <div className="mb-4">
                                    <h5 className="text-warning mb-3">Mô tả:</h5>
                                    <p>{movie.description}</p>
                                </div>
                                <div className="d-flex gap-3">
                                    <Button
                                        variant="danger"
                                        size="lg"
                                        className="d-flex align-items-center"
                                        onClick={HandleSeeMovieButton}
                                        disabled={loading}
                                    >
                                        {loading ? (
                                            <Spinner animation="border" size="sm" className="me-2" />
                                        ) : (
                                            <FaPlay className="me-2" />
                                        )}
                                        Xem Phim
                                    </Button>
                                    <Button
                                        variant={isFavorite ? "danger" : "outline-light"}
                                        size="lg"
                                        className="d-flex align-items-center"
                                        onClick={handleFavoriteClick}
                                        disabled={loadingFavorite}
                                        title={!isAuthenticated() ? "Vui lòng đăng nhập để yêu thích phim" : ""}
                                    >
                                        {loadingFavorite ? (
                                            <Spinner animation="border" size="sm" className="me-2" />
                                        ) : (
                                            <FaHeart className="me-2" />
                                        )}
                                        {isFavorite ? "Bỏ yêu thích" : "Yêu thích"}
                                    </Button>
                                    <Button
                                        variant="outline-light"
                                        size="lg"
                                        className="d-flex align-items-center"
                                        onClick={() => setShowFeedbackModal(true)}
                                    >
                                        <FaComment className="me-2" />
                                        Góp ý/Báo cáo
                                    </Button>
                                    <Button 
                                        variant="outline-light"
                                        size="lg"
                                        className="d-flex align-items-center"
                                        onClick={() => setShowRatingModal(true)}
                                        title={isAuthenticated ? 'Vui lòng đăng nhập để đánh giá phim!' : ''}
                                    >
                                        <FaStar className="me-auto" />
                                        Đánh giá
                                    </Button>
                                </div>
                            </div>
                        </Col>
                    </Row>
                </Container>
            </div>
            <Container className="py-5 bg-white">
                <CommentSection movieId={movie.id} movieTitle={movie.title} />
            </Container>
            <Footer />

            {/* Modal Góp ý/Báo cáo */}
            <Modal show={showFeedbackModal} onHide={() => setShowFeedbackModal(false)} centered>
                <Modal.Header closeButton>
                    <Modal.Title>Góp ý/Báo cáo</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form>
                        <Form.Group className="mb-3">
                            <Form.Label>Tên phim</Form.Label>
                            <Form.Control
                                type="text"
                                value={movie?.title || ''}
                                disabled
                            />
                        </Form.Group>
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
                <Modal show={showRatingModal} onHide={() => setShowRatingModal(false)} centered>
                    <Modal.Header closeButton>
                        <Modal.Title>Đánh giá</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                    <Form>
                        <Form.Group className="mb-3">
                        <Form.Label>Tên phim</Form.Label>
                        <Form.Control type="text" value={movie.title} disabled />
                        </Form.Group>
                        <Form.Group className="mb-3">
                        <Form.Label>Đánh giá của bạn</Form.Label>
                        <div className="d-flex align-items-center">
                            {[1, 2, 3, 4, 5].map((star) => (
                            <FaStar
                                key={star}
                                size={30}
                                color={(hoverRating || newRating || rating) >= star ? '#ffc107' : '#e4e5e9'} // Dùng newRating hoặc hoverRating
                                style={{ cursor: hasRated ? 'default' : 'pointer' }}
                                onMouseEnter={() => handleStarHover(star)}
                                onMouseLeave={handleStarLeave} // Thêm sự kiện mouse leave
                                onClick={() => handleStarClick(star)}
                            />
                            ))}
                            <span className="ms-2">
                            {(hoverRating || newRating || rating) > 0 ? `${hoverRating || newRating|| rating} sao` : 'Chưa chọn'}
                            </span>
                        </div>
                        {hasRated && (
                            <p className="text-muted mt-2">Bạn đã đánh giá {userRating} sao cho phim này.</p>
                        )}
                        </Form.Group>
                    </Form>
                    </Modal.Body>
                    <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowRatingModal(false)}>
                        Hủy
                    </Button>
                    <Button
                        variant="primary"
                        onClick={handleRatingSubmit} // Gọi hàm submit
                        disabled={hasRated  || ratingLoading  || newRating === 0}
                    >
                        {ratingLoading ? (
                        <>
                            <Spinner animation="border" size="sm" className="me-2" />
                            Đang gửi...
                        </>
                        ) : (
                        'Gửi đánh giá'
                        )}
                    </Button>
            </Modal.Footer>
        </Modal>
        </div>
    );
};

export default DetailsMovie;