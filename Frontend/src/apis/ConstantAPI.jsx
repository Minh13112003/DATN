export const URL_BASE = import.meta.env.VITE_API_URL;
export const URL_CHATBOT_BASE = import.meta.env.VITE_API_CHATBOT_URL;

// Auth API
export const URL_Login = `${URL_BASE}/account/login`;
export const URL_Register = `${URL_BASE}/account/register`;
export const URL_Profie = `${URL_BASE}/account/userinfor`;
export const URL_SendOTP = `${URL_BASE}/account/sendOTP`;
export const URL_VerifyOTP = `${URL_BASE}/account/verifyOTP`;
export const URL_ResetPassword = `${URL_BASE}/account/ChangePassword`;
export const URL_GetVip = `${URL_BASE}/account/GetVip`;
export const URL_ChangeInfor = `${URL_BASE}/account/ChangeAccountInfor`;
export const URL_GetUser = `${URL_BASE}/account/GetAllUser`;
export const URL_DeleteUser = `${URL_BASE}/account/delete`;

// Movie API

export const URL_SearchMovie = `${URL_BASE}/movie/SearchMovie`;
export const URL_GetMovieBySlugType = `${URL_BASE}/movie/GetMovieBySlugType`;       
export const URL_GetMovieBySlugCategory = `${URL_BASE}/movie/GetMovieBySlugCategory`;
export const URL_GetMovieBySlugNation = `${URL_BASE}/movie/GetMovieBySlugNation`;
export const URL_GetMovieByFavorite = `${URL_BASE}/movie/FavoriteMovies`;
export const URL_GetMovieByHistory = `${URL_BASE}/movie/GetHistoryMovie`;
export const URL_GetMovieByStatus = `${URL_BASE}/movie/GetMovieByStatus`;
export const URL_GetMovieByType = `${URL_BASE}/movie/GetMovieByType`;
export const URL_AddMovie = `${URL_BASE}/movie/addmovie`;
export const URL_DeleteMovie = `${URL_BASE}/movie/delete`;
export const URL_AdvanceSearch = `${URL_BASE}/movie/AdvanceSearch`;
export const URL_NewestMovie = `${URL_BASE}/movie/GetNewestMovie`;
export const URL_GetAllMovie = `${URL_BASE}/movie/ShowAllMovieCategory`;
export const URL_GetMovieByTitle = `${URL_BASE}/movie/GetMovieBySlugTitle`;
export const URL_ToggleMovie = `${URL_BASE}/movie/ToggleFavoriteMovie`;
export const URL_AddHistory = `${URL_BASE}/movie/AddHistory`;
export const URL_UpdateMovie = `${URL_BASE}/movie/update`;
export const URL_GetMovieById = `${URL_BASE}/movie/GetMovieById`;
export const URL_GetMovieByActor = `${URL_BASE}/movie/GetMovieByActor`;

//Report API
export const URL_Upreport = `${URL_BASE}/report/UpReport`;
export const URL_GetCommentReport = `${URL_BASE}/report/GetCommentReport`;
export const URL_ExecuteReport = `${URL_BASE}/report/ExecuteReport`
export const URL_ResponseCommentReport = `${URL_BASE}/report/ResponseCommentReport`
export const URL_GetReportComment = `${URL_BASE}/report/GetReportComment`
export const URL_GetReportMovie = `${URL_BASE}/report/GetReportMovie`
export const URL_GetReportSystem = `${URL_BASE}/report/GetReportSystem`
export const URL_ReceiveReport= `${URL_BASE}/report/ReceiveReport`
export const URL_ResponseReport= `${URL_BASE}/report/ResponseReport`
export const URL_SelfReport= `${URL_BASE}/report/GetSelfReport`


//Comment API
export const URL_AddComment = `${URL_BASE}/comment/AddComment`;
export const URL_GetComment = `${URL_BASE}/comment/GetCommentBySlugTitle`;
export const URL_UpdateComment = `${URL_BASE}/comment/UpdateComment`;
export const URL_DeleteComment = `${URL_BASE}/comment/DeleteComment`;
export const URL_GetAllComment = `${URL_BASE}/comment/GetAllComment`;


//Payment API
export const URL_CreatePayment = `${URL_BASE}/payment/Payos/create-payment`;
export const URL_GetPaymentOrder = `${URL_BASE}/payment/Payos/GetPaymentOrder`;
export const URL_DetailPayment = `${URL_BASE}/payment/Payos/GetDetailPayment`;
export const URRL_CancelPayment = `${URL_BASE}/payment/Payos/CancelPayment`;

//LinkMovie API
export const URL_GetAllLinkMovie = `${URL_BASE}/linkmovie/GetAllLinkMovie`;
export const URL_AddLinkMovie = `${URL_BASE}/linkmovie/AddLinkMovie`;
export const URL_UpdateLinkMovie = `${URL_BASE}/linkmovie/UpdateLinkMovie`;
export const URL_DeleteLinkMovie = `${URL_BASE}/linkmovie/DeleteMovie`;
export const URL_GetLinkMovieById = `${URL_BASE}/linkmovie/GetLinkMovieByIdMovie`;

//RatingAPI
export const URL_AddRating = `${URL_BASE}/movierating/AddRating`;
export const URL_GetRating = `${URL_BASE}/movierating/GetRating`;

//MixAPI
export const URL_Navbar = `${URL_BASE}/mixapi/getCategoryAndMovieType`;
export const URL_GetCategoryAndMovieType = `${URL_BASE}/mixapi/getNumberOfMovieAndCategory`;
export const URL_GetNumberMix = `${URL_BASE}/mixapi/MoviePointView`;

//CategoryAPI
export const URL_GetAllCategory = `${URL_BASE}/category/getall`;
export const URL_UpdateCategory = `${URL_BASE}/category/update`;
export const URL_AddCategory = `${URL_BASE}/category/addCategory`;
export const URL_DeleteCategory = `${URL_BASE}/category/deleteCategory`;

//ImageAPI
export const URL_UploadImage = `${URL_BASE}/profile/upload-Image`;
export const URL_UploadAvatar = `${URL_BASE}/profile/upload-avatar`;
export const URL_UploadBackground = `${URL_BASE}/profile/upload-Background`;

//ChatBot
export const URL_Chat = `${URL_CHATBOT_BASE}/chat`;