import { defineStore } from 'pinia'
import axiosInstance from '@/api/axios'
import { useToast } from 'vue-toastification'

const toast = useToast()
const ORDER_URL = '/Order'

/* ========================
   Helpers
======================== */

const defaultState = () => ({
  orders: [],
  currentOrder: null,
  loading: false,
  pagination: {
    currentPage: 1,
    pageSize: 10,
    totalItems: 0,
    totalPages: 0
  }
})

const getErrorMessage = (error, fallback = 'Erro interno do servidor') =>
  error?.message || fallback

const buildCreatePayload = (orderData) => ({
  customerId: orderData.customerId,
  observation: orderData.observation,
  listCreatedItem: orderData.listCreatedItem || []
})

const buildUpdatePayload = (orderData) => ({
  observation: orderData.observation,
  listCreatedItem: orderData.listCreatedItem || [],
  listUpdatedItem: orderData.listUpdatedItem || [],
  listDeletedItem: orderData.listDeletedItem || []
})

/* ========================
   Store
======================== */

export const useOrderStore = defineStore('orderStore', {
  state: () => defaultState(),

  getters: {
    isLoading: (state) => state.loading,
    getOrderById: (state) => (id) =>
      state.orders.find(order => order.id === parseInt(id))
  },

  actions: {
    async fetchOrders(page = 1, pageSize = 10, silent = false) {
      try {
        this.loading = true

        const response = await axiosInstance.post(
          `${ORDER_URL}/search`,
          { orderByDescending: false },
          { params: { pageNumber: page, pageSize } }
        )

        const result = response.result
        if (result) {
          this.orders = result.items || []
          this.pagination = {
            currentPage: result.currentPage || page,
            pageSize: result.pageSize || pageSize,
            totalItems: result.totalItems || 0,
            totalPages: result.totalPages || 0
          }

          return { success: true }
        }
      } catch (error) {
        const message = getErrorMessage(error)
        toast.error(message)
        return { success: false, message }
      } finally {
        this.loading = false
      }
    },

    async fetchOrderById(id) {
      try {
        this.loading = true

        const response = await axiosInstance.get(
          `${ORDER_URL}/${id}`
        )

        if (!response?.result) {
          const message =
            response?.errorMessage || 'Erro ao carregar pedido'
          toast.error(message)
          return { success: false, message }
        }

        this.currentOrder = response.result

        return { success: true, data: response.result }
      } catch (error) {
        const message = getErrorMessage(error)
        toast.error(message)
        return { success: false, message }
      } finally {
        this.loading = false
      }
    },

    async createOrder(orderData) {
      try {
        this.loading = true

        const response = await axiosInstance.post(
          ORDER_URL,
          buildCreatePayload(orderData)
        )

        if (!response?.result) {
          const message =
            response?.errorMessage || 'Erro ao criar pedido'
          toast.error(message)
          return { success: false, message }
        }

        toast.success('Pedido criado com sucesso!')

        await this.fetchOrders(
          this.pagination.currentPage,
          this.pagination.pageSize
        )

        return { success: true, data: response.result }
      } catch (error) {
        const message = getErrorMessage(error)
        toast.error(message)
        return { success: false, message }
      } finally {
        this.loading = false
      }
    },

    async updateOrder(id, orderData) {
      try {
        this.loading = true

        const response = await axiosInstance.put(
          `${ORDER_URL}/${id}`,
          buildUpdatePayload(orderData)
        )

        if (!response?.result) {
          const message =
            response?.errorMessage || 'Erro ao atualizar pedido'
          toast.error(message)
          return { success: false, message }
        }

        toast.success('Pedido atualizado com sucesso!')

        await this.fetchOrders(
          this.pagination.currentPage,
          this.pagination.pageSize
        )

        return { success: true, data: response.result }
      } catch (error) {
        const message = getErrorMessage(error)
        toast.error(message)
        return { success: false, message }
      } finally {
        this.loading = false
      }
    },

    async closeOrder(id) {
      try {
        this.loading = true

        await axiosInstance.put(`${ORDER_URL}/${id}/close`)

        toast.success('Pedido fechado com sucesso!')

        await this.fetchOrders(
          this.pagination.currentPage,
          this.pagination.pageSize
        )

        return { success: true }
      } catch (error) {
        const message = getErrorMessage(error)
        toast.error(message)
        return { success: false, message }
      } finally {
        this.loading = false
      }
    },

    async deleteOrder(id) {
      try {
        this.loading = true

        await axiosInstance.delete(`${ORDER_URL}/${id}`)

        toast.success('Pedido excluído com sucesso!')

        await this.fetchOrders(
          this.pagination.currentPage,
          this.pagination.pageSize
        )

        return { success: true }
      } catch (error) {
        const message = getErrorMessage(error)
        toast.error(message)
        return { success: false, message }
      } finally {
        this.loading = false
      }
    },

    clearCurrentOrder() {
      this.currentOrder = null
    }
  },

  persist: true
})