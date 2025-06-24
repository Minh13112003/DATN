import axios from "axios";
import Cookies from 'js-cookie';

const axiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  timeout: 10000,
});

const refreshToken = async () => {
  try {
    const userName = Cookies.get("username");
    const refreshToken = Cookies.get('refreshToken');
    const response = await axiosInstance.post('/account/refresh-token', {userName, refreshToken });
    const { accessToken, newRefreshToken } = response.data;

    Cookies.set('accessToken', accessToken, { expires: 1 });
    Cookies.set('refreshToken', newRefreshToken, { expires: 7 });

    return accessToken;
  } catch (error) {
    Cookies.remove('accessToken');
    Cookies.remove('refreshToken');
    // window.location.href = '/login';
    throw error;
  }
};

axiosInstance.interceptors.request.use(
  config => {
    const token = Cookies.get('accessToken');  
    // const token = localStorage.getItem('userToken');
    config.headers["ngrok-skip-browser-warning"] = "true";
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }

    if (config.data instanceof FormData || 
      (config.data && config.data.constructor && config.data.constructor.name === 'FormData')) {
      delete config.headers['Content-Type'];
    }
    
    return config;
  }, 
  error => Promise.reject(error)
);

axiosInstance.interceptors.response.use(
  response => response,
  async error => {
    const originalRequest = error.config;

    if (
      originalRequest &&
      error.response?.status === 401 &&
      !originalRequest.headers['x-retry']
    ) {
      originalRequest.headers['x-retry'] = 'true';
      try {
        const newAccessToken = await refreshToken();
        originalRequest.headers['Authorization'] = `Bearer ${newAccessToken}`;
        return axiosInstance(originalRequest);
      } catch (refreshError) {
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

const token = Cookies.get('accessToken');

export const apiService = {
  get: (url, params = {}, config = {}) => axiosInstance.get(url, { ...config, params }),
  post: (url, data, config = {}) => axiosInstance.post(url, data, config),
  post_form: (url, data, config = {}) => axiosInstance.post(url, data, config),
  put: (url, data, config = {}) => axiosInstance.put(url, data, config),
  patch: (url, data, config = {}) => axiosInstance.patch(url, data, config),
  delete: (url, config = {}) => axiosInstance.delete(url, config),
};

export default axiosInstance;