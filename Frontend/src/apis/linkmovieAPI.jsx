import * as urls from "./ConstantAPI";
import { apiService } from "../utils/interceptorAxios";

export const GetAllLinkMovie = () =>{
    return apiService.get(urls.URL_GetAllLinkMovie);
}
export const AddLinkMovie = (payload) => {
    return apiService.post_form(urls.URL_AddLinkMovie, payload);
}
export const UpdateLinkMovie = (payload) => {
    return apiService.put(urls.URL_UpdateLinkMovie, payload);
}
export const DeleteLinkMovie = (IdLinkMovie) => {
    return apiService.delete(`${urls.URL_DeleteLinkMovie}/${IdLinkMovie}`);
}
export const GetLinkMovieByIdMovie = (IdLinkMovie) => {
    return apiService.get(`${urls.URL_GetLinkMovieById}/${IdLinkMovie}`);
}