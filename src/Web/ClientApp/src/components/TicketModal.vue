<template>
  <div
    class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center p-4 z-50"
  >
    <div
      class="bg-white rounded-lg shadow-xl max-w-2xl w-full max-h-screen overflow-y-auto"
    >
      <div class="px-6 py-4 border-b border-gray-200">
        <h3 class="text-lg font-medium text-gray-900">
          {{ isEdit ? "Edit Ticket" : "New Support Ticket" }}
        </h3>
      </div>

      <form @submit.prevent="handleSubmit" class="p-6 space-y-6">
        <!-- Title Field -->
        <div>
          <label
            for="title"
            class="block text-sm font-medium text-gray-700 mb-1"
          >
            Title *
          </label>
          <input
            id="title"
            v-model="form.title"
            type="text"
            required
            class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            :class="{ 'border-red-300': errors.title }"
            placeholder="Brief description of the issue"
          />
          <p v-if="errors.title" class="mt-1 text-sm text-red-600">
            {{ errors.title }}
          </p>
        </div>

        <!-- Description Field -->
        <div>
          <label
            for="description"
            class="block text-sm font-medium text-gray-700 mb-1"
          >
            Description *
          </label>
          <textarea
            id="description"
            v-model="form.description"
            rows="4"
            required
            class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            :class="{ 'border-red-300': errors.description }"
            placeholder="Detailed description of the issue, steps to reproduce, expected behavior..."
          ></textarea>
          <p v-if="errors.description" class="mt-1 text-sm text-red-600">
            {{ errors.description }}
          </p>
        </div>

        <!-- Client and Priority Row -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <!-- Client Field -->
          <div>
            <label
              for="clientId"
              class="block text-sm font-medium text-gray-700 mb-1"
            >
              Client *
            </label>
            <select
              id="clientId"
              v-model="form.clientId"
              required
              class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              :class="{ 'border-red-300': errors.clientId }"
            >
              <option value="">Select a client</option>
              <option
                v-for="client in availableClients"
                :key="client.id"
                :value="client.id"
              >
                {{ client.name }}
                {{ client.company ? `(${client.company})` : "" }}
              </option>
            </select>
            <p v-if="errors.clientId" class="mt-1 text-sm text-red-600">
              {{ errors.clientId }}
            </p>
          </div>

          <!-- Priority Field -->
          <div>
            <label
              for="priority"
              class="block text-sm font-medium text-gray-700 mb-1"
            >
              Priority *
            </label>
            <select
              id="priority"
              v-model="form.priority"
              required
              class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              :class="{ 'border-red-300': errors.priority }"
            >
              <option value="">Select priority</option>
              <option value="low">Low</option>
              <option value="medium">Medium</option>
              <option value="high">High</option>
            </select>
            <p v-if="errors.priority" class="mt-1 text-sm text-red-600">
              {{ errors.priority }}
            </p>
          </div>
        </div>

        <!-- Status (Edit only) and Due Date Row -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <!-- Status Field (Edit only) -->
          <div v-if="isEdit">
            <label
              for="status"
              class="block text-sm font-medium text-gray-700 mb-1"
            >
              Status
            </label>
            <select
              id="status"
              v-model="form.status"
              class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            >
              <option value="open">Open</option>
              <option value="in_progress">In Progress</option>
              <option value="completed">Completed</option>
            </select>
          </div>

          <!-- Due Date Field -->
          <div>
            <label
              for="dueDate"
              class="block text-sm font-medium text-gray-700 mb-1"
            >
              Due Date
            </label>
            <input
              id="dueDate"
              v-model="form.dueDate"
              type="date"
              class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              :class="{ 'border-red-300': errors.dueDate }"
            />
            <p v-if="errors.dueDate" class="mt-1 text-sm text-red-600">
              {{ errors.dueDate }}
            </p>
          </div>
        </div>

        <!-- Estimated Hours and Assigned To Row -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <!-- Estimated Hours -->
          <div>
            <label
              for="estimatedHours"
              class="block text-sm font-medium text-gray-700 mb-1"
            >
              Estimated Hours
            </label>
            <input
              id="estimatedHours"
              v-model.number="form.estimatedHours"
              type="number"
              step="0.5"
              min="0"
              class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              :class="{ 'border-red-300': errors.estimatedHours }"
              placeholder="0.0"
            />
            <p v-if="errors.estimatedHours" class="mt-1 text-sm text-red-600">
              {{ errors.estimatedHours }}
            </p>
          </div>

          <!-- Assigned To (Edit only) -->
          <div v-if="isEdit">
            <label
              for="assignedTo"
              class="block text-sm font-medium text-gray-700 mb-1"
            >
              Assigned To
            </label>
            <input
              id="assignedTo"
              v-model="form.assignedTo"
              type="text"
              class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              :class="{ 'border-red-300': errors.assignedTo }"
              placeholder="Team member name"
            />
            <p v-if="errors.assignedTo" class="mt-1 text-sm text-red-600">
              {{ errors.assignedTo }}
            </p>
          </div>
        </div>

        <!-- Tags Field -->
        <div>
          <label
            for="tags"
            class="block text-sm font-medium text-gray-700 mb-1"
          >
            Tags
          </label>
          <input
            id="tags"
            v-model="tagsInput"
            type="text"
            class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            placeholder="bug, feature, urgent (comma-separated)"
          />
          <p class="mt-1 text-xs text-gray-500">
            Enter tags separated by commas
          </p>

          <!-- Display current tags -->
          <div
            v-if="form.tags && form.tags.length > 0"
            class="mt-2 flex flex-wrap gap-2"
          >
            <span
              v-for="tag in form.tags"
              :key="tag"
              class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-800"
            >
              {{ tag }}
              <button
                type="button"
                @click="removeTag(tag)"
                class="ml-1 text-blue-600 hover:text-blue-800"
              >
                ×
              </button>
            </span>
          </div>
        </div>

        <!-- General Error -->
        <div
          v-if="errors.general"
          class="bg-red-50 border border-red-200 rounded-lg p-3"
        >
          <p class="text-sm text-red-600">{{ errors.general }}</p>
        </div>

        <!-- Form Actions -->
        <div class="flex justify-end space-x-3 pt-6 border-t border-gray-200">
          <button
            type="button"
            @click="$emit('close')"
            class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 focus:ring-2 focus:ring-blue-500"
            :disabled="loading"
          >
            Cancel
          </button>
          <button
            type="submit"
            class="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 focus:ring-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed flex items-center space-x-2"
            :disabled="loading"
          >
            <div
              v-if="loading"
              class="animate-spin rounded-full h-4 w-4 border-b-2 border-white"
            ></div>
            <span>{{ isEdit ? "Update" : "Create" }} Ticket</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from "vue";
import { useTicketsStore } from "../stores/tickets";
import { useClientsStore } from "../stores/clients";
import type { Ticket, CreateTicketRequest } from "../types/ticket";

interface Props {
  ticket?: Ticket | null;
}

interface Emits {
  (e: "close"): void;
  (e: "saved"): void;
}

const props = withDefaults(defineProps<Props>(), {
  ticket: null,
});

const emit = defineEmits<Emits>();

const ticketsStore = useTicketsStore();
const clientsStore = useClientsStore();

// Enum mapping functions
const priorityToEnum = (priority: string): number => {
  const mapping: Record<string, number> = {
    low: 1,
    medium: 2,
    high: 3,
    urgent: 4,
  };
  return mapping[priority] || 2;
};

const enumToPriority = (enumValue: number): string => {
  const mapping: Record<number, string> = {
    1: "low",
    2: "medium",
    3: "high",
    4: "urgent",
  };
  return mapping[enumValue] || "medium";
};

const statusToEnum = (status: string): number => {
  const mapping: Record<string, number> = {
    open: 1,
    in_progress: 2,
    pending_review: 3,
    resolved: 4,
    closed: 5,
    cancelled: 6,
  };
  return mapping[status] || 1;
};

const enumToStatus = (enumValue: number): string => {
  const mapping: Record<number, string> = {
    1: "open",
    2: "in_progress",
    3: "pending_review",
    4: "resolved",
    5: "closed",
    6: "cancelled",
  };
  return mapping[enumValue] || "open";
};

// State - using strings for form values, will convert when sending to API
const loading = ref(false);
const form = ref({
  title: "",
  description: "",
  clientId: null as number | null,
  priority: "" as string, // Use string for form
  status: "open" as string, // Use string for form
  dueDate: "",
  estimatedHours: null as number | null,
  assignedTo: "",
  tags: [] as string[],
});

const tagsInput = ref("");
const errors = ref<Record<string, string>>({});

// Computed
const isEdit = computed(() => !!props.ticket);
const availableClients = computed(() =>
  clientsStore.clients.filter((c) => c.isActive)
);

// Watch for prop changes to populate form
watch(
  () => props.ticket,
  (ticket) => {
    if (ticket) {
      form.value = {
        title: ticket.title,
        description: ticket.description,
        clientId: ticket.clientId,
        priority: enumToPriority(ticket.priority), // Convert enum to string
        status: enumToStatus(ticket.status), // Convert enum to string
        dueDate: ticket.dueDate ? ticket.dueDate.split("T")[0] : "",
        estimatedHours: ticket.estimatedHours || null,
        assignedTo: ticket.assignedToId || "", // Fixed property name
        tags: [], // Tags not supported yet in backend DTO
      };
      tagsInput.value = ""; // Tags not supported yet
    } else {
      // Reset form for new ticket
      form.value = {
        title: "",
        description: "",
        clientId: null,
        priority: "medium", // String default
        status: "open", // String default
        dueDate: "",
        estimatedHours: null,
        assignedTo: "",
        tags: [],
      };
      tagsInput.value = "";
    }
    errors.value = {};
  },
  { immediate: true }
);

// Watch tags input for real-time parsing
watch(tagsInput, (value) => {
  if (value) {
    form.value.tags = value
      .split(",")
      .map((tag) => tag.trim())
      .filter((tag) => tag.length > 0);
  } else {
    form.value.tags = [];
  }
});

// Methods
const validateForm = (): boolean => {
  errors.value = {};

  if (!form.value.title.trim()) {
    errors.value.title = "Title is required";
  } else if (form.value.title.length > 200) {
    errors.value.title = "Title must be less than 200 characters";
  }

  if (!form.value.description.trim()) {
    errors.value.description = "Description is required";
  } else if (form.value.description.length > 2000) {
    errors.value.description = "Description must be less than 2000 characters";
  }

  if (!form.value.clientId) {
    errors.value.clientId = "Please select a client";
  }

  if (!form.value.priority) {
    errors.value.priority = "Please select a priority";
  }

  if (form.value.dueDate && new Date(form.value.dueDate) < new Date()) {
    errors.value.dueDate = "Due date cannot be in the past";
  }

  if (form.value.estimatedHours !== null && form.value.estimatedHours < 0) {
    errors.value.estimatedHours = "Estimated hours cannot be negative";
  }

  return Object.keys(errors.value).length === 0;
};

const removeTag = (tagToRemove: string) => {
  form.value.tags = form.value.tags.filter((tag) => tag !== tagToRemove);
  tagsInput.value = form.value.tags.join(", ");
};

const handleSubmit = async () => {
  if (!validateForm()) return;

  loading.value = true;

  try {
    let result;

    // Helper function to convert date string to UTC ISO string
    const convertToUTC = (dateString: string): string | undefined => {
      if (!dateString) return undefined;
      // Create date object and ensure it's treated as UTC
      const date = new Date(dateString + "T00:00:00.000Z");
      return date.toISOString();
    };

    if (isEdit.value && props.ticket) {
      // Update existing ticket - convert to enums and fix dates
      result = await ticketsStore.updateTicket(props.ticket.id, {
        title: form.value.title,
        description: form.value.description,
        status: statusToEnum(form.value.status), // Convert to enum
        priority: priorityToEnum(form.value.priority), // Convert to enum
        assignedToId: form.value.assignedTo || undefined,
        dueDate: convertToUTC(form.value.dueDate), // Convert to UTC
        estimatedHours: form.value.estimatedHours || 0,
      });
    } else {
      // Create new ticket - convert to enums and fix dates
      const createData: CreateTicketRequest = {
        title: form.value.title,
        description: form.value.description,
        clientId: form.value.clientId!,
        priority: priorityToEnum(form.value.priority), // Convert to enum
        dueDate: convertToUTC(form.value.dueDate), // Convert to UTC
        estimatedHours: form.value.estimatedHours || undefined,
      };
      result = await ticketsStore.createTicket(createData);
    }

    if (result.success) {
      emit("saved");
    } else {
      errors.value.general = result.error || "An error occurred";
    }
  } catch (error) {
    console.error("Form submission error:", error);
    errors.value.general = "An unexpected error occurred";
  } finally {
    loading.value = false;
  }
};
</script>
