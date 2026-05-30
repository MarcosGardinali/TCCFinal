<template>
  <div class="table-wrapper">
    <!-- Loading State -->
    <div v-if="loading" class="loading-state">
      <i class="fas fa-spinner fa-spin"></i>
      <span>{{ loadingText }}</span>
    </div>

    <!-- Empty State -->
    <div v-else-if="items.length === 0" class="empty-state">
      <div class="empty-icon">{{ emptyIcon }}</div>
      <h3 class="empty-title">{{ emptyTitle }}</h3>
      <p class="empty-message">{{ emptyMessage }}</p>
      <slot name="empty-actions"></slot>
    </div>

    <!-- Table -->
    <div v-else class="table-container">
      <table class="data-table">
        <thead>
          <tr>
            <th
              v-for="column in columns"
              :key="column.key"
              :class="column.headerClass"
              @click="column.sortable ? handleSort(column.key) : null"
              :style="{ cursor: column.sortable ? 'pointer' : 'default' }"
            >
              {{ column.label }}
              <i
                v-if="column.sortable && sortBy === column.key"
                :class="[
                  'fas',
                  sortOrder === 'asc' ? 'fa-sort-up' : 'fa-sort-down'
                ]"
                class="sort-icon"
              ></i>
            </th>
            <th v-if="hasActions" class="actions-header">Ações</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, index) in items" :key="getItemKey(item, index)">
            <td
              v-for="column in columns"
              :key="column.key"
              :class="column.cellClass"
              :data-label="column.label"
            >
              <slot
                :name="`cell-${column.key}`"
                :item="item"
                :value="getNestedValue(item, column.key)"
                :index="index"
              >
                {{ formatValue(getNestedValue(item, column.key), column) }}
              </slot>
            </td>
            <td v-if="hasActions" class="actions">
              <slot name="actions" :item="item" :index="index"></slot>
            </td>
          </tr>
        </tbody>
      </table>

      <!-- Pagination -->
      <div v-if="showPagination && pagination.totalPages > 1" class="pagination">
        <button
          @click="$emit('page-change', pagination.currentPage - 1)"
          :disabled="pagination.currentPage <= 1"
          class="btn btn-secondary"
        >
          <i class="fas fa-chevron-left"></i>
        </button>

        <span class="page-info">
          Página {{ pagination.currentPage }} de {{ pagination.totalPages }}
          ({{ pagination.totalItems }} {{ itemName }})
        </span>

        <button
          @click="$emit('page-change', pagination.currentPage + 1)"
          :disabled="pagination.currentPage >= pagination.totalPages"
          class="btn btn-secondary"
        >
          <i class="fas fa-chevron-right"></i>
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import { computed } from 'vue'

export default {
  name: 'BaseTable',
  props: {
    items: {
      type: Array,
      required: true
    },
    columns: {
      type: Array,
      required: true
    },
    loading: {
      type: Boolean,
      default: false
    },
    loadingText: {
      type: String,
      default: 'Carregando...'
    },
    emptyIcon: {
      type: String,
      default: '📋'
    },
    emptyTitle: {
      type: String,
      default: 'Nenhum item encontrado'
    },
    emptyMessage: {
      type: String,
      default: 'Não há dados para exibir'
    },
    itemName: {
      type: String,
      default: 'itens'
    },
    pagination: {
      type: Object,
      default: () => ({
        currentPage: 1,
        totalPages: 1,
        totalItems: 0,
        pageSize: 10
      })
    },
    showPagination: {
      type: Boolean,
      default: true
    },
    sortBy: {
      type: String,
      default: ''
    },
    sortOrder: {
      type: String,
      default: 'asc',
      validator: (value) => ['asc', 'desc'].includes(value)
    },
    itemKey: {
      type: String,
      default: 'id'
    }
  },
  emits: ['page-change', 'sort-change'],
  setup(props, { emit, slots }) {
    const hasActions = computed(() => !!slots.actions)

    const getItemKey = (item, index) => {
      return item[props.itemKey] || index
    }

    const getNestedValue = (obj, path) => {
      return path.split('.').reduce((current, key) => current?.[key], obj)
    }

    const formatValue = (value, column) => {
      if (value === null || value === undefined) {
        return '-'
      }

      if (column.formatter && typeof column.formatter === 'function') {
        return column.formatter(value)
      }

      if (column.type === 'currency') {
        return new Intl.NumberFormat('pt-BR', {
          style: 'currency',
          currency: 'BRL'
        }).format(value)
      }

      if (column.type === 'date') {
        return new Date(value).toLocaleDateString('pt-BR')
      }

      if (column.type === 'datetime') {
        return new Date(value).toLocaleString('pt-BR')
      }

      return value
    }

    const handleSort = (columnKey) => {
      let newOrder = 'asc'
      if (props.sortBy === columnKey && props.sortOrder === 'asc') {
        newOrder = 'desc'
      }
      emit('sort-change', { sortBy: columnKey, sortOrder: newOrder })
    }

    return {
      hasActions,
      getItemKey,
      getNestedValue,
      formatValue,
      handleSort
    }
  }
}
</script>

<style lang="scss" scoped>
@import './BaseTable.scss';
</style>
