<template>
  <div class="auth-layout">
    <div class="auth-container">
      <div class="auth-card">
        <div class="auth-content">
          <router-view />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'AuthLayout'
}
</script>

<style lang="scss" scoped>
.auth-layout {
  min-height: 100vh;
  background: linear-gradient(135deg, $primary-color 0%, $primary-dark 100%);
  display: flex;
  align-items: center;
  justify-content: center;
}

.auth-container {
  width: 100%;
}

.auth-card {
  background: $white;
  border-radius: $border-radius-lg;
  box-shadow: $shadow-lg;
  overflow: hidden;
}

.auth-title {
  font-size: $font-size-3xl;
  font-weight: 700;
  color: $primary-color;
  margin-bottom: $spacing-2;
}

.auth-subtitle {
  color: $gray-600;
  font-size: $font-size-sm;
}

@media (max-width: $breakpoint-sm) {
  .auth-layout {
    padding: $spacing-2;
  }
  
  .auth-title {
    font-size: $font-size-2xl;
  }
}
</style>
