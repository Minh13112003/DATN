import * as urls from "./ConstantAPI";
import { apiService } from "../utils/interceptorAxios";

export const UpComment = (payload) =>{
    return apiService.post_form(urls.URL_AddComment, payload);
}
export const GetComment = (Idcomment) =>{
    return apiService.get(`${urls.URL_GetComment}/${Idcomment}`);
}
export const UpdateComment = (payload) => {
    return apiService.put(urls.URL_UpdateComment, payload);
}
export const DeleteComment = (Idcomment) => {
    return apiService.delete(`${urls.URL_DeleteComment}/${Idcomment}`)
}
export const GetAllComment = () => {
    return apiService.get(urls.URL_GetAllComment);
}