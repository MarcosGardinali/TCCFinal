<template>
  <div class="sales-report-view">
    <div class="page-header">
      <div class="header-content">
        <h1 class="page-title">Relatório de Vendas</h1>
        <p class="page-subtitle">Análise de vendas e faturamento com origem dos pedidos</p>
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
            Período de Análise
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
            @click="loadReportData"
            :disabled="loading"
          >
            {{ loading ? 'Gerando...' : 'Gerar Relatório' }}
          </BaseButton>
        </div>
      </div>
    </div>

    <!-- Summary Cards -->
    <div class="summary-cards-sale" v-if="!loading && summary.totalSalesCount !== null">
      <div class="summary-card">
        <div class="card-icon sales">
          <i class="fas fa-dollar-sign"></i>
        </div>
        <div class="card-content">
          <h3 class="card-value">R$ {{ formatCurrency(summary.totalRevenue) }}</h3>
          <p class="card-label">Receita Total</p>
        </div>
      </div>

      <div class="summary-card">
        <div class="card-icon orders">
          <i class="fas fa-shopping-cart"></i>
        </div>
        <div class="card-content">
          <h3 class="card-value">{{ summary.totalSalesCount }}</h3>
          <p class="card-label">Total de Vendas</p>
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
        <div class="card-icon products">
          <i class="fas fa-box"></i>
        </div>
        <div class="card-content">
          <h3 class="card-value">{{ summary.totalItemsSoldCount }}</h3>
          <p class="card-label">Itens Vendidos</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue'
import BaseButton from '@/components/shared/BaseButton/BaseButton.vue'
import BaseInput from '@/components/shared/BaseInput/BaseInput.vue'
import { getSalesMetrics } from '@/api/reports.js'

export default {
  name: 'SalesReportView',
  components: {
    BaseButton,
    BaseInput,
  },
  setup() {
    const loading = ref(false)
    
    const filters = ref({
      startDate: new Date(new Date().setDate(new Date().getDate() - 30)).toISOString().split('T')[0],
      endDate: new Date().toISOString().split('T')[0]
    })

    const summary = ref({
      totalRevenue: 0,
      totalSalesCount: null,
      averageTicket: 0,
      totalItemsSoldCount: 0,
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
        const response = await getSalesMetrics(startDate, endDate)
        
        if (response && response.result) {
          summary.value = response.result
        }
      } catch (error) {
        console.error('Error loading sales report data:', error)
      } finally {
        loading.value = false
      }
    }

    onMounted(loadReportData)

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
      loadReportData,
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

.summary-card .card-icon.ticket {
  background-color: #e0f2fe;
  color: #0ea5e9;
}

@media (max-width: 768px) {
  .date-inputs {
    grid-template-columns: 1fr;
  }
}
</style>
