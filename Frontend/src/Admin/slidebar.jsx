import { FaFilm, FaList, FaUsers, FaComments, FaEnvelope } from 'react-icons/fa';

export const slidebarMenus = [
    {
        title: 'Quản lý Phim',
        icon: <FaFilm />,
        items: [
            { title: 'Danh sách phim', link: '/quan-ly/phim/danh-sach' },
            { title: 'Danh sách tập phim', link: '/quan-ly/phim/tap-phim' },
        ]
    },
    {
        title: 'Quản lý Thể loại',
        icon: <FaList />,
        items: [
            { title: 'Danh sách thể loại', link: '/quan-ly/the-loai/danh-sach' },
        ]
    },
    {
        title: 'Quản lý Tài khoản',
        icon: <FaUsers />,
        items: [
            { title: 'Danh sách người dùng', link: '/quan-ly/tai-khoan/danh-sach' },
        ]
    },
    {
        title: 'Quản lý Bình luận',
        icon: <FaComments />,
        items: [
            { title: 'Danh sách bình luận', link: '/quan-ly/binh-luan' },
        ]
    },
    {
        title: 'Quản lý Báo cáo',
        icon: <FaEnvelope />,
        items: [
            { title: 'Danh sách báo cáo', link: '/quan-ly/bao-cao' },
        ]
    }
];