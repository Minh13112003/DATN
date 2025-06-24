import * as urls from "./ConstantAPI";
import { apiService } from "../utils/interceptorAxios";

export const UploadImage = (payload) => {
    return apiService.post_form(urls.URL_UploadImage, payload);
}
export const UploadAvatar = (payload) =>{
    return apiService.post_form(urls.URL_UploadAvatar, payload);
}
export const UploadBackground = (payload) =>{
    return apiService.post_form(urls.URL_UploadBackground, payload);
}
