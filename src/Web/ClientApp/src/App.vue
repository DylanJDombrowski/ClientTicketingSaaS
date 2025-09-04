<template>
  <div id="app" class="min-h-screen bg-gray-50">
    <!-- Navigation Header -->
    <nav class="bg-white shadow-sm border-b border-gray-200">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16">
          <div class="flex items-center">
            <router-link to="/" class="flex items-center">
              <div class="flex-shrink-0">
                <h1 class="text-xl font-bold text-gray-900">TicketSaaS</h1>
              </div>
            </router-link>

            <!-- Main Navigation -->
            <div class="hidden md:ml-10 md:flex md:space-x-8">
              <router-link
                to="/dashboard"
                class="text-gray-500 hover:text-gray-700 px-3 py-2 rounded-md text-sm font-medium"
                active-class="text-blue-600 border-b-2 border-blue-600"
              >
                Dashboard
              </router-link>
              <router-link
                to="/clients"
                class="text-gray-500 hover:text-gray-700 px-3 py-2 rounded-md text-sm font-medium"
                active-class="text-blue-600 border-b-2 border-blue-600"
              >
                Clients
              </router-link>
              <router-link
                to="/tickets"
                class="text-gray-500 hover:text-gray-700 px-3 py-2 rounded-md text-sm font-medium"
                active-class="text-blue-600 border-b-2 border-blue-600"
              >
                Tickets
              </router-link>
            </div>
          </div>

          <!-- User Menu -->
          <div class="flex items-center space-x-4">
            <div
              v-if="authStore.isAuthenticated"
              class="flex items-center space-x-4"
            >
              <span class="text-sm text-gray-700">{{
                authStore.currentUser?.email
              }}</span>
              <button
                @click="logout"
                class="bg-gray-100 hover:bg-gray-200 px-3 py-2 rounded-md text-sm font-medium text-gray-700"
              >
                Logout
              </button>
            </div>
            <div v-else>
              <router-link
                to="/login"
                class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium"
              >
                Login
              </router-link>
            </div>
          </div>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <main class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
      <router-view />
    </main>

    <!-- Toast Notifications -->
    <div
      v-if="notificationStore.notifications.length > 0"
      class="fixed top-4 right-4 space-y-2 z-50"
    >
      <div
        v-for="notification in notificationStore.notifications"
        :key="notification.id"
        class="bg-white rounded-lg shadow-lg p-4 border-l-4"
        :class="{
          'border-green-400': notification.type === 'success',
          'border-red-400': notification.type === 'error',
          'border-yellow-400': notification.type === 'warning',
          'border-blue-400': notification.type === 'info',
        }"
      >
        <div class="flex">
          <div class="flex-shrink-0">
            <CheckCircleIcon
              v-if="notification.type === 'success'"
              class="h-5 w-5 text-green-400"
            />
            <XCircleIcon
              v-else-if="notification.type === 'error'"
              class="h-5 w-5 text-red-400"
            />
            <ExclamationTriangleIcon
              v-else-if="notification.type === 'warning'"
              class="h-5 w-5 text-yellow-400"
            />
            <InformationCircleIcon v-else class="h-5 w-5 text-blue-400" />
          </div>
          <div class="ml-3">
            <p class="text-sm font-medium text-gray-900">
              {{ notification.message }}
            </p>
          </div>
          <div class="ml-auto pl-3">
            <button
              @click="notificationStore.removeNotification(notification.id)"
              class="text-gray-400 hover:text-gray-600"
            >
              <XMarkIcon class="h-5 w-5" />
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { RouterView, RouterLink, useRouter } from "vue-router";
import { useAuthStore } from "./stores/auth";
import { useNotificationStore } from "./stores/notifications";
import {
  CheckCircleIcon,
  XCircleIcon,
  ExclamationTriangleIcon,
  InformationCircleIcon,
  XMarkIcon,
} from "@heroicons/vue/24/outline";

const authStore = useAuthStore();
const notificationStore = useNotificationStore();
const router = useRouter();

const logout = async () => {
  await authStore.logout();
  router.push("/login");
};
</script>

<style>
#app {
  font-family: "Inter", -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto,
    sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}
</style>
