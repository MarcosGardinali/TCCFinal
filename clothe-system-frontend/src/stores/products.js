import { defineStore } from 'pinia'
import axiosInstance from '@/api/axios'
import { useToast } from 'vue-toastification'

const toast = useToast()
const PRODUCT_URL = '/Product'

/* ========================
   Helpers
======================== */

const defaultState = () => ({
  products: [],
  currentProduct: null,
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

const buildProductPayload = (productData) => ({
  code: productData.code,
  name: productData.name,
  description: productData.description,
  price: productData.price,
  stockQuantity: productData.stockQuantity,
  isActive: productData.isActive,
  brand: productData.brand,
  category: productData.category
})

/* ========================
   Store
======================== */

export const useProductStore = defineStore('productStore', {
  state: () => defaultState(),

  getters: {
    isLoading: (state) => state.loading,
    getProductById: (state) => (id) =>
      state.products.find(product => product.id === parseInt(id))
  },

  actions: {
    async fetchProducts(page = 1, pageSize = 10, silent = false) {
      try {
        this.loading = true

        const response = await axiosInstance.post(
          `${PRODUCT_URL}/search`,
          { orderByDescending: false },
          { params: { pageNumber: page, pageSize } }
        )

        const result = response.result

        if (result) {
          this.products = result.items || []

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
        if (!silent) toast.error(message)
        return { success: false, message }
      } finally {
        this.loading = false
      }
    },

    async fetchProductById(id) {
      try {
        this.loading = true

        const response = await axiosInstance.get(
          `${PRODUCT_URL}/${id}`
        )

        if (!response?.result) {
          const message =
            response?.errorMessage || 'Erro ao carregar produto'
          toast.error(message)
          return { success: false, message }
        }

        this.currentProduct = response.result

        return { success: true, data: response.result }
      } catch (error) {
        const message = getErrorMessage(error)
        toast.error(message)
        return { success: false, message }
      } finally {
        this.loading = false
      }
    },

    async createProduct(productData) {
      try {
        this.loading = true

        const response = await axiosInstance.post(
          PRODUCT_URL,
          buildProductPayload(productData)
        )

        if (!response?.result) {
          const message =
            response?.errorMessage || 'Erro ao criar produto'
          toast.error(message)
          return { success: false, message }
        }

        toast.success('Produto criado com sucesso!')

        await this.fetchProducts(
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

    async updateProduct(id, productData) {
      try {
        this.loading = true

        const response = await axiosInstance.put(
          `${PRODUCT_URL}/${id}`,
          buildProductPayload(productData)
        )

        if (!response?.result) {
          const message =
            response?.errorMessage || 'Erro ao atualizar produto'
          toast.error(message)
          return { success: false, message }
        }

        toast.success('Produto atualizado com sucesso!')

        await this.fetchProducts(
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

    async deleteProduct(id) {
      try {
        this.loading = true

        await axiosInstance.delete(`${PRODUCT_URL}/${id}`)

        toast.success('Produto excluído com sucesso!')

        await this.fetchProducts(
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

    async searchProductByCode(code) {
      try {
        this.loading = true

        const response = await axiosInstance.post(
          `${PRODUCT_URL}/GetByIdentifier`,
          { code }
        )

        if (!response?.result) {
          const message =
            response?.errorMessage || 'Produto não encontrado'
          toast.error(message)
          return { success: false, message }
        }

        return { success: true, data: response.result }
      } catch (error) {
        const message = getErrorMessage(error)
        toast.error(message)
        return { success: false, message }
      } finally {
        this.loading = false
      }
    },

    clearCurrentProduct() {
      this.currentProduct = null
    }
  },

  persist: true
})