<template>
  <div class="space-y-6">
    <!-- Loading State -->
    <div v-if="loading && !ticket" class="text-center py-12">
      <div
        class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"
      ></div>
      <p class="mt-4 text-gray-600">Loading ticket...</p>
    </div>

    <!-- Error State -->
    <div
      v-else-if="error"
      class="bg-red-50 border border-red-200 rounded-lg p-4"
    >
      <div class="flex">
        <ExclamationTriangleIcon class="h-5 w-5 text-red-400" />
        <div class="ml-3">
          <h3 class="text-sm font-medium text-red-800">Error loading ticket</h3>
          <p class="mt-1 text-sm text-red-700">{{ error }}</p>
          <button
            @click="loadTicket"
            class="mt-2 text-sm text-red-600 hover:text-red-500 underline"
          >
            Try again
          </button>
        </div>
      </div>
    </div>

    <!-- Ticket Content -->
    <div v-else-if="ticket">
      <!-- Header -->
      <div class="flex items-center justify-between">
        <div class="flex items-center space-x-4">
          <button @click="goBack" class="text-gray-400 hover:text-gray-600">
            <ArrowLeftIcon class="h-6 w-6" />
          </button>
          <div>
            <h1 class="text-2xl font-bold text-gray-900">{{ ticket.title }}</h1>
            <div class="flex items-center space-x-4 mt-1 text-sm text-gray-500">
              <span>Ticket #{{ ticket.id }}</span>
              <span>•</span>
              <span>{{ ticket.clientName }}</span>
              <span>•</span>
              <span>Created {{ formatDate(ticket.createdAt) }}</span>
            </div>
          </div>
        </div>

        <div class="flex items-center space-x-3">
          <button
            @click="editTicket"
            class="bg-white border border-gray-300 text-gray-700 px-4 py-2 rounded-lg hover:bg-gray-50 flex items-center space-x-2"
          >
            <PencilIcon class="h-4 w-4" />
            <span>Edit</span>
          </button>
          <button
            @click="addTimeEntry"
            class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg flex items-center space-x-2"
          >
            <ClockIcon class="h-4 w-4" />
            <span>Log Time</span>
          </button>
        </div>
      </div>

      <!-- Status Bar -->
      <div class="bg-white rounded-lg shadow p-6">
        <div class="flex items-center justify-between">
          <div class="flex items-center space-x-6">
            <!-- Status -->
            <div class="flex items-center space-x-2">
              <span class="text-sm font-medium text-gray-500">Status:</span>
              <select
                v-model="ticket.status"
                @change="updateStatus"
                class="border-0 bg-transparent text-sm font-medium focus:ring-2 focus:ring-blue-500 rounded"
                :class="{
                  'text-red-800': ticket.status === 'open',
                  'text-yellow-800': ticket.status === 'in_progress',
                  'text-green-800': ticket.status === 'completed',
                }"
              >
                <option value="open">Open</option>
                <option value="in_progress">In Progress</option>
                <option value="completed">Completed</option>
              </select>
            </div>

            <!-- Priority -->
            <div class="flex items-center space-x-2">
              <span class="text-sm font-medium text-gray-500">Priority:</span>
              <span
                class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium"
                :class="{
                  'bg-red-100 text-red-800': ticket.priority === 'high',
                  'bg-yellow-100 text-yellow-800': ticket.priority === 'medium',
                  'bg-green-100 text-green-800': ticket.priority === 'low',
                }"
              >
                {{ ticket.priority }} Priority
              </span>
            </div>

            <!-- Due Date -->
            <div v-if="ticket.dueDate" class="flex items-center space-x-2">
              <span class="text-sm font-medium text-gray-500">Due:</span>
              <span
                :class="{
                  'text-red-600':
                    isOverdue(ticket.dueDate) && ticket.status !== 'completed',
                  'text-gray-700':
                    !isOverdue(ticket.dueDate) || ticket.status === 'completed',
                }"
                class="text-sm"
              >
                {{ formatDate(ticket.dueDate) }}
              </span>
            </div>
          </div>

          <!-- Time Tracking -->
          <div class="flex items-center space-x-4 text-sm">
            <div v-if="ticket.estimatedHours" class="text-gray-600">
              Estimated: {{ ticket.estimatedHours }}h
            </div>
            <div class="font-medium">
              Logged: {{ ticket.actualHours || 0 }}h
            </div>
            <div
              v-if="ticket.estimatedHours && ticket.actualHours"
              class="text-gray-500"
            >
              ({{
                Math.round((ticket.actualHours / ticket.estimatedHours) * 100)
              }}%)
            </div>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Main Content -->
        <div class="lg:col-span-2 space-y-6">
          <!-- Description -->
          <div class="bg-white rounded-lg shadow p-6">
            <h2 class="text-lg font-medium text-gray-900 mb-4">Description</h2>
            <div class="prose prose-sm max-w-none">
              <p class="whitespace-pre-wrap text-gray-700">
                {{ ticket.description }}
              </p>
            </div>
          </div>

          <!-- Comments -->
          <div class="bg-white rounded-lg shadow">
            <div class="px-6 py-4 border-b border-gray-200">
              <h2 class="text-lg font-medium text-gray-900">
                Comments & Updates
              </h2>
            </div>

            <!-- Add Comment Form -->
            <div class="p-6 border-b border-gray-200">
              <form @submit.prevent="submitComment">
                <div class="mb-4">
                  <textarea
                    v-model="newComment"
                    rows="3"
                    placeholder="Add a comment..."
                    class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                    required
                  ></textarea>
                </div>
                <div class="flex items-center justify-between">
                  <div class="flex items-center">
                    <input
                      id="isInternal"
                      v-model="commentIsInternal"
                      type="checkbox"
                      class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                    />
                    <label for="isInternal" class="ml-2 text-sm text-gray-700">
                      Internal comment (not visible to client)
                    </label>
                  </div>
                  <button
                    type="submit"
                    :disabled="!newComment.trim() || addingComment"
                    class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg disabled:opacity-50 disabled:cursor-not-allowed flex items-center space-x-2"
                  >
                    <div
                      v-if="addingComment"
                      class="animate-spin rounded-full h-4 w-4 border-b-2 border-white"
                    ></div>
                    <span>Add Comment</span>
                  </button>
                </div>
              </form>
            </div>

            <!-- Comments List -->
            <div class="divide-y divide-gray-200">
              <div
                v-for="comment in ticket.comments"
                :key="comment.id"
                class="p-6"
              >
                <div class="flex items-start space-x-3">
                  <div class="bg-gray-200 rounded-full p-2">
                    <UserIcon class="h-5 w-5 text-gray-600" />
                  </div>
                  <div class="flex-1 min-w-0">
                    <div class="flex items-center justify-between">
                      <div class="flex items-center space-x-2">
                        <p class="text-sm font-medium text-gray-900">
                          {{ comment.authorName }}
                        </p>
                        <span
                          v-if="comment.isInternal"
                          class="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-yellow-100 text-yellow-800"
                        >
                          Internal
                        </span>
                      </div>
                      <p class="text-sm text-gray-500">
                        {{ formatDate(comment.createdAt) }}
                      </p>
                    </div>
                    <div class="mt-2">
                      <p class="text-sm text-gray-700 whitespace-pre-wrap">
                        {{ comment.content }}
                      </p>
                    </div>
                  </div>
                </div>
              </div>

              <!-- No Comments State -->
              <div
                v-if="!ticket.comments || ticket.comments.length === 0"
                class="p-6 text-center text-gray-500"
              >
                <ChatBubbleLeftIcon
                  class="h-8 w-8 mx-auto mb-2 text-gray-400"
                />
                <p>No comments yet. Add the first comment above.</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Sidebar -->
        <div class="space-y-6">
          <!-- Ticket Info -->
          <div class="bg-white rounded-lg shadow p-6">
            <h3 class="text-lg font-medium text-gray-900 mb-4">
              Ticket Details
            </h3>
            <dl class="space-y-3">
              <div>
                <dt class="text-sm font-medium text-gray-500">Client</dt>
                <dd class="text-sm text-gray-900">{{ ticket.clientName }}</dd>
              </div>
              <div v-if="ticket.assignedTo">
                <dt class="text-sm font-medium text-gray-500">Assigned To</dt>
                <dd class="text-sm text-gray-900">{{ ticket.assignedTo }}</dd>
              </div>
              <div>
                <dt class="text-sm font-medium text-gray-500">Created</dt>
                <dd class="text-sm text-gray-900">
                  {{ formatDateTime(ticket.createdAt) }}
                </dd>
              </div>
              <div>
                <dt class="text-sm font-medium text-gray-500">Last Updated</dt>
                <dd class="text-sm text-gray-900">
                  {{ formatDateTime(ticket.updatedAt) }}
                </dd>
              </div>
              <div v-if="ticket.tags && ticket.tags.length > 0">
                <dt class="text-sm font-medium text-gray-500 mb-2">Tags</dt>
                <dd class="flex flex-wrap gap-1">
                  <span
                    v-for="tag in ticket.tags"
                    :key="tag"
                    class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-800"
                  >
                    {{ tag }}
                  </span>
                </dd>
              </div>
            </dl>
          </div>

          <!-- Time Entries -->
          <div class="bg-white rounded-lg shadow">
            <div class="px-6 py-4 border-b border-gray-200">
              <div class="flex items-center justify-between">
                <h3 class="text-lg font-medium text-gray-900">Time Entries</h3>
                <button
                  @click="addTimeEntry"
                  class="text-blue-600 hover:text-blue-800 text-sm font-medium"
                >
                  + Add Time
                </button>
              </div>
            </div>

            <div class="divide-y divide-gray-200">
              <div
                v-for="entry in ticket.timeEntries"
                :key="entry.id"
                class="p-4"
              >
                <div class="flex items-start justify-between">
                  <div class="flex-1">
                    <div class="flex items-center space-x-2">
                      <span class="text-sm font-medium text-gray-900">
                        {{ entry.hours }}h
                      </span>
                      <span
                        v-if="entry.billable"
                        class="inline-flex items-center px-1.5 py-0.5 rounded text-xs font-medium bg-green-100 text-green-800"
                      >
                        Billable
                      </span>
                    </div>
                    <p class="text-sm text-gray-600 mt-1">
                      {{ entry.description }}
                    </p>
                    <div class="flex items-center text-xs text-gray-500 mt-1">
                      <span>{{ entry.createdBy }}</span>
                      <span class="mx-1">•</span>
                      <span>{{ formatDate(entry.date) }}</span>
                    </div>
                  </div>
                  <div class="text-right">
                    <div
                      v-if="entry.hourlyRate && entry.billable"
                      class="text-sm font-medium text-gray-900"
                    >
                      ${{ (entry.hours * entry.hourlyRate).toFixed(2) }}
                    </div>
                  </div>
                </div>
              </div>

              <!-- No Time Entries State -->
              <div
                v-if="!ticket.timeEntries || ticket.timeEntries.length === 0"
                class="p-6 text-center text-gray-500"
              >
                <ClockIcon class="h-8 w-8 mx-auto mb-2 text-gray-400" />
                <p>No time logged yet.</p>
                <button
                  @click="addTimeEntry"
                  class="mt-2 text-blue-600 hover:text-blue-800 text-sm font-medium"
                >
                  Log your first time entry
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modals -->
    <TicketModal
      v-if="showEditModal"
      :ticket="ticket"
      @close="showEditModal = false"
      @saved="onTicketUpdated"
    />

    <TimeEntryModal
      v-if="showTimeModal"
      :ticket-id="ticketId"
      @close="showTimeModal = false"
      @saved="onTimeEntrySaved"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useTicketsStore } from "../stores/tickets";
import { useNotificationStore } from "../stores/notifications";
import type { Ticket, TicketStatus } from "../types/ticket";
import {
  ArrowLeftIcon,
  PencilIcon,
  ClockIcon,
  ExclamationTriangleIcon,
  UserIcon,
  ChatBubbleLeftIcon,
} from "@heroicons/vue/24/outline";
import TicketModal from "../components/TicketModal.vue";
import TimeEntryModal from "../components/TimeEntryModal.vue";

interface Props {
  id: number;
}

const props = defineProps<Props>();
const route = useRoute();
const router = useRouter();
const ticketsStore = useTicketsStore();
const notificationStore = useNotificationStore();

// State
const ticket = ref<Ticket | null>(null);
const loading = ref(false);
const error = ref<string | null>(null);
const showEditModal = ref(false);
const showTimeModal = ref(false);
const newComment = ref("");
const commentIsInternal = ref(false);
const addingComment = ref(false);

// Computed
const ticketId = computed(() => props.id || Number(route.params.id));

// Methods
const loadTicket = async () => {
  loading.value = true;
  error.value = null;

  try {
    const loadedTicket = await ticketsStore.fetchTicket(ticketId.value);
    ticket.value = loadedTicket;
  } catch (err: any) {
    error.value = "Failed to load ticket";
    console.error("Load ticket error:", err);
  } finally {
    loading.value = false;
  }
};

const goBack = () => {
  router.push("/tickets");
};

const editTicket = () => {
  showEditModal.value = true;
};

const addTimeEntry = () => {
  showTimeModal.value = true;
};

const updateStatus = async () => {
  if (!ticket.value) return;

  try {
    await ticketsStore.updateTicketStatus(ticket.value.id, ticket.value.status);
    notificationStore.success("Ticket status updated!");
  } catch (error) {
    notificationStore.error("Failed to update status");
    console.error("Update status error:", error);
  }
};

const submitComment = async () => {
  if (!newComment.value.trim() || !ticket.value) return;

  addingComment.value = true;

  try {
    const result = await ticketsStore.addComment(ticket.value.id, {
      content: newComment.value,
      isInternal: commentIsInternal.value,
    });

    if (result.success) {
      newComment.value = "";
      commentIsInternal.value = false;
      notificationStore.success("Comment added!");
    } else {
      notificationStore.error(result.error || "Failed to add comment");
    }
  } catch (error) {
    notificationStore.error("Failed to add comment");
    console.error("Add comment error:", error);
  } finally {
    addingComment.value = false;
  }
};

const onTicketUpdated = () => {
  showEditModal.value = false;
  loadTicket(); // Refresh ticket data
  notificationStore.success("Ticket updated successfully!");
};

const onTimeEntrySaved = () => {
  showTimeModal.value = false;
  loadTicket(); // Refresh to show new time entry
  notificationStore.success("Time entry logged!");
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

const formatDateTime = (dateString: string) => {
  const date = new Date(dateString);
  return date.toLocaleDateString("en-US", {
    month: "short",
    day: "numeric",
    year: "numeric",
    hour: "numeric",
    minute: "2-digit",
  });
};

const isOverdue = (dueDate: string) => {
  return new Date(dueDate) < new Date();
};

const handleFileUpload = (files: any[]) => {
  // Handle file upload logic here
  console.log("Files uploaded:", files);
  notificationStore.success(`${files.length} file(s) uploaded successfully!`);
};

const handleFileError = (error: string) => {
  notificationStore.error(`Upload error: ${error}`);
};

// Lifecycle
onMounted(() => {
  loadTicket();
});
</script>
