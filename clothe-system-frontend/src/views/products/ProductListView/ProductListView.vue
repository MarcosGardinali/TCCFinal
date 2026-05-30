<template>
  <div class="page-container">
    <div class="page-header">
      <div class="header-content">
        <h1 class="page-title">Produtos</h1>
        <p class="page-subtitle">Gerencie os produtos do sistema</p>
      </div>
      <div class="header-actions">
        <BaseButton
          variant="primary"
          icon="plus"
          tag="router-link"
          to="/products/new"
        >
          Novo Produto
        </BaseButton>
      </div>
    </div>

    <!-- Busca -->
    <BaseSearch
      v-model="searchTerm"
      placeholder="Buscar por nome ou código..."
      :loading="isLoading"
      @search="handleSearch"
    />

    <!-- Tabela -->
    <BaseTable
      :items="products"
      :columns="columns"
      :loading="isLoading"
      :pagination="pagination"
      :empty-title="'Nenhum produto encontrado'"
      :empty-message="'Comece criando seu primeiro produto'"
      :item-name="'produtos'"
      @page-change="changePage"
    >
      <template #empty-actions>
        <BaseButton
          variant="primary"
          icon="plus"
          tag="router-link"
          to="/products/new"
        >
          Criar Produto
        </BaseButton>
      </template>

      <template #cell-description="{ item }">
        <div class="product-description">
          {{ item.description || 'Sem descrição' }}
        </div>
      </template>

      <template #cell-price="{ item }">
        {{ formatCurrency(item.price) }}
      </template>

      <template #cell-brand="{ item }">
        {{ item.brand || 'Sem marca' }}
      </template>

      <template #cell-category="{ item }">
        {{ item.category || 'Sem categoria' }}
      </template>

      <template #actions="{ item }">
        <BaseButton
          variant="info"
          size="sm"
          icon="eye"
          tag="router-link"
          :to="`/products/${item.id}`"
          title="Visualizar"
        />
        <BaseButton
          variant="warning"
          size="sm"
          icon="edit"
          tag="router-link"
          :to="`/products/${item.id}/edit`"
          title="Editar"
        />
        <BaseButton
          variant="danger"
          size="sm"
          icon="trash"
          @click="confirmDelete(item)"
          title="Excluir"
        />
      </template>
    </BaseTable>

    <!-- Modal de Confirmação -->
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
      <p>Tem certeza que deseja excluir o produto <strong>{{ getProductName(productToDelete) }}</strong>?</p>
    </BaseModal>
  </div>
</template>

<script>
import { useProductStore } from '@/stores/products'
import { mapState, mapActions } from 'pinia'
import BaseSearch from '@/components/shared/BaseSearch/BaseSearch.vue'
import BaseTable from '@/components/shared/BaseTable/BaseTable.vue'
import BaseModal from '@/components/shared/BaseModal/BaseModal.vue'
import BaseButton from '@/components/shared/BaseButton/BaseButton.vue'

export default {
  name: 'ProductListView',
  components: {
    BaseSearch,
    BaseTable,
    BaseModal,
    BaseButton
  },
  data() {
    return {
      searchTerm: '',
      showDeleteModal: false,
      productToDelete: null,
      columns: [
        {
          key: 'code',
          label: 'Código',
          sortable: true
        },
        {
          key: 'description',
          label: 'Descrição',
          sortable: true
        },
        {
          key: 'price',
          label: 'Preço',
          sortable: true
        },
        /*{
          key: 'brand',
          label: 'Marca',
          sortable: true
        },
        {
          key: 'category',
          label: 'Categoria',
          sortable: true
        }*/
      ]
    }
  },
  computed: {
    ...mapState(useProductStore, ['products', 'isLoading', 'pagination']),
    
    getProductName() {
      return (product) => product?.name || product?.description || 'Produto'
    }
  },
  async mounted() {
    await this.loadProducts()
  },
  methods: {
    ...mapActions(useProductStore, ['fetchProducts', 'deleteProduct']),
    
    async loadProducts(page = 1) {
      await this.fetchProducts(page, 10)
    },
    async handleSearch() {
      if (this.searchTerm.trim()) {
        console.log('Buscar por:', this.searchTerm)
      } else {
        await this.loadProducts()
      }
    },
    async changePage(page) {
      if (page >= 1 && page <= this.pagination.totalPages) {
        await this.loadProducts(page)
      }
    },
    formatCurrency(value) {
      return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
      }).format(value || 0)
    },
    confirmDelete(product) {
      this.productToDelete = product
      this.showDeleteModal = true
    },
    closeDeleteModal() {
      this.showDeleteModal = false
      this.productToDelete = null
    },
    async handleDelete() {
      if (this.productToDelete) {
        const result = await this.deleteProduct(this.productToDelete.id)
        if (result.success) {
          this.closeDeleteModal()
          await this.loadProducts()
        }
      }
    }
  }
}
</script>

<style lang="scss" scoped>
@import './ProductListView.scss';
</style>
