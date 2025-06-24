import React, { useState, useEffect, useContext } from 'react';
import { Container, Row, Col, Button } from 'react-bootstrap';
import { Checkbox, Space, Card, Select, Radio } from 'antd';
import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import Carousel from './Carosel';
import MoiCapNhat from './Moicapnhat';
import Navbar from './Navbar';
import Footer from './Footer';
import { DataContext, useData } from '../ContextAPI/ContextNavbar';
import './Dashboard.css'; 
import "../App.css"; 

const Dashboard = () => {
    const { categories = [], movieTypes = [], nations = [], statuses = [], statusMap = {} } = useContext(DataContext);
    const { dashboardMovies, newestMovies, finishedMovies } = useData();
    const [filters, setFilters] = useState({
        genre: [],
        country: undefined,
        type: undefined,
        status: [],
        search: '',
        page: 1,
        pageSize: 12
    });
    const [showAdvancedSearch, setShowAdvancedSearch] = useState(false);
    const navigate = useNavigate();
    

    const handleFilterChange = (type, checkedValues) => {
        setFilters(prev => ({
            ...prev,
            [type]: Array.isArray(checkedValues) ? checkedValues : [checkedValues]
        }));
    };

    const handleSearch = () => {
        const { genre, country, type, status } = filters;

        const params = new URLSearchParams();
        if (genre.length > 0) params.append('genres', genre.join(','));
        if (country?.length > 0) params.append('countries', country.join(','));
        if (type?.length > 0) params.append('type', type.join(','));
        if (status?.length > 0) params.append('status', status.join(',')); // chỉ 1 phần tử cũng xử lý như array
        const encodedParam = params.toString();
        navigate(`/tim-kiem-nang-cao/${encodedParam}?page=1`);        
    };

    // Hàm helper để chuyển đổi dữ liệu an toàn
    const createOptions = (data, labelKey = 'name', valueKey = 'id') => {
        if (!Array.isArray(data)) {
            return [];
        }
        const options = data
            .filter(item => {
                const isValid = item && item[labelKey] && item[valueKey];
                return isValid;
            })
            .map(item => ({
                label: item[labelKey],
                value: item[valueKey]
            }));
        return options;
    };
    
    return (
        <div className="dashboard-container">
            <title>Trang chủ</title>
            <Navbar 
                categories={categories} 
                movieTypes={movieTypes} 
                nations={nations} 
                statuses={statuses} 
                statusMap={statusMap} 
            />
            
            {/* Hero Section với Carousel */}
            <section className="hero-section mb-5">
                <div className="container-fluid p-0">
                    <Carousel movie={dashboardMovies}/>
                </div>
            </section>

            {/* Tìm kiếm nâng cao */}
            <section className="search-section py-4">
                <div className="container">
                    <Card className="mb-4">
                        <Space direction="vertical" style={{ width: '100%' }} size="large">
                            <div className="d-flex justify-content-between align-items-center mb-3">
                                <h5 className="mb-0">Tìm kiếm nâng cao</h5>
                                <Button 
                                    variant="link" 
                                    className="p-0"
                                    onClick={() => setShowAdvancedSearch(!showAdvancedSearch)}
                                >
                                    {showAdvancedSearch ? 'Thu gọn ▲' : 'Mở rộng ▼'}
                                </Button>
                            </div>
                            
                            {showAdvancedSearch && (
                                <div className="advanced-search show">
                                    <div className="mb-3">
                                        <label className="block mb-2">Thể loại:</label>
                                        <Checkbox.Group
                                            options={createOptions(categories, 'nameCategories', 'slugNameCategories')}
                                            onChange={(checkedValues) => handleFilterChange('genre', checkedValues)}
                                        />
                                    </div>

                                    <div className="mb-3">
                                        <label className="block mb-2">Quốc gia:</label>
                                        <Select
                                            allowClear
                                            placeholder="Chọn quốc gia"
                                            options={createOptions(nations, 'nation', 'slugNation')}
                                            onChange={(value) => handleFilterChange('country', value)}
                                            value={filters.country}
                                            style={{ width: '100%' }}
                                        />
                                    </div>

                                    <div className="mb-3">
                                        <label className="block mb-2">Loại phim:</label>
                                        <Select
                                            allowClear
                                            placeholder="Chọn loại phim"
                                            options={createOptions(movieTypes, 'nameMovieType', 'slugNameMovieType')}
                                            onChange={(checkedValues) => handleFilterChange('type', checkedValues)}
                                            value={filters.type}
                                            style={{ width: '100%' }}
                                        />
                                    </div>

                                    <div className="mb-3">
                                        <label className="block mb-2">Trạng thái phim:</label>
                                        <Select
                                            allowClear
                                            placeholder="Chọn trạng thái"
                                            options={statuses
                                                .filter(status => statusMap[status] !== undefined)
                                                .map(status => ({
                                                    label: statusMap[status],
                                                    value: status
                                                }))
                                            }
                                            onChange={(value) => handleFilterChange('status', value ? [value] : [])}
                                            value={filters.status.length > 0 ? filters.status[0] : undefined}
                                            style={{ width: '100%' }}
                                        />
                                    </div>
                                    <button
                                onClick={handleSearch}
                                className="w-full bg-primary text-white py-2 px-4 rounded hover:bg-primary-dark transition-colors"
                            >
                                Tìm kiếm
                            </button>
                                </div>
                                
                            )}
                            
                        </Space>
                    </Card>
                </div>
            </section>

            {/* Main Content Section */}
            <section className="main-content py-5">
                <div className="container">
                    <div className="row">
                        <div className="col-12">
                            <h2 className="section-title text-center mb-4">
                                <span className="border-bottom border-danger pb-2">Phim Mới Cập Nhật</span>
                            </h2>
                            <MoiCapNhat movies={newestMovies} type="Mới cập nhật" links = "/moi-cap-nhat"/>
                        </div>
                    </div>

                    <div className="row mt-5">
                        <div className="col-12">
                            <h2 className="section-title text-center mb-4">
                                <span className="border-bottom border-success pb-2">Phim Đã Hoàn Thành</span>
                            </h2>
                            <MoiCapNhat movies={finishedMovies} type="Đã hoàn thành" links = "/trang-thai/da-hoan-thanh"/>
                        </div>
                    </div>

                    <div className="row mt-5">
                        <div className="col-12">
                            <h2 className="section-title text-center mb-4">
                                <span className="border-bottom border-primary pb-2">Tất Cả Phim</span>
                            </h2>
                            <MoiCapNhat movies={dashboardMovies} type="Toàn bộ phim" links= "/tat-ca-phim"/>
                        </div>
                    </div>
                </div>
            </section>

            <Footer/>
        </div>
    );
};

export default Dashboard;

