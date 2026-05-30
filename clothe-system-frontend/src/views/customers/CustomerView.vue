<template>
  <div class="customer-view">
    <div class="page-header">
      <div class="header-content">
        <h1 class="page-title">Visualizar Cliente</h1>
        <p class="page-subtitle">Informações detalhadas do cliente</p>
      </div>
      
      <div class="header-actions">
        <BaseButton
          variant="outline"
          icon="arrow-left"
          tag="router-link"
          to="/customers"
        >
          Voltar
        </BaseButton>

        <BaseButton
          v-if="customer"
          variant="warning"
          icon="edit"
          tag="router-link"
          :to="`/customers/${customer.id}/edit`"
        >
          Editar
        </BaseButton>
      </div>
    </div>
    
    <div v-if="loading" class="loading-state">
      <LoadingSpinner message="Carregando dados do cliente..." />
    </div>

    <div v-else-if="!customer" class="error-state">
      <div class="error-icon">❌</div>
      <h3 class="error-title">Cliente não encontrado</h3>
      <p class="error-message">O cliente solicitado não foi encontrado.</p>
      <router-link to="/customers" class="btn btn-primary">
        Voltar para Lista
      </router-link>
    </div>

    <div v-else class="customer-view">
      <!-- Customer Header -->
      <div class="customer-header">
        <div class="customer-main-info">
          <div class="customer-avatar">
            <i class="fas fa-user"></i>
          </div>
          <div class="customer-title-section">
            <h1 class="customer-name">{{ customer.firstName }} {{ customer.lastName }}</h1>
            <p class="customer-email">{{ customer.email || 'Email não informado' }}</p>
            <div class="customer-contact">
              <span class="contact-item">
                <i class="fas fa-phone"></i>
                {{ formatPhone(customer.mobilePhoneNumber) }}
              </span>
              <span class="contact-item">
                <i class="fas fa-id-card"></i>
                {{ formatCPF(customer.cpf) }}
              </span>
            </div>
          </div>
        </div>
        <div class="customer-actions">
          <BaseButton
            variant="warning"
            icon="edit"
            tag="router-link"
            :to="`/customers/${customer.id}/edit`"
          >
            Editar Cliente
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

      <!-- Customer Information Cards -->
      <div class="customer-info-grid">
        <!-- Personal Information Card -->
        <div class="info-card">
          <div class="card-header">
            <h3 class="card-title">
              <i class="fas fa-user-circle"></i>
              Dados Pessoais
            </h3>
          </div>
          <div class="card-content">
            <div class="info-row">
              <span class="info-label">Nome Completo:</span>
              <span class="info-value">{{ customer.firstName }} {{ customer.lastName }}</span>
            </div>
            <div class="info-row">
              <span class="info-label">CPF:</span>
              <span class="info-value">{{ formatCPF(customer.cpf) }}</span>
            </div>
            <div class="info-row">
              <span class="info-label">RG:</span>
              <span class="info-value">{{ formatRG(customer.rg) }}</span>
            </div>
            <div class="info-row">
              <span class="info-label">Data de Nascimento:</span>
              <span class="info-value">{{ formatDate(customer.birthDate) }}</span>
            </div>
            <div class="info-row">
              <span class="info-label">Telefone:</span>
              <span class="info-value">{{ formatPhone(customer.mobilePhoneNumber) }}</span>
            </div>
            <div class="info-row">
              <span class="info-label">E-mail:</span>
              <span class="info-value">{{ customer.email || 'Não informado' }}</span>
            </div>
          </div>
        </div>

        <!-- Address Card -->
        <div class="info-card">
          <div class="card-header">
            <h3 class="card-title">
              <i class="fas fa-map-marker-alt"></i>
              Endereço
            </h3>
          </div>
          <div class="card-content">
            <div class="info-row">
              <span class="info-label">CEP:</span>
              <span class="info-value">{{ formatCEP(customer.postalCode) || 'Não informado' }}</span>
            </div>
            <div class="info-row">
              <span class="info-label">Logradouro:</span>
              <span class="info-value">{{ customer.street }}, {{ customer.number }}</span>
            </div>
            <div v-if="customer.complement" class="info-row">
              <span class="info-label">Complemento:</span>
              <span class="info-value">{{ customer.complement }}</span>
            </div>
            <div class="info-row">
              <span class="info-label">Bairro:</span>
              <span class="info-value">{{ customer.neighborhood }}</span>
            </div>
            <div class="info-row">
              <span class="info-label">Cidade:</span>
              <span class="info-value">{{ customer.cityName }}, {{ customer.stateAbbreviation }}</span>
            </div>
          </div>
        </div>

        <!-- System Information Card -->
        <div class="info-card">
          <div class="card-header">
            <h3 class="card-title">
              <i class="fas fa-clock"></i>
              Informações do Sistema
            </h3>
          </div>
          <div class="card-content">
            <div class="info-row">
              <span class="info-label">ID:</span>
              <span class="info-value">#{{ customer.id }}</span>
            </div>
            <div class="info-row">
              <span class="info-label">Data de Cadastro:</span>
              <span class="info-value">{{ formatDateTime(customer.createdAt) }}</span>
            </div>
            <div v-if="customer.updatedAt" class="info-row">
              <span class="info-label">Última Alteração:</span>
              <span class="info-value">{{ formatDateTime(customer.updatedAt) }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { useCustomerStore } from '@/stores/customers'
import { mapState, mapActions } from 'pinia'
import { formatCPF, formatRG, formatPhone, formatCEP, formatDate, formatDateTime } from '@/utils/formatters'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import BaseButton from '@/components/shared/BaseButton/BaseButton.vue'

export default {
  name: 'CustomerView',
  components: {
    LoadingSpinner,
    BaseButton
  },
  props: {
    id: {
      type: String,
      required: true
    }
  },
  data() {
    return {
      loading: false
    }
  },
  computed: {
    ...mapState(useCustomerStore, { customer: 'currentCustomer' })
  },
  async mounted() {
    await this.loadCustomer()
  },
  methods: {
    ...mapActions(useCustomerStore, ['fetchCustomerById']),
    
    async loadCustomer() {
      this.loading = true
      const result = await this.fetchCustomerById(this.id)
      this.loading = false
    },
    confirmDelete() {
      if (confirm('Tem certeza que deseja excluir este cliente?')) {
        this.deleteCustomer()
      }
    },
    async deleteCustomer() {
      // Implementar lógica de exclusão
      console.log('Excluir cliente:', this.customer.id)
    },
    formatCPF,
    formatRG,
    formatPhone,
    formatCEP,
    formatDate,
    formatDateTime
  }
}
</script>

<style lang="scss" scoped>
.customer-view-container {
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

.customer-view {
  display: flex;
  flex-direction: column;
  gap: $spacing-6;
  padding: 1rem;
}

.customer-header {
  background: $primary-color;
  border-radius: $border-radius-xl;
  padding: $spacing-6;
  color: white;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  box-shadow: $shadow-md;
}

.customer-main-info {
  display: flex;
  gap: $spacing-5;
  align-items: center;
}

.customer-avatar {
  width: 100px;
  height: 100px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 1px solid rgba(255, 255, 255, 0.2);
  flex-shrink: 0;

  i {
    font-size: 3rem;
    color: rgba(255, 255, 255, 0.8);
  }
}

.customer-name {
  font-size: 1.75rem;
  font-weight: 600;
  margin: 0 0 $spacing-1 0;
}

.customer-email {
  opacity: 0.8;
  margin: 0 0 $spacing-2 0;
}

.customer-contact {
  display: flex;
  gap: $spacing-4;
  opacity: 0.8;

  .contact-item {
    display: flex;
    align-items: center;
    gap: $spacing-2;
  }
}

.customer-actions {
  display: flex;
  flex-direction: column;
  gap: $spacing-3;
  flex-shrink: 0;
}

.customer-info-grid {
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
}

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

.loading-state, .error-state {
  text-align: center;
  padding: $spacing-10 $spacing-6;
  background-color: $gray-50;
  border-radius: $border-radius-lg;
}

.error-icon {
  font-size: 3rem;
  margin-bottom: $spacing-4;
  color: $error-color;
}

.error-title {
  font-size: $font-size-xl;
  font-weight: 600;
  color: $text-primary;
  margin-bottom: $spacing-2;
}

.error-message {
  color: $text-secondary;
  margin-bottom: $spacing-5;
}

// Responsive
@media (max-width: 768px) {
  .page-header, .customer-header {
    flex-direction: column;
    gap: $spacing-4;
    align-items: stretch;
  }

  .customer-main-info, .customer-title-section {
    align-items: center;
    text-align: center;
  }

  .customer-actions {
    flex-direction: row;
    justify-content: center;
    width: 100%;
  }

  .customer-info-grid {
    grid-template-columns: 1fr;
  }
}
</style>
