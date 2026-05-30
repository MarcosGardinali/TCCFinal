<template>
  <div class="products-report-view">
    <div class="page-header">
      <div class="header-content">
        <h1 class="page-title">Relatório de Produtos</h1>
        <p class="page-subtitle">Análise de produtos e controle de estoque</p>
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
    <div class="summary-cards-product">
      <div class="summary-card">
        <div class="card-icon products">
          <i class="fas fa-box"></i>
        </div>
        <div class="card-content">
          <h3 class="card-value">{{ summary.totalProducts }}</h3>
          <p class="card-label">Total de Produtos</p>
        </div>
      </div>

      <div class="summary-card">
        <div class="card-icon revenue">
          <i class="fas fa-dollar-sign"></i>
        </div>
        <div class="card-content">
          <h3 class="card-value">R$ {{ formatCurrency(summary.averagePrice) }}</h3>
          <p class="card-label">Preço Médio</p>
        </div>
      </div>

      <div class="summary-card" v-if="topProduct.code">
        <div class="card-icon top-product">
          <i class="fas fa-star"></i>
        </div>
        <div class="card-content">
          <h3 class="card-value">{{ topProduct.description }}</h3>
          <p class="card-label">Produto Mais Vendido</p>
          <span class="card-change neutral">Código: {{ topProduct.code }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue'
import BaseButton from '@/components/shared/BaseButton/BaseButton.vue'
import { getProductMetrics, getTopProduct } from '@/api/reports.js'

export default {
  name: 'ProductsReportView',
  components: {
    BaseButton,
  },
  setup() {
    const loading = ref(true)

    const summary = ref({
      totalProducts: 0,
      averagePrice: 0,
    })

    const topProduct = ref({})

    const loadReportData = async () => {
      try {
        loading.value = true
        
        const metricsResponse = await getProductMetrics()
        if (metricsResponse && metricsResponse.result) {
          summary.value.totalProducts = metricsResponse.result.totalProducts
          summary.value.averagePrice = metricsResponse.result.averagePrice
        }

        const topProductResponse = await getTopProduct()
        if (topProductResponse && topProductResponse.result) {
          topProduct.value = topProductResponse.result
        }

      } catch (error) {
        console.error('Error loading product report data:', error)
      } finally {
        loading.value = false
      }
    }

    onMounted(loadReportData)

    const formatCurrency = (value) => {
      if (typeof value !== 'number') {
        return '0,00'
      }
      return new Intl.NumberFormat('pt-BR', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      }).format(value)
    }

    return {
      summary,
      topProduct,
      formatCurrency,
      loading
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/styles/variables.scss';
@import './ReportStyles.scss';

.summary-card .card-icon.top-product {
  background-color: #e0f2fe;
  color: #0ea5e9;
}
</style>
