<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div class="flex justify-between items-center">
      <div>
        <h1 class="text-3xl font-bold text-gray-900">Clients</h1>
        <p class="mt-2 text-sm text-gray-600">
          Manage your client contacts and information
        </p>
      </div>
      <button
        @click="openCreateModal"
        class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg flex items-center space-x-2"
      >
        <PlusIcon class="h-5 w-5" />
        <span>New Client</span>
      </button>
    </div>

    <!-- Loading State -->
    <div
      v-if="clientsStore.loading && clients.length === 0"
      class="text-center py-12"
    >
      <div
        class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"
      ></div>
      <p class="mt-4 text-gray-600">Loading clients...</p>
    </div>

    <!-- Error State -->
    <div
      v-else-if="clientsStore.error"
      class="bg-red-50 border border-red-200 rounded-lg p-4"
    >
      <div class="flex">
        <ExclamationTriangleIcon class="h-5 w-5 text-red-400" />
        <div class="ml-3">
          <h3 class="text-sm font-medium text-red-800">
            Error loading clients
          </h3>
          <p class="mt-1 text-sm text-red-700">{{ clientsStore.error }}</p>
          <button
            @click="retry"
            class="mt-2 text-sm text-red-600 hover:text-red-500 underline"
          >
            Try again
          </button>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-else-if="clients.length === 0" class="text-center py-12">
      <UserGroupIcon class="h-12 w-12 text-gray-400 mx-auto" />
      <h3 class="mt-2 text-sm font-medium text-gray-900">No clients yet</h3>
      <p class="mt-1 text-sm text-gray-500">
        Get started by creating your first client.
      </p>
      <button
        @click="openCreateModal"
        class="mt-4 bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg"
      >
        Add Client
      </button>
    </div>

    <!-- Clients Grid -->
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div
        v-for="client in clients"
        :key="client.id"
        class="bg-white rounded-lg shadow border border-gray-200 hover:shadow-md transition-shadow"
      >
        <div class="p-6">
          <div class="flex items-center justify-between mb-4">
            <div class="flex items-center space-x-3">
              <div class="bg-blue-100 rounded-full p-2">
                <UserIcon class="h-6 w-6 text-blue-600" />
              </div>
              <div>
                <h3 class="text-lg font-medium text-gray-900">
                  {{ client.name }}
                </h3>
                <p class="text-sm text-gray-500">
                  {{ client.company || "Individual" }}
                </p>
              </div>
            </div>
            <div class="flex space-x-2">
              <button
                @click="editClient(client)"
                class="text-gray-400 hover:text-gray-600"
                title="Edit client"
              >
                <PencilIcon class="h-4 w-4" />
              </button>
              <button
                @click="confirmDelete(client)"
                class="text-gray-400 hover:text-red-600"
                title="Delete client"
              >
                <TrashIcon class="h-4 w-4" />
              </button>
            </div>
          </div>

          <div class="space-y-2">
            <div class="flex items-center text-sm text-gray-600">
              <EnvelopeIcon class="h-4 w-4 mr-2" />
              <a :href="`mailto:${client.email}`" class="hover:text-blue-600">
                {{ client.email }}
              </a>
            </div>
            <div
              v-if="client.phone"
              class="flex items-center text-sm text-gray-600"
            >
              <PhoneIcon class="h-4 w-4 mr-2" />
              <a :href="`tel:${client.phone}`" class="hover:text-blue-600">
                {{ client.phone }}
              </a>
            </div>
          </div>

          <div class="mt-4 pt-4 border-t border-gray-200">
            <div class="flex items-center justify-between">
              <span
                class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                :class="
                  client.isActive
                    ? 'bg-green-100 text-green-800'
                    : 'bg-gray-100 text-gray-800'
                "
              >
                {{ client.isActive ? "Active" : "Inactive" }}
              </span>
              <router-link
                :to="`/clients/${client.id}`"
                class="text-blue-600 hover:text-blue-800 text-sm font-medium"
              >
                View Details →
              </router-link>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create/Edit Client Modal -->
    <ClientModal
      v-if="showModal"
      :client="selectedClient"
      @close="closeModal"
      @saved="onClientSaved"
    />

    <!-- Delete Confirmation Modal -->
    <ConfirmModal
      v-if="showDeleteModal"
      title="Delete Client"
      :message="`Are you sure you want to delete ${clientToDelete?.name}? This action cannot be undone.`"
      confirm-text="Delete"
      confirm-style="danger"
      @confirm="deleteClient"
      @cancel="showDeleteModal = false"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { useClientsStore } from "../stores/clients";
import { useNotificationStore } from "../stores/notifications";
import type { Client } from "../types/client";
import {
  PlusIcon,
  UserIcon,
  UserGroupIcon,
  PencilIcon,
  TrashIcon,
  EnvelopeIcon,
  PhoneIcon,
  ExclamationTriangleIcon,
} from "@heroicons/vue/24/outline";
import ClientModal from "../components/ClientModal.vue";
import ConfirmModal from "../components/ConfirmModal.vue";

const clientsStore = useClientsStore();
const notificationStore = useNotificationStore();

// State
const showModal = ref(false);
const showDeleteModal = ref(false);
const selectedClient = ref<Client | null>(null);
const clientToDelete = ref<Client | null>(null);

// Computed
const clients = computed(() => clientsStore.clients);

// Methods
const openCreateModal = () => {
  selectedClient.value = null;
  showModal.value = true;
};

const editClient = (client: Client) => {
  selectedClient.value = client;
  showModal.value = true;
};

const closeModal = () => {
  showModal.value = false;
  selectedClient.value = null;
};

const onClientSaved = () => {
  closeModal();
  notificationStore.addNotification({
    type: "success",
    message: selectedClient.value
      ? "Client updated successfully!"
      : "Client created successfully!",
  });
};

const confirmDelete = (client: Client) => {
  clientToDelete.value = client;
  showDeleteModal.value = true;
};

const deleteClient = async () => {
  if (!clientToDelete.value) return;

  const result = await clientsStore.deleteClient(clientToDelete.value.id);

  if (result.success) {
    notificationStore.addNotification({
      type: "success",
      message: "Client deleted successfully!",
    });
  } else {
    notificationStore.addNotification({
      type: "error",
      message: result.error || "Failed to delete client",
    });
  }

  showDeleteModal.value = false;
  clientToDelete.value = null;
};

const retry = () => {
  clientsStore.fetchClients();
};

// Lifecycle
onMounted(() => {
  clientsStore.fetchClients();
});
</script>
