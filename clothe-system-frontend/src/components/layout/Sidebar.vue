<template>
  <aside
    class="sidebar"
    :class="{
      'is-open': isOpen,
      'is-mini': !isOpen && !isMobile,
      'is-mobile': isMobile
    }"
  >
    <div class="sidebar-header">
      <div class="header-logo">
        <img src="@/assets/logo.png" alt="ClotheSystem Logo" class="logo-image" />
        <h2 v-show="isOpen || isMobile">ClotheSystem</h2>
      </div>
    </div>

    <nav class="sidebar-nav">
      <ul>
        <li v-for="item in menu" :key="item.path">
          <router-link
            :to="item.path"
            class="nav-link"
            active-class="active"
            :title="!isOpen && !isMobile ? item.label : ''"
            @click="handleMobileClick"
          >
            <span class="nav-icon">{{ item.icon }}</span>
            <span v-show="isOpen || isMobile">
              {{ item.label }}
            </span>
          </router-link>
        </li>
      </ul>
    </nav>
  </aside>

  <!-- Overlay -->
  <div
    v-if="isOpen && isMobile"
    class="overlay"
    @click="$emit('toggle')"
  />
</template>
<script>
import { ref, onMounted, onUnmounted } from 'vue'

export default {
  name: 'Sidebar',
  props: {
    isOpen: Boolean
  },
  emits: ['toggle'],
  setup(props, { emit }) {
    const isMobile = ref(window.innerWidth < 1024)

    const handleResize = () => {
      isMobile.value = window.innerWidth < 1024
    }

    const handleMobileClick = () => {
      if (isMobile.value) emit('toggle')
    }

    const menu = [
      { path: '/dashboard', label: 'Dashboard', icon: '📊' },
      { path: '/customers', label: 'Clientes', icon: '👥' },
      { path: '/products', label: 'Produtos', icon: '👕' },
      { path: '/orders', label: 'Pedidos', icon: '📋' },
      { path: '/reports', label: 'Relatórios', icon: '📈' }
    ]

    onMounted(() => window.addEventListener('resize', handleResize))
    onUnmounted(() => window.removeEventListener('resize', handleResize))

    return { isMobile, menu, handleMobileClick }
  }
}
</script>
<style lang="scss" scoped>

.sidebar {
  position: fixed;
  background: $primary-color;
  transition: width 0.3s ease, transform 0.3s ease;
  z-index: 1000;
  overflow-x: hidden;
  color: $white;
}

/* ===== DESKTOP ===== */
@media (min-width: 1024px) {
  .sidebar {
    top: 0;
    left: 0;
    height: 100vh;
    width: 70px; /* Default mini state */
  }

  .sidebar.is-open {
    width: 250px;
  }

  .sidebar.is-mini .nav-link {
    justify-content: center;
  }

  .sidebar.is-mini .nav-icon {
    margin-right: 0;
  }
}

/* ===== MOBILE (BOTTOM SHEET) ===== */
@media (max-width: 1023px) {
  .sidebar {
    left: 0;
    bottom: 0;
    width: 100vw;
    height: auto;
    max-height: 50vh;
    border-radius: 20px 20px 0 0;
    transform: translateY(100%);
    box-shadow: 0 -5px 15px rgba(0,0,0,0.1);
  }

  .sidebar.is-open {
    transform: translateY(0);
  }
}

/* Overlay */
.overlay {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,0.5);
  z-index: 999;
}

/* Header */
.sidebar-header {
  padding: 1.5rem 1rem;
  border-bottom: 1px solid rgba(255,255,255,0.1);
  display: flex;
  align-items: center;
}

.header-logo {
  display: flex;
  align-items: center;
  gap: 12px;
}

.logo-image {
  width: 35px;
  height: 35px;
  flex-shrink: 0;
}

.sidebar-header h2 {
    color: $white;
    font-weight: 600;
}

/* Nav */
.sidebar-nav {
  padding: 1rem 0;
  overflow-y: auto;
  flex: 1;
}

.nav-link {
  display: flex;
  align-items: center;
  padding: 0.9rem 1rem;
  margin: 0.2rem 0.5rem;
  color: rgba(255,255,255,0.8);
  text-decoration: none;
  border-radius: $border-radius-md;
  transition: 0.2s;

  &:hover {
    background: rgba(255,255,255,0.1);
    color: white;
  }

  &.active {
    background: $secondary-color;
    color: white;
    font-weight: 500;
  }
}

.nav-icon {
  margin-right: 12px;
  font-size: 1.2rem;
  width: 24px;
  text-align: center;
}

</style>