<template>
  <div class="order-form-view">
    <div class="page-header">
      <div class="header-content">
        <h1 class="page-title">{{ isEdit ? 'Editar' : 'Novo' }} Pedido</h1>
        <p class="page-subtitle">{{ isEdit ? 'Atualize as informações do pedido' : 'Cadastre um novo pedido no sistema' }}</p>
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

    <OrderForm
      :order="order"
      :loading="isLoading"
      @submit="handleSubmit"
      @cancel="handleCancel"
    />
  </div>
</template>

<script>
import { useOrderStore } from '@/stores/orders'
import { mapState, mapActions } from 'pinia'
import OrderForm from '@/components/modules/orders/OrderForm/OrderForm.vue'
import BaseButton from '@/components/shared/BaseButton/BaseButton.vue'

export default {
  name: 'OrderFormView',
  components: {
    OrderForm,
    BaseButton
  },
  data() {
    return {
      order: null
    }
  },
  computed: {
    ...mapState(useOrderStore, ['isLoading']),
    isEdit() {
      return !!this.$route.params.id
    }
  },
  async mounted() {
    await this.loadOrder()
  },
  methods: {
    ...mapActions(useOrderStore, ['fetchOrderById', 'updateOrder', 'createOrder']),
    
    async loadOrder() {
      if (this.isEdit) {
        const result = await this.fetchOrderById(this.$route.params.id)
        if (result.success) {
          this.order = result.data
        } else {
          this.$router.push('/orders')
        }
      }
    },
    async handleSubmit(orderData) {
      let result
      
      if (this.isEdit) {
        result = await this.updateOrder(this.$route.params.id, orderData)
      } else {
        result = await this.createOrder(orderData)
      }

      if (result.success) {
        this.$router.push('/orders')
      }
    },
    handleCancel() {
      this.$router.push('/orders')
    }
  }
}
</script>

<style lang="scss" scoped>
@import './OrderFormView.scss';
</style>
