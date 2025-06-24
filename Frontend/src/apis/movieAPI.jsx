import * as urls from "./ConstantAPI";
import { apiService } from "../utils/interceptorAxios";

export const AddMovie = (data) => {
    return apiService.post_form(urls.URL_AddMovie, data);  
}

export const UpdateMovie = (data) => {
    return apiService.put(urls.URL_UpdateMovie, data);
}
export const DeleteMovie = (IdMovie) => {
    return apiService.delete(`${urls.URL_DeleteMovie}/${IdMovie}`);
}

export const GetDashboard = () => {
    return apiService.get(urls.URL_Navbar);
}
export const SearchMovie = (params, pageNumber,ItemPerSize) => {
    return apiService.get(`${urls.URL_SearchMovie}?keyword=${params}&pageNumber=${pageNumber}&pageSize=${ItemPerSize}`);
}
export const FavoriteMovies = (pageNumber, ItemPerSize) =>{
    return apiService.get(`${urls.URL_GetMovieByFavorite}?pageNumber=${pageNumber}&pageSize=${ItemPerSize}`);
}
export const SearchMovieByCategory = (category, pageNumber, ItemPerSize) => {
    return apiService.get(`${urls.URL_GetMovieBySlugCategory}/${category}?pageNumber=${pageNumber}&pageSize=${ItemPerSize}`);
}
export const SearchHistory = (pageNumber,ItemPerSize) =>{
    return apiService.get(`${urls.URL_GetMovieByHistory}?pageNumber=${pageNumber}&pageSize=${ItemPerSize}`);
}
export const SearchMovieByNation = (params, pageNumber,ItemPerSize) => {
    return apiService.get(`${urls.URL_GetMovieBySlugNation}/${params}?pageNumber=${pageNumber}&pageSize=${ItemPerSize}`);
}
export const SearchMovieByStatus = (params, pageNumber, ItemPerSize) => {
    return apiService.get(`${urls.URL_GetMovieByStatus}/${params}?pageNumber=${pageNumber}&pageSize=${ItemPerSize}`);        
}
export const SearchMovieByType = (params, pageNumber, ItemPerSize) => {
    return apiService.get(`${urls.URL_GetMovieBySlugType}/${params}?pageNumber=${pageNumber}&pageSize=${ItemPerSize}`);
}
export const AdvanceSearch = (params, pageNumber, ItemPerSize) => {
    return apiService.get(`${urls.URL_AdvanceSearch}?genres=${params.genres}&type=${params.type}&countries=${params.countries}&status=${params.status}&pageNumber=${pageNumber}&pageSize=${ItemPerSize}`);
}
export const GetNewestMovie = (pageNumber,ItemPerSize) =>{
    return apiService.get(`${urls.URL_NewestMovie}?pageNumber=${pageNumber}&pageSize=${ItemPerSize}`);
}
export const GetAllMovie = (pageNumber, ItemPerSize) => {
    return apiService.get(`${urls.URL_GetAllMovie}?pageNumber=${pageNumber}&pageSize=${ItemPerSize}`);
}
export const GetMovieByTitle = (params) =>{
    return apiService.get(`${urls.URL_GetMovieByTitle}/${params}`);
}
export const ToggleFavoriteMovie = (param) =>{
    return apiService.post(`${urls.URL_ToggleMovie}/${param}`);
}
export const AddHistory=(param) =>{
    return apiService.post(urls.URL_AddHistory,param);
}
export const GetMovieById = (param) => {
    return apiService.get(`${urls.URL_GetMovieById}/${param}`);
}
export const GetMovieByActor = (param, pageNumber, ItemPerSize) => {
    return apiService.get(`${urls.URL_GetMovieByActor}/${param}?pageNumber=${pageNumber}&pageSize=${ItemPerSize}`);
}
