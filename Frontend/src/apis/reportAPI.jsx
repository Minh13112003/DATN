import * as urls from "./ConstantAPI";
import { apiService } from "../utils/interceptorAxios";

export const UpReport = (payload) => {
    return apiService.post_form(urls.URL_Upreport, payload);
}
export const ExecuteCommentReport = (Idcomment) =>{
    return apiService.post(`${urls.URL_ExecuteReport}/${Idcomment}`);
}
export const ResponseCommentReport = (ResponseReport) =>{
    return apiService.post(urls.URL_ResponseCommentReport, ResponseReport);
}

export const GetCommentReport = (Idcomment) =>{
    return apiService.get(`${urls.URL_GetCommentReport}/${Idcomment}`)
}
export const GetReportComment = () =>{
    return apiService.get(`${urls.URL_GetReportComment}`)
}

export const GetReportMovie = () =>{
    return apiService.get(`${urls.URL_GetReportMovie}`)
}

export const GetReportSystem = () =>{
    return apiService.get(`${urls.URL_GetReportSystem}`)
}

export const ReceiveReport = (IdReport) =>{
    return apiService.put(`${urls.URL_ReceiveReport}/${IdReport}`)
}

export const ResponseReport = (idReport, response) =>{
    return apiService.put(`${urls.URL_ResponseReport}`, {idReport, response})
}

export const SelfReport = () =>{
    return apiService.get(`${urls.URL_SelfReport}`)
}
