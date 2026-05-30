<template>
  <div class="customers-report-view">
    <div class="page-header">
      <div class="header-content">
        <h1 class="page-title">Relatório de Clientes</h1>
        <p class="page-subtitle">Análise completa dos clientes cadastrados</p>
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

    <!-- Summary Cards -->
    <div class="summary-cards-customer">
      <div class="summary-card">
        <div class="card-icon customers">
          <i class="fas fa-users"></i>
        </div>
        <div class="card-content">
          <h3 class="card-value">{{ summary.totalCustomers }}</h3>
          <p class="card-label">Total de Clientes</p>
        </div>
      </div>

      <div class="summary-card">
        <div class="card-icon active">
          <i class="fas fa-user-check"></i>
        </div>
        <div class="card-content">
          <h3 class="card-value">{{ summary.activeCustomers }}</h3>
          <p class="card-label">Clientes Ativos</p>
          <span class="card-change positive">{{ summary.activePercentage }}% do total</span>
        </div>
      </div>

      <div class="summary-card">
        <div class="card-icon age">
          <i class="fas fa-birthday-cake"></i>
        </div>
        <div class="card-content">
          <h3 class="card-value">{{ summary.averageAge }}</h3>
          <p class="card-label">Idade Média</p>
        </div>
      </div>

      <div class="summary-card" v-if="topCustomer.firstName">
        <div class="card-icon top-customer">
          <i class="fas fa-trophy"></i>
        </div>
        <div class="card-content">
          <h3 class="card-value">{{ topCustomer.firstName }} {{ topCustomer.lastName }}</h3>
          <p class="card-label">Cliente com Mais Pedidos</p>
        </div>
      </div>
    </div>

    <!-- Charts Section -->
    <div class="charts-section-customer">
      <div class="chart-card">
        <div class="card-header">
          <h3 class="card-title">
            <i class="fas fa-chart-line"></i>
            Cadastros por Mês
          </h3>
        </div>
        <div class="card-content">
          <LineChart
            id="customers-chart"
            :data="registrationsData"
            :labels="registrationsLabels"
            chart-title="Novos Clientes"
            color="#10b981"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import BaseButton from '@/components/shared/BaseButton/BaseButton.vue'
import LineChart from '@/components/shared/Charts/LineChart/LineChart.vue'
import { getCustomerMetrics, getTopCustomer } from '@/api/reports.js'

export default {
  name: 'CustomersReportView',
  components: {
    BaseButton,
    LineChart,
  },
  setup() {
    const loading = ref(true)

    const summary = ref({
      totalCustomers: 0,
      activeCustomers: 0,
      averageAge: 0,
      activePercentage: 0,
    })

    const topCustomer = ref({})

    const chartData = ref({
      registrations: [],
    })

    const monthLabels = {
      1: 'Jan', 2: 'Fev', 3: 'Mar', 4: 'Abr', 5: 'Mai', 6: 'Jun',
      7: 'Jul', 8: 'Ago', 9: 'Set', 10: 'Out', 11: 'Nov', 12: 'Dez'
    };

    const loadReportData = async () => {
      try {
        loading.value = true
        
        const metricsResponse = await getCustomerMetrics()
        if (metricsResponse && metricsResponse.result) {
          const metrics = metricsResponse.result
          summary.value.totalCustomers = metrics.totalCustomers
          summary.value.activeCustomers = metrics.activeCustomersCount
          summary.value.averageAge = metrics.averageAge
          summary.value.activePercentage = metrics.totalCustomers > 0 
            ? Math.round((metrics.activeCustomersCount / metrics.totalCustomers) * 100)
            : 0

          if (metrics.newCustomersPerMonth) {
            chartData.value.registrations = metrics.newCustomersPerMonth;
          }
        }

        const topCustomerResponse = await getTopCustomer()
        if (topCustomerResponse && topCustomerResponse.result) {
          topCustomer.value = topCustomerResponse.result
        }

      } catch (error) {
        console.error('Error loading customer report data:', error)
      } finally {
        loading.value = false
      }
    }

    onMounted(loadReportData)

    const registrationsData = computed(() => {
      return chartData.value.registrations.map(item => item.quantity);
    });

    const registrationsLabels = computed(() => {
      return chartData.value.registrations.map(item => `${monthLabels[item.month]}/${item.year}`);
    });

    const exportReport = () => {
      // Implementar exportação para PDF
      alert('Funcionalidade de exportação será implementada')
    }

    return {
      summary,
      topCustomer,
      registrationsData,
      registrationsLabels,
      exportReport,
      loading
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/styles/variables.scss';
@import './ReportStyles.scss';

.summary-card .card-icon.top-customer {
  background-color: #fff5cc;
  color: #ffc107;
}

</style>
