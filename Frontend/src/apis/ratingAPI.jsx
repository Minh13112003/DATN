import * as urls from "./ConstantAPI";
import { apiService } from "../utils/interceptorAxios";

export const AddRating = (payload) =>{
    return apiService.post_form(urls.URL_AddRating, payload);
}
export const GetRating = (idmovie) =>{
    return apiService.get(`${urls.URL_GetRating}/${idmovie}`);
}