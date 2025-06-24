import { FaFacebook, FaTwitter, FaInstagram, FaYoutube } from 'react-icons/fa';

const Footer = () => {
    return (
        <footer className="bg-dark text-white py-5 mt-5">
            <div className="container">
                <div className="row">
                    {/* Thông tin website */}
                    <div className="col-md-4 mb-4">
                        <h5 className="mb-3">Về Chúng Tôi</h5>
                        <p className="text">
                            Website xem phim trực tuyến với chất lượng cao, cập nhật nhanh chóng và đa dạng thể loại phim.
                        </p>
                    </div>

                    {/* Liên kết nhanh */}
                    <div className="col-md-4 mb-4">
                        <h5 className="mb-3">Liên Kết Nhanh</h5>
                        <ul className="list-unstyled">
                            <li className="mb-2">
                                <a href="/" className="text-decoration-none">Trang Chủ</a>
                            </li>
                            <li className="mb-2">
                                <a href="/loai-phim/phim-le" className="text-decoration-none">Phim Lẻ</a>
                            </li>
                            <li className="mb-2">
                                <a href="/loai-phim/phim-bo" className="text-decoration-none">Phim Bộ</a>
                            </li>
                            <li className="mb-2">
                                <a href="/the-loai/phim-hoat-hinh" className="text-decoration-none">Phim Hoạt Hình</a>
                            </li>
                        </ul>
                    </div>

                    {/* Liên hệ */}
                    <div className="col-md-4 mb-4">
                        <h5 className="mb-3">Theo Dõi Chúng Tôi</h5>
                        <div className="social-links">
                            <a href="#" className=" me-3 fs-5">
                                <FaFacebook />
                            </a>
                            <a href="#" className="me-3 fs-5">
                                <FaTwitter />
                            </a>
                            <a href="#" className="me-3 fs-5">
                                <FaInstagram />
                            </a>
                            <a href="#" className="fs-5">
                                <FaYoutube />
                            </a>
                        </div>
                    </div>
                </div>

                {/* Copyright */}
                <div className="row mt-4">
                    <div className="col-12">
                        <hr className="bg-secondary" />
                        <p className="text-center mb-0">
                            &copy; {new Date().getFullYear()} Copyright by NguyenNhatMinh. All rights reserved.
                        </p>
                    </div>
                </div>
            </div>
        </footer>
    );
};

export default Footer;
