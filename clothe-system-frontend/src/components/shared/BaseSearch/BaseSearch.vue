<template>
  <div class="search-container">
    <div class="search-box">
      <input
        v-model="searchValue"
        type="text"
        :placeholder="placeholder"
        @keyup.enter="handleSearch"
        @input="handleInput"
        class="search-input"
      />
      <button
        @click="handleSearch"
        class="btn btn-secondary search-btn"
        :disabled="loading"
      >
        <i v-if="loading" class="fas fa-spinner fa-spin"></i>
        <i v-else class="fas fa-search"></i>
      </button>
      <button
        v-if="showClearButton && searchValue"
        @click="handleClear"
        class="btn btn-outline clear-btn"
        title="Limpar busca"
      >
        <i class="fas fa-times"></i>
      </button>
    </div>
    
    <!-- Filtros Avançados -->
    <div v-if="showAdvancedFilters" class="advanced-filters">
      <slot name="filters"></slot>
    </div>
  </div>
</template>

<script>
import { ref, watch } from 'vue'

export default {
  name: 'BaseSearch',
  props: {
    modelValue: {
      type: String,
      default: ''
    },
    placeholder: {
      type: String,
      default: 'Buscar...'
    },
    loading: {
      type: Boolean,
      default: false
    },
    showClearButton: {
      type: Boolean,
      default: true
    },
    showAdvancedFilters: {
      type: Boolean,
      default: false
    },
    debounceTime: {
      type: Number,
      default: 0
    }
  },
  emits: ['update:modelValue', 'search', 'clear', 'input'],
  setup(props, { emit }) {
    const searchValue = ref(props.modelValue)
    let debounceTimer = null

    // Sincronizar com o v-model
    watch(() => props.modelValue, (newValue) => {
      searchValue.value = newValue
    })

    const handleSearch = () => {
      emit('update:modelValue', searchValue.value)
      emit('search', searchValue.value)
    }

    const handleInput = () => {
      emit('update:modelValue', searchValue.value)
      emit('input', searchValue.value)

      // Debounce para busca automática
      if (props.debounceTime > 0) {
        clearTimeout(debounceTimer)
        debounceTimer = setTimeout(() => {
          emit('search', searchValue.value)
        }, props.debounceTime)
      }
    }

    const handleClear = () => {
      searchValue.value = ''
      emit('update:modelValue', '')
      emit('clear')
      emit('search', '')
    }

    return {
      searchValue,
      handleSearch,
      handleInput,
      handleClear
    }
  }
}
</script>

<style lang="scss" scoped>
@import './BaseSearch.scss';
</style>
