import axios from './axios'

const searchProducts = async (filter) => {
  const response = await axios.post('/product/search', filter)
  return response.result
}

const getProductById = async (id) => {
  const response = await axios.get(`/product/${id}`)
  return response.result
}

const createProduct = async (product) => {
  const response = await axios.post('/product', product)
  return response.result
}

const updateProduct = async (id, product) => {
  const response = await axios.put(`/product/${id}`, product)
  return response.result
}

const deleteProduct = async (id) => {
  const response = await axios.delete(`/product/${id}`)
  return response.result
}

const getTopOrdersProduct = async () => {
    const response = await axios.get('/product/top-orders')
    return response.result
}

const getStockLevels = async () => {
    const response = await axios.get('/product/stock-levels')
    return response.result
}

const getSummary = async () => {
    const response = await axios.get('/product/summary')
    return response.result
}

export default {
  searchProducts,
  getProductById,
  createProduct,
  updateProduct,
  deleteProduct,
  getTopOrdersProduct,
  getStockLevels,
  getSummary
}
