<template>
  <div class="page-container">
    <div class="page-header">
      <div class="header-content">
        <h1 class="page-title">Clientes</h1>
        <p class="page-subtitle">Gerencie os clientes do sistema</p>
      </div>
      <div class="header-actions">
        <router-link to="/customers/new" class="btn btn-primary">
          <i class="fas fa-plus"></i>
          Novo Cliente
        </router-link>
      </div>
    </div>

    <!-- Busca -->
    <BaseSearch
      v-model="searchTerm"
      placeholder="Buscar por nome ou CPF..."
      :loading="isLoading"
      @search="handleSearch"
    />

    <!-- Tabela -->
    <BaseTable
      :items="customers"
      :columns="columns"
      :loading="isLoading"
      :pagination="pagination"
      :empty-title="'Nenhum cliente encontrado'"
      :empty-message="'Comece criando seu primeiro cliente'"
      :item-name="'clientes'"
      @page-change="changePage"
    >
      <template #empty-actions>
        <router-link to="/customers/new" class="btn btn-primary">
          <i class="fas fa-plus"></i>
          Criar Cliente
        </router-link>
      </template>

      <template #cell-name="{ item }">
        <div class="customer-name">
          {{ item.firstName }} {{ item.lastName }}
        </div>
      </template>

      <template #cell-phone="{ item }">
        {{ formatPhone(item.mobilePhoneNumber) }}
      </template>

      <template #cell-address="{ item }">
        {{ item.cityName }}, {{ item.stateAbbreviation }}
      </template>

      <template #actions="{ item }">
        <router-link
          :to="`/customers/${item.id}`"
          class="btn btn-sm btn-info"
          title="Visualizar"
        >
          <i class="fas fa-eye"></i>
        </router-link>
        <router-link
          :to="`/customers/${item.id}/edit`"
          class="btn btn-sm btn-warning"
          title="Editar"
        >
          <i class="fas fa-edit"></i>
        </router-link>
        <button
          @click="confirmDelete(item)"
          class="btn btn-sm btn-danger"
          title="Excluir"
        >
          <i class="fas fa-trash"></i>
        </button>
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
      <p>Tem certeza que deseja excluir o cliente <strong>{{ customerToDelete?.firstName }} {{ customerToDelete?.lastName }}</strong>?</p>
    </BaseModal>
  </div>
</template>

<script>
import { useCustomerStore } from '@/stores/customers'
import { mapState, mapActions } from 'pinia'
import BaseSearch from '@/components/shared/BaseSearch/BaseSearch.vue'
import BaseTable from '@/components/shared/BaseTable/BaseTable.vue'
import BaseModal from '@/components/shared/BaseModal/BaseModal.vue'
import { formatPhone } from '@/utils/formatters'

export default {
  name: 'CustomerListView',
  components: {
    BaseSearch,
    BaseTable,
    BaseModal
  },
  data() {
    return {
      searchTerm: '',
      showDeleteModal: false,
      customerToDelete: null,
      columns: [
        {
          key: 'name',
          label: 'Nome',
          sortable: true
        },
        {
          key: 'cpf',
          label: 'CPF',
          sortable: true
        },
        {
          key: 'phone',
          label: 'Telefone'
        },
        {
          key: 'email',
          label: 'E-mail',
          sortable: true
        },
        {
          key: 'address',
          label: 'Cidade/Estado'
        }
      ]
    }
  },
  computed: {
    ...mapState(useCustomerStore, ['customers', 'isLoading', 'pagination'])
  },
  async mounted() {
    await this.loadCustomers()
  },
  methods: {
    ...mapActions(useCustomerStore, ['fetchCustomers', 'deleteCustomer']),
    
    async loadCustomers(page = 1) {
      await this.fetchCustomers(page, 10)
    },
    async handleSearch() {
      if (this.searchTerm.trim()) {
        console.log('Buscar por:', this.searchTerm)
      } else {
        await this.loadCustomers()
      }
    },
    async changePage(page) {
      if (page >= 1 && page <= this.pagination.totalPages) {
        await this.loadCustomers(page)
      }
    },
    confirmDelete(customer) {
      this.customerToDelete = customer
      this.showDeleteModal = true
    },
    closeDeleteModal() {
      this.showDeleteModal = false
      this.customerToDelete = null
    },
    async handleDelete() {
      if (this.customerToDelete) {
        const result = await this.deleteCustomer(this.customerToDelete.id)
        if (result.success) {
          this.closeDeleteModal()
        }
      }
    },
    formatPhone
  }
}
</script>

<style lang="scss" scoped>
@import '@/styles/variables.scss';

.page-container {
  padding: $spacing-6;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: $spacing-6;
  gap: $spacing-4;
}

.header-content {
  flex: 1;
}

.page-title {
  font-size: $font-size-3xl;
  font-weight: 700;
  color: $gray-900;
  margin-bottom: $spacing-2;
}

.page-subtitle {
  color: $gray-600;
  font-size: $font-size-lg;
}

.header-actions {
  display: flex;
  gap: $spacing-3;
}

.customer-name {
  font-weight: 500;
}

.btn {
  display: inline-flex;
  align-items: center;
  gap: $spacing-2;
  padding: $spacing-3 $spacing-4;
  border: none;
  border-radius: $border-radius;
  font-size: $font-size-base;
  font-weight: 500;
  text-decoration: none;
  cursor: pointer;
  transition: all 0.2s ease;

  &.btn-primary {
    background: $primary-color;
    color: $white;

    &:hover {
      background: $primary-dark;
    }
  }

  &.btn-sm {
    padding: $spacing-1 $spacing-2;
    font-size: $font-size-sm;

    &.btn-info {
      background: $info-color;
      color: $white;

      &:hover {
        background: #0891b2;
      }
    }

    &.btn-warning {
      background: $warning-color;
      color: $white;

      &:hover {
        background: #d97706;
      }
    }

    &.btn-danger {
      background: $error-color;
      color: $white;

      &:hover {
        background: #dc2626;
      }
    }
  }
}

@media (max-width: $breakpoint-md) {
  .page-header {
    flex-direction: column;
    align-items: stretch;
  }
}
</style>
