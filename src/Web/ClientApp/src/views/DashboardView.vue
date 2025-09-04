<template>
  <div class="space-y-6">
    <!-- Welcome Header -->
    <div class="bg-white rounded-lg shadow p-6">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900">
            Welcome back, {{ authStore.currentUser?.email?.split("@")[0] }}!
          </h1>
          <p class="mt-1 text-sm text-gray-600">
            Here's what's happening with your business today.
          </p>
        </div>
        <div class="flex space-x-3">
          <router-link
            to="/clients"
            class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg flex items-center space-x-2"
          >
            <PlusIcon class="h-4 w-4" />
            <span>New Client</span>
          </router-link>
        </div>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <!-- Total Clients -->
      <div class="bg-white rounded-lg shadow p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="bg-blue-100 rounded-lg p-3">
              <UserGroupIcon class="h-6 w-6 text-blue-600" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500">Total Clients</p>
            <p class="text-2xl font-bold text-gray-900">
              {{ stats.totalClients }}
            </p>
          </div>
        </div>
      </div>

      <!-- Open Tickets -->
      <div class="bg-white rounded-lg shadow p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="bg-yellow-100 rounded-lg p-3">
              <TicketIcon class="h-6 w-6 text-yellow-600" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500">Open Tickets</p>
            <p class="text-2xl font-bold text-gray-900">
              {{ stats.openTickets }}
            </p>
          </div>
        </div>
      </div>

      <!-- Completed This Week -->
      <div class="bg-white rounded-lg shadow p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="bg-green-100 rounded-lg p-3">
              <CheckCircleIcon class="h-6 w-6 text-green-600" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500">Completed This Week</p>
            <p class="text-2xl font-bold text-gray-900">
              {{ stats.completedThisWeek }}
            </p>
          </div>
        </div>
      </div>

      <!-- Revenue This Month -->
      <div class="bg-white rounded-lg shadow p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="bg-purple-100 rounded-lg p-3">
              <CurrencyDollarIcon class="h-6 w-6 text-purple-600" />
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500">Revenue This Month</p>
            <p class="text-2xl font-bold text-gray-900">
              ${{ stats.revenueThisMonth.toLocaleString() }}
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Recent Activity -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Recent Tickets -->
      <div class="bg-white rounded-lg shadow">
        <div class="p-6 border-b border-gray-200">
          <div class="flex items-center justify-between">
            <h2 class="text-lg font-medium text-gray-900">Recent Tickets</h2>
            <router-link
              to="/tickets"
              class="text-sm text-blue-600 hover:text-blue-800 font-medium"
            >
              View all →
            </router-link>
          </div>
        </div>
        <div class="p-6">
          <div v-if="recentTickets.length === 0" class="text-center py-8">
            <TicketIcon class="h-12 w-12 text-gray-400 mx-auto" />
            <p class="mt-2 text-sm text-gray-500">No tickets yet</p>
            <router-link
              to="/tickets"
              class="mt-2 text-sm text-blue-600 hover:text-blue-800"
            >
              Create your first ticket
            </router-link>
          </div>
          <div v-else class="space-y-4">
            <div
              v-for="ticket in recentTickets"
              :key="ticket.id"
              class="flex items-center justify-between p-3 bg-gray-50 rounded-lg"
            >
              <div class="flex items-center space-x-3">
                <div
                  class="w-3 h-3 rounded-full"
                  :class="{
                    'bg-red-400': ticket.priority === 'high',
                    'bg-yellow-400': ticket.priority === 'medium',
                    'bg-green-400': ticket.priority === 'low',
                  }"
                ></div>
                <div>
                  <p class="text-sm font-medium text-gray-900">
                    {{ ticket.title }}
                  </p>
                  <p class="text-xs text-gray-500">{{ ticket.clientName }}</p>
                </div>
              </div>
              <span
                class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                :class="{
                  'bg-yellow-100 text-yellow-800': ticket.status === 'open',
                  'bg-blue-100 text-blue-800': ticket.status === 'in_progress',
                  'bg-green-100 text-green-800': ticket.status === 'completed',
                }"
              >
                {{ ticket.status.replace("_", " ") }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Clients -->
      <div class="bg-white rounded-lg shadow">
        <div class="p-6 border-b border-gray-200">
          <div class="flex items-center justify-between">
            <h2 class="text-lg font-medium text-gray-900">Recent Clients</h2>
            <router-link
              to="/clients"
              class="text-sm text-blue-600 hover:text-blue-800 font-medium"
            >
              View all →
            </router-link>
          </div>
        </div>
        <div class="p-6">
          <div v-if="recentClients.length === 0" class="text-center py-8">
            <UserGroupIcon class="h-12 w-12 text-gray-400 mx-auto" />
            <p class="mt-2 text-sm text-gray-500">No clients yet</p>
            <router-link
              to="/clients"
              class="mt-2 text-sm text-blue-600 hover:text-blue-800"
            >
              Add your first client
            </router-link>
          </div>
          <div v-else class="space-y-4">
            <div
              v-for="client in recentClients"
              :key="client.id"
              class="flex items-center justify-between p-3 bg-gray-50 rounded-lg"
            >
              <div class="flex items-center space-x-3">
                <div class="bg-blue-100 rounded-full p-2">
                  <UserIcon class="h-4 w-4 text-blue-600" />
                </div>
                <div>
                  <p class="text-sm font-medium text-gray-900">
                    {{ client.name }}
                  </p>
                  <p class="text-xs text-gray-500">{{ client.email }}</p>
                </div>
              </div>
              <router-link
                :to="`/clients/${client.id}`"
                class="text-blue-600 hover:text-blue-800 text-sm font-medium"
              >
                View →
              </router-link>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="bg-white rounded-lg shadow p-6">
      <h2 class="text-lg font-medium text-gray-900 mb-4">Quick Actions</h2>
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <router-link
          to="/clients"
          class="flex items-center p-4 bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors"
        >
          <PlusIcon class="h-8 w-8 text-blue-600" />
          <div class="ml-3">
            <p class="text-sm font-medium text-gray-900">Add Client</p>
            <p class="text-xs text-gray-500">Create new client profile</p>
          </div>
        </router-link>

        <router-link
          to="/tickets"
          class="flex items-center p-4 bg-green-50 rounded-lg hover:bg-green-100 transition-colors"
        >
          <TicketIcon class="h-8 w-8 text-green-600" />
          <div class="ml-3">
            <p class="text-sm font-medium text-gray-900">New Ticket</p>
            <p class="text-xs text-gray-500">Create support ticket</p>
          </div>
        </router-link>

        <router-link
          to="/reports"
          class="flex items-center p-4 bg-purple-50 rounded-lg hover:bg-purple-100 transition-colors"
        >
          <ChartBarIcon class="h-8 w-8 text-purple-600" />
          <div class="ml-3">
            <p class="text-sm font-medium text-gray-900">View Reports</p>
            <p class="text-xs text-gray-500">Analytics & insights</p>
          </div>
        </router-link>

        <router-link
          to="/profile"
          class="flex items-center p-4 bg-yellow-50 rounded-lg hover:bg-yellow-100 transition-colors"
        >
          <CogIcon class="h-8 w-8 text-yellow-600" />
          <div class="ml-3">
            <p class="text-sm font-medium text-gray-900">Settings</p>
            <p class="text-xs text-gray-500">Account preferences</p>
          </div>
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useAuthStore } from "../stores/auth";
import {
  PlusIcon,
  UserGroupIcon,
  TicketIcon,
  CheckCircleIcon,
  CurrencyDollarIcon,
  UserIcon,
  ChartBarIcon,
  CogIcon,
} from "@heroicons/vue/24/outline";

const authStore = useAuthStore();

// State
const stats = ref({
  totalClients: 0,
  openTickets: 0,
  completedThisWeek: 0,
  revenueThisMonth: 0,
});

const recentTickets = ref([]);
const recentClients = ref([]);
const loading = ref(false);

// Methods
const loadDashboardData = async () => {
  loading.value = true;

  try {
    // TODO: Implement API calls when backend endpoints are ready
    // For now, using mock data

    stats.value = {
      totalClients: 24,
      openTickets: 8,
      completedThisWeek: 12,
      revenueThisMonth: 15750,
    };

    recentTickets.value = [
      {
        id: 1,
        title: "Website login issue",
        clientName: "Acme Corp",
        status: "open",
        priority: "high",
      },
      {
        id: 2,
        title: "Email configuration",
        clientName: "Tech Solutions",
        status: "in_progress",
        priority: "medium",
      },
      {
        id: 3,
        title: "Database backup",
        clientName: "StartUp Inc",
        status: "completed",
        priority: "low",
      },
    ];

    recentClients.value = [
      {
        id: 1,
        name: "John Smith",
        email: "john@acme.com",
      },
      {
        id: 2,
        name: "Sarah Johnson",
        email: "sarah@techsolutions.com",
      },
      {
        id: 3,
        name: "Mike Wilson",
        email: "mike@startup.io",
      },
    ];
  } catch (error) {
    console.error("Error loading dashboard data:", error);
  } finally {
    loading.value = false;
  }
};

// Lifecycle
onMounted(() => {
  loadDashboardData();
});
</script>
