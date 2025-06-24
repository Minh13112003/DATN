import * as urls from "./ConstantAPI";
import { apiService } from "../utils/interceptorAxios";

export const GetAll = () => {
    return apiService.get(urls.URL_GetAllCategory);
}
export const UpdateCategory = (payload) =>{
    return apiService.post_form(urls.URL_UpdateCategory, payload);
}
export const AddCategory = (payload) => {
    return apiService.put(urls.URL_AddCategory, payload);
}
export const DeleteCategory = (IdCategory) => {
    return apiService.delete(`${urls.URL_DeleteCategory}/${IdCategory}`);
}