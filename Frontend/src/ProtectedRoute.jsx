import { Navigate, Outlet } from 'react-router-dom';
import Cookies from 'js-cookie';
import { jwtDecode } from 'jwt-decode';

const ProtectedRoute = ({ children, isAdminRoute }) => {
  const token = Cookies.get('accessToken');
  let isAdmin = false;

  // Kiểm tra token và vai trò
  if (token) {
    try {
      const decoded = jwtDecode(token);
      isAdmin = decoded.role === 'Admin';
    } catch (error) {
      console.error('Error decoding token:', error);
    }
  }

  // Logic cho các route Admin (bắt đầu bằng /quan-ly)
  if (isAdminRoute) {
    if (!token) {
      // Chưa đăng nhập -> chuyển hướng về /
      return <Navigate to="/" replace />;
    }
    if (!isAdmin) {
      // Không phải Admin -> chuyển hướng về /
      return <Navigate to="/" replace />;
    }
    // Là Admin -> cho phép truy cập
    return children || <Outlet />;
  }

  // Logic cho các route không phải Admin
  if (isAdmin) {
    // Là Admin -> chuyển hướng về /quan-ly
    return <Navigate to="/quan-ly" replace />;
  }

  // Không phải Admin hoặc chưa đăng nhập -> cho phép truy cập
  return children || <Outlet />;
};

export default ProtectedRoute;