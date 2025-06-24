import React, { useContext, useEffect, useState } from 'react';
import { DataContext } from '../ContextAPI/ContextNavbar';
import Navbar from '../Dashboard/Navbar';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { Card, Row, Col, Container, Button, ButtonGroup } from 'react-bootstrap';
import { 
    FaFilm, FaList, FaUsers, FaAngleLeft, 
    FaAngleRight, FaArrowLeft, FaTachometerAlt,
    FaComments, FaEnvelope, FaChartLine, FaChartBar, FaChartPie
} from 'react-icons/fa';
import axios from 'axios';
import Footer from '../Dashboard/Footer';
import './AdminStyles.css';
import { GetMovieRatePoint, GetDashBoardAdmin } from '../apis/mixAPI';
import { slidebarMenus } from './slidebar';

// Import Chart.js components
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  ArcElement,
  Title,
  Tooltip,
  Legend,
} from 'chart.js';
import { Line, Bar, Pie } from 'react-chartjs-2';

// Register Chart.js components
ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  ArcElement,
  Title,
  Tooltip,
  Legend
);

const DashBoardAdmin = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const { categories, movieTypes, nations, statuses, statusMap } = useContext(DataContext);
    const [stats, setStats] = useState({
        numberOfMovie: 0,
        numberOfCategory: 0,
        numberOfComments: 0,
        numberOfFeedbacks: 0,
        pendingFeedbacks: 0,
        statDay: [],
        statMonth: [],
        statYear: [],
        listCategoryAndNumber: [],
        listMovieStatus: []
    });
    const [isSidebarOpen, setIsSidebarOpen] = useState(true);
    const [timeRange, setTimeRange] = useState('day'); // 'day', 'month', 'year'
    
    // Dữ liệu cho biểu đồ
    const [viewsData, setViewsData] = useState({
        labels: [],
        datasets: []
    });
    
    const [categoryData, setCategoryData] = useState({
        labels: [],
        datasets: []
    });
    
    const [statusData, setStatusData] = useState({
        labels: [],
        datasets: []
    });
    const [movieRatingData, setMovieRatingData] = useState({
        labels: [],
        datasets: []
    });
    const [sortType, setSortType] = useState('point'); // 'none', 'point', 'view'

    const sidebarMenus = [
            {
                title: 'Quản lý Phim',
                icon: <FaFilm />,
                items: [
                    {
                        title: 'Danh sách phim',
                        link: '/quan-ly/phim/danh-sach'
                    },
                    
                    {
                        title: 'Danh sách tập phim',
                        link: '/quan-ly/phim/tap-phim'
                    }
                ]
            },
            {
                title: 'Quản lý Thể loại',
                icon: <FaList />,
                items: [
                    {
                        title: 'Danh sách thể loại',
                        link: '/quan-ly/the-loai/danh-sach'
                    },
                    {
                        title: 'Thêm thể loại',
                        link: '/quan-ly/the-loai/them-moi'
                    }
                ]
            },
            {
                title: 'Quản lý Tài khoản',
                icon: <FaUsers />,
                items: [
                    {
                        title: 'Danh sách người dùng',
                        link: '/quan-ly/tai-khoan/danh-sach'
                    },
                    {
                        title: 'Thêm người dùng',
                        link: '/quan-ly/tai-khoan/them-moi'
                    }
                ]
            },
            {
                title: 'Quản lý Bình luận',
                icon: <FaComments />,
                items: [
                    {
                        title: 'Danh sách bình luận',
                        link: '/quan-ly/binh-luan'
                    },
                    {
                        title: 'Bình luận bị báo cáo',
                        link: '/quan-ly/binh-luan/bao-cao'
                    }
                ]
            },
            {
                title: 'Quản lý Góp ý',
                icon: <FaEnvelope />,
                items: [
                    {
                        title: 'Danh sách góp ý',
                        link: '/quan-ly/gop-y'
                    }
                ]
            }
        ];
        

    useEffect(() => {
        const fetchStats = async () => {
            try {
                const response = await GetDashBoardAdmin();
                setStats(response.data);
                updateChartData(response.data, timeRange);
                updateCategoryChart(response.data.listCategoryAndNumber);
                updateStatusChart(response.data.listMovieStatus);
            } catch (error) {
                console.error('Error fetching stats:', error);
            }
        };
        fetchStats();
        const fetchMovieRatings = async (sortType) => {
            try {
                const response = await GetMovieRatePoint(sortType); // Adjust URL as needed
                updateMovieRatingChart(response.data, sortType);
            } catch (error) {
                console.error('Error fetching movie ratings:', error);
            }
        };
        fetchMovieRatings(sortType);
    }, [sortType]);
    
    
    
    const updateChartData = (data, range) => {
        let labels = [];
        let viewCounts = [];

        switch (range) {
            case 'day':
                labels = data.statDay.map(item => item.timePeriod);
                viewCounts = data.statDay.map(item => item.viewCount);
                break;
            case 'month':
                labels = data.statMonth.map(item => item.timePeriod);
                viewCounts = data.statMonth.map(item => item.viewCount);
                break;
            case 'year':
                labels = data.statYear.map(item => item.timePeriod);
                viewCounts = data.statYear.map(item => item.viewCount);
                break;
            default:
                break;
        }

        const viewsChartData = {
            labels: labels,
            datasets: [
                {
                    label: 'Lượt xem phim',
                    data: viewCounts,
                    borderColor: 'rgb(53, 162, 235)',
                    backgroundColor: 'rgba(53, 162, 235, 0.5)',
                    tension: 0.3,
                },
            ],
        };
        setViewsData(viewsChartData);
    };

    const handleTimeRangeChange = (range) => {
        setTimeRange(range);
        updateChartData(stats, range);
    };

    // Hàm cập nhật dữ liệu biểu đồ thể loại
    const updateCategoryChart = (categoryData) => {
        const categoryChartData = {
            labels: categoryData.map(item => item.categoryName),
            datasets: [
                {
                    label: 'Số lượng phim',
                    data: categoryData.map(item => item.movieCount),
                    backgroundColor: [
                        'rgba(255, 99, 132, 0.7)',
                        'rgba(54, 162, 235, 0.7)',
                        'rgba(255, 206, 86, 0.7)',
                        'rgba(75, 192, 192, 0.7)',
                        'rgba(153, 102, 255, 0.7)',
                        'rgba(255, 159, 64, 0.7)',
                        'rgba(199, 199, 199, 0.7)',
                        'rgba(83, 102, 255, 0.7)',
                        'rgba(40, 159, 64, 0.7)',
                        'rgba(210, 199, 199, 0.7)',
                    ],
                },
            ],
        };
        setCategoryData(categoryChartData);
    };

    // Hàm cập nhật dữ liệu biểu đồ trạng thái
    const updateStatusChart = (statusData) => {
        const statusChartData = {
            labels: statusData.map(item => item.statusText),
            datasets: [
                {
                    label: 'Số lượng phim',
                    data: statusData.map(item => item.movieCount),
                    backgroundColor: [
                        'rgba(54, 162, 235, 0.7)',
                        'rgba(255, 206, 86, 0.7)',
                        'rgba(75, 192, 192, 0.7)',
                        'rgba(255, 99, 132, 0.7)',
                        'rgba(153, 102, 255, 0.7)',
                        'rgba(255, 159, 64, 0.7)',
                    ],
                    borderColor: [
                        'rgb(54, 162, 235)',
                        'rgb(255, 206, 86)',
                        'rgb(75, 192, 192)',
                        'rgb(255, 99, 132)',
                        'rgb(153, 102, 255)',
                        'rgb(255, 159, 64)',
                    ],
                    borderWidth: 1,
                },
            ],
        };
        setStatusData(statusChartData);
    };
    const updateMovieRatingChart = (data, sort) => {
        let sortedData = [...data];
        if (sort === 'point') {
            sortedData.sort((a, b) => b.point - a.point);
        } else if (sort === 'view') {
            sortedData.sort((a, b) => b.view - a.view);
        }

        const movieRatingChartData = {
            labels: sortedData.map(item => item.title),
            datasets: [
                {
                    label: 'Điểm đánh giá',
                    data: sortedData.map(item => item.point),
                    backgroundColor: 'rgba(255, 99, 132, 0.7)',
                    borderColor: 'rgb(255, 99, 132)',
                    borderWidth: 1,
                },
                {
                    label: 'Lượt xem',
                    data: sortedData.map(item => item.view),
                    backgroundColor: 'rgba(54, 162, 235, 0.7)',
                    borderColor: 'rgb(54, 162, 235)',
                    borderWidth: 1,
                },
            ],
        };
        setMovieRatingData(movieRatingChartData);
    };

    const handleSortChange = (type) => {
        setSortType(type);
    };
    const movieRatingOptions = {
        responsive: true,
        plugins: {
            legend: {
                position: 'top',
            },
            title: {
                display: true,
                text: 'Điểm đánh giá và Lượt xem theo phim',
                font: { size: 16 }
            },
            tooltip: {
                callbacks: {
                    label: function(context) {
                        let label = context.dataset.label || '';
                        if (label) {
                            label += ': ';
                        }
                        label += context.parsed.y;
                        return label;
                    }
                }
            }
        },
        scales: {
            x: {
                ticks: {
                    maxRotation: 45,
                    minRotation: 45,
                    autoSkip: false
                }
            },
            y: {
                beginAtZero: true,
                title: {
                    display: true,
                    text: 'Giá trị'
                }
            }
        }
    };

    // Cấu hình cho biểu đồ
    const lineOptions = {
        responsive: true,
        plugins: {
            legend: {
                position: 'top',
            },
            title: {
                display: true,
                text: 'Lượt xem phim theo tháng',
                font: {
                    size: 16
                }
            },
        },
        scales: {
            y: {
                beginAtZero: true,
            }
        }
    };
    
    const barOptions = {
        responsive: true,
        plugins: {
            legend: {
                position: 'top',
            },
            title: {
                display: true,
                text: 'Số lượng phim theo thể loại',
                font: {
                    size: 16
                }
            },
        },
        scales: {
            y: {
                beginAtZero: true,
                ticks: {
                    stepSize: 1
                }
            }
        }
    };
    
    const pieOptions = {
        responsive: true,
        plugins: {
            legend: {
                position: 'right',
                labels: {
                    boxWidth: 15,
                    padding: 15
                }
            },
            title: {
                display: true,
                text: 'Tỉ lệ phim theo trạng thái',
                font: {
                    size: 16
                }
            },
        },
    };

    return (
        <div>
            <title>Quản lý</title>
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
                        <div className="d-flex align-items-center mb-4">
                            <FaTachometerAlt className="me-2" />
                            <span className="fw-bold">Dashboard</span>
                        </div>
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

                {/* Toggle Button */}
                <Button
                    variant="dark"
                    className="sidebar-toggle-fixed"
                    onClick={() => setIsSidebarOpen(!isSidebarOpen)}
                >
                    {isSidebarOpen ? <FaAngleLeft /> : <FaAngleRight />}
                </Button>

                {/* Main Content */}
                <div className={`admin-content ${isSidebarOpen ? '' : 'expanded'}`}>
                    <Container fluid>
                        <h2 className="text-center mb-5">Tổng quan hệ thống</h2>

                        <Row className="stats-cards g-4 mb-5">
                            <Col md={6} lg={3}>
                                <div className="stats-card">
                                    <div className="stats-icon bg-primary">
                                        <FaFilm />
                                    </div>
                                    <div className="stats-info">
                                        <h3>{stats.numberOfMovie}</h3>
                                        <p>Tổng số phim</p>
                                    </div>
                                </div>
                            </Col>
                            <Col md={6} lg={3}>
                                <div className="stats-card">
                                    <div className="stats-icon bg-success">
                                        <FaList />
                                    </div>
                                    <div className="stats-info">
                                        <h3>{stats.numberOfCategory}</h3>
                                        <p>Tổng số thể loại</p>
                                    </div>
                                </div>
                            </Col>
                            <Col md={6} lg={3}>
                                <div className="stats-card">
                                    <div className="stats-icon bg-warning">
                                        <FaComments />
                                    </div>
                                    <div className="stats-info">
                                        <h3>{stats.numberOfComments || 0}</h3>
                                        <p>Tổng số bình luận</p>
                                    </div>
                                </div>
                            </Col>
                            <Col md={6} lg={3}>
                                <div className="stats-card">
                                    <div className="stats-icon bg-danger">
                                        <FaEnvelope />
                                    </div>
                                    <div className="stats-info">
                                        <h3>{stats.numberOfFeedbacks || 0}</h3>
                                        <p>Góp ý ({stats.pendingFeedbacks || 0} chưa xử lý)</p>
                                    </div>
                                </div>
                            </Col>
                        </Row>
                        
                        {/* Biểu đồ */}
                        <h3 className="text-center mb-4">Thống kê chi tiết</h3>
                        
                        {/* Biểu đồ đường - Lượt xem theo thời gian */}
                        <Card className="mb-5 shadow-sm">
                            <Card.Header className="bg-light">
                                <div className="d-flex align-items-center justify-content-between">
                                    <div className="d-flex align-items-center">
                                        <FaChartLine className="me-2 text-primary" />
                                        <h5 className="mb-0">Thống kê lượt xem</h5>
                                    </div>
                                    <ButtonGroup>
                                        <Button 
                                            variant={timeRange === 'day' ? 'primary' : 'outline-primary'}
                                            onClick={() => handleTimeRangeChange('day')}
                                        >
                                            Theo ngày
                                        </Button>
                                        <Button 
                                            variant={timeRange === 'month' ? 'primary' : 'outline-primary'}
                                            onClick={() => handleTimeRangeChange('month')}
                                        >
                                            Theo tháng
                                        </Button>
                                        <Button 
                                            variant={timeRange === 'year' ? 'primary' : 'outline-primary'}
                                            onClick={() => handleTimeRangeChange('year')}
                                        >
                                            Theo năm
                                        </Button>
                                    </ButtonGroup>
                                </div>
                            </Card.Header>
                            <Card.Body>
                                <div style={{ height: '300px' }}>
                                    <Line options={lineOptions} data={viewsData} />
                                </div>
                            </Card.Body>
                        </Card>
                        
                        <Row className="mb-5">
                            {/* Biểu đồ cột - Số lượng phim theo thể loại */}
                            <Col lg={7}>
                                <Card className="h-100 shadow-sm">
                                    <Card.Header className="bg-light">
                                        <div className="d-flex align-items-center">
                                            <FaChartBar className="me-2 text-success" />
                                            <h5 className="mb-0">Phim theo thể loại</h5>
                                        </div>
                                    </Card.Header>
                                    <Card.Body>
                                        <div style={{ height: '300px' }}>
                                            <Bar options={barOptions} data={categoryData} />
                                        </div>
                                    </Card.Body>
                                </Card>
                            </Col>
                            
                            {/* Biểu đồ tròn - Tỉ lệ phim theo trạng thái */}
                            <Col lg={5}>
                                <Card className="h-100 shadow-sm">
                                    <Card.Header className="bg-light">
                                        <div className="d-flex align-items-center">
                                            <FaChartPie className="me-2 text-danger" />
                                            <h5 className="mb-0">Phim theo trạng thái</h5>
                                        </div>
                                    </Card.Header>
                                    <Card.Body>
                                        <div style={{ height: '300px', display: 'flex', justifyContent: 'center' }}>
                                            <Pie options={pieOptions} data={statusData} />
                                        </div>
                                    </Card.Body>
                                </Card>
                            </Col>
                        </Row>
                        <Card className="mb-5 shadow-sm">
                            <Card.Header className="bg-light">
                                <div className="d-flex align-items-center justify-content-between">
                                    <div className="d-flex align-items-center">
                                        <FaChartBar className="me-2 text-info" />
                                        <h5 className="mb-0">Điểm đánh giá và Lượt xem phim</h5>
                                    </div>
                                    <ButtonGroup>
                                        <Button 
                                            variant={sortType === 'point' ? 'info' : 'outline-info'}
                                            onClick={() => handleSortChange('point')}
                                        >
                                            Sắp xếp theo điểm
                                        </Button>
                                        <Button 
                                            variant={sortType === 'view' ? 'info' : 'outline-info'}
                                            onClick={() => handleSortChange('view')}
                                        >
                                            Sắp xếp theo lượt xem
                                        </Button>
                                    </ButtonGroup>
                                </div>
                            </Card.Header>
                            <Card.Body>
                                <div style={{ height: '400px' }}>
                                    <Bar options={movieRatingOptions} data={movieRatingData} />
                                </div>
                            </Card.Body>
                        </Card>
                    </Container>
                </div>
            </div>
            <Footer />
        </div>
    );
};

export default DashBoardAdmin;