<template>
  <header class="header">
    <div class="header-left">
      <button 
        class="sidebar-toggle"
        @click="$emit('toggle-sidebar')"
      >
        ☰
      </button>
      
      <h1 class="page-title">{{ pageTitle }}</h1>
    </div>
    
    <div class="header-right">
      <div class="user-menu">
        <button 
          class="user-button"
          @click="toggleUserMenu"
        >
          <span class="user-avatar">👤</span>
          <span class="user-name">Usuário</span>
          <span class="dropdown-arrow">▼</span>
        </button>
        
        <div 
          v-if="userMenuOpen" 
          class="user-dropdown"
          @click.stop
        >
          <button 
            class="dropdown-item"
            @click="handleLogout"
          >
            Sair
          </button>
        </div>
      </div>
    </div>
  </header>
</template>

<script>
import { useAuthStore } from '@/stores/auth'
import { mapActions } from 'pinia'

export default {
  name: 'Header',
  emits: ['toggle-sidebar'],
  data() {
    return {
      userMenuOpen: false
    }
  },
  computed: {
    pageTitle() {
      const titles = {
        'Dashboard': 'Dashboard',
        'CustomerList': 'Clientes',
        'CustomerCreate': 'Novo Cliente',
        'CustomerEdit': 'Editar Cliente',
        'CustomerView': 'Visualizar Cliente',
        'ProductList': 'Produtos',
        'ProductCreate': 'Novo Produto',
        'ProductEdit': 'Editar Produto',
        'ProductView': 'Visualizar Produto',
        'OrderList': 'Pedidos',
        'OrderCreate': 'Novo Pedido',
        'OrderEdit': 'Editar Pedido',
        'OrderView': 'Visualizar Pedido'
      }

      return titles[this.$route.name] || 'OutfitTrack'
    }
  },
  mounted() {
    document.addEventListener('click', this.handleClickOutside)
  },
  beforeUnmount() {
    document.removeEventListener('click', this.handleClickOutside)
  },
  methods: {
    ...mapActions(useAuthStore, ['logout']),
    
    toggleUserMenu() {
      this.userMenuOpen = !this.userMenuOpen
    },
    handleLogout() {
      this.logout()
      this.$router.push('/auth/login')
    },
    handleClickOutside(event) {
      if (!event.target.closest('.user-menu')) {
        this.userMenuOpen = false
      }
    }
  }
}
</script>

<style lang="scss" scoped>
.header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: $spacing-4 $spacing-6;
  background: $surface;
  border-bottom: 1px solid $border;
  height: 65px;
  position: sticky;
  top: 0;
  z-index: $z-sticky;
}

.header-left {
  display: flex;
  align-items: center;
  gap: $spacing-4;
}

.sidebar-toggle {
  background: none;
  border: none;
  font-size: 1.5rem;
  color: $text-primary;
  cursor: pointer;
  padding: $spacing-2;
  border-radius: $border-radius;
  transition: background-color 0.2s ease;
  
  &:hover {
    background-color: rgba($primary-color, 0.1);
  }
}

.page-title {
  font-size: $font-size-xl;
  font-weight: 600;
  color: $text-primary;
  margin: 0;
}

.header-right {
  display: flex;
  align-items: center;
}

.user-menu {
  position: relative;
}

.user-button {
  display: flex;
  align-items: center;
  gap: $spacing-2;
  background: none;
  border: none;
  padding: $spacing-2 $spacing-3;
  border-radius: $border-radius-full;
  cursor: pointer;
  transition: background-color 0.2s ease;
  
  &:hover {
    background-color: $gray-50;
  }
}

.user-avatar {
  font-size: $font-size-lg;
  background-color: $primary-light;
  color: $white;
  border-radius: 50%;
  width: 32px;
  height: 32px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.user-name {
  font-weight: 500;
  color: $text-secondary;
}

.dropdown-arrow {
  font-size: $font-size-xs;
  color: $text-muted;
  transition: transform 0.2s ease;
  
  .user-button:hover & {
    transform: translateY(2px);
  }
}

.user-dropdown {
  position: absolute;
  top: calc(100% + 8px);
  right: 0;
  background: $white;
  border: 1px solid $border;
  border-radius: $border-radius-md;
  box-shadow: $shadow-lg;
  min-width: 150px;
  z-index: $z-dropdown;
  padding: $spacing-2 0;
}

.dropdown-item {
  display: block;
  width: 100%;
  padding: $spacing-2 $spacing-4;
  background: none;
  border: none;
  text-align: left;
  color: $text-primary;
  cursor: pointer;
  transition: background-color 0.2s ease;
  
  &:hover {
    background-color: $gray-50;
    color: $primary-color;
  }
}

@media (max-width: $breakpoint-sm) {
  .header {
    padding: $spacing-3 $spacing-4;
  }
  
  .page-title {
    display: none; // Hide title on very small screens for more space
  }
  
  .user-name {
    display: none;
  }
}
</style>
