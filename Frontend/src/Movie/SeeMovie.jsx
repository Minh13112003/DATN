import React, { useState, useEffect, useContext } from 'react';
import { Container, Row, Col, Button } from 'react-bootstrap';
import { useParams, useNavigate } from 'react-router-dom';
import Navbar from '../Dashboard/Navbar';
import Footer from '../Dashboard/Footer';
import { DataContext } from "../ContextAPI/ContextNavbar";
import CommentSection from './CommentSection'
import { GetLinkMovieByIdMovie } from '../apis/linkmovieAPI';
import { GetMovieById } from '../apis/movieAPI';
import { GetVip } from '../apis/authAPI';
import { FaUserTie, FaHeart, FaEye, FaPlay, FaCalendar, FaTheaterMasks, FaClock, FaLanguage, FaRocket, FaComment, FaStar, FaArrowLeft } from 'react-icons/fa';
import PanelMovie from './PanelMovie';
const SeeMovie = () => {
    const { idAndSlug } = useParams();
    const navigate = useNavigate();
    const parts = idAndSlug.split('__');
    const id = parts[0];
    const slugTitle = parts[1];
    const episode = parts.length === 3 && parts[2].startsWith('Tap_')
        ? parseInt(parts[2].split('Tap_')[1])
        : null;
    const { categories, movieTypes, nations, statuses, statusMap } = useContext(DataContext);
    const [videoData, setVideoData] = useState([]);
    const [selectedEpisode, setSelectedEpisode] = useState(null);
    const [movie, setMovie] = useState([]);
    const [checkMovie, setCheckMovie] = useState(false);
    const [statusMovie, setStatusMovie] = useState('');
    const handleGoBack = () => {
        navigate(-1);
    };

    useEffect(() => {
        const fetchMovie = async () => {
          try {
            // Lấy trạng thái VIP
            const isVipResponse = await GetVip();
            const isVip = isVipResponse.data;
      
            // Lấy thông tin phim
            const movieResponse = await GetMovieById(id);
            console.log(">>> ", movieResponse.data)
            if (!movieResponse?.data) {
              console.error('GetMovieById failed or returned unexpected data');
              navigate("/");
              return;
            }
            const movie = movieResponse.data;
            console.log("movie: ", movie)
      
            // Kiểm tra trạng thái VIP và phim
            if (isVip === false && movie[0].isVip === true) {
              setCheckMovie(true);
              setStatusMovie('Yêu cầu VIP')
            //   navigate("/");
              return;
            }
            
            // Kiểm tra trạng thái phim
            if (movie[0].statusText === "Sắp chiếu" || movie[0].statusText === "Chưa có lịch") {
              console.log(movie.statusText);
            //   navigate("/");
                setCheckMovie(true);
                setStatusMovie(movie[0].statusText)
            //   return;
            }
      
            // Lấy danh sách link phim
            const linkResponse = await GetLinkMovieByIdMovie(id);
            if (!linkResponse?.data) {
              console.error('GetLinkMovieByIdMovie failed or returned unexpected data');
              navigate("/");
              return;
            }
      
            // Sắp xếp theo episode
            const sortedData = linkResponse.data.sort((a, b) => a.episode - b.episode);
            setMovie(linkResponse.data);
            setVideoData(sortedData);
      
            // Kiểm tra episode trong URL
            if (episode && sortedData.length > 0) {
              const selectedEp = sortedData.find(ep => ep.episode === parseInt(episode));
              if (selectedEp) {
                setSelectedEpisode(selectedEp);
              } else {
                navigate("/*");
              }
            } else if (sortedData.length > 0) {
              setSelectedEpisode(sortedData[0]);
            }
      
          } catch (error) {
            console.error('Error fetching movie data:', error);
            navigate("/");
          }
        };
      
        fetchMovie();
      }, [id, episode, navigate]);

    // Hàm lấy link nhúng YouTube
    const getEmbedUrl = (url) => {
        if (!url || typeof url !== 'string') return "";
    
        if (url.includes('watch?v=')) {
            const videoId = url.split('watch?v=')[1];
            return `https://www.youtube.com/embed/${videoId}`;
        }
    
        // Nếu là link m3u8 hoặc mp4, giữ nguyên
        return url;
    };

    const handleEpisodeChange = (selectedEp) => {
        setSelectedEpisode(selectedEp);
        const newUrl = `/xem-phim/${id}__${slugTitle}__Tap_${selectedEp.episode}`;
        navigate(newUrl, { replace: true });
    };

    return (
        <div>
            <title>Xem phim</title>
            <Navbar categories={categories} movieTypes={movieTypes} nations={nations} statuses={statuses} statusMap={statusMap} />
            {checkMovie ?
            <Container className="mt-7 text-center">
                <PanelMovie status={statusMovie}/>                
            </Container>

                :
            <Container className="mt-4 text-center">
                {/* <h2 className="mb-4" style={{color : 'white'}}>Xem Phim</h2> */}

                <h2 className="mb-4" style={{color : 'White', margin : '70px'}}>Xem Phim {movie[0]?.title}</h2>
                <Row className="justify-content-center">
                    <Col md={8}>
                        {selectedEpisode && (
                            <div className="ratio ratio-16x9">
                                <iframe                                   
                                    src={getEmbedUrl(selectedEpisode.urlMovie)}
                                    title="Movie Player"
                                    allowFullScreen
                                    className="w-100 border rounded shadow"
                                    style={{
                                        colorScheme: 'normal',
                                        accentColor: '#007bff'
                                    }}
                                    allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"
                                    webkitallowfullscreen="true"
                                    mozallowfullscreen="true"
                                ></iframe>
                            </div>
                        )}
                    </Col>
                </Row>
                {videoData.length > 1 && (
                    <Row className="mt-4 justify-content-center">
                        <Col md={8}>
                            <h5>Chọn tập:</h5>
                            <div className="d-flex flex-wrap gap-2 justify-content-center">
                                {videoData.map((episode) => (
                                    <Button 
                                        key={episode.idLinkMovie} 
                                        variant={selectedEpisode?.episode === episode.episode ? 'danger' : 'outline-secondary'}
                                        onClick={() => handleEpisodeChange(episode)}
                                    >
                                        Tập {episode.episode}
                                    </Button>
                                ))}
                            </div>
                        </Col>
                        
                    </Row>
                )}
            </Container>                
                }

            {videoData && videoData.length > 0 && (
                <Container className="py-5 bg-white">
                    <CommentSection 
                        movieId={videoData[0].idMovie} 
                        movieTitle={videoData[0].title} 
                    />
                </Container>
            )}
            <Footer />
        </div>
    );
};

export default SeeMovie;
