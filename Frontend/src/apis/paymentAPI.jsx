import * as urls from "./ConstantAPI";
import { apiService } from "../utils/interceptorAxios";

export const CreatePaymentURL = (type) => {
    return apiService.post(`${urls.URL_CreatePayment}?type=${type}`);
}
export const GetPaymentOrders = () => {
    return apiService.get(urls.URL_GetPaymentOrder);
}
export const GetDetailPayment = (orderCode) =>{
    return apiService.get(`${urls.URL_DetailPayment}/${orderCode}`);
}
export const CancelPayment = (orderCode) => {
    return apiService.post(`${urls.URRL_CancelPayment}/${orderCode}`);
}