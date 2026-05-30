import axios from './axios'

const searchOrders = async (filter) => {
  const response = await axios.post('/order/search', filter)
  return response.result
}

const getOrderById = async (id) => {
  const response = await axios.get(`/order/${id}`)
  return response.result
}

const createOrder = async (order) => {
  const response = await axios.post('/order', order)
  return response.result
}

const updateOrder = async (id, order) => {
  const response = await axios.put(`/order/${id}`, order)
  return response.result
}

const deleteOrder = async (id) => {
  const response = await axios.delete(`/order/${id}`)
  return response.result
}

const getCompleteMetrics = async () => {
    const response = await axios.get('/order/metrics')
    return response.result
}

export default {
    searchOrders,
    getOrderById,
    createOrder,
    updateOrder,
    deleteOrder,
    getCompleteMetrics
}