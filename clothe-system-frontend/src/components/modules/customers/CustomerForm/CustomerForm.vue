<template>
  <form @submit.prevent="handleSubmit" class="customer-form">
    <!-- Personal Information Section -->
    <div class="form-section">
      <div class="section-header">
        <div class="section-icon">
          <i class="fas fa-user"></i>
        </div>
        <div class="section-info">
          <h3 class="section-title">Informações Pessoais</h3>
          <p class="section-subtitle">Dados básicos do cliente</p>
        </div>
      </div>

      <div class="form-grid">
        <BaseInput
          v-model="form.firstName"
          label="Nome"
          type="text"
          placeholder="Nome do cliente"
          :error="errors.firstName"
          required
        />
        <BaseInput
          v-model="form.lastName"
          label="Sobrenome"
          type="text"
          placeholder="Sobrenome do cliente"
          :error="errors.lastName"
          required
        />
        <BaseInput
          v-model="form.birthDate"
          label="Data de Nascimento"
          type="date"
          :error="errors.birthDate"
          required
        />
        <BaseInput
          v-model="form.cpf"
          label="CPF"
          type="cpf"
          placeholder="000.000.000-00"
          :error="errors.cpf"
          required
        />
        <BaseInput
          v-model="form.rg"
          label="RG"
          type="rg"
          placeholder="00.000.000-0"
          :error="errors.rg"
          required
        />
        <BaseInput
          v-model="form.mobilePhoneNumber"
          label="Telefone"
          type="tel"
          placeholder="(00) 00000-0000"
          :error="errors.mobilePhoneNumber"
          required
        />
      </div>

      <div class="form-row">
        <BaseInput
          v-model="form.email"
          label="E-mail"
          type="email"
          placeholder="cliente@email.com"
          :error="errors.email"
          required
          full-width
        />
      </div>
    </div>

    <!-- Address Section -->
    <div class="form-section">
      <div class="section-header">
        <div class="section-icon">
          <i class="fas fa-map-marker-alt"></i>
        </div>
        <div class="section-info">
          <h3 class="section-title">Endereço</h3>
          <p class="section-subtitle">Localização do cliente</p>
        </div>
      </div>

      <div class="form-grid">
        <BaseInput
          v-model="form.postalCode"
          label="CEP"
          type="cep"
          placeholder="00000-000"
          :error="errors.postalCode"
          @blur="handleCEPBlur"
        />

        <BaseInput
          v-model="form.street"
          label="Logradouro"
          type="text"
          placeholder="Nome da rua"
          :error="errors.street"
        />

        <BaseInput
          v-model="form.number"
          label="Número"
          type="text"
          placeholder="123"
          :error="errors.number"
        />

        <BaseInput
          v-model="form.complement"
          label="Complemento"
          type="text"
          placeholder="Apto, Bloco, etc."
          :error="errors.complement"
        />

        <BaseInput
          v-model="form.neighborhood"
          label="Bairro"
          type="text"
          placeholder="Nome do bairro"
          :error="errors.neighborhood"
        />

        <BaseInput
          v-model="form.cityName"
          label="Cidade"
          type="text"
          placeholder="Nome da cidade"
          :error="errors.cityName"
        />

        <BaseInput
          v-model="form.stateAbbreviation"
          label="Estado"
          type="select"
          placeholder="Selecione o estado"
          :options="stateOptions"
          :error="errors.stateAbbreviation"
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
        {{ isEdit ? 'Atualizar' : 'Criar' }} Cliente
      </BaseButton>
    </div>
  </form>
</template>

<script>
import { ref, computed, watch } from 'vue'
import BaseInput from '@/components/shared/BaseInput/BaseInput.vue'
import BaseButton from '@/components/shared/BaseButton/BaseButton.vue'
import * as yup from 'yup'

// Estados brasileiros
const BrazilianStates = [
  { name: 'Acre', abbreviation: 'AC' },
  { name: 'Alagoas', abbreviation: 'AL' },
  { name: 'Amapá', abbreviation: 'AP' },
  { name: 'Amazonas', abbreviation: 'AM' },
  { name: 'Bahia', abbreviation: 'BA' },
  { name: 'Ceará', abbreviation: 'CE' },
  { name: 'Distrito Federal', abbreviation: 'DF' },
  { name: 'Espírito Santo', abbreviation: 'ES' },
  { name: 'Goiás', abbreviation: 'GO' },
  { name: 'Maranhão', abbreviation: 'MA' },
  { name: 'Mato Grosso', abbreviation: 'MT' },
  { name: 'Mato Grosso do Sul', abbreviation: 'MS' },
  { name: 'Minas Gerais', abbreviation: 'MG' },
  { name: 'Pará', abbreviation: 'PA' },
  { name: 'Paraíba', abbreviation: 'PB' },
  { name: 'Paraná', abbreviation: 'PR' },
  { name: 'Pernambuco', abbreviation: 'PE' },
  { name: 'Piauí', abbreviation: 'PI' },
  { name: 'Rio de Janeiro', abbreviation: 'RJ' },
  { name: 'Rio Grande do Norte', abbreviation: 'RN' },
  { name: 'Rio Grande do Sul', abbreviation: 'RS' },
  { name: 'Rondônia', abbreviation: 'RO' },
  { name: 'Roraima', abbreviation: 'RR' },
  { name: 'Santa Catarina', abbreviation: 'SC' },
  { name: 'São Paulo', abbreviation: 'SP' },
  { name: 'Sergipe', abbreviation: 'SE' },
  { name: 'Tocantins', abbreviation: 'TO' }
]

export default {
  name: 'CustomerForm',
  components: {
    BaseInput,
    BaseButton
  },
  props: {
    customer: {
      type: Object,
      default: null
    },
    loading: {
      type: Boolean,
      default: false
    }
  },
  emits: ['submit', 'cancel'],
  setup(props, { emit }) {
    const form = ref({
      firstName: '',
      lastName: '',
      birthDate: '',
      cpf: '',
      rg: '',
      mobilePhoneNumber: '',
      email: '',
      postalCode: '',
      street: '',
      number: '',
      complement: '',
      neighborhood: '',
      cityName: '',
      stateAbbreviation: ''
    })

    const errors = ref({})
    
    const isEdit = computed(() => !!props.customer)
    
    const stateOptions = computed(() => 
      BrazilianStates.map(state => ({
        value: state.abbreviation,
        label: `${state.name} (${state.abbreviation})`
      }))
    )

    // Schema de validação
    const customerSchema = yup.object({
      firstName: yup.string().required('Nome é obrigatório'),
      lastName: yup.string().required('Sobrenome é obrigatório'),
      birthDate: yup.date().required('Data de nascimento é obrigatória'),
      cpf: yup.string().required('CPF é obrigatório'),
      rg: yup.string().required('RG é obrigatório'),
      mobilePhoneNumber: yup.string().required('Telefone é obrigatório'),
      email: yup.string().email('E-mail inválido').required('E-mail é obrigatório'),
      postalCode: yup.string(),
      street: yup.string(),
      number: yup.string(),
      complement: yup.string(),
      neighborhood: yup.string(),
      cityName: yup.string(),
      stateAbbreviation: yup.string()
    })

    // Carregar dados do cliente para edição
    watch(() => props.customer, (customer) => {
      if (customer) {
        console.log('Carregando dados do cliente:', customer)
        
        // Formatar data de nascimento para o formato do input date
        let birthDate = ''
        if (customer.birthDate) {
          const date = new Date(customer.birthDate)
          birthDate = date.toISOString().split('T')[0]
        }
        
        form.value = {
          firstName: customer.firstName || '',
          lastName: customer.lastName || '',
          birthDate: birthDate,
          cpf: customer.cpf || '',
          rg: customer.rg || '',
          mobilePhoneNumber: customer.mobilePhoneNumber || '',
          email: customer.email || '',
          postalCode: customer.postalCode || '',
          street: customer.street || '',
          number: customer.number || '',
          complement: customer.complement || '',
          neighborhood: customer.neighborhood || '',
          cityName: customer.cityName || '',
          stateAbbreviation: customer.stateAbbreviation || ''
        }
        
        console.log('Formulário preenchido:', form.value)
      }
    }, { immediate: true })

    const validateForm = async () => {
      try {
        await customerSchema.validate(form.value, { abortEarly: false })
        errors.value = {}
        return true
      } catch (error) {
        const newErrors = {}
        error.inner.forEach(err => {
          newErrors[err.path] = err.message
        })
        errors.value = newErrors
        return false
      }
    }

    // Buscar endereço por CEP
    const handleCEPBlur = async () => {
      const cep = form.value.postalCode?.replace(/\D/g, '')

      if (cep && cep.length === 8) {
        try {
          const response = await fetch(`https://viacep.com.br/ws/${cep}/json/`)
          const data = await response.json()

          if (!data.erro) {
            form.value.street = data.logradouro || ''
            form.value.neighborhood = data.bairro || ''
            form.value.cityName = data.localidade || ''
            form.value.stateAbbreviation = data.uf || ''
          }
        } catch (error) {
          console.warn('Erro ao buscar CEP:', error)
        }
      }
    }

    const handleSubmit = async () => {
      const isValid = await validateForm()
      if (isValid) {
        // Preparar dados para envio
        const customerData = { ...form.value }
        
        // Converter data de nascimento para o formato esperado pela API
        if (customerData.birthDate) {
          customerData.birthDate = new Date(customerData.birthDate).toISOString()
        }
        
        console.log('Enviando dados:', customerData)
        emit('submit', customerData)
      }
    }

    return {
      form,
      errors,
      isEdit,
      stateOptions,
      handleSubmit,
      handleCEPBlur
    }
  }
}
</script>

<style lang="scss" scoped>
@import './CustomerForm.scss';
</style>
