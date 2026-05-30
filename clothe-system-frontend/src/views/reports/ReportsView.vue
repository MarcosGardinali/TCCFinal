<template>
  <div class="reports-view">
    <div class="page-header">
      <div class="header-content">
        <h1 class="page-title">Relatórios</h1>
        <p class="page-subtitle">Análises e relatórios do sistema</p>
      </div>
    </div>

    <div class="reports-grid">
      <!-- Customer Reports -->
      <div class="report-card">
        <div class="report-icon customers">
          <i class="fas fa-users"></i>
        </div>
        <div class="report-content">
          <h3 class="report-title">Relatório de Clientes</h3>
          <p class="report-description">Análise completa dos clientes cadastrados</p>
        </div>
        <div class="report-actions">
          <BaseButton
            variant="primary"
            icon="chart-bar"
            @click="navigateToReport('customers')"
          >
            Ver Relatório
          </BaseButton>
        </div>
      </div>

      <!-- Product Reports -->
      <div class="report-card">
        <div class="report-icon products">
          <i class="fas fa-box"></i>
        </div>
        <div class="report-content">
          <h3 class="report-title">Relatório de Produtos</h3>
          <p class="report-description">Análise de produtos e estoque</p>
        </div>
        <div class="report-actions">
          <BaseButton
            variant="primary"
            icon="chart-bar"
            @click="navigateToReport('products')"
          >
            Ver Relatório
          </BaseButton>
        </div>
      </div>

      <!-- Order Reports -->
      <div class="report-card">
        <div class="report-icon orders">
          <i class="fas fa-shopping-cart"></i>
        </div>
        <div class="report-content">
          <h3 class="report-title">Relatório de Pedidos</h3>
          <p class="report-description">Análise de pedidos por período</p>
        </div>
        <div class="report-actions">
          <BaseButton
            variant="primary"
            icon="chart-bar"
            @click="navigateToReport('orders')"
          >
            Ver Relatório
          </BaseButton>
        </div>
      </div>

      <!-- Sales Reports -->
      <div class="report-card">
        <div class="report-icon sales">
          <i class="fas fa-dollar-sign"></i>
        </div>
        <div class="report-content">
          <h3 class="report-title">Relatório de Vendas</h3>
          <p class="report-description">Análise de vendas e faturamento</p>
        </div>
        <div class="report-actions">
          <BaseButton
            variant="primary"
            icon="chart-bar"
            @click="navigateToReport('sales')"
          >
            Ver Relatório
          </BaseButton>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import BaseButton from '@/components/shared/BaseButton/BaseButton.vue'

export default {
  name: 'ReportsView',
  components: {
    BaseButton
  },
  setup() {
    const router = useRouter()

    // Mock data for statistics
    const mockStats = ref({
      totalCustomers: 156,
      newCustomersThisMonth: 23,
      totalProducts: 89,
      lowStockProducts: 12,
      totalOrders: 342,
      pendingOrders: 18,
      totalRevenue: 45280.50,
      revenueGrowth: 15.3
    })

    const navigateToReport = (reportType) => {
      router.push(`/reports/${reportType}`)
    }

    const formatCurrency = (value) => {
      return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
      }).format(value).replace('R$', '').trim()
    }

    return {
      mockStats,
      navigateToReport,
      formatCurrency
    }
  }
}
</script>

<style lang="scss" scoped>
.reports-view {
  padding: $spacing-6;
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  text-align: center;
  margin-bottom: $spacing-6;
}

.page-title {
  font-size: 2.5rem;
  font-weight: 700;
  color: $text-primary;
  margin-bottom: $spacing-1;
}

.page-subtitle {
  color: $text-secondary;
  font-size: $font-size-lg;
}

.reports-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: $spacing-6;
}

.report-card {
  background: $surface;
  border-radius: $border-radius-xl;
  border: 1px solid $border;
  box-shadow: $shadow-sm;
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  padding-top: 0.5rem;

  &:hover {
    transform: translateY(-5px);
    box-shadow: $shadow-md;
  }
}

.report-card-header {
  display: flex;
  align-items: center;
  gap: $spacing-5;
  padding: $spacing-5;
}

.report-icon {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  color: $white;
  flex-shrink: 0;
  margin: 0.5rem;

  &.customers { background: $primary-color; }
  &.products { background: $success-color; }
  &.orders { background: $secondary-color; }
  &.sales { background: $warning-color; }
}

.report-title {
  font-size: $font-size-xl;
  font-weight: 600;
  color: $text-primary;
  margin: 0;
}

.report-content {
  padding: 0 $spacing-5 $spacing-5;
  flex: 1;
}

.report-description {
  color: $text-secondary;
  margin: 0 0 $spacing-4 0;
  line-height: 1.5;
}

.report-stats {
  display: flex;
  flex-direction: column;
  gap: $spacing-3;
  margin-bottom: $spacing-5;
}

.stat-item {
  display: flex;
  align-items: center;
  gap: $spacing-3;
  font-size: $font-size-sm;
  color: $text-primary;
  
  i {
    color: $text-secondary;
    width: 16px;
    text-align: center;
  }
}

.report-actions {
  display: flex;
  padding: $spacing-4;
  background-color: $gray-50;
  border-top: 1px solid $border;
}

// Responsive
@media (max-width: 768px) {
  .reports-view {
    padding: $spacing-4;
  }
  
  .reports-grid {
    grid-template-columns: 1fr;
    gap: $spacing-4;
  }
  
  .page-title {
    font-size: 2rem;
  }
}
</style>
