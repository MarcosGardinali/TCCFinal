import { defineStore } from 'pinia'
import axiosInstance from '@/api/axios'
import { useToast } from 'vue-toastification'

const toast = useToast()
const AUTH_URL = '/Authentication'

const defaultState = () => ({
  user: null,
  token: localStorage.getItem('token'),
  loading: false,
})

export const useAuthStore = defineStore('authStore', {
  state: () => defaultState(),
  
  getters: {
    isAuthenticated: (state) => !!state.token,
    isLoading: (state) => state.loading,
  },
  
  actions: {
    async login(credentials) {
      try {
        this.loading = true
        const response = await axiosInstance.post(`${AUTH_URL}/Authenticate`, {
          email: credentials.email,
          password: credentials.password
        })

        if (response.result) {
          this.token = response.result.token
          localStorage.setItem('token', response.result.token)
          toast.success('Login realizado com sucesso!')
          return { success: true }
        } else {
          const errorMessage = response.errorMessage || 'Erro ao fazer login'
          toast.error(errorMessage)
          return { success: false, message: errorMessage }
        }
      } catch (error) {
        console.error('Erro no login:', error)
        const errorMessage = error.message || 'Erro interno do servidor'
        toast.error(errorMessage)
        return { success: false, message: errorMessage }
      } finally {
          localStorage.setItem('token', this.token)
        this.loading = false
      }
    },

    async register(userData) {
      try {
        this.loading = true
        const response = await axiosInstance.post('/User', {
          email: userData.email,
          password: userData.password,
          confirmPassword: userData.confirmPassword
        })

        if (response.result) {
          toast.success('Usuário criado com sucesso! Faça login para continuar.')
          return { success: true }
        } else {
          const errorMessage = response.errorMessage || 'Erro ao criar usuário'
          toast.error(errorMessage)
          return { success: false, message: errorMessage }
        }
      } catch (error) {
        console.error('Erro no registro:', error)
        const errorMessage = error.message || 'Erro interno do servidor'
        toast.error(errorMessage)
        return { success: false, message: errorMessage }
      } finally {
        this.loading = false
      }
    },

    logout() {
      this.user = null
      this.token = null
      localStorage.removeItem('token')
      toast.info('Logout realizado com sucesso!')
    },

    checkAuth() {
      const storedToken = localStorage.getItem('token')
      if (storedToken) {
        this.token = storedToken
      }
    }
  },
  
  persist: true,
})
