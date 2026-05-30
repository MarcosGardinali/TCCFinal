<template>
  <div class="main-layout">
    <Sidebar :is-open="sidebarOpen" @toggle="toggleSidebar" />
    
    <div class="main-content" :class="{ 'sidebar-open': sidebarOpen }">
      <Header @toggle-sidebar="toggleSidebar" />
      
      <main class="content">
        <router-view />
      </main>
    </div>
  </div>
</template>

<script>
import Sidebar from './Sidebar.vue'
import Header from './Header.vue'

export default {
  name: 'MainLayout',
  components: {
    Sidebar,
    Header
  },
  data() {
    const isDesktop = window.innerWidth >= 1024;
    return {
      sidebarOpen: isDesktop,
      isDesktop: isDesktop,
    }
  },
  mounted() {
    window.addEventListener('resize', this.handleResize)
  },
  beforeUnmount() {
    window.removeEventListener('resize', this.handleResize)
  },
  methods: {
    toggleSidebar() {
      this.sidebarOpen = !this.sidebarOpen
    },
    handleResize() {
      const currentlyDesktop = window.innerWidth >= 1024;
      if (this.isDesktop !== currentlyDesktop) {
        this.isDesktop = currentlyDesktop;
        this.sidebarOpen = currentlyDesktop;
      }
    }
  }
}
</script>

<style lang="scss" scoped>
.main-layout {
  display: flex;
  min-height: 100vh;
  background-color: $gray-50;
}

.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  transition: margin-left 0.3s ease;
  margin-left: 0;
  width: 100%;
}

.content {
  flex: 1;
  overflow-y: auto;
}

@media (min-width: $breakpoint-lg) {
  .main-content {
    margin-left: 70px; // Margem para sidebar mini

    &.sidebar-open {
      margin-left: 250px; // Margem para sidebar aberta
    }
  }
}

@media (max-width: $breakpoint-lg) {
  .main-content {
    margin-left: 0 !important;
  }

  .content {
    padding: $spacing-2;
  }
}
</style>
