import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

// Layouts
import AuthLayout from '@/components/layout/AuthLayout.vue'
import MainLayout from '@/components/layout/MainLayout.vue'

// Views
import Login from '@/views/auth/Login/Login.vue'
import Dashboard from '@/views/Dashboard.vue'
import CustomerListView from '@/views/customers/CustomerListView.vue'
import CustomerFormView from '@/views/customers/CustomerFormView.vue'
import CustomerView from '@/views/customers/CustomerView.vue'
import ProductListView from '@/views/products/ProductListView/ProductListView.vue'
import ProductFormView from '@/views/products/ProductFormView/ProductFormView.vue'
import ProductView from '@/views/products/ProductView.vue'
import OrderListView from '@/views/orders/OrderListView/OrderListView.vue'
import OrderFormView from '@/views/orders/OrderFormView/OrderFormView.vue'
import OrderView from '@/views/orders/OrderView/OrderView.vue'

// Reports
import ReportsView from '@/views/reports/ReportsView.vue'
import CustomersReportView from '@/views/reports/CustomersReportView.vue'
import ProductsReportView from '@/views/reports/ProductsReportView.vue'
import OrdersReportView from '@/views/reports/OrdersReportView.vue'
import SalesReportView from '@/views/reports/SalesReportView.vue'

const routes = [
  {
    path: '/auth',
    component: AuthLayout,
    children: [
      {
        path: 'login',
        name: 'Login',
        component: Login,
        meta: { requiresGuest: true }
      }
    ]
  },
  {
    path: '/',
    component: MainLayout,
    meta: { requiresAuth: true },
    redirect: () => {
      const authStore = useAuthStore()
      return authStore.isAuthenticated ? '/dashboard' : '/auth/login'
    },
    children: [
      {
        path: 'dashboard',
        name: 'Dashboard',
        component: Dashboard
      },
      {
        path: 'customers',
        name: 'CustomerList',
        component: CustomerListView
      },
      {
        path: 'customers/new',
        name: 'CustomerCreate',
        component: CustomerFormView
      },
      {
        path: 'customers/:id/edit',
        name: 'CustomerEdit',
        component: CustomerFormView,
        props: true
      },
      {
        path: 'customers/:id',
        name: 'CustomerView',
        component: CustomerView,
        props: true
      },
      {
        path: 'products',
        name: 'ProductList',
        component: ProductListView
      },
      {
        path: 'products/new',
        name: 'ProductCreate',
        component: ProductFormView
      },
      {
        path: 'products/:id/edit',
        name: 'ProductEdit',
        component: ProductFormView,
        props: true
      },
      {
        path: 'products/:id',
        name: 'ProductView',
        component: ProductView,
        props: true
      },
      {
        path: 'orders',
        name: 'OrderList',
        component: OrderListView
      },
      {
        path: 'orders/new',
        name: 'OrderCreate',
        component: OrderFormView
      },
      {
        path: 'orders/:id/edit',
        name: 'OrderEdit',
        component: OrderFormView,
        props: true
      },
      {
        path: 'orders/:id',
        name: 'OrderView',
        component: OrderView,
        props: true
      },
      // Reports Routes
      {
        path: 'reports',
        name: 'Reports',
        component: ReportsView
      },
      {
        path: 'reports/customers',
        name: 'CustomersReport',
        component: CustomersReportView
      },
      {
        path: 'reports/products',
        name: 'ProductsReport',
        component: ProductsReportView
      },
      {
        path: 'reports/orders',
        name: 'OrdersReport',
        component: OrdersReportView
      },
      {
        path: 'reports/sales',
        name: 'SalesReport',
        component: SalesReportView
      }
    ]
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/'
  },
  {
    path: '/auth',
    redirect: '/auth/login'
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Guards de navegação
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/auth/login')
  } else if (to.meta.requiresGuest && authStore.isAuthenticated) {
    next('/')
  } else {
    next()
  }
})

export default router
