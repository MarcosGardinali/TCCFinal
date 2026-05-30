<template>
  <form @submit.prevent="handleSubmit" class="product-form">
    <!-- Product Information Section -->
    <div class="form-section">
      <div class="section-header">
        <div class="section-icon">
          <i class="fas fa-box"></i>
        </div>
        <div class="section-info">
          <h3 class="section-title">Informações do Produto</h3>
          <p class="section-subtitle">Dados básicos do produto</p>
        </div>
      </div>

      <div class="form-grid">
        <BaseInput
          v-model="form.code"
          label="Código"
          type="text"
          placeholder="Ex: PROD001"
          :error="errors.code"
          required
        />

        <BaseInput
          v-model="form.name"
          label="Nome"
          type="text"
          placeholder="Nome do produto"
          :error="errors.name"
          required
        />

        <BaseInput
          v-model="form.price"
          label="Preço"
          type="currency"
          placeholder="0,00"
          :error="errors.price"
          required
        />
      </div>

      <div class="form-row">
        <BaseInput
          v-model="form.description"
          label="Descrição"
          type="textarea"
          placeholder="Descrição detalhada do produto"
          :error="errors.description"
          :rows="4"
          full-width
        />
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
        {{ isEdit ? 'Atualizar' : 'Criar' }} Produto
      </BaseButton>
    </div>
  </form>
</template>

<script>
import BaseInput from '@/components/shared/BaseInput/BaseInput.vue'
import BaseButton from '@/components/shared/BaseButton/BaseButton.vue'
import * as yup from 'yup'

export default {
  name: 'ProductForm',
  components: {
    BaseInput,
    BaseButton
  },
  props: {
    product: {
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
        code: '',
        name: '',
        description: '',
        price: '',
      },
      errors: {}
    }
  },
  computed: {
    isEdit() {
      return !!this.product
    },
    // Schema de validação
    schema() {
      return yup.object({
        code: yup.string().required('Código é obrigatório'),
        name: yup.string().required('Nome é obrigatório'),
        description: yup.string(),
        price: yup.string()
          .required('Preço é obrigatório')
          .test('min-value', 'Preço deve ser maior que zero', function(value) {
            const numValue = parseFloat(value) / 100
            return numValue > 0
          }),
      })
    }
  },
  watch: {
    // Carregar dados do produto para edição
    product: {
      handler(product) {
        if (product) {
          console.log('Carregando dados do produto:', product)

          this.form = {
            code: product.code || '',
            name: product.description || product.name || '', // Usar description como nome
            description: product.description || '',
            price: product.price ? Math.round(product.price * 100).toString() : '', // Converter para centavos
          }

          console.log('Formulário preenchido:', this.form)
        }
      },
      immediate: true
    }
  },
  methods: {
    async validateForm() {
      try {
        await this.schema.validate(this.form, { abortEarly: false })
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

      const productData = {
        code: this.form.code,
        name: this.form.name,
        description: this.form.description || null,
        price: parseFloat(this.form.price) / 100, // Converte centavos para reais
      }

      console.log('Enviando dados do produto:', productData)
      this.$emit('submit', productData)
    }
  }
}
</script>

<style lang="scss" scoped>
@import './ProductForm.scss';
</style>
