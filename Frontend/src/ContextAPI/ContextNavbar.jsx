import { createContext, useContext, useEffect, useState } from "react";
import { apiService } from "../utils/interceptorAxios";
import _ from "lodash";
import { GetDashboard, SearchMovie } from "../apis/movieAPI";

const DataContext = createContext();
const apiUrl = import.meta.env.VITE_API_URL;

const DataProvider = ({ children }) => {
    const [categories, setCategories] = useState([]);
    const [movieTypes, setMovieTypes] = useState([]);
    const [nations, setNations] = useState([]);
    const [statuses, setStatuses] = useState([]);
    const [dashboardMovies, setDashboardMovies] = useState([]); // Movie cho Dashboard
    const [searchMovies, setSearchMovies] = useState([]); // Movie cho Search
    const [newestMovies, setNewestMovies] = useState([]);
    const [finishedMovies, setFinishedMovies] = useState([]);
    const [keyword, setKeyword] = useState("");
    const [movieTitle, setMovieTitle] = useState([]);

    const statusMap = {
        "0": "Chưa có lịch",
        "1": "Sắp chiếu",
        "2": "Đang cập nhật",
        "3": "Đang chiếu",
        "4": "Đã kết thúc",
        "5": "Đã hoàn thành"
    };

    const processMovieData = (movies) => {
        return movies.map(movie => ({
            ...movie,
            image: (movie.image && 
                   typeof movie.image === 'string' && 
                   movie.image.trim() !== "" && 
                   movie.image !== "null") ? movie.image.trim() : null,
            title: movie.title || "Không có tiêu đề",
            nameCategories: movie.nameCategories || "Chưa phân loại",
            slugmovie: movie.slugmovie || "Không có slug"
        }));
    };

    // Fetch dữ liệu cho Dashboard
    useEffect(() => {
        const fetchDashboardData = async () => {
            try {
                const response = await GetDashboard();
                const data = response.data;
                
                setCategories(data.category || []);
                setMovieTypes(data.movieTypes || []);
                setNations(data.nations || []);
                setStatuses(data.statuses || []);
                setMovieTitle(data.movieTitle || []);
                const processedMovies = processMovieData(data.movie || []);
                const processedNewestMovie = processMovieData(data.updateNewestMovie || []);
                const processedFinishedMovie = processMovieData(data.finishedMovie || []);
                
                setDashboardMovies(processedMovies);
                setNewestMovies(processedNewestMovie);
                setFinishedMovies(processedFinishedMovie);
            } catch (error) {
                console.error("Lỗi khi lấy dữ liệu dashboard:", error);
            }
        };

        fetchDashboardData();
    }, []);

    // Fetch dữ liệu cho Search
    const fetchSearchMovies = async (searchKeyword) => {
        if (!searchKeyword) {
            setSearchMovies([]); // Clear kết quả tìm kiếm khi không có keyword
            return;
        }
        try {
            // const response = await axios.get(
            //     `${apiUrl}/movie/SearchMovie?Keyword=${searchKeyword}`);
            
            const response = await SearchMovie(searchKeyword, 1, 10);

            const processedMovies = processMovieData(response.data.movies);
            setSearchMovies(processedMovies);
        } catch (error) {
            console.error("Lỗi khi tìm kiếm phim:", error);
            setSearchMovies([]);
        }
    };

    const debouncedSearch = _.debounce(fetchSearchMovies, 1000);

    useEffect(() => {
        debouncedSearch(keyword);
        return () => debouncedSearch.cancel();
    }, [keyword]);

    return (
        <DataContext.Provider 
            value={{ 
                categories, 
                movieTypes, 
                nations, 
                statuses,
                newestMovies,
                finishedMovies, 
                dashboardMovies, // Movie cho Dashboard
                searchMovies,    // Movie cho Search
                statusMap, 
                keyword, 
                setKeyword,
                movieTitle 
            }}
        >
            {children}
        </DataContext.Provider>
    );
};

export const useData = () => {
    return useContext(DataContext);
}
export { DataContext, DataProvider };

