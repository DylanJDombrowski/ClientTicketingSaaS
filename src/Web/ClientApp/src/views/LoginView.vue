<template>
  <div
    class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8"
  >
    <div class="max-w-md w-full space-y-8">
      <div>
        <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
          Sign in to TicketSaaS
        </h2>
        <p class="mt-2 text-center text-sm text-gray-600">
          Or
          <router-link
            to="/register"
            class="font-medium text-blue-600 hover:text-blue-500"
          >
            create a new account
          </router-link>
        </p>
      </div>

      <form @submit.prevent="handleSubmit" class="mt-8 space-y-6">
        <div class="space-y-4">
          <div>
            <label for="email" class="block text-sm font-medium text-gray-700">
              Email address
            </label>
            <input
              id="email"
              v-model="form.email"
              type="email"
              required
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-lg focus:outline-none focus:ring-blue-500 focus:border-blue-500 focus:z-10"
              :class="{ 'border-red-300': errors.email }"
              placeholder="Enter your email"
            />
            <p v-if="errors.email" class="mt-1 text-sm text-red-600">
              {{ errors.email }}
            </p>
          </div>

          <div>
            <label
              for="password"
              class="block text-sm font-medium text-gray-700"
            >
              Password
            </label>
            <input
              id="password"
              v-model="form.password"
              type="password"
              required
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-lg focus:outline-none focus:ring-blue-500 focus:border-blue-500 focus:z-10"
              :class="{ 'border-red-300': errors.password }"
              placeholder="Enter your password"
            />
            <p v-if="errors.password" class="mt-1 text-sm text-red-600">
              {{ errors.password }}
            </p>
          </div>
        </div>

        <div
          v-if="errors.general"
          class="bg-red-50 border border-red-200 rounded-lg p-3"
        >
          <p class="text-sm text-red-600">{{ errors.general }}</p>
        </div>

        <div>
          <button
            type="submit"
            :disabled="loading"
            class="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-lg text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span
              v-if="loading"
              class="absolute left-0 inset-y-0 flex items-center pl-3"
            >
              <div
                class="animate-spin rounded-full h-5 w-5 border-b-2 border-white"
              ></div>
            </span>
            {{ loading ? "Signing in..." : "Sign in" }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useRouter, useRoute } from "vue-router";
import { useAuthStore } from "../stores/auth";
import { useNotificationStore } from "../stores/notifications";

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();
const notificationStore = useNotificationStore();

// State
const loading = ref(false);
const form = ref({
  email: "",
  password: "",
});

const errors = ref<Record<string, string>>({});

// Methods
const validateForm = (): boolean => {
  errors.value = {};

  if (!form.value.email.trim()) {
    errors.value.email = "Email is required";
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.value.email)) {
    errors.value.email = "Please enter a valid email address";
  }

  if (!form.value.password.trim()) {
    errors.value.password = "Password is required";
  } else if (form.value.password.length < 6) {
    errors.value.password = "Password must be at least 6 characters";
  }

  return Object.keys(errors.value).length === 0;
};

const handleSubmit = async () => {
  if (!validateForm()) return;

  loading.value = true;
  errors.value = {};

  try {
    const result = await authStore.login(form.value.email, form.value.password);

    if (result.success) {
      notificationStore.addNotification({
        type: "success",
        message: "Welcome back!",
      });

      // Redirect to intended page or dashboard
      const redirect = (route.query.redirect as string) || "/dashboard";
      router.push(redirect);
    } else {
      errors.value.general = result.error || "Login failed";
    }
  } catch (error) {
    console.error("Login error:", error);
    errors.value.general = "An unexpected error occurred";
  } finally {
    loading.value = false;
  }
};
</script>
