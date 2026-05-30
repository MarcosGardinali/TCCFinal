
<template>
  <div class="orders-report-view">
    <div class="page-header">
      <div class="header-content">
        <h1 class="page-title">Relatório de Pedidos</h1>
        <p class="page-subtitle">Análise de pedidos por período específico</p>
      </div>
      <div class="header-actions">
        <BaseButton
          variant="outline"
          icon="arrow-left"
          @click="$router.push('/reports')"
        >
          Voltar
        </BaseButton>
      </div>
    </div>

    <!-- Period Filter -->
    <div class="filter-section">
      <div class="filter-card">
        <div class="filter-header">
          <h3 class="filter-title">
            <i class="fas fa-calendar-alt"></i>
            Filtrar por Período
          </h3>
        </div>
        <div class="filter-content">
          <div class="date-inputs">
            <BaseInput
              v-model="filters.startDate"
              label="Data Inicial"
              type="date"
            />
            <BaseInput
              v-model="filters.endDate"
              label="Data Final"
              type="date"
            />
          </div>
          <BaseButton
            variant="primary"
            icon="search"
            @click="applyFilters"
            :disabled="loading"
          >
            {{ loading ? 'Gerando...' : 'Gerar Relatório' }}
          </BaseButton>
        </div>
      </div>
    </div>

    <!-- Summary Cards -->
    <div class="summary-cards-order" v-if="!loading && summary.totalOrders !== null">
      <div class="summary-card">
        <div class="card-icon orders">
          <i class="fas fa-shopping-cart"></i>
        </div>
        <div class="card-content">
          <h3 class="card-value">{{ summary.totalOrders }}</h3>
          <p class="card-label">Total de Pedidos</p>
        </div>
      </div>

      <div class="summary-card">
        <div class="card-icon revenue">
          <i class="fas fa-dollar-sign"></i>
        </div>
        <div class="card-content">
          <h3 class="card-value">R$ {{ formatCurrency(summary.totalRevenue) }}</h3>
          <p class="card-label">Receita Total</p>
        </div>
      </div>

      <div class="summary-card">
        <div class="card-icon pending">
          <i class="fas fa-clock"></i>
        </div>
        <div class="card-content">
          <h3 class="card-value">{{ summary.pendingOrdersCount }}</h3>
          <p class="card-label">Pedidos Pendentes</p>
        </div>
      </div>

       <div class="summary-card">
        <div class="card-icon ticket">
          <i class="fas fa-receipt"></i>
        </div>
        <div class="card-content">
          <h3 class="card-value">R$ {{ formatCurrency(summary.averageTicket) }}</h3>
          <p class="card-label">Ticket Médio</p>
        </div>
      </div>

      <div class="summary-card">
        <div class="card-icon conversion">
          <i class="fas fa-bullseye"></i>
        </div>
        <div class="card-content">
          <h3 class="card-value">{{ summary.conversionRate.toFixed(2) }}%</h3>
          <p class="card-label">Taxa de Conversão</p>
        </div>
      </div>

       <div class="summary-card">
        <div class="card-icon return">
          <i class="fas fa-undo"></i>
        </div>
        <div class="card-content">
          <h3 class="card-value">{{ summary.returnRate.toFixed(2) }}%</h3>
          <p class="card-label">Taxa de Devolução</p>
        </div>
      </div>
    </div>

    <!-- Charts Section -->
    <div class="charts-section-order" v-if="!loading && summary.totalOrders !== null">
      <div class="chart-card">
        <div class="card-header">
          <h3 class="card-title">
            <i class="fas fa-chart-bar"></i>
            Pedidos por Dia
          </h3>
        </div>
        <div class="card-content">
          <BarChart
            id="orders-timeline"
            :data="timelineData"
            :labels="timelineLabels"
            chart-title="Pedidos por Dia"
            color="#667eea"
          />
        </div>
      </div>

      <div class="chart-card">
        <div class="card-header">
          <h3 class="card-title">
            <i class="fas fa-chart-pie"></i>
            Status dos Pedidos
          </h3>
        </div>
        <div class="card-content">
          <DonutChart
            id="orders-status"
            :data="statusData"
            :labels="statusLabels"
          />
        </div>
      </div>
    </div>

  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import BaseButton from '@/components/shared/BaseButton/BaseButton.vue'
import BaseInput from '@/components/shared/BaseInput/BaseInput.vue'
import BarChart from '@/components/shared/Charts/BarChart/BarChart.vue'
import DonutChart from '@/components/shared/Charts/DonutChart/DonutChart.vue'
import { getOrderMetrics } from '@/api/reports.js'

export default {
  name: 'OrdersReportView',
  components: {
    BaseButton,
    BaseInput,
    BarChart,
    DonutChart
  },
  setup() {
    const loading = ref(false)

    const filters = ref({
      startDate: new Date(new Date().setDate(new Date().getDate() - 30)).toISOString().split('T')[0],
      endDate: new Date().toISOString().split('T')[0]
    })

    const summary = ref({
      totalOrders: null,
      totalRevenue: 0,
      pendingOrdersCount: 0,
      conversionRate: 0,
      averageTicket: 0,
      averageReturnTime: 0,
      returnRate: 0,
    })

    const chartData = ref({
      ordersPerDay: [],
      ordersByStatusCount: []
    })

    const loadReportData = async () => {
      if (!filters.value.startDate || !filters.value.endDate) {
        alert('Por favor, selecione as datas de início e fim.');
        return;
      }
      try {
        loading.value = true
        const startDate = `${filters.value.startDate}T00:00:00`;
        const endDate = `${filters.value.endDate}T23:59:59`;
        const response = await getOrderMetrics(startDate, endDate)
        
        if (response && response.result) {
          const { result } = response;
          summary.value = { ...result };
          chartData.value.ordersPerDay = result.ordersPerDay || [];
          chartData.value.ordersByStatusCount = result.ordersByStatusCount || [];
        }

      } catch (error) {
        console.error('Error loading order report data:', error)
      } finally {
        loading.value = false
      }
    }

    onMounted(loadReportData)

    const timelineData = computed(() => chartData.value.ordersPerDay.map(item => item.quantity))
    const timelineLabels = computed(() => chartData.value.ordersPerDay.map(item => new Date(item.date).toLocaleDateString('pt-BR')))

    const statusTranslations = {
      'Pending': 'Pendente',
      'Closed': 'Fechado'
    };

    const statusData = computed(() => chartData.value.ordersByStatusCount.map(item => item.quantity))
    const statusLabels = computed(() => chartData.value.ordersByStatusCount.map(item => statusTranslations[item.status] || item.status))

    const applyFilters = () => {
      loadReportData()
    }

    const formatCurrency = (value) => {
      if (typeof value !== 'number') return '0,00'
      return new Intl.NumberFormat('pt-BR', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      }).format(value)
    }

    return {
      filters,
      summary,
      loading,
      applyFilters,
      timelineData,
      timelineLabels,
      statusData,
      statusLabels,
      formatCurrency,
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/styles/variables.scss';
@import './ReportStyles.scss';

.filter-section {
  margin-bottom: $spacing-8;
}

.filter-card {
  background: white;
  border-radius: 24px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.08);
  overflow: hidden;
}

.filter-header {
  padding: $spacing-4 $spacing-6;
  border-bottom: 1px solid $gray-200;
}

.filter-title {
  font-size: $font-size-lg;
  font-weight: 700;
  color: $gray-900;
  margin: 0;
  display: flex;
  align-items: center;
  gap: $spacing-2;

  i {
    color: #667eea;
  }
}

.filter-content {
  padding: $spacing-6;
  display: flex;
  flex-direction: column;
  gap: $spacing-4;
}

.date-inputs {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: $spacing-4;
}

.summary-card .card-icon.pending {
  background-color: #fef3c7;
  color: #d97706;
}
.summary-card .card-icon.ticket {
  background-color: #e0f2fe;
  color: #0ea5e9;
}
.summary-card .card-icon.conversion {
  background-color: #dcfce7;
  color: #22c55e;
}
.summary-card .card-icon.return {
  background-color: #fee2e2;
  color: #ef4444;
}

@media (max-width: 768px) {
  .date-inputs {
    grid-template-columns: 1fr;
  }
}
</style>
