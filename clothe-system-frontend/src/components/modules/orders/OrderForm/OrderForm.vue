<template>
  <form @submit.prevent="handleSubmit" class="order-form">
    <!-- Order Information Section -->
    <div class="form-section">
      <div class="section-header">
        <div class="section-icon">
          <i class="fas fa-shopping-cart"></i>
        </div>
        <div class="section-info">
          <h3 class="section-title">Informações do Pedido</h3>
          <p class="section-subtitle">Dados básicos do pedido</p>
        </div>
      </div>

      <div class="form-grid">
        <BaseInput
          v-model="form.customerId"
          label="Cliente"
          type="select"
          placeholder="Selecione o cliente"
          :options="customerOptions"
          :error="errors.customerId"
          required
        />
      </div>

      <div class="form-row">
        <BaseInput
          v-model="form.observation"
          label="Observações"
          type="textarea"
          placeholder="Observações sobre o pedido (opcional)"
          :error="errors.observation"
          :rows="4"
          full-width
        />
      </div>
    </div>

    <!-- Products Section -->
    <div class="form-section">
      <div class="section-header">
        <div class="section-icon">
          <i class="fas fa-box"></i>
        </div>
        <div class="section-info">
          <h3 class="section-title">Produtos do Pedido</h3>
          <p class="section-subtitle">Adicione os produtos desejados</p>
        </div>
        <div class="section-actions">
          <BaseButton
            type="button"
            variant="primary"
            icon="plus"
            @click="addProduct"
            :disabled="!hasProducts"
            :title="!hasProducts ? 'Nenhum produto disponível' : 'Adicionar produto ao pedido'"
          >
            Adicionar Produto
          </BaseButton>
        </div>
      </div>

      <div v-if="!hasProducts" class="empty-state">
        <div class="empty-icon">
          <i class="fas fa-exclamation-triangle"></i>
        </div>
        <h4 class="empty-title">Nenhum produto cadastrado</h4>
        <p class="empty-message">É necessário ter produtos cadastrados para criar um pedido.</p>
        <router-link to="/products/new" class="empty-action">
          <BaseButton variant="primary" icon="plus">
            Cadastrar Produto
          </BaseButton>
        </router-link>
      </div>

      <div v-else-if="form.items.length === 0" class="empty-state">
        <div class="empty-icon">
          <i class="fas fa-shopping-bag"></i>
        </div>
        <h4 class="empty-title">Nenhum produto adicionado</h4>
        <p class="empty-message">Clique em "Adicionar Produto" para começar a montar o pedido.</p>
      </div>

      <div v-else class="products-list">
        <div v-for="(item, index) in form.items" :key="index" class="product-item">
          <div class="product-icon">
            <i class="fas fa-tshirt"></i>
          </div>
          <div class="product-fields">
            <BaseInput
              v-model="item.productId"
              label="Produto"
              type="select"
              placeholder="Selecione o produto"
              :options="productOptions"
              :error="errors[`items.${index}.productId`]"
              required
            />

            <BaseInput
              v-model="item.variation"
              label="Variação"
              type="text"
              placeholder="Ex: Tamanho M, Cor Azul"
              :error="errors[`items.${index}.variation`]"
            />
          </div>

          <BaseButton
            type="button"
            variant="danger"
            size="sm"
            icon="trash"
            @click="removeProduct(index)"
            title="Remover produto"
            class="remove-button"
          />
        </div>
      </div>

      <div v-if="errors.items" class="error-message">
        <i class="fas fa-exclamation-circle"></i>
        {{ errors.items }}
      </div>
    </div>

    <!-- Form Actions -->
    <div class="form-actions">
      <BaseButton
        variant="outline"
        icon="arrow-left"
        @click="$emit('cancel')"
      >
        Cancelar
      </BaseButton>
      <BaseButton
        type="submit"
        variant="primary"
        :icon="isEdit ? 'save' : 'plus'"
        :loading="loading"
      >
        {{ isEdit ? 'Atualizar' : 'Criar' }} Pedido
      </BaseButton>
    </div>
  </form>
</template>

<script>
import { useCustomerStore } from '@/stores/customers'
import { useProductStore } from '@/stores/products'
import { mapState, mapActions } from 'pinia'
import BaseInput from '@/components/shared/BaseInput/BaseInput.vue'
import BaseButton from '@/components/shared/BaseButton/BaseButton.vue'
import * as yup from 'yup'

export default {
  name: 'OrderForm',
  components: {
    BaseInput,
    BaseButton
  },
  props: {
    order: {
      type: Object,
      default: null
    },
    loading: {
      type: Boolean,
      default: false
    }
  },
  emits: ['submit', 'cancel'],
  data() {
    return {
      form: {
        customerId: '',
        observation: '',
        items: []
      },
      errors: {}
    }
  },
  computed: {
    ...mapState(useCustomerStore, ['customers']),
    ...mapState(useProductStore, ['products']),
    
    isEdit() {
      return !!this.order
    },
    hasProducts() {
      return this.products && this.products.length > 0
    },
    hasCustomers() {
      return this.customers && this.customers.length > 0
    },
    customerOptions() {
      return this.customers.map(customer => ({
        value: customer.id,
        label: `${customer.firstName} ${customer.lastName} - ${customer.cpf}`
      }))
    },
    productOptions() {
      return this.products.map(product => ({
        value: product.id,
        label: `${product.name || product.description} - ${this.formatCurrency(product.price)}`
      }))
    }
  },
  watch: {
    order: {
      handler(order) {
        if (order) {
          this.form = {
            customerId: order.customerId?.toString() || '',
            observation: order.observation || '',
            items: order.listOrderItem?.map(item => ({
              productId: item.productId?.toString() || '',
              variation: item.variation || ''
            })) || []
          }
        }
      },
      immediate: true
    }
  },
  async mounted() {
    await this.loadData()
  },
  methods: {
    ...mapActions(useCustomerStore, { fetchCustomers: 'fetchCustomers' }),
    ...mapActions(useProductStore, { fetchProducts: 'fetchProducts' }),
    
    async loadData() {
      try {
        await Promise.all([
          this.fetchCustomers(1, 100),
          this.fetchProducts(1, 100)
        ])
      } catch (error) {
        console.error('Erro ao carregar dados:', error)
      }
    },

    formatCurrency(value) {
      return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
      }).format(value || 0)
    },
    addProduct() {
      this.form.items.push({
        productId: '',
        variation: ''
      })
    },
    removeProduct(index) {
      this.form.items.splice(index, 1)
    },
    async validateForm() {
      try {
        const orderSchema = yup.object({
          customerId: yup.number().required('Cliente é obrigatório'),
          observation: yup.string(),
          items: yup.array().min(1, 'Adicione pelo menos um produto ao pedido')
        })

        if (this.products.length > 0 && this.form.items.length === 0) {
          this.errors.items = 'Adicione pelo menos um produto ao pedido'
          return false
        }

        await orderSchema.validate(this.form, { abortEarly: false })
        this.errors = {}
        return true
      } catch (error) {
        const newErrors = {}
        error.inner.forEach(err => {
          newErrors[err.path] = err.message
        })
        this.errors = newErrors
        return false
      }
    },
    async handleSubmit() {
      const isValid = await this.validateForm()
      if (!isValid) return

      const orderData = {
        customerId: parseInt(this.form.customerId),
        observation: this.form.observation || null,
        listCreatedItem: this.form.items.map(item => ({
          productId: parseInt(item.productId),
          variation: item.variation || null
        }))
      }

      this.$emit('submit', orderData)
    }
  }
}
</script>

<style lang="scss" scoped>
@import './OrderForm.scss';
</style>
