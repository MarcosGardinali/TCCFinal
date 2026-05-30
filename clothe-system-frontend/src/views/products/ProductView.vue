<template>
  <div class="product-view">
    <div class="page-header">
      <div class="header-content">
        <h1 class="page-title">
          {{ product ? getProductName(product) : 'Carregando...' }}
        </h1>
        <p class="page-subtitle">Detalhes do produto</p>
      </div>

      <div class="header-actions">
        <BaseButton
          variant="outline"
          icon="arrow-left"
          tag="router-link"
          to="/products"
        >
          Voltar
        </BaseButton>
        <BaseButton
          v-if="product"
          variant="warning"
          icon="edit"
          tag="router-link"
          :to="`/products/${product.id}/edit`"
        >
          Editar
        </BaseButton>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="content-card">
      <div class="loading-state">
        <div class="loading-spinner"></div>
        <p>Carregando produto...</p>
      </div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="content-card">
      <div class="error-state">
        <div class="error-icon">⚠️</div>
        <h3 class="error-title">Erro ao carregar produto</h3>
        <p class="error-message">{{ error }}</p>
        <BaseButton
          variant="primary"
          @click="loadProduct"
        >
          Tentar novamente
        </BaseButton>
      </div>
    </div>

    <!-- Product Details -->
    <div v-else-if="product" class="product-view">
      <!-- Product Header -->
      <div class="product-header">
        <div class="product-main-info">
          <div class="product-image-placeholder">
            <i class="fas fa-box-open"></i>
          </div>
          <div class="product-title-section">
            <h1 class="product-title">{{ product.description || 'Produto sem nome' }}</h1>
            <p class="product-code">Código: {{ product.code || 'N/A' }}</p>
            <div class="product-price">
              <span class="price-label">Preço:</span>
              <span class="price-value">{{ formatCurrency(product.price) }}</span>
            </div>
          </div>
        </div>
        <div class="product-actions">
          <BaseButton
            variant="warning"
            icon="edit"
            tag="router-link"
            :to="`/products/${product.id}/edit`"
          >
            Editar Produto
          </BaseButton>
          <BaseButton
            variant="danger"
            icon="trash"
            @click="confirmDelete"
          >
            Excluir
          </BaseButton>
        </div>
      </div>

      <!-- Product Information Cards -->
      <div class="product-info-grid">
        <!-- Basic Information Card -->
        <div class="info-card">
          <div class="card-header">
            <h3 class="card-title">
              <i class="fas fa-info-circle"></i>
              Informações Básicas
            </h3>
          </div>
          <div class="card-content">
            <div class="info-row">
              <span class="info-label">Código:</span>
              <span class="info-value">{{ product.code || 'Não informado' }}</span>
            </div>
            <div class="info-row">
              <span class="info-label">Descrição:</span>
              <span class="info-value">{{ product.description || 'Não informado' }}</span>
            </div>
            <div class="info-row">
              <span class="info-label">Preço:</span>
              <span class="info-value price-highlight">{{ formatCurrency(product.price) }}</span>
            </div>
          </div>
        </div>

        <!-- Additional Information Card 
        <div class="info-card">
          <div class="card-header">
            <h3 class="card-title">
              <i class="fas fa-tags"></i>
              Informações Adicionais
            </h3>
          </div>
          <div class="card-content">
            <div class="info-row">
              <span class="info-label">Marca:</span>
              <span class="info-value">{{ product.brand || 'Não informado' }}</span>
            </div>
            <div class="info-row">
              <span class="info-label">Categoria:</span>
              <span class="info-value">{{ product.category || 'Não informado' }}</span>
            </div>
          </div>
        </div>
        -->

        <!-- Dates Card -->
        <div class="info-card">
          <div class="card-header">
            <h3 class="card-title">
              <i class="fas fa-calendar"></i>
              Datas
            </h3>
          </div>
          <div class="card-content">
            <div class="info-row">
              <span class="info-label">Data de Cadastro:</span>
              <span class="info-value">{{ formatDate(product.creationDate) }}</span>
            </div>
            <div v-if="product.changeDate" class="info-row">
              <span class="info-label">Última Alteração:</span>
              <span class="info-value">{{ formatDate(product.changeDate) }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { useProductStore } from '@/stores/products'
import { mapState, mapActions } from 'pinia'
import BaseButton from '@/components/shared/BaseButton/BaseButton.vue'

export default {
  name: 'ProductView',
  components: {
    BaseButton
  },
  data() {
    return {
      product: null,
      error: ''
    }
  },
  computed: {
    ...mapState(useProductStore, ['isLoading'])
  },
  async mounted() {
    await this.loadProduct()
  },
  methods: {
    ...mapActions(useProductStore, ['fetchProductById']),
    
    async loadProduct() {
      try {
        this.error = ''
        const result = await this.fetchProductById(this.$route.params.id)
        if (result.success) {
          this.product = result.data
        } else {
          this.error = result.message || 'Produto não encontrado'
        }
      } catch (err) {
        console.error('Erro ao carregar produto:', err)
        this.error = 'Erro interno do servidor'
      }
    },
    // Funções auxiliares
    getProductName(product) {
      if (!product) return 'Produto não encontrado'
      return product.description || 'Nome não informado'
    },
    getProductPrice(product) {
      if (!product) return 0
      return product.price || 0
    },
    getProductStock(product) {
      if (!product) return 'N/A'
      return 'N/A' // Campo não disponível na API atual
    },
    getStatusClass(product) {
      if (!product) return 'status-unknown'
      // Como não temos campo de status na API atual, assumir sempre ativo
      return 'status-active'
    },
    getStatusText(product) {
      if (!product) return 'Desconhecido'
      // Como não temos campo de status na API atual, assumir sempre ativo
      return 'Ativo'
    },
    getStockClass(product) {
      const stock = this.getProductStock(product)
      if (stock === 0) return 'stock-empty'
      if (stock < 10) return 'stock-low'
      return 'stock-normal'
    },
    formatCurrency(value) {
      return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
      }).format(value || 0)
    },
    formatDate(dateString) {
      if (!dateString) return '-'
      try {
        return new Date(dateString).toLocaleDateString('pt-BR', {
          year: 'numeric',
          month: 'long',
          day: 'numeric',
          hour: '2-digit',
          minute: '2-digit'
        })
      } catch (error) {
        return '-'
      }
    }
  }
}
</script>

<style lang="scss" scoped>
.product-view-container {
  padding: $spacing-6;
  max-width: 1200px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: $spacing-6;
  padding-bottom: $spacing-4;
  border-bottom: 1px solid $border;
}

.page-title {
  font-size: $font-size-2xl;
  font-weight: 600;
  color: $text-primary;
  margin: 0;
}

.header-actions {
  display: flex;
  gap: $spacing-3;
}

.product-view {
  display: flex;
  flex-direction: column;
  gap: $spacing-6;
  padding: 1rem;
}

.product-header {
  background: $primary-color;
  border-radius: $border-radius-xl;
  padding: $spacing-6;
  color: white;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  box-shadow: $shadow-md;
}

.product-main-info {
  display: flex;
  gap: $spacing-5;
  align-items: center;
}

.product-image-placeholder {
  width: 100px;
  height: 100px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: $border-radius-lg;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 1px solid rgba(255, 255, 255, 0.2);
  flex-shrink: 0;

  i {
    font-size: 2.5rem;
    color: rgba(255, 255, 255, 0.8);
  }
}

.product-title-section .product-title {
  font-size: 1.75rem;
  font-weight: 600;
  margin: 0 0 $spacing-1 0;
}

.product-title-section .product-code {
  font-size: $font-size-base;
  opacity: 0.8;
  margin: 0 0 $spacing-2 0;
}

.product-price .price-value {
  font-size: 1.5rem;
  font-weight: 600;
  color: $white;
}

.product-actions {
  display: flex;
  flex-direction: column;
  gap: $spacing-3;
  flex-shrink: 0;
}

.product-info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(320px, 1fr));
  gap: $spacing-6;
}

.info-card {
  background: $surface;
  border-radius: $border-radius-lg;
  border: 1px solid $border;
  overflow: hidden;
  transition: all 0.3s ease;

  &:hover {
    box-shadow: $shadow-md;
    transform: translateY(-2px);
  }
}

.card-header {
  background: $gray-50;
  padding: $spacing-4;
  border-bottom: 1px solid $border;

  .card-title {
    font-size: $font-size-base;
    font-weight: 600;
    color: $text-primary;
    margin: 0;
    display: flex;
    align-items: center;
    gap: $spacing-3;

    i {
      color: $text-secondary;
    }
  }
}

.card-content {
  padding: $spacing-4;
}

.info-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: $spacing-3;
  border-bottom: 1px solid $gray-50;

  &:last-child {
    border-bottom: none;
  }
}

.info-label {
  font-size: $font-size-sm;
  color: $text-secondary;
  font-weight: 500;
}

.info-value {
  font-size: $font-size-base;
  color: $text-primary;
  font-weight: 500;
  text-align: right;
}

.price-highlight {
  font-weight: 600;
  color: $primary-color;
}

.loading-state, .error-state {
  text-align: center;
  padding: $spacing-10 $spacing-6;
  background-color: $gray-50;
  border-radius: $border-radius-lg;
}

// Responsive
@media (max-width: 768px) {
  .page-header, .product-header {
    flex-direction: column;
    gap: $spacing-4;
    align-items: stretch;
  }

  .product-main-info {
    flex-direction: column;
    text-align: center;
    align-items: center;
  }

  .product-actions {
    flex-direction: row;
    justify-content: center;
    width: 100%;
  }

  .product-info-grid {
    grid-template-columns: 1fr;
  }
}
</style>
