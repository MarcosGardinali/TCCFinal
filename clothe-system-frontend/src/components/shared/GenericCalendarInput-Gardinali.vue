<template>
  <div class="calendar" :class="[{ not_required: showNotRequired }]">
    <label
      class="calendar-label"
      :class="[{ required: isRequired }, { disabled: disabled }]"
      :for="id"
    >
      {{ label }}
    </label>
    <div>
      <VueDatePicker
        v-model="date"

        :placeholder="placeholder"
        :format="formatDate"
        :preview-format="formatDate"
        :partial-range="false"
        :range="isRange"
        :enable-time-picker="false"
        :max-date="noDateLimit ? null : !isExpiresDate ? new Date() : null"
        :min-date="noDateLimit ? null : !isExpiresDate ? null : new Date(new Date().setDate(new Date().getDate() + 1))"

        :disabled="disabled"
        auto-apply
        :monthPicker="monthPicker"
        :style="{ marginBottom: showNotRequired ? '4px' : '' }"
      >
        <template #arrow-left>
          <span>⬅️</span>
        </template>
        <template #arrow-right>
          <span>➡️</span>
        </template>
        <template #input-icon>
          <span>📅</span>
        </template>
      </VueDatePicker>
    </div>
    <span v-if="showNotRequired" class="calendar-message">{{ `*Não obrigatório` }}</span>
  </div>
</template>

<script>
import { VueDatePicker } from '@vuepic/vue-datepicker'
import '@vuepic/vue-datepicker/dist/main.css'

export default {
  name: 'GenericCalendarInput',
  components: { VueDatePicker },
  props: {
    id: {
      type: String,
      required: true
    },
    label: {
      type: String,
      required: true
    },
    placeholder: {
      type: String,
      required: true
    },
    isRequired: {
      default: false,
      type: Boolean
    },
    modelValue: {
      required: true,
      type: null
    },
    isRange: {
      type: Boolean,
      default: false
    },
    disabled: {
      type: Boolean,
      default: false
    },
    monthPicker: {
      type: Boolean,
      default: false
    },
    format: {
      type: String,
      default: ''
    },
    isExpiresDate: {
      type: Boolean,
      required: false,
      default: false
    },
    showNotRequired: {
      type: Boolean,
      default: false
    },
    noDateLimit: {
      type: Boolean,
      default: false
    }
  },
  emits: ['update:modelValue'],
  data: () => ({
    date: null,
    fillColor: 'var(--structureIconsColor)',
    systemLanguage: localStorage.getItem('systemLanguage') || 'pt-BR'
  }),
  watch: {
    date(newValue) {
      // Se for range e só tiver uma data, duplicar para a segunda
      if (this.isRange && newValue && !Array.isArray(newValue)) {
        newValue = [newValue, newValue]
      }
      // Se for range e array com apenas um elemento, duplicar
      if (this.isRange && Array.isArray(newValue) && newValue.length === 1) {
        newValue = [newValue[0], newValue[0]]
      }
      this.$emit('update:modelValue', newValue)
    }
  },
  mounted() {
    this.date = this.modelValue || null
  },
  methods: {
    clearDate() {
      this.date = null
    },
    fillDate(date) {
      this.date = date
    },
    formatDate(date) {
      if (!date) return ''
      
      const formatSingleDate = (d) => {
        if (!d) return ''
        const dateObj = new Date(d)
        const day = String(dateObj.getDate()).padStart(2, '0')
        const month = String(dateObj.getMonth() + 1).padStart(2, '0')
        const year = dateObj.getFullYear()
        return `${day}/${month}/${year}`
      }

      if (this.isRange && Array.isArray(date)) {
        const start = formatSingleDate(date[0])
        const end = formatSingleDate(date[1])
        return `${start} - ${end}`
      }

      return formatSingleDate(date)
    }
  }
}
</script>

<style lang="scss">
.dp {
  &__menu {
    border: 1px solid #cedcea;
    border-radius: 0.5rem;
  }

  &__inner_nav {
    background: #fafcfe;
    transition: all 0.4s;

    svg {
      transform: scale(0.8);

      &.right-arrow {
        transform: scale(0.8) rotate(180deg);
      }
    }

    &:hover {
      background: #fafcfe;
    }
  }

  &__arrow {
    &_top {
      display: none;
    }
  }

  &__month_year_select {
    color: #203348;
    font-family: 'Open-Sans';
    font-size: 0.875rem;
    font-style: normal;
    font-weight: 400;
    line-height: normal;

    transition: all 0.4s;

    &:hover {
      background: #fafcfe;
    }
  }

  &__overlay {
    &_action {
      display: none;
    }

    &_col {
      width: 100%;
    }

    &_cell {
      background: #e9f3fd;

      color: #203348;
      font-family: 'Open-Sans';
      font-size: 0.75rem;
      font-style: normal;
      font-weight: 400;
      line-height: normal;

      transition: all 0.4s;

      &:hover {
        background: var(--structurePrimaryButton);
        color: #fff;
      }

      &_active {
        background: var(--structurePrimaryButton);

        color: #fff;
        font-family: 'Open-Sans';
        font-size: 0.75rem;
        font-style: normal;
        font-weight: 400;
        line-height: normal;
      }
    }

    &_container {
      &::-webkit-scrollbar {
        width: 4px;
        height: 6px;
      }

      &::-webkit-scrollbar-track {
        border-radius: 10px;
      }

      &::-webkit-scrollbar-thumb {
        background: var(--structurePrimaryButton);
        border-radius: 10px;
      }

      &::-webkit-scrollbar-thumb:hover {
        background: var(--structurePrimaryButton);
      }
    }
  }

  &__cell {
    &_inner {
      color: #203348;

      font-family: 'Open-Sans';
      font-size: 0.75rem;
      font-style: normal;
      font-weight: 400;
      line-height: normal;

      &:hover {
        border: 1px solid var(--structurePrimaryButton);
      }
    }

    &_offset {
      color: #c0c4cc;
    }

    &_disabled {
      color: #c0c4cc;

      &:hover {
        border: 0px;
      }
    }
  }

  &__range {
    &_start,
    &_between,
    &_end {
      background: #667eea !important;
      border-radius: 5px;
      color: #fff !important;

      &:hover {
        background: #667eea !important;
        border-radius: 5px;
        color: #fff !important;
      }
    }
  }

  &__active_date {
    background: #667eea !important;
    color: #fff !important;
  }

  &__today {
    border: 1px solid #667eea !important;
  }

  &__calendar {
    &_header {
      margin-top: 1.375rem;
      margin-bottom: -0.625rem;

      &_separator {
        height: 0px;
      }

      &_item {
        color: #cedcea;
        font-family: 'Open-Sans';
        font-size: 0.75rem;
        font-style: normal;
        font-weight: 400;
        line-height: normal;
        text-transform: capitalize;
      }
    }
  }

  &__date {
    &_hover {
      &_start:hover,
      &_end:hover,
      &_between:hover {
        background: #667eea !important;
        color: #fff !important;
      }
    }
  }

  &__range_hover {
    background: #667eea !important;
    color: #fff !important;
  }

  &__input {
    background: #fff;
    border: 1px solid #203348 !important;
    color: #203348;

    &_icon {
      display: flex;
      justify-content: flex-end;
      right: 0.625rem;

      &_pad {
        padding-inline-start: 0.6875rem;
      }
    }

    &.disabled {
      pointer-events: none;
      opacity: 0.3;
    }
  }

  &__clear_icon {
    display: none;
  }

  &--tp-wrap {
    display: none;
  }

  &__disabled {
    border: 1px solid #cedcea !important;

    &::placeholder {
      color: #cedcea !important;
    }
  }
}

.calendar {
  width: 100%;

  &-label {
    color: #203348;

    font-size: 0.875rem;
    font-weight: 400;
    line-height: 21px;

    &.required {
      color: #fa0303;
    }

    &.disabled {
      color: #cedcea;
    }
  }

  &-message {
    font-size: 0.78rem;
  }
}

@media (min-width: 992px) {
  .calendar {

    &.not_required {
      margin-bottom: 0;
    }
  }
}
</style>