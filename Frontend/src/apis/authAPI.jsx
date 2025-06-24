import * as urls from "./ConstantAPI";
import { apiService } from "../utils/interceptorAxios";

export const Login = (payload) => { 
    return apiService.post_form(urls.URL_Login, payload);  
}

export const Register = (payload) => {
    return apiService.post(urls.URL_Register, payload);  
}
export const ProfileInfor = (username) => {
    return apiService.post_form(urls.URL_Profie, username)
}
export const SendOTP = (username, type) => {
    return apiService.post(`${urls.URL_SendOTP}/${username}/${type}`);
}
export const VerifyOTP = (username, otp) => {
    return apiService.post(`${urls.URL_VerifyOTP}/${username}/${otp}`);
}
export const ResetPassword = (payload) => {
    return apiService.post(urls.URL_ResetPassword, payload);
}
export const GetVip =() => {
    return apiService.get(urls.URL_GetVip);
}
export const ChangeInfor = (payload) => {
    return apiService.put(urls.URL_ChangeInfor, payload);
}
export const GetUser = () =>{
    return apiService.get(urls.URL_GetUser);
}
export const DeleteUser = (username) =>{
    return apiService.delete(`${urls.URL_DeleteUser}/${username}`);
}
 