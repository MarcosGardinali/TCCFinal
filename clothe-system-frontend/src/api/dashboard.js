import axiosInstance from './axios'
import { StateNumbers } from '@/utils/enums/states'

export const dashboardService = {
  // Buscar métricas completas
  async getCompleteMetrics() {
    try {
      const response = await axiosInstance.get('/order/metrics')
      return { success: true, data: response.result }
    } catch (error) {
      console.error('Erro ao buscar métricas completas:', error)
      return { success: false, error: error.message }
    }
  },

  // Buscar total de clientes
  async getTotalCustomers() {
    try {
      const response = await axiosInstance.get('/customer/count')
      return { success: true, data: response.result }
    } catch (error) {
      console.error('Erro ao buscar total de clientes:', error)
      return { success: false, error: error.message }
    }
  },

  // Buscar total de produtos
  async getTotalProducts() {
    try {
      const response = await axiosInstance.get('/product/count')
      return { success: true, data: response.result }
    } catch (error) {
      console.error('Erro ao buscar total de produtos:', error)
      return { success: false, error: error.message }
    }
  },

  // Buscar pedidos para gráfico por período
  async getOrdersForChart(startDate, endDate) {
    try {
      const response = await axiosInstance.post('/order/search', {
        dateFrom: startDate,
        dateTo: endDate
      }, {
        params: { pageNumber: 1, pageSize: 1000 }
      })
      return { success: true, data: response.result?.items || [] }
    } catch (error) {
      console.error('Erro ao buscar pedidos para gráfico:', error)
      return { success: false, error: error.message }
    }
  },

  // Buscar clientes com condicionais ativas
  async getCustomersWithActiveOrders() {
    try {

      
      // Buscar todos os clientes e pedidos
      const [customersResponse, ordersResponse] = await Promise.all([
        axiosInstance.post('/customer/search', {}, { params: { pageNumber: 1, pageSize: 1000 } }),
        axiosInstance.post('/order/search', {}, { params: { pageNumber: 1, pageSize: 1000 } })
      ])
      
      const customers = customersResponse.result.items
      const orders = ordersResponse.result.items
      
      // Filtrar pedidos com status ativo (0 = Pending, 1 = AwaitingClosure)
      const activeOrders = orders.filter(order => order.status === 0 || order.status === 1)
      
      // Obter IDs únicos de clientes com pedidos ativos
      const customerIdsWithActiveOrders = new Set(
        activeOrders.map(order => order.customerId)
      )
      
      // Filtrar clientes que têm pedidos ativos e processar dados
      const customersWithActiveOrders = customers
        .filter(customer => customerIdsWithActiveOrders.has(customer.id))
        .map(customer => ({
          ...customer,
          stateAbbreviation: StateNumbers[customer.stateAbbreviation] || customer.stateAbbreviation
        }))
      
      return { success: true, data: customersWithActiveOrders }
    } catch (error) {
      console.error('Erro ao buscar clientes com pedidos ativos:', error)
      return { success: false, error: error.message }
    }
  },
  // Buscar todos os dados do dashboard em uma única requisição (mês atual)
  async getDashboardAll() {
    try {
      const response = await axiosInstance.get('/dashboard')
      // Normalize response shape: some serializers use `result` or `Result`
      const payload = response?.result ?? response?.Result ?? response
      return { success: true, data: payload }
    } catch (error) {
      console.error('Erro ao buscar dados completos do dashboard:', error)
      return { success: false, error: error.message }
    }
  }
}