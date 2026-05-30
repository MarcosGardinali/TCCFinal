import { defineStore } from 'pinia'
import axiosInstance from '@/api/axios'
import { useToast } from 'vue-toastification'
import { BrazilianStates, StateNumbers } from '@/utils/enums/states'

const toast = useToast()
const CUSTOMER_URL = '/Customer'

/* ========================
   Helpers
======================== */

const defaultState = () => ({
  customers: [],
  currentCustomer: null,
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

const processCustomerData = (customer) => {
  if (!customer) return customer
  return {
    ...customer,
    stateAbbreviation:
      StateNumbers[customer.stateAbbreviation] || customer.stateAbbreviation
  }
}

const processCustomerList = (customers) =>
  Array.isArray(customers)
    ? customers.map(processCustomerData)
    : customers

const buildCustomerPayload = (customerData) => ({
  firstName: customerData.firstName,
  lastName: customerData.lastName,
  birthDate: customerData.birthDate,
  cpf: customerData.cpf,
  street: customerData.street,
  complement: customerData.complement,
  neighborhood: customerData.neighborhood,
  number: customerData.number,
  cityName: customerData.cityName,
  stateAbbreviation:
    BrazilianStates[customerData.stateAbbreviation],
  postalCode: customerData.postalCode,
  rg: customerData.rg,
  mobilePhoneNumber: customerData.mobilePhoneNumber,
  email: customerData.email
})

/* ========================
   Store
======================== */

export const useCustomerStore = defineStore('customerStore', {
  state: () => defaultState(),

  getters: {
    isLoading: (state) => state.loading,
    getCustomerById: (state) => (id) =>
      state.customers.find(
        (customer) => customer.id === parseInt(id)
      )
  },

  actions: {
    async fetchCustomers(page = 1, pageSize = 10, silent = false) {
      try {
        this.loading = true

        const response = await axiosInstance.post(
          `${CUSTOMER_URL}/search`,
          { orderByDescending: false },
          { params: { pageNumber: page, pageSize } }
        )

        const result = response.result

        if (result) {
          this.customers = processCustomerList(result.items || [])

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

    async fetchCustomerById(id) {
      try {
        this.loading = true

        const response = await axiosInstance.get(
          `${CUSTOMER_URL}/${id}`
        )

        if (!response?.result) {
          const message =
            response?.errorMessage || 'Erro ao carregar cliente'
          toast.error(message)
          return { success: false, message }
        }

        this.currentCustomer = processCustomerData(response.result)

        return { success: true, data: response.result }
      } catch (error) {
        const message = getErrorMessage(error)
        toast.error(message)
        return { success: false, message }
      } finally {
        this.loading = false
      }
    },

    async createCustomer(customerData) {
      try {
        this.loading = true

        const response = await axiosInstance.post(
          CUSTOMER_URL,
          buildCustomerPayload(customerData)
        )

        if (!response?.result) {
          const message =
            response?.errorMessage || 'Erro ao criar cliente'
          toast.error(message)
          return { success: false, message }
        }

        toast.success('Cliente criado com sucesso!')
        await this.fetchCustomers(
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

    async updateCustomer(id, customerData) {
      try {
        this.loading = true

        const response = await axiosInstance.put(
          `${CUSTOMER_URL}/${id}`,
          buildCustomerPayload(customerData)
        )

        if (!response?.result) {
          const message =
            response?.errorMessage || 'Erro ao atualizar cliente'
          toast.error(message)
          return { success: false, message }
        }

        toast.success('Cliente atualizado com sucesso!')
        await this.fetchCustomers(
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

    async deleteCustomer(id) {
      try {
        this.loading = true

        await axiosInstance.delete(`${CUSTOMER_URL}/${id}`)

        toast.success('Cliente excluído com sucesso!')
        await this.fetchCustomers(
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

    clearCurrentCustomer() {
      this.currentCustomer = null
    }
  },

  persist: true
})