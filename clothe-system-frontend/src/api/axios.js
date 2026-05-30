import axios from 'axios'

const defaultHost = typeof window !== 'undefined' ? `${window.location.protocol}//${window.location.hostname}:3002` : 'http://localhost:3002'
const apiBase = (import.meta.env && import.meta.env.VITE_API_URL) ? import.meta.env.VITE_API_URL : defaultHost

const axiosInstance = axios.create({
  baseURL: `${apiBase}/api`,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
})

axiosInstance.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => Promise.reject(error)
)

axiosInstance.interceptors.response.use(
  (response) => response.data,
  (error) => {
    if (error.response) {
      const { status, data } = error.response
      
      if (status === 401) {
        localStorage.removeItem('token')
        return Promise.reject({ message: 'Sessão expirada. Faça login novamente.' })
      }
      
      if (status === 403) {
        return Promise.reject({ message: 'Acesso negado.' })
      }
      
      if (status === 404) {
        return Promise.reject({ message: 'Recurso não encontrado.' })
      }
      
      if (status >= 500) {
        return Promise.reject({ message: 'Erro interno do servidor.' })
      }
      
      return Promise.reject({
        message: data?.message || data?.errors?.[0] || 'Erro na requisição.',
        data: data
      })
    } else if (error.request) {
      return Promise.reject({ message: 'Erro de conexão. Verifique sua internet.' })
    } else {
      return Promise.reject({ message: 'Erro na configuração da requisição.' })
    }
  }
)

export default axiosInstance