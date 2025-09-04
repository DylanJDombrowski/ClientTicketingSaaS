<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div
      class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4"
    >
      <div>
        <h1 class="text-3xl font-bold text-gray-900">Support Tickets</h1>
        <p class="mt-2 text-sm text-gray-600">
          Manage and track customer support requests
        </p>
      </div>
      <button
        @click="openCreateModal"
        class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg flex items-center space-x-2"
      >
        <PlusIcon class="h-5 w-5" />
        <span>New Ticket</span>
      </button>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-4">
      <div class="bg-white rounded-lg shadow p-4">
        <div class="flex items-center">
          <div class="bg-blue-100 rounded-lg p-2">
            <TicketIcon class="h-5 w-5 text-blue-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm font-medium text-gray-500">Total</p>
            <p class="text-xl font-bold text-gray-900">
              {{ ticketStats.total }}
            </p>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-lg shadow p-4">
        <div class="flex items-center">
          <div class="bg-red-100 rounded-lg p-2">
            <ClockIcon class="h-5 w-5 text-red-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm font-medium text-gray-500">Open</p>
            <p class="text-xl font-bold text-gray-900">
              {{ ticketStats.open }}
            </p>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-lg shadow p-4">
        <div class="flex items-center">
          <div class="bg-yellow-100 rounded-lg p-2">
            <ArrowPathIcon class="h-5 w-5 text-yellow-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm font-medium text-gray-500">In Progress</p>
            <p class="text-xl font-bold text-gray-900">
              {{ ticketStats.inProgress }}
            </p>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-lg shadow p-4">
        <div class="flex items-center">
          <div class="bg-green-100 rounded-lg p-2">
            <CheckCircleIcon class="h-5 w-5 text-green-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm font-medium text-gray-500">Completed</p>
            <p class="text-xl font-bold text-gray-900">
              {{ ticketStats.completed }}
            </p>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-lg shadow p-4">
        <div class="flex items-center">
          <div class="bg-orange-100 rounded-lg p-2">
            <ExclamationTriangleIcon class="h-5 w-5 text-orange-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm font-medium text-gray-500">Overdue</p>
            <p class="text-xl font-bold text-gray-900">
              {{ ticketStats.overdue }}
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white rounded-lg shadow p-4">
      <div class="flex flex-col sm:flex-row gap-4">
        <!-- Search -->
        <div class="flex-1">
          <div class="relative">
            <MagnifyingGlassIcon
              class="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400"
            />
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search tickets..."
              class="pl-10 pr-4 py-2 w-full border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            />
          </div>
        </div>

        <!-- Status Filter -->
        <select
          v-model="statusFilter"
          class="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
        >
          <option value="">All Status</option>
          <option value="open">Open</option>
          <option value="in_progress">In Progress</option>
          <option value="completed">Completed</option>
        </select>

        <!-- Priority Filter -->
        <select
          v-model="priorityFilter"
          class="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
        >
          <option value="">All Priority</option>
          <option value="high">High</option>
          <option value="medium">Medium</option>
          <option value="low">Low</option>
        </select>

        <!-- Clear Filters -->
        <button
          v-if="hasActiveFilters"
          @click="clearFilters"
          class="px-4 py-2 text-gray-600 hover:text-gray-800 border border-gray-300 rounded-lg hover:bg-gray-50"
        >
          Clear
        </button>
      </div>
    </div>

    <!-- Loading State -->
    <div
      v-if="ticketsStore.loading && filteredTickets.length === 0"
      class="text-center py-12"
    >
      <div
        class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"
      ></div>
      <p class="mt-4 text-gray-600">Loading tickets...</p>
    </div>

    <!-- Error State -->
    <div
      v-else-if="ticketsStore.error"
      class="bg-red-50 border border-red-200 rounded-lg p-4"
    >
      <div class="flex">
        <ExclamationTriangleIcon class="h-5 w-5 text-red-400" />
        <div class="ml-3">
          <h3 class="text-sm font-medium text-red-800">
            Error loading tickets
          </h3>
          <p class="mt-1 text-sm text-red-700">{{ ticketsStore.error }}</p>
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
    <div v-else-if="filteredTickets.length === 0" class="text-center py-12">
      <TicketIcon class="h-12 w-12 text-gray-400 mx-auto" />
      <h3 class="mt-2 text-sm font-medium text-gray-900">
        {{
          hasActiveFilters ? "No tickets match your filters" : "No tickets yet"
        }}
      </h3>
      <p class="mt-1 text-sm text-gray-500">
        {{
          hasActiveFilters
            ? "Try adjusting your search criteria"
            : "Get started by creating your first support ticket."
        }}
      </p>
      <button
        v-if="!hasActiveFilters"
        @click="openCreateModal"
        class="mt-4 bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg"
      >
        Create Ticket
      </button>
    </div>

    <!-- Tickets List -->
    <div v-else class="bg-white rounded-lg shadow overflow-hidden">
      <div class="divide-y divide-gray-200">
        <div
          v-for="ticket in filteredTickets"
          :key="ticket.id"
          class="p-6 hover:bg-gray-50 cursor-pointer transition-colors"
          @click="viewTicket(ticket)"
        >
          <div class="flex items-center justify-between">
            <div class="flex-1 min-w-0">
              <div class="flex items-center space-x-3">
                <!-- Priority Indicator -->
                <div
                  class="w-3 h-3 rounded-full flex-shrink-0"
                  :class="{
                    'bg-red-400': ticket.priority === 'high',
                    'bg-yellow-400': ticket.priority === 'medium',
                    'bg-green-400': ticket.priority === 'low',
                  }"
                ></div>

                <!-- Ticket Info -->
                <div class="flex-1 min-w-0">
                  <div class="flex items-center space-x-2">
                    <h3 class="text-lg font-medium text-gray-900 truncate">
                      {{ ticket.title }}
                    </h3>
                    <span class="text-sm text-gray-500">#{{ ticket.id }}</span>
                  </div>
                  <div
                    class="mt-1 flex items-center text-sm text-gray-500 space-x-4"
                  >
                    <span>{{ ticket.clientName }}</span>
                    <span>•</span>
                    <span>{{ formatDate(ticket.createdAt) }}</span>
                    <span v-if="ticket.dueDate">•</span>
                    <span
                      v-if="ticket.dueDate"
                      :class="{
                        'text-red-600':
                          isOverdue(ticket.dueDate) &&
                          ticket.status !== 'completed',
                        'text-gray-500':
                          !isOverdue(ticket.dueDate) ||
                          ticket.status === 'completed',
                      }"
                    >
                      Due {{ formatDate(ticket.dueDate) }}
                    </span>
                  </div>
                  <p class="mt-2 text-sm text-gray-600 line-clamp-2">
                    {{ ticket.description }}
                  </p>
                </div>
              </div>
            </div>

            <!-- Actions -->
            <div class="flex items-center space-x-4 ml-4">
              <!-- Time Info -->
              <div
                v-if="ticket.estimatedHours || ticket.actualHours"
                class="text-right"
              >
                <div class="text-xs text-gray-500">Time</div>
                <div class="text-sm font-medium">
                  {{ ticket.actualHours || 0 }}h
                  <span v-if="ticket.estimatedHours" class="text-gray-400">
                    / {{ ticket.estimatedHours }}h
                  </span>
                </div>
              </div>

              <!-- Status Badge -->
              <span
                class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                :class="{
                  'bg-red-100 text-red-800': ticket.status === 'open',
                  'bg-yellow-100 text-yellow-800':
                    ticket.status === 'in_progress',
                  'bg-green-100 text-green-800': ticket.status === 'completed',
                }"
              >
                {{ statusLabels[ticket.status] }}
              </span>

              <!-- Quick Actions -->
              <div class="flex items-center space-x-2">
                <button
                  @click.stop="editTicket(ticket)"
                  class="text-gray-400 hover:text-gray-600"
                  title="Edit ticket"
                >
                  <PencilIcon class="h-4 w-4" />
                </button>
                <button
                  @click.stop="confirmDelete(ticket)"
                  class="text-gray-400 hover:text-red-600"
                  title="Delete ticket"
                >
                  <TrashIcon class="h-4 w-4" />
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create/Edit Ticket Modal -->
    <TicketModal
      v-if="showModal"
      :ticket="selectedTicket"
      @close="closeModal"
      @saved="onTicketSaved"
    />

    <!-- Delete Confirmation Modal -->
    <ConfirmModal
      v-if="showDeleteModal"
      title="Delete Ticket"
      :message="`Are you sure you want to delete ticket '${ticketToDelete?.title}'? This action cannot be undone.`"
      confirm-text="Delete"
      confirm-style="danger"
      @confirm="deleteTicket"
      @cancel="showDeleteModal = false"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from "vue";
import { useRouter } from "vue-router";
import { useTicketsStore } from "../stores/tickets";
import { useClientsStore } from "../stores/clients";
import { useNotificationStore } from "../stores/notifications";
import type { Ticket, TicketStatus, TicketPriority } from "../types/ticket";
import {
  PlusIcon,
  TicketIcon,
  ClockIcon,
  ArrowPathIcon,
  CheckCircleIcon,
  ExclamationTriangleIcon,
  MagnifyingGlassIcon,
  PencilIcon,
  TrashIcon,
} from "@heroicons/vue/24/outline";
import TicketModal from "../components/TicketModal.vue";
import ConfirmModal from "../components/ConfirmModal.vue";

const router = useRouter();
const ticketsStore = useTicketsStore();
const clientsStore = useClientsStore();
const notificationStore = useNotificationStore();

// State
const showModal = ref(false);
const showDeleteModal = ref(false);
const selectedTicket = ref<Ticket | null>(null);
const ticketToDelete = ref<Ticket | null>(null);
const searchQuery = ref("");
const statusFilter = ref<TicketStatus | "">("");
const priorityFilter = ref<TicketPriority | "">("");

// Computed
const filteredTickets = computed(() => {
  let tickets = ticketsStore.tickets;

  if (searchQuery.value) {
    const search = searchQuery.value.toLowerCase();
    tickets = tickets.filter(
      (t) =>
        t.title.toLowerCase().includes(search) ||
        t.description.toLowerCase().includes(search) ||
        t.clientName.toLowerCase().includes(search)
    );
  }

  if (statusFilter.value) {
    tickets = tickets.filter((t) => t.status === statusFilter.value);
  }

  if (priorityFilter.value) {
    tickets = tickets.filter((t) => t.priority === priorityFilter.value);
  }

  return tickets.sort(
    (a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
  );
});

const ticketStats = computed(() => ticketsStore.ticketStats);

const hasActiveFilters = computed(() => {
  return !!(searchQuery.value || statusFilter.value || priorityFilter.value);
});

const statusLabels = {
  open: "Open",
  in_progress: "In Progress",
  completed: "Completed",
};

// Methods
const openCreateModal = () => {
  selectedTicket.value = null;
  showModal.value = true;
};

const editTicket = (ticket: Ticket) => {
  selectedTicket.value = ticket;
  showModal.value = true;
};

const viewTicket = (ticket: Ticket) => {
  router.push(`/tickets/${ticket.id}`);
};

const closeModal = () => {
  showModal.value = false;
  selectedTicket.value = null;
};

const onTicketSaved = () => {
  closeModal();
  notificationStore.addNotification({
    type: "success",
    message: selectedTicket.value
      ? "Ticket updated successfully!"
      : "Ticket created successfully!",
  });
};

const confirmDelete = (ticket: Ticket) => {
  ticketToDelete.value = ticket;
  showDeleteModal.value = true;
};

const deleteTicket = async () => {
  if (!ticketToDelete.value) return;

  const result = await ticketsStore.deleteTicket(ticketToDelete.value.id);

  if (result.success) {
    notificationStore.addNotification({
      type: "success",
      message: "Ticket deleted successfully!",
    });
  } else {
    notificationStore.addNotification({
      type: "error",
      message: result.error || "Failed to delete ticket",
    });
  }

  showDeleteModal.value = false;
  ticketToDelete.value = null;
};

const retry = () => {
  ticketsStore.fetchTickets();
};

const clearFilters = () => {
  searchQuery.value = "";
  statusFilter.value = "";
  priorityFilter.value = "";
};

const formatDate = (dateString: string) => {
  const date = new Date(dateString);
  return date.toLocaleDateString("en-US", {
    month: "short",
    day: "numeric",
    year:
      date.getFullYear() !== new Date().getFullYear() ? "numeric" : undefined,
  });
};

const isOverdue = (dueDate: string) => {
  return new Date(dueDate) < new Date();
};

// Watch for filter changes and apply them to store
watch([searchQuery, statusFilter, priorityFilter], () => {
  ticketsStore.setFilters({
    search: searchQuery.value || undefined,
    status: statusFilter.value || undefined,
    priority: priorityFilter.value || undefined,
  });
});

// Lifecycle
onMounted(async () => {
  // Load tickets and clients
  await Promise.all([ticketsStore.fetchTickets(), clientsStore.fetchClients()]);
});
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  line-clamp: 2;
}
</style>
