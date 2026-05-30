<template>
  <div class="product-form-view">
    <div class="page-header">
      <div class="header-content">
        <h1 class="page-title">{{ isEdit ? 'Editar' : 'Novo' }} Produto</h1>
        <p class="page-subtitle">{{ isEdit ? 'Atualize as informações do produto' : 'Cadastre um novo produto no sistema' }}</p>
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

    <ProductForm
      :product="product"
      :loading="isLoading"
      @submit="handleSubmit"
      @cancel="handleCancel"
    />
  </div>
</template>

<script>
import { useProductStore } from '@/stores/products'
import { mapState, mapActions } from 'pinia'
import ProductForm from '@/components/modules/products/ProductForm/ProductForm.vue'
import BaseButton from '@/components/shared/BaseButton/BaseButton.vue'

export default {
  name: 'ProductFormView',
  components: {
    ProductForm,
    BaseButton
  },
  data() {
    return {
      product: null
    }
  },
  computed: {
    ...mapState(useProductStore, ['isLoading']),
    isEdit() {
      return !!this.$route.params.id
    }
  },
  async mounted() {
    await this.loadProduct()
  },
  methods: {
    ...mapActions(useProductStore, ['fetchProductById', 'updateProduct', 'createProduct']),
    
    async loadProduct() {
      if (this.isEdit) {
        const result = await this.fetchProductById(this.$route.params.id)
        if (result.success) {
          this.product = result.data
        } else {
          this.$router.push('/products')
        }
      }
    },
    async handleSubmit(productData) {
      let result
      
      if (this.isEdit) {
        result = await this.updateProduct(this.$route.params.id, productData)
      } else {
        result = await this.createProduct(productData)
      }

      if (result.success) {
        this.$router.push('/products')
      }
    },
    handleCancel() {
      this.$router.push('/products')
    }
  }
}
</script>

<style lang="scss" scoped>
@import './ProductFormView.scss';
</style>
