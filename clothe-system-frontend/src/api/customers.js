import axios from './axios'

const searchCustomers = async (filter) => {
  const response = await axios.post('/customer/search', filter)
  return response.result
}

const getCustomerById = async (id) => {
  const response = await axios.get(`/customer/${id}`)
  return response.result
}

const createCustomer = async (customer) => {
  const response = await axios.post('/customer', customer)
  return response.result
}

const updateCustomer = async (id, customer) => {
  const response = await axios.put(`/customer/${id}`, customer)
  return response.result
}

const deleteCustomer = async (id) => {
  const response = await axios.delete(`/customer/${id}`)
  return response.result
}

const getTopOrdersCustomer = async () => {
    const response = await axios.get('/customer/top-orders')
    return response.result
}

const getCustomersCount = async () => {
    const response = await axios.get('/customer/count')
    return response.result
}

export default {
    searchCustomers,
    getCustomerById,
    createCustomer,
    updateCustomer,
    deleteCustomer,
    getTopOrdersCustomer,
    getCustomersCount
}