import axios from './axios';

export const getCustomerMetrics = () => {
  return axios.get('/Customer/metrics');
};

export const getTopCustomer = () => {
  return axios.get('/Customer/top-orders');
};

export const getOrderMetrics = (startDate, endDate) => {
  return axios.get('/Order/metrics', { params: { startDate, endDate } });
};

export const getSalesMetrics = (startDate, endDate) => {
  return axios.get('/Order/sales-metrics', { params: { startDate, endDate } });
};

export const getProductMetrics = () => {
  return axios.get('/Product/metrics');
};

export const getTopProduct = () => {
  return axios.get('/Product/top-orders');
};
