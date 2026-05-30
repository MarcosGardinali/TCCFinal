<template>
  <div class="order-view-container">
    <div class="page-header">
      <div class="header-content">
        <h1 class="page-title">Pedido #{{ order?.id || 'Carregando...' }}</h1>
        <p class="page-subtitle">Detalhes completos do pedido</p>
      </div>
      <div class="header-actions">
        <BaseButton
          variant="outline"
          icon="arrow-left"
          @click="goBack"
        >
          Voltar
        </BaseButton>
        <BaseButton
          v-if="order?.status === 0"
          variant="warning"
          icon="edit"
          :to="`/orders/${order.id}/edit`"
          tag="router-link"
        >
          Editar
        </BaseButton>
        <BaseButton
          v-if="order?.status === 0"
          variant="success"
          icon="check"
          @click="confirmCloseOrder"
        >
          Fechar Pedido
        </BaseButton>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="loading-state">
      <div class="loading-spinner"></div>
      <span>Carregando pedido...</span>
    </div>

    <!-- Order Details -->
    <div v-else-if="order" class="order-view">
      <!-- Order Header -->
      <div class="order-header">
        <div class="order-main-info">
          <div class="order-icon">
            <i class="fas fa-shopping-cart"></i>
          </div>
          <div class="order-title-section">
            <h1 class="order-number">Pedido #{{ order.id }}</h1>
            <p class="order-customer">{{ customer?.firstName || order.customer?.firstName || 'Cliente' }} {{ customer?.lastName || order.customer?.lastName || '' }}</p>
            <div class="order-meta">
              <span class="meta-item">
                <i class="fas fa-calendar"></i>
                {{ formatDate(getOrderCreationDate(order)) }}
              </span>
              <span class="meta-item">
                <i class="fas fa-clock"></i>
                {{ formatTime(getOrderCreationDate(order)) }}
              </span>
            </div>
          </div>
        </div>
        <div class="order-status-section">
          <div style="display: flex; gap: 8px; flex-direction: column; align-items: flex-end;">
            <div style="display: flex; gap: 8px;">
              <div class="status-badge" :class="getStatusClass(order.status)">
                <i :class="getStatusIcon(order.status)"></i>
                {{ getStatusText(order.status) }}
              </div>
              <div v-if="isExpired(order)" class="status-badge status-expired" title="Prazo de 3 dias vencido">
                <i class="fas fa-exclamation-triangle"></i> Vencido
              </div>
            </div>
            <div class="order-total">
              <span class="total-label">Total</span>
              <span class="total-value">{{ formatCurrency(order.totalValue) }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Order Information Grid -->
      <div class="order-info-grid">
        <!-- Customer Information Card -->
        <div class="info-card">
          <div class="card-header">
            <h3 class="card-title">
              <i class="fas fa-user"></i>
              Informações do Cliente
            </h3>
          </div>
          <div class="card-content">
            <div v-if="customer" class="customer-info">
              <div class="customer-avatar">
                <i class="fas fa-user"></i>
              </div>
              <div class="customer-details">
                <h4 class="customer-name">{{ customer.firstName || '' }} {{ customer.lastName || '' }}</h4>
                <div class="info-row">
                  <span class="info-label">CPF:</span>
                  <span class="info-value">{{ formatCPF(customer.cpf) }}</span>
                </div>
                <div class="info-row">
                  <span class="info-label">E-mail:</span>
                  <span class="info-value">{{ customer.email || '-' }}</span>
                </div>
                <div class="info-row">
                  <span class="info-label">Telefone:</span>
                  <span class="info-value">{{ formatPhone(customer.mobilePhoneNumber) }}</span>
                </div>
                <div class="info-row">
                  <span class="info-label">Endereço:</span>
                  <span class="info-value">
                    {{ customer.street || '' }}, {{ customer.number || '' }}
                    {{ customer.complement ? ` - ${customer.complement}` : '' }}<br>
                    {{ customer.neighborhood || '' }}, {{ customer.cityName || '' }} - {{ customer.stateAbbreviation || '' }}<br>
                    CEP: {{ formatCEP(customer.postalCode) }}
                  </span>
                </div>
              </div>
            </div>
            <div v-else class="loading-text">
              <i class="fas fa-spinner fa-spin"></i>
              Carregando informações do cliente...
            </div>
          </div>
        </div>

        <!-- Order Timeline Card -->
        <div class="info-card">
          <div class="card-header">
            <h3 class="card-title">
              <i class="fas fa-history"></i>
              Timeline do Pedido
            </h3>
          </div>
          <div class="card-content">
            <div class="timeline">
              <div class="timeline-item completed">
                <div class="timeline-icon">
                  <i class="fas fa-plus"></i>
                </div>
                <div class="timeline-content">
                  <h4>Pedido Criado</h4>
                  <p>{{ formatDateTime(getOrderCreationDate(order)) }}</p>
                </div>
              </div>
              <div v-if="getOrderClosingDate(order)" class="timeline-item completed">
                <div class="timeline-icon">
                  <i class="fas fa-check"></i>
                </div>
                <div class="timeline-content">
                  <h4>Pedido Finalizado</h4>
                  <p>{{ formatDateTime(getOrderClosingDate(order)) }}</p>
                </div>
              </div>
              <div v-else class="timeline-item pending">
                <div class="timeline-icon">
                  <i class="fas fa-clock"></i>
                </div>
                <div class="timeline-content">
                  <h4>Aguardando Finalização</h4>
                  <p>Pedido em andamento</p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Order Items Card -->
        <div class="info-card items-card">
          <div class="card-header">
            <h3 class="card-title">
              <i class="fas fa-shopping-bag"></i>
              Itens do Pedido
            </h3>
            <span class="items-count">{{ getOrderItems(order).length }} itens</span>
          </div>
          <div class="card-content">
            <div v-if="getOrderItems(order).length > 0" class="items-list">
              <div v-for="item in getOrderItems(order)" :key="item.id || item.productId" class="item-card">
                <div class="item-icon">
                  <i class="fas fa-tshirt"></i>
                </div>
                <div class="item-info">
                  <h4 class="item-name">{{ getItemName(item) }}</h4>
                  <p v-if="item.variation" class="item-variation">{{ item.variation }}</p>
                  <div class="item-meta">
                    <span class="item-code">
                      <i class="fas fa-barcode"></i>
                      {{ getItemCode(item) }}
                    </span>
                    <span class="item-price">
                      <i class="fas fa-tag"></i>
                      {{ formatCurrency(getItemPrice(item)) }}
                    </span>
                  </div>
                  <div v-if="item.status !== undefined" class="item-status-badge" :class="getItemStatusClass(item.status)">
                    {{ getItemStatusText(item.status) }}
                  </div>
                </div>
              </div>
            </div>
            <div v-else class="empty-items">
              <div class="empty-icon">
                <i class="fas fa-shopping-bag"></i>
              </div>
              <p>Nenhum item adicionado a este pedido</p>
            </div>
          </div>
        </div>

        <!-- Observations Card -->
        <div v-if="order.observation" class="info-card observations-card">
          <div class="card-header">
            <h3 class="card-title">
              <i class="fas fa-sticky-note"></i>
              Observações
            </h3>
          </div>
          <div class="card-content">
            <div class="observation-content">
              <p class="observation-text">{{ order.observation }}</p>
            </div>
          </div>
        </div>

        <!-- Order Summary Card -->
        <div class="info-card summary-card">
          <div class="card-header">
            <h3 class="card-title">
              <i class="fas fa-calculator"></i>
              Resumo do Pedido
            </h3>
          </div>
          <div class="card-content">
            <div class="summary-content">
              <div class="summary-row">
                <span class="summary-label">Quantidade de Itens:</span>
                <span class="summary-value">{{ getOrderItems(order).length }}</span>
              </div>
              <div class="summary-row">
                <span class="summary-label">Data de Criação:</span>
                <span class="summary-value">{{ formatDate(getOrderCreationDate(order)) }}</span>
              </div>
              <div v-if="getOrderClosingDate(order)" class="summary-row">
                <span class="summary-label">Data de Fechamento:</span>
                <span class="summary-value">{{ formatDate(getOrderClosingDate(order)) }}</span>
              </div>
              <div class="summary-row total-row">
                <span class="summary-label">Valor Total:</span>
                <span class="summary-value total">{{ formatCurrency(order.totalValue) }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Error State -->
    <div v-else class="error-state">
      <div class="error-icon">⚠️</div>
      <h3 class="error-title">Pedido não encontrado</h3>
      <p class="error-message">O pedido solicitado não foi encontrado ou foi removido.</p>
      <BaseButton variant="primary" @click="goBack">
        Voltar para Lista
      </BaseButton>
    </div>

    <!-- Close Order Modal -->
    <CloseOrderModal
      :show="showCloseModal"
      :order="order"
      :items="itemsForClosing"
      :loading="isLoading"
      @close="closeCloseModal"
      @confirm="handleCloseOrder"
    />
  </div>
</template>

<script>
import { useOrderStore } from '@/stores/orders'
import { useCustomerStore } from '@/stores/customers'
import { useProductStore } from '@/stores/products'
import { mapState, mapActions } from 'pinia'
import BaseButton from '@/components/shared/BaseButton/BaseButton.vue'
import BaseModal from '@/components/shared/BaseModal/BaseModal.vue'
import CloseOrderModal from '@/components/modules/orders/CloseOrderModal.vue'

export default {
  name: 'OrderView',
  components: {
    BaseButton,
    BaseModal,
    CloseOrderModal
  },
  data() {
    return {
      order: null,
      customer: null,
      products: {},
      loading: true,
      showCloseModal: false,
      itemsForClosing: []
    }
  },
  computed: {
    ...mapState(useOrderStore, ['isLoading'])
  },
  async mounted() {
    await this.loadOrder()
  },
  methods: {
    ...mapActions(useOrderStore, { fetchOrder: 'fetchOrderById', closeOrderAction: 'closeOrder', updateOrderAction: 'updateOrder' }),
    ...mapActions(useCustomerStore, { fetchCustomer: 'fetchCustomerById' }),
    ...mapActions(useProductStore, { fetchProduct: 'fetchProductById' }),
    
    async loadOrder() {
      try {
        this.loading = true
        const result = await this.fetchOrder(this.$route.params.id)

        if (result?.success && result.data) {
          this.order = result.data

          if (this.order?.customerId) {
            try {
              const customerResult = await this.fetchCustomer(this.order.customerId)
              if (customerResult?.success && customerResult.data) {
                this.customer = customerResult.data
              }
            } catch (customerError) {
              console.error('Erro ao carregar cliente:', customerError)
            }
          }

          await this.loadProductsForItems()
        } else {
          this.order = null
        }
      } catch (error) {
        console.error('Erro ao carregar pedido:', error)
        this.order = null
      } finally {
        this.loading = false
      }
    },
    
    async loadProductsForItems() {
      if (!this.order) return

      const items = this.getOrderItems(this.order)
      const productIds = items.map(item => item.productId).filter(id => id)

      for (const productId of productIds) {
        if (!this.products[productId]) {
          try {
            const result = await this.fetchProduct(productId)
            if (result.success) {
              this.products[productId] = result.data
            }
          } catch (error) {
            console.error(`Erro ao carregar produto ${productId}:`, error)
          }
        }
      }
    },
    
    goBack() {
      this.$router.push('/orders')
    },
    getStatusClass(status) {
      switch (parseInt(status)) {
        case 0: return 'status-open'
        case 1: return 'status-awaiting'
        case 2: return 'status-closed'
        default: return 'status-unknown'
      }
    },
    getStatusText(status) {
      switch (parseInt(status)) {
        case 0: return 'Aberto'
        case 1: return 'Aguardando Fechamento'
        case 2: return 'Fechado'
        default: return 'Desconhecido'
      }
    },
    getStatusIcon(status) {
      switch (parseInt(status)) {
        case 0: return 'fas fa-clock'
        case 1: return 'fas fa-hourglass-half'
        case 2: return 'fas fa-check-circle'
        default: return 'fas fa-question-circle'
      }
    },
    getItemStatusClass(status) {
      switch (status) {
        case 0: return 'item-status-pending'
        case 1: return 'item-status-completed'
        default: return 'item-status-unknown'
      }
    },
    formatTime(dateString) {
      if (!dateString) return '-'
      return new Date(dateString).toLocaleTimeString('pt-BR', {
        hour: '2-digit',
        minute: '2-digit'
      })
    },
    formatDateTime(dateString) {
      if (!dateString) return '-'
      return new Date(dateString).toLocaleString('pt-BR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      })
    },
    formatDate(dateString) {
      if (!dateString) return '-'
      return new Date(dateString).toLocaleDateString('pt-BR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric'
      })
    },
    
    getOrderCreationDate(order) {
      if (!order) return null
      return order.creationDate || order.createdAt || order.dateCreated || order.created || null
    },
    
    getOrderClosingDate(order) {
      if (!order) return null
      return order.closingDate || order.closedAt || order.dateClosed || order.closed || null
    },
    isExpired(order) {
      if (!order || order.status === 2) return false;
      const dateString = this.getOrderCreationDate(order);
      if (!dateString) return false;
      
      const createdDate = new Date(dateString);
      const expirationDate = new Date(createdDate);
      expirationDate.setDate(expirationDate.getDate() + 3);
      
      const now = new Date();
      return now > expirationDate;
    },
    formatCurrency(value) {
      return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
      }).format(value || 0)
    },
    formatCPF(cpf) {
      if (!cpf) return '-'
      return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4')
    },
    formatPhone(phone) {
      if (!phone) return '-'
      const cleaned = phone.replace(/\D/g, '')
      if (cleaned.length === 11) {
        return cleaned.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3')
      }
      if (cleaned.length === 10) {
        return cleaned.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3')
      }
      return phone
    },
    formatCEP(cep) {
      if (!cep) return '-'
      return cep.replace(/(\d{5})(\d{3})/, '$1-$2')
    },
    confirmCloseOrder() {
      this.prepareItemsForClosing()
      this.showCloseModal = true
    },
    
    prepareItemsForClosing() {
      const items = this.getOrderItems(this.order)
      this.itemsForClosing = items
        .filter(item => item.status === 0) // Apenas itens "Em andamento"
        .map(item => ({
          ...item,
          newStatus: null
        }))
    },
    closeCloseModal() {
      this.showCloseModal = false
    },
    async handleCloseOrder() {
      try {
        // Primeiro, atualizar os status dos itens
        if (this.itemsForClosing.length > 0) {
          const updateResult = await this.updateOrderAction(this.order.id, {
            observation: this.order.observation,
            listCreatedItem: [],
            listUpdatedItem: this.itemsForClosing.map(item => ({
              id: item.id,
              inputUpdate: {
                variation: item.variation || null,
                status: parseInt(item.newStatus)
              }
            })),
            listDeletedItem: []
          })
          
          if (!updateResult?.success) {
            console.error('Erro ao atualizar itens:', updateResult?.message)
            return
          }
        }
        
        // Depois, fechar o pedido
        const result = await this.closeOrderAction(this.order.id)
        if (result?.success) {
          this.closeCloseModal()
          await this.loadOrder()
        } else {
          console.error('Erro ao fechar pedido:', result?.message || 'Erro desconhecido')
        }
      } catch (error) {
        console.error('Erro ao fechar pedido:', error)
      }
    },
    
    getOrderItems(order) {
      if (!order) return []
      if (order.listOrderItem && Array.isArray(order.listOrderItem)) {
        return order.listOrderItem
      }
      if (order.items && Array.isArray(order.items)) {
        return order.items
      }
      if (order.orderItems && Array.isArray(order.orderItems)) {
        return order.orderItems
      }
      if (order.listItem && Array.isArray(order.listItem)) {
        return order.listItem
      }
      return []
    },
    
    getItemName(item) {
      if (item.productId && this.products[item.productId]) {
        return this.products[item.productId].name
      }
      if (item.product?.name) return item.product.name
      if (item.productName) return item.productName
      if (item.name) return item.name
      return 'Produto não encontrado'
    },
    
    getItemCode(item) {
      if (item.productId && this.products[item.productId]) {
        return this.products[item.productId].code
      }
      if (item.product?.code) return item.product.code
      if (item.productCode) return item.productCode
      if (item.code) return item.code
      return 'N/A'
    },
    
    getItemPrice(item) {
      if (item.productId && this.products[item.productId]) {
        return this.products[item.productId].price
      }
      if (item.product?.price) return item.product.price
      if (item.price) return item.price
      if (item.unitPrice) return item.unitPrice
      return 0
    },
    
    getItemStatusText(status) {
      switch (parseInt(status)) {
        case 0: return 'Em andamento'
        case 1: return 'Devolvido'
        case 2: return 'Comprado'
        default: return 'Desconhecido'
      }
    }
  }
}
</script>

<style lang="scss" scoped>
@import './OrderView.scss';
</style>
