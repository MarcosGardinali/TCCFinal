<template>
  <div class="customer-form-view">
    <div class="page-header">
      <div class="header-content">
        <h1 class="page-title">{{ isEdit ? 'Editar' : 'Novo' }} Cliente</h1>
        <p class="page-subtitle">{{ isEdit ? 'Atualize as informações do cliente' : 'Cadastre um novo cliente no sistema' }}</p>
      </div>
      <div class="header-actions">
        <BaseButton
          variant="outline"
          icon="arrow-left"
          @click="handleCancel"
        >
          Voltar
        </BaseButton>
      </div>
    </div>

    <CustomerForm
      :customer="customer"
      :loading="isLoading"
      @submit="handleSubmit"
      @cancel="handleCancel"
    />
  </div>
</template>

<script>
import { useCustomerStore } from '@/stores/customers'
import { mapState, mapActions } from 'pinia'
import CustomerForm from '@/components/modules/customers/CustomerForm/CustomerForm.vue'
import BaseButton from '@/components/shared/BaseButton/BaseButton.vue'

export default {
  name: 'CustomerFormView',
  components: {
    CustomerForm,
    BaseButton
  },
  data() {
    return {
      customer: null
    }
  },
  computed: {
    ...mapState(useCustomerStore, ['isLoading']),
    isEdit() {
      return !!this.$route.params.id
    }
  },
  async mounted() {
    await this.loadCustomer()
  },
  methods: {
    ...mapActions(useCustomerStore, ['fetchCustomerById', 'updateCustomer', 'createCustomer']),
    
    async loadCustomer() {
      if (this.isEdit) {
        const result = await this.fetchCustomerById(this.$route.params.id)
        if (result.success) {
          this.customer = result.data
        } else {
          this.$router.push('/customers')
        }
      }
    },
    async handleSubmit(customerData) {
      let result
      
      if (this.isEdit) {
        result = await this.updateCustomer(this.$route.params.id, customerData)
      } else {
        result = await this.createCustomer(customerData)
      }

      if (result.success) {
        this.$router.push('/customers')
      }
    },
    handleCancel() {
      this.$router.push('/customers')
    }
  }
}
</script>

<style lang="scss" scoped>
.customer-form-view {
  padding: $spacing-6;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  max-width: 1000px;
  margin: 0 auto $spacing-6 auto;
  padding-bottom: $spacing-5;
  border-bottom: 1px solid $border;
}

.page-title {
  font-size: 2rem;
  font-weight: 600;
  color: $text-primary;
}

.page-subtitle {
  color: $text-secondary;
  font-size: $font-size-base;
  margin-top: $spacing-1;
}

@media (max-width: 768px) {
  .customer-form-view {
    padding: $spacing-4;
  }

  .page-header {
    flex-direction: column;
    gap: $spacing-4;
    text-align: center;
    margin-bottom: $spacing-5;
  }
}
</style>
