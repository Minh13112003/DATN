import * as urls from "./ConstantAPI";
import { apiService } from "../utils/interceptorAxios";

export const GetDashBoardAdmin = () =>{
    return apiService.get(urls.URL_GetCategoryAndMovieType);
}
export const GetMovieRatePoint = (param) =>{
    return apiService.get(`${urls.URL_GetNumberMix}?sortBy=${param}`);
}