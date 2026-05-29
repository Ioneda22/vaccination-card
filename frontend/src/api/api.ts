import axios from "axios";

// URL base da API (configurável via .env -> VITE_API_BASE_URL).
const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5087/api";

// Chave usada para guardar o token JWT no localStorage.
export const TOKEN_STORAGE_KEY = "vaccinationcard.token";

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Interceptor de request: injeta o token JWT no cabeçalho Authorization.
api.interceptors.request.use((config) => {
  const token = localStorage.getItem(TOKEN_STORAGE_KEY);
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Interceptor de response: em caso de 401 (token expirado/inválido),
// limpa a sessão e redireciona para o login.
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem(TOKEN_STORAGE_KEY);
      if (window.location.pathname !== "/login") {
        window.location.href = "/login";
      }
    }
    return Promise.reject(error);
  },
);

export default api;
