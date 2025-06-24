import React from 'react';
import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

const Panel = ({ status }) => {
  const navigate = useNavigate();

  // Tùy chỉnh thông báo dựa trên trạng thái
  const getMessage = () => {
    if (status === 'Sắp chiếu') {
      return {
        title: 'Phim sắp ra mắt, chờ chút nha!',
        content: 'Phim này đang <strong>sắp chiếu</strong>, chưa có link xem ngay đâu. Theo dõi lịch chiếu để không bỏ lỡ nha! 😎',
      };
    } else if (status === 'Chưa có lịch') {
      return {
        title: 'Chưa có lịch chiếu, tiếc quá!',
        content: 'Hiện tại phim này <strong>chưa có lịch chiếu</strong>, chưa thể xem được. Quay lại sau để cập nhật thông tin mới nhất! 😊',
      };
    } else if (status === 'Yêu cầu VIP') {
      return {
        title: 'Phim này cần VIP mới xem được!',
        content: 'Phim này chỉ dành cho <strong>thành viên VIP</strong>. Nâng cấp tài khoản ngay để thưởng thức liền tay nha! 😍',
      };
    } else {
      // Trường hợp mặc định (nếu prop status không hợp lệ)
      return {
        title: 'Ôi, phim này chưa xem được!',
        content: 'Phim đang trong trạng thái chờ, chưa có link xem. Hãy quay lại sau để thưởng thức nhé! 😋',
      };
    }
  };

  const { title, content } = getMessage();

  return (
    <div
      className="d-flex justify-content-center align-items-center"
      style={{
        minHeight: '100vh',
        backgroundColor: '#1a1a1a', // Nền tối, giống các trang xem phim
        padding: '20px',
      }}
    >
      <div
        className="card text-center bg-dark text-white border-0 shadow-lg"
        style={{
          maxWidth: '500px',
          padding: '30px',
          borderRadius: '10px',
        }}
      >
        <h3 className="mb-3" style={{ fontWeight: 'bold', color: '#ff4d4d' }}>
          {title}
        </h3>
        <p
          className="mb-4"
          style={{ fontSize: '18px', color: '#ccc' }}
          dangerouslySetInnerHTML={{ __html: content }}
        />
        <button
          className="btn btn-danger btn-lg"
          onClick={() => navigate('/')}
          style={{
            padding: '10px 30px',
            fontWeight: 'bold',
            borderRadius: '8px',
          }}
        >
          Về trang chủ
        </button>
      </div>
    </div>
  );
};

export default Panel;