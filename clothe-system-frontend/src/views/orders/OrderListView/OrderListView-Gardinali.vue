<template>
  <div class="page-container">
    <div class="page-header">
      <div class="header-content">
        <h1 class="page-title">Pedidos</h1>
        <p class="page-subtitle">Gerencie os pedidos do sistema</p>
      </div>
      <div class="header-actions">
        <BaseButton
          variant="primary"
          icon="plus"
          tag="router-link"
          to="/orders/new"
        >
          Novo Pedido
        </BaseButton>
      </div>
    </div>

    <!-- Busca -->
    <BaseSearch
      v-model="searchTerm"
      placeholder="Buscar por número do pedido..."
      :loading="isLoading"
      @search="handleSearch"
    />

    <!-- Tabela -->
    <BaseTable
      :items="orders"
      :columns="columns"
      :loading="isLoading"
      :pagination="pagination"
      :empty-title="'Nenhum pedido encontrado'"
      :empty-message="'Comece criando seu primeiro pedido'"
      :item-name="'pedidos'"
      @page-change="changePage"
    >
      <template #empty-actions>
        <BaseButton
          variant="primary"
          icon="plus"
          tag="router-link"
          to="/orders/new"
        >
          Criar Pedido
        </BaseButton>
      </template>

      <template #cell-orderNumber="{ item }">
        <span class="order-number">#{{ item.id || 'N/A' }}</span>
      </template>

      <template #cell-customer="{ item }">
        <div class="customer-info">
          <span v-if="item.customerName">
            {{ item.customerName }}
          </span>
          <span v-else-if="item.customer">
            {{ item.customer.firstName }} {{ item.customer.lastName }}
          </span>
          <span v-else class="text-muted">
            Cliente não informado
          </span>
        </div>
      </template>

      <template #cell-status="{ item }">
        <div style="display: flex; gap: 8px; align-items: center; justify-content: center;">
          <span class="status-badge" :class="getStatusClass(item.status)">
            {{ getStatusText(item.status) }}
          </span>
          <span v-if="isExpired(item)" class="status-badge status-expired" title="Prazo de 3 dias vencido">
            <i class="fas fa-exclamation-triangle"></i> Vencido
          </span>
        </div>
      </template>

      <template #cell-date="{ item }">
        {{ formatDate(item.creationDate || item.orderDate || item.date) }}
      </template>

      <template #cell-items="{ item }">
        <span class="items-count">
          {{ getItemsCount(item) }} {{ getItemsCount(item) === 1 ? 'item' : 'itens' }}
        </span>
      </template>

      <template #actions="{ item }">
        <BaseButton
          variant="info"
          size="sm"
          icon="eye"
          tag="router-link"
          :to="`/orders/${item.id}`"
          title="Visualizar"
        />
        <BaseButton
          v-if="item.status === 0"
          variant="warning"
          size="sm"
          icon="edit"
          tag="router-link"
          :to="`/orders/${item.id}/edit`"
          title="Editar"
        />
        <BaseButton
          v-if="item.status === 0"
          variant="success"
          size="sm"
          icon="check"
          @click="confirmClose(item)"
          title="Fechar Pedido"
        />
        <BaseButton
          v-if="item.status === 0"
          variant="danger"
          size="sm"
          icon="trash"
          @click="confirmDelete(item)"
          title="Excluir"
        />
      </template>
    </BaseTable>

    <!-- Close Order Modal -->
    <CloseOrderModal
      :show="showCloseModal"
      :order="orderToClose"
      :items="itemsForClosing"
      :loading="isLoading"
      @close="closeCloseModal"
      @confirm="handleClose"
    />

    <!-- Modal de Confirmação para Excluir -->
    <BaseModal
      :show="showDeleteModal"
      title="Confirmar Exclusão"
      subtitle="Esta ação não pode ser desfeita"
      confirm-type="danger"
      confirm-text="Excluir"
      :loading="isLoading"
      @close="closeDeleteModal"
      @confirm="handleDelete"
    >
      <p>Tem certeza que deseja excluir o pedido <strong>#{{ orderToDelete?.id }}</strong>?</p>
    </BaseModal>
  </div>
</template>

<script>
import { useOrderStore } from '@/stores/orders'
import { mapState, mapActions } from 'pinia'
import BaseSearch from '@/components/shared/BaseSearch/BaseSearch.vue'
import BaseTable from '@/components/shared/BaseTable/BaseTable.vue'
import BaseModal from '@/components/shared/BaseModal/BaseModal.vue'
import BaseButton from '@/components/shared/BaseButton/BaseButton.vue'
import CloseOrderModal from '@/components/modules/orders/CloseOrderModal.vue'

export default {
  name: 'OrderListView',
  components: {
    BaseSearch,
    BaseTable,
    BaseModal,
    BaseButton,
    CloseOrderModal
  },
  data() {
    return {
      searchTerm: '',
      showCloseModal: false,
      showDeleteModal: false,
      orderToClose: null,
      orderToDelete: null,
      itemsForClosing: [],
      columns: [
        {
          key: 'orderNumber',
          label: 'Número',
          sortable: true
        },
        {
          key: 'customer',
          label: 'Cliente'
        },
        {
          key: 'date',
          label: 'Data de Criação',
          sortable: true
        },
        {
          key: 'status',
          label: 'Status'
        },
        {
          key: 'items',
          label: 'Itens'
        }
      ]
    }
  },
  computed: {
    ...mapState(useOrderStore, ['orders', 'isLoading', 'pagination'])
  },
  async mounted() {
    await this.loadOrders()
  },
  methods: {
    ...mapActions(useOrderStore, ['fetchOrders', 'closeOrder', 'deleteOrder', 'updateOrder', 'fetchOrderById']),
    
    async loadOrders(page = 1) {
      await this.fetchOrders(page, 10)
    },
    async handleSearch() {
      if (this.searchTerm.trim()) {
        console.log('Buscar por:', this.searchTerm)
      } else {
        await this.loadOrders()
      }
    },
    async changePage(page) {
      if (page >= 1 && page <= this.pagination.totalPages) {
        await this.loadOrders(page)
      }
    },
    getStatusClass(status) {
      if (status === undefined || status === null) return 'status-unknown'
      switch (parseInt(status)) {
        case 0: return 'status-open'
        case 1: return 'status-awaiting'
        case 2: return 'status-closed'
        default: return 'status-unknown'
      }
    },
    getStatusText(status) {
      if (status === undefined || status === null) return 'Desconhecido'
      switch (parseInt(status)) {
        case 0: return 'Aberto'
        case 1: return 'Aguardando Fechamento'
        case 2: return 'Fechado'
        default: return 'Desconhecido'
      }
    },
    formatDate(dateString) {
      if (!dateString) return '-'
      try {
        return new Date(dateString).toLocaleDateString('pt-BR')
      } catch (error) {
        return '-'
      }
    },
    getItemsCount(item) {
      if (item.listOrderItem && Array.isArray(item.listOrderItem)) {
        return item.listOrderItem.length
      }
      if (item.items && Array.isArray(item.items)) {
        return item.items.length
      }
      if (item.orderItems && Array.isArray(item.orderItems)) {
        return item.orderItems.length
      }
      if (item.listItem && Array.isArray(item.listItem)) {
        return item.listItem.length
      }
      return 0
    },
    isExpired(order) {
      if (order.status === 2) return false;
      const dateString = order.creationDate || order.orderDate || order.date;
      if (!dateString) return false;
      
      const createdDate = new Date(dateString);
      const expirationDate = new Date(createdDate);
      expirationDate.setDate(expirationDate.getDate() + 3);
      
      const now = new Date();
      return now > expirationDate;
    },
    async confirmClose(order) {
      this.orderToClose = order
      await this.prepareItemsForClosing(order)
      this.showCloseModal = true
    },
    
    async prepareItemsForClosing(order) {
      // Buscar detalhes completos do pedido
      const result = await this.fetchOrderById(order.id)
      if (result?.success && result.data) {
        const fullOrder = result.data
        const items = this.getOrderItems(fullOrder)
        this.itemsForClosing = items
          .filter(item => item.status === 0) // Apenas itens "Em andamento"
          .map(item => ({
            ...item,
            newStatus: null
          }))
      } else {
        this.itemsForClosing = []
      }
    },
    closeCloseModal() {
      this.showCloseModal = false
      this.orderToClose = null
      this.itemsForClosing = []
    },
    async handleClose() {
      try {
        // Primeiro, atualizar os status dos itens se houver
        if (this.itemsForClosing.length > 0) {
          const updateResult = await this.updateOrder(this.orderToClose.id, {
            observation: this.orderToClose.observation,
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
        const result = await this.closeOrder(this.orderToClose.id)
        if (result.success) {
          this.closeCloseModal()
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
    confirmDelete(order) {
      this.orderToDelete = order
      this.showDeleteModal = true
    },
    closeDeleteModal() {
      this.showDeleteModal = false
      this.orderToDelete = null
    },
    async handleDelete() {
      if (this.orderToDelete) {
        const result = await this.deleteOrder(this.orderToDelete.id)
        if (result.success) {
          this.closeDeleteModal()
        }
      }
    }
  }
}
</script>

<style lang="scss" scoped>
@import './OrderListView.scss';
</style>
