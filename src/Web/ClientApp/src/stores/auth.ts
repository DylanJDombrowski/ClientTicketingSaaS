import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import type { User } from '../types/auth';
import { authApi } from '../services/api/auth';

export const useAuthStore = defineStore('auth', () => {
  // State
  const currentUser = ref<User | null>(null);
  const token = ref<string | null>(localStorage.getItem('auth_token'));
  const loading = ref(false);
  const error = ref<string | null>(null);

  // Getters
  const isAuthenticated = computed(() => !!currentUser.value && !!token.value);
  const userRole = computed(() => currentUser.value?.role);
  const tenantId = computed(() => currentUser.value?.tenantId);

  // Actions
  const login = async (email: string, password: string) => {
    loading.value = true;
    error.value = null;

    try {
      const response = await authApi.login({ email, password });

      // Store token
      token.value = response.token;
      localStorage.setItem('auth_token', response.token);

      // Get user profile
      await fetchCurrentUser();

      return { success: true };
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Login failed';
      return { success: false, error: error.value };
    } finally {
      loading.value = false;
    }
  };

  const register = async (userData: {
    email: string;
    password: string;
    confirmPassword: string;
    tenantName: string;
  }) => {
    loading.value = true;
    error.value = null;

    try {
      const response = await authApi.register(userData);

      // Store token
      token.value = response.token;
      localStorage.setItem('auth_token', response.token);

      // Get user profile
      await fetchCurrentUser();

      return { success: true };
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Registration failed';
      return { success: false, error: error.value };
    } finally {
      loading.value = false;
    }
  };

  const logout = async () => {
    try {
      await authApi.logout();
    } catch (err) {
      // Continue with logout even if API call fails
      console.warn('Logout API call failed:', err);
    } finally {
      // Clear local state
      currentUser.value = null;
      token.value = null;
      localStorage.removeItem('auth_token');
    }
  };

  const fetchCurrentUser = async () => {
    if (!token.value) return;

    try {
      const user = await authApi.getCurrentUser();
      currentUser.value = user;
    } catch (err) {
      // Token might be invalid, clear it
      console.error('Failed to fetch current user:', err);
      await logout();
    }
  };

  const refreshToken = async () => {
    if (!token.value) return false;

    try {
      const response = await authApi.refreshToken();
      token.value = response.token;
      localStorage.setItem('auth_token', response.token);
      return true;
    } catch (err) {
      await logout();
      return false;
    }
  };

  // Initialize auth state on store creation
  const initialize = async () => {
    if (token.value) {
      await fetchCurrentUser();
    }
  };

  return {
    // State
    currentUser,
    token,
    loading,
    error,

    // Getters
    isAuthenticated,
    userRole,
    tenantId,

    // Actions
    login,
    register,
    logout,
    fetchCurrentUser,
    refreshToken,
    initialize,
  };
});
