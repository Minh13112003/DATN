import Dashboard from './Dashboard/Dashboard';
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import ListFilm from './ListMovie/ListFilm';  
import { DataProvider } from './ContextAPI/ContextNavbar';
import ListTypeFilm from './ListMovie/ListTypeFilm';
import ListNationFilm from './ListMovie/ListNationFilm';
import ListStatusFilm from './ListMovie/ListStatusFilm';
import AuthForm from './LoginAndRegis/AuthForm';  
import Profile from './LoginAndRegis/Profile';
import DetailsMovie from './Movie/DetailsMovie';
import SeeMovie from './Movie/SeeMovie';
import DashBoardAdmin from './Admin/DashBoardAdmin';
import MovieManagement from './Admin/MovieManagement';
import CategoryManagement from './Admin/CategoryManagement';
import ListFavoriteFilm from './ListMovie/ListFavouriteFilm';
import 'react-toastify/dist/ReactToastify.css';
import { ToastContainer } from 'react-toastify';
import EpisodeManagement from './Admin/EpisodeManagement';
import ErrorPage from './ErrorPage';
import ListHistoryFilm from './ListMovie/ListHistoryFilm';
import ListSearchAdvance from './ListMovie/ListSearchAdvance';
import ListNewestFilm from './ListMovie/ListNewestFilm';
import ListAllFilm from './ListMovie/ListAllFilm';
import ForgotPassword from './LoginAndRegis/ForgotPassword';
import ListSearchFilm from './ListMovie/ListSearchFilm';
import Payment from './Payment/Payment';
import PaymentSuccess from './Payment/PaymentSuccess';
import PaymentCancel from './Payment/PaymentCancel';
import PaymentHistory from './Payment/PaymentHistory';
import CommentManagement from './Admin/CommentManagement';
import AccountManagement from './Admin/AccountManagement';
import ReportManagement from './Admin/ReportManagement';
import ReportHistory from './LoginAndRegis/ReportHistory';
import ChatbotAssistant from './ChatbotAssistant';
import ListActorFilm from './ListMovie/ListActorFilm';
import ProtectedRoute from './ProtectedRoute'; // Import ProtectedRoute

function App() {
  return (
    <>
      <ToastContainer />
      <DataProvider>
        <Router>
          <Routes>
            {/* Các route không phải Admin */}
            <Route
              path='/'
              element={
                <ProtectedRoute>
                  <Dashboard />
                </ProtectedRoute>
              }
            />
            <Route
              path='/tai-khoan/auth'
              element={
                <ProtectedRoute>
                  <AuthForm />
                </ProtectedRoute>
              }
            />
            <Route
              path='/the-loai/:category'
              element={
                <ProtectedRoute>
                  <ListFilm />
                </ProtectedRoute>
              }
            />
            <Route
              path='/loai-phim/:movieTypeSlug'
              element={
                <ProtectedRoute>
                  <ListTypeFilm />
                </ProtectedRoute>
              }
            />
            <Route
              path='/quoc-gia/:nationSlug'
              element={
                <ProtectedRoute>
                  <ListNationFilm />
                </ProtectedRoute>
              }
            />
            <Route
              path='/trang-thai/:movieStatusSlug'
              element={
                <ProtectedRoute>
                  <ListStatusFilm />
                </ProtectedRoute>
              }
            />
            <Route
              path='/tai-khoan/thong-tin-ca-nhan'
              element={
                <ProtectedRoute>
                  <Profile />
                </ProtectedRoute>
              }
            />
            <Route
              path='/chi-tiet-phim/:idAndSlug'
              element={
                <ProtectedRoute>
                  <DetailsMovie />
                </ProtectedRoute>
              }
            />
            <Route
              path='/xem-phim/:idAndSlug'
              element={
                <ProtectedRoute>
                  <SeeMovie />
                </ProtectedRoute>
              }
            />
            <Route
              path='/yeu-thich'
              element={
                <ProtectedRoute>
                  <ListFavoriteFilm />
                </ProtectedRoute>
              }
            />
            <Route
              path='/lich-su-xem'
              element={
                <ProtectedRoute>
                  <ListHistoryFilm />
                </ProtectedRoute>
              }
            />
            <Route
              path='/tim-kiem-nang-cao/:param'
              element={
                <ProtectedRoute>
                  <ListSearchAdvance />
                </ProtectedRoute>
              }
            />
            <Route
              path='/moi-cap-nhat'
              element={
                <ProtectedRoute>
                  <ListNewestFilm />
                </ProtectedRoute>
              }
            />
            <Route
              path='/tat-ca-phim'
              element={
                <ProtectedRoute>
                  <ListAllFilm />
                </ProtectedRoute>
              }
            />
            <Route
              path='/quen-mat-khau'
              element={
                <ProtectedRoute>
                  <ForgotPassword />
                </ProtectedRoute>
              }
            />
            <Route
              path='/tim-kiem'
              element={
                <ProtectedRoute>
                  <ListSearchFilm />
                </ProtectedRoute>
              }
            />
            <Route
              path='/thanh-toan'
              element={
                <ProtectedRoute>
                  <Payment />
                </ProtectedRoute>
              }
            />
            <Route
              path='/thanh-toan/thanh-cong'
              element={
                <ProtectedRoute>
                  <PaymentSuccess />
                </ProtectedRoute>
              }
            />
            <Route
              path='/thanh-toan/huy'
              element={
                <ProtectedRoute>
                  <PaymentCancel />
                </ProtectedRoute>
              }
            />
            <Route
              path='/danh-sach-thanh-toan'
              element={
                <ProtectedRoute>
                  <PaymentHistory />
                </ProtectedRoute>
              }
            />
            <Route
              path='/lich-su-bao-cao'
              element={
                <ProtectedRoute>
                  <ReportHistory />
                </ProtectedRoute>
              }
            />
            <Route
              path='/dien-vien/:slugActor'
              element={
                <ProtectedRoute>
                  <ListActorFilm />
                </ProtectedRoute>
              }
            />
            <Route
              path='/dao-dien/:slugActor'
              element={
                <ProtectedRoute>
                  <ListActorFilm />
                </ProtectedRoute>
              }
            />

            {/* Các route Admin */}
            <Route
              path='/quan-ly'
              element={
                <ProtectedRoute isAdminRoute>
                  <DashBoardAdmin />
                </ProtectedRoute>
              }
            />
            <Route
              path='/quan-ly/phim/danh-sach'
              element={
                <ProtectedRoute isAdminRoute>
                  <MovieManagement />
                </ProtectedRoute>
              }
            />
            <Route
              path='/quan-ly/the-loai/danh-sach'
              element={
                <ProtectedRoute isAdminRoute>
                  <CategoryManagement />
                </ProtectedRoute>
              }
            />
            <Route
              path='/quan-ly/phim/tap-phim'
              element={
                <ProtectedRoute isAdminRoute>
                  <EpisodeManagement />
                </ProtectedRoute>
              }
            />
            <Route
              path='/quan-ly/binh-luan'
              element={
                <ProtectedRoute isAdminRoute>
                  <CommentManagement />
                </ProtectedRoute>
              }
            />
            <Route
              path='/quan-ly/tai-khoan/danh-sach'
              element={
                <ProtectedRoute isAdminRoute>
                  <AccountManagement />
                </ProtectedRoute>
              }
            />
            <Route
              path='/quan-ly/bao-cao'
              element={
                <ProtectedRoute isAdminRoute>
                  <ReportManagement />
                </ProtectedRoute>
              }
            />

            {/* Route cho trang lỗi */}
            <Route path='*' element={<ErrorPage />} />
          </Routes>
          <ChatbotAssistant />
        </Router>
      </DataProvider>
    </>
  );
}

export default App;