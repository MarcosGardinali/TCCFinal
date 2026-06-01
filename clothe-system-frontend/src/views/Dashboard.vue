<template>
  <div class="dashboard">
    <div class="dashboard-header">
      <h1 class="dashboard-title">Dashboard</h1>
      <p class="dashboard-subtitle">Visão geral do seu negócio</p>
    </div>

    <!-- Stats Cards -->
    <div class="stats-grid">
      <div class="stat-card orders">
        <div class="stat-icon">
          <i class="fas fa-shopping-cart"></i>
        </div>
        <div class="stat-content">
          <h3 class="stat-value">
            <span v-if="loading.stats" class="loading-text">...</span>
            <span v-else>{{ stats.orders }}</span>
          </h3>
          <p class="stat-label">Total de Pedidos</p>
          <span class="stat-change positive">Condicionais cadastradas</span>
        </div>
      </div>

      <div class="stat-card customers">
        <div class="stat-icon">
          <i class="fas fa-users"></i>
        </div>
        <div class="stat-content">
          <h3 class="stat-value">
            <span v-if="loading.stats" class="loading-text">...</span>
            <span v-else>{{ stats.customers }}</span>
          </h3>
          <p class="stat-label">Total de Clientes</p>
          <span class="stat-change positive">Clientes cadastrados</span>
        </div>
      </div>

      <div class="stat-card products">
        <div class="stat-icon">
          <i class="fas fa-box"></i>
        </div>
        <div class="stat-content">
          <h3 class="stat-value">
            <span v-if="loading.stats" class="loading-text">...</span>
            <span v-else>{{ stats.products }}</span>
          </h3>
          <p class="stat-label">Total de Produtos</p>
          <span class="stat-change neutral">Produtos no catálogo</span>
        </div>
      </div>

      <div class="stat-card revenue">
        <div class="stat-icon">
          <i class="fas fa-dollar-sign"></i>
        </div>
        <div class="stat-content">
          <h3 class="stat-value">
            <span v-if="loading.stats" class="loading-text">...</span>
            <span v-else>{{ stats.revenue }}</span>
          </h3>
          <p class="stat-label">Receita do Mês</p>
          <span class="stat-change positive">Itens vendidos</span>
        </div>
      </div>
    </div>

    <!-- Charts and Map Section -->
    <div class="dashboard-content">
      <!-- First Row: Chart and Map -->
      <div class="dashboard-row">
        <!-- Orders Chart -->
        <div class="chart-card">
          <div class="card-header">
            <h3 class="card-title">
              <i class="fas fa-chart-bar"></i>
              Pedidos nos Últimos 30 Dias
            </h3>
          </div>
          <div class="card-content">
            <BarChart
              id="orders-chart"
              :data="chartData"
              :labels="chartLabels"
              chart-title="Pedidos"
              :colors="['#7FB3D3']"
            />
          </div>
        </div>

        <!-- Customers Map -->
        <div class="map-card">
          <div class="card-header">
            <h3 class="card-title">
              <i class="fas fa-map-marker-alt"></i>
              Localização dos Clientes
            </h3>
            <div class="card-actions">
              <span class="customer-count">
                <span v-if="loading.customers">Carregando...</span>
                <span v-else>{{ customersWithOpenOrders.length }} clientes com condicionais ativas</span>
              </span>
            </div>
          </div>
          <div class="card-content">
            <CustomersMap :customers="customersWithOpenOrders" />
          </div>
        </div>
      </div>

        <!-- Quick Actions -->
        <div class="actions-card">
          <div class="card-header">
            <h3 class="card-title">
              <i class="fas fa-bolt"></i>
              Ações Rápidas
            </h3>
          </div>
          <div class="card-content">
            <div class="actions-grid">
              <router-link to="/customers/new" class="action-item">
                <div class="action-icon customers">
                  <i class="fas fa-user-plus"></i>
                </div>
                <div class="action-content">
                  <h4 class="action-title">Novo Cliente</h4>
                  <p class="action-description">Cadastrar cliente</p>
                </div>
              </router-link>

              <router-link to="/products/new" class="action-item">
                <div class="action-icon products">
                  <i class="fas fa-plus-circle"></i>
                </div>
                <div class="action-content">
                  <h4 class="action-title">Novo Produto</h4>
                  <p class="action-description">Cadastrar produto</p>
                </div>
              </router-link>

              <router-link to="/orders/new" class="action-item">
                <div class="action-icon orders">
                  <i class="fas fa-shopping-bag"></i>
                </div>
                <div class="action-content">
                  <h4 class="action-title">Novo Pedido</h4>
                  <p class="action-description">Criar pedido</p>
                </div>
              </router-link>

              <router-link to="/reports" class="action-item">
                <div class="action-icon orders">
                  <i class="fas fa-chart-line"></i>
                </div>
                <div class="action-content">
                  <h4 class="action-title">Relatórios</h4>
                  <p class="action-description">Acessar relatórios</p>
                </div>
              </router-link>
            </div>
          </div>
        </div>
    </div>
  </div>
</template>

<script>
import { dashboardService } from '@/api/dashboard'
import BarChart from '@/components/shared/Charts/BarChart/BarChart.vue'
import CustomersMap from '@/components/maps/CustomersMap.vue'
import GenericCalendarInput from '@/components/shared/GenericCalendarInput.vue'

export default {
  name: 'Dashboard',
  components: {
    BarChart,
    CustomersMap,
    GenericCalendarInput
  },

  data() {
    return {
      stats: {
        customers: 0,
        products: 0,
        orders: 0,
        revenue: 'R$ 0,00'
      },
      dateRange: [
        this.getTodayDate(),
        this.getTodayDate()
      ],
      ordersChartData: [],
      customersWithOpenOrdersList: [],
      loading: {
        stats: false,
        chart: false,
        customers: false
      },
    }
  },
  computed: {
    chartData() {
      return this.ordersChartData.map(item => item.count || 0)
    },
    chartLabels() {
      return this.ordersChartData.map(item => {
        const date = new Date(item.date)
        return date.toLocaleDateString('pt-BR', { day: '2-digit', month: 'short' })
      })
    },
    customersWithOpenOrders() {
      return this.customersWithOpenOrdersList || []
    }
  },
  async mounted() {
    await this.loadDashboard()
  },
  methods: {
    async loadDashboard() {
      this.loading.stats = true
      this.loading.chart = true
      this.loading.customers = true
      try {
        const res = await dashboardService.getDashboardAll()
        console.log('Dashboard response:', res)
        if (res.success && res.data) {
          const data = res.data
          console.log('Dashboard data:', data)

          // Stats
          const metrics = data.orderMetrics || {}
          this.stats.orders = metrics.totalOrders || 0
          this.stats.revenue = this.formatCurrency(metrics.totalRevenue || 0)
          this.stats.customers = (data.customerMetrics && data.customerMetrics.totalCustomers) || 0
          this.stats.products = data.totalProducts || 0

          // Chart data (orders per day)
          const ordersPerDay = (metrics.ordersPerDay || []).map(item => ({ date: item.date || item.Date, count: item.quantity || item.Quantity }))
          this.ordersChartData = ordersPerDay

          // Map markers: active orders with customer included
          this.customersWithOpenOrdersList = data.activeOrders || []
          console.log('Orders para mapa:', this.customersWithOpenOrdersList)
          console.log('Total de orders:', this.customersWithOpenOrdersList.length)
          if (this.customersWithOpenOrdersList.length > 0) {
            console.log('Primeiro order:', this.customersWithOpenOrdersList[0])
            console.log('Tem customer?', this.customersWithOpenOrdersList[0].customer)
          }
        } else {
          console.error('Response sem sucesso:', res)
          this.generateFallbackChartData()
        }
      } catch (error) {
        console.error('Erro ao carregar dashboard:', error)
        this.generateFallbackChartData()
      } finally {
        this.loading.stats = false
        this.loading.chart = false
        this.loading.customers = false
      }
    },
    
    generateFallbackChartData() {
      const data = []
      
      if (!this.dateRange || !Array.isArray(this.dateRange) || this.dateRange.length < 2) {
        // Usar período padrão se não houver dateRange válido
        const endDate = new Date()
        const startDate = new Date()
        startDate.setDate(startDate.getDate() - 30)
        
        for (let d = new Date(startDate); d <= endDate; d.setDate(d.getDate() + 1)) {
          data.push({
            date: new Date(d).toISOString(),
            count: 0
          })
        }
      } else {
        const startDate = new Date(this.dateRange[0])
        const endDate = new Date(this.dateRange[1])
        
        for (let d = new Date(startDate); d <= endDate; d.setDate(d.getDate() + 1)) {
          data.push({
            date: new Date(d).toISOString(),
            count: 0
          })
        }
      }
      
      this.ordersChartData = data
    },


    formatCurrency(value) {
      return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
      }).format(value)
    },

    getDateDaysAgo(days) {
      const date = new Date()
      date.setDate(date.getDate() - days)
      return new Date(date.getFullYear(), date.getMonth(), date.getDate())
    },

    getTodayDate() {
      const today = new Date()
      return new Date(today.getFullYear(), today.getMonth(), today.getDate())
    },

    formatTime(timestamp) {
      const now = new Date()
      const time = new Date(timestamp)
      const diffInMinutes = Math.floor((now - time) / (1000 * 60))

      if (diffInMinutes < 1) {
        return 'Agora mesmo'
      } else if (diffInMinutes < 60) {
        return `${diffInMinutes} min atrás`
      } else if (diffInMinutes < 1440) {
        const hours = Math.floor(diffInMinutes / 60)
        return `${hours}h atrás`
      } else {
        const days = Math.floor(diffInMinutes / 1440)
        return `${days}d atrás`
      }
    }
  }
}
</script>

<style lang="scss" scoped>
.dashboard {
  padding: $spacing-6;
}

.dashboard-header {
  margin-bottom: $spacing-6;
}

.dashboard-title {
  font-size: 2.5rem;
  font-weight: 700;
  color: $text-primary;
  margin-bottom: $spacing-1;
}

.dashboard-subtitle {
  color: $text-secondary;
  font-size: $font-size-lg;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: $spacing-6;
  margin-bottom: $spacing-6;
}

.stat-card {
  background: $surface;
  border: 1px solid $border;
  border-radius: $border-radius-xl;
  padding: $spacing-6;
  display: flex;
  align-items: center;
  gap: $spacing-5;
  transition: all 0.3s ease;

  &:hover {
    transform: translateY(-5px);
    box-shadow: $shadow-md;
  }
}

.stat-icon {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  color: $white;
  flex-shrink: 0;

  .orders & { background-color: $primary-color; }
  .customers & { background-color: $secondary-color; }
  .products & { background-color: $success-color; }
  .revenue & { background-color: $warning-color; }
}

.stat-content {
  flex: 1;
}

.stat-value {
  font-size: 1.75rem;
  font-weight: 600;
  color: $text-primary;
}

.stat-label {
  font-size: $font-size-sm;
  color: $text-secondary;
}

.stat-change {
  font-size: $font-size-xs;
  font-weight: 500;

  &.positive { color: $success-color; }
  &.negative { color: $error-color; }
  &.neutral { color: $text-muted; }
}

.dashboard-content {
  display: flex;
  flex-direction: column;
  gap: $spacing-6;
}

.dashboard-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: $spacing-6;
}

.chart-card, .map-card, .actions-card, .activity-card {
  background: $surface;
  border: 1px solid $border;
  border-radius: $border-radius-xl;
  box-shadow: $shadow-sm;
  overflow: hidden;
}

.card-header {
  padding: $spacing-4 $spacing-5;
  border-bottom: 1px solid $border;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.card-title {
  font-size: $font-size-lg;
  font-weight: 600;
  color: $text-primary;
  margin: 0;
  display: flex;
  align-items: center;
  gap: $spacing-3;
}

.card-actions .customer-count {
  font-size: $font-size-sm;
  color: $text-secondary;
}

.card-content {
  padding: $spacing-5;
}

.actions-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: $spacing-4;
}

.action-item {
  display: flex;
  align-items: center;
  gap: $spacing-4;
  padding: $spacing-3;
  border-radius: $border-radius-md;
  text-decoration: none;
  color: inherit;
  transition: background-color 0.2s ease;

  &:hover {
    background: $gray-50;
  }
}

.action-icon {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.1rem;
  color: $white;
  flex-shrink: 0;

  &.customers { background: $primary-color; }
  &.products { background: $success-color; }
  &.orders { background: $secondary-color; }
}

.action-title {
  font-weight: 500;
  color: $text-primary;
  margin: 0;
}

.action-description {
  font-size: $font-size-sm;
  color: $text-secondary;
  margin: 0;
}

.loading-text {
  color: $text-muted;
  animation: pulse 1.5s ease-in-out infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.6; }
}

// Responsive
@media (max-width: 1024px) {
  .dashboard-row {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .stats-grid, .actions-grid {
    grid-template-columns: 1fr;
  }

  .dashboard-title {
    font-size: 2rem;
  }

  .stat-card {
    flex-direction: column;
    text-align: center;
  }
}
</style>
