<template>
  <div class="auth-container">
    <!-- Left Side - Branding -->
    <div class="auth-branding">
      <!-- Lottie Animation Background -->
      <div ref="lottieContainer" class="lottie-background"></div>

      <div class="branding-content">
        <div class="logo-section">
          <div class="logo">
            <img src="@/assets/logo.png" alt="OutfitTrack Logo" class="logo-image" />
          </div>
          <h1 class="brand-name">ClotheSystem</h1>
        </div>

        <div class="brand-description">
          <h2 class="tagline">Gerencie seu negócio de moda com estilo</h2>
          <p class="description">
            Sistema completo para controle de estoque, vendas e clientes.
            Transforme sua loja em um negócio digital moderno e eficiente.
          </p>
        </div>

        <div class="features">
          <div class="feature">
            <i class="fas fa-chart-line"></i>
            <span>Relatórios em tempo real</span>
          </div>
          <div class="feature">
            <i class="fas fa-users"></i>
            <span>Gestão de clientes</span>
          </div>
          <div class="feature">
            <i class="fas fa-box"></i>
            <span>Controle de estoque</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Right Side - Auth Form -->
    <div class="auth-form-section">
      <div class="form-container">
        <!-- Login Form -->
        <transition name="fade" mode="out-in">
          <div v-if="!isRegisterMode" key="login" class="form-content">
            <div class="form-header">
              <h2 class="form-title">Bem-vindo de volta!</h2>
              <p class="form-subtitle">Entre com suas credenciais para acessar o sistema</p>
            </div>

            <form @submit.prevent="handleLogin" class="auth-form">
              <BaseInput
                v-model="loginForm.email"
                label="E-mail"
                type="email"
                placeholder="seu@email.com"
                :error="loginErrors.email"
                required
                icon="envelope"
              />

              <BaseInput
                v-model="loginForm.password"
                label="Senha"
                :type="showLoginPassword ? 'text' : 'password'"
                placeholder="Sua senha"
                :error="loginErrors.password"
                required
                icon="lock"
                has-icon
              >
                <template #icon>
                  <button
                    type="button"
                    class="password-toggle"
                    @click="showLoginPassword = !showLoginPassword"
                    :title="showLoginPassword ? 'Ocultar senha' : 'Mostrar senha'"
                  >
                    <i :class="showLoginPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
                  </button>
                </template>
              </BaseInput>

              <div class="form-options">
                <label class="checkbox-container">
                  <input type="checkbox" v-model="loginForm.rememberMe" />
                  <span class="checkmark"></span>
                  Lembrar de mim
                </label>

                <a href="#" class="forgot-link"> Esqueceu a senha? </a>
              </div>

              <BaseButton type="submit" variant="primary" size="lg" block :loading="loading">
                <i class="fas fa-sign-in-alt"></i>
                Entrar
              </BaseButton>
            </form>

            <div class="form-footer">
              <div class="divider">
                <span>ou</span>
              </div>

              <p class="register-prompt">
                Não tem uma conta?
                <a href="#" @click.prevent="toggleMode" class="register-link">
                  Cadastre-se gratuitamente
                </a>
              </p>
            </div>
          </div>

          <!-- Register Form -->
          <div v-else key="register" class="form-content">
            <div class="form-header">
              <h2 class="form-title">Criar sua conta</h2>
              <p class="form-subtitle">Preencha os dados para se cadastrar no sistema</p>
            </div>

            <form @submit.prevent="handleRegister" class="auth-form">
              <BaseInput
                v-model="registerForm.email"
                label="E-mail"
                type="email"
                placeholder="seu@email.com"
                :error="registerErrors.email"
                required
                icon="envelope"
              />

              <BaseInput
                v-model="registerForm.password"
                label="Senha"
                :type="showRegisterPassword ? 'text' : 'password'"
                placeholder="Sua senha"
                :error="registerErrors.password"
                required
                icon="lock"
                has-icon
              >
                <template #icon>
                  <button
                    type="button"
                    class="password-toggle"
                    @click="showRegisterPassword = !showRegisterPassword"
                    :title="showRegisterPassword ? 'Ocultar senha' : 'Mostrar senha'"
                  >
                    <i :class="showRegisterPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
                  </button>
                </template>
              </BaseInput>

              <BaseInput
                v-model="registerForm.confirmPassword"
                label="Confirmar Senha"
                :type="showConfirmPassword ? 'text' : 'password'"
                placeholder="Confirme sua senha"
                :error="registerErrors.confirmPassword"
                required
                icon="lock"
                has-icon
              >
                <template #icon>
                  <button
                    type="button"
                    class="password-toggle"
                    @click="showConfirmPassword = !showConfirmPassword"
                    :title="showConfirmPassword ? 'Ocultar senha' : 'Mostrar senha'"
                  >
                    <i :class="showConfirmPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
                  </button>
                </template>
              </BaseInput>

              <div class="password-hint">
                <small>
                  A senha deve ter pelo menos 8 caracteres, incluindo uma letra maiúscula, um
                  número e um caractere especial.
                </small>
              </div>

              <BaseButton type="submit" variant="primary" size="lg" block :loading="loading">
                <i class="fas fa-user-plus"></i>
                Criar Conta
              </BaseButton>
            </form>

            <div class="form-footer">
              <div class="divider">
                <span>ou</span>
              </div>

              <p class="register-prompt">
                Já tem uma conta?
                <a href="#" @click.prevent="toggleMode" class="register-link"> Entrar </a>
              </p>
            </div>
          </div>
        </transition>
      </div>
    </div>
  </div>
</template>

<script>
import { useAuthStore } from '@/stores/auth';
import { loginSchema, registerSchema } from '@/utils/validation';
import { mapState, mapActions } from 'pinia';
import BaseInput from '@/components/shared/BaseInput/BaseInput.vue';
import BaseButton from '@/components/shared/BaseButton/BaseButton.vue';
import lottie from 'lottie-web';
import clothesAnimation from '@/assets/lootieFiles/clothes.json';

export default {
  name: 'Login',
  components: {
    BaseInput,
    BaseButton,
  },
  data() {
    return {
      isRegisterMode: false,
      // Password visibility
      showLoginPassword: false,
      showRegisterPassword: false,
      showConfirmPassword: false,
      // Login form
      loginForm: {
        email: '',
        password: '',
        rememberMe: false,
      },
      loginErrors: {},
      // Register form
      registerForm: {
        email: '',
        password: '',
        confirmPassword: '',
      },
      registerErrors: {},
    };
  },
  computed: {
    ...mapState(useAuthStore, ['loading']),
  },
  mounted() {
    this.initLottieAnimation();
  },
  methods: {
    ...mapActions(useAuthStore, ['login', 'register']),

    initLottieAnimation() {
      if (this.$refs.lottieContainer) {
        lottie.loadAnimation({
          container: this.$refs.lottieContainer,
          renderer: 'svg',
          loop: true,
          autoplay: true,
          animationData: clothesAnimation,
        });
      }
    },

    toggleMode() {
      this.isRegisterMode = !this.isRegisterMode;
      this.loginErrors = {};
      this.registerErrors = {};
    },

    async validateLoginForm() {
      try {
        await loginSchema.validate(this.loginForm, { abortEarly: false });
        this.loginErrors = {};
        return true;
      } catch (error) {
        const newErrors = {};
        error.inner.forEach((err) => {
          newErrors[err.path] = err.message;
        });
        this.loginErrors = newErrors;
        return false;
      }
    },

    async validateRegisterForm() {
      try {
        await registerSchema.validate(this.registerForm, { abortEarly: false });
        this.registerErrors = {};
        return true;
      } catch (error) {
        const newErrors = {};
        error.inner.forEach((err) => {
          newErrors[err.path] = err.message;
        });
        this.registerErrors = newErrors;
        return false;
      }
    },

    async handleLogin() {
      const isValid = await this.validateLoginForm();
      if (!isValid) return;

      const result = await this.login(this.loginForm);
      if (result.success) {
        this.$router.push('/');
      }
    },

    async handleRegister() {
      const isValid = await this.validateRegisterForm();
      if (!isValid) return;

      const result = await this.register(this.registerForm);
      if (result.success) {
        this.isRegisterMode = false;
        this.registerForm = {
          email: '',
          password: '',
          confirmPassword: '',
        };
      }
    },
  },
};
</script>

<style lang="scss" scoped>
@import './Login.scss';
</style>
