<template>
  <div
    class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center p-4 z-50"
  >
    <div class="bg-white rounded-lg shadow-xl max-w-md w-full">
      <div class="px-6 py-4 border-b border-gray-200">
        <h3 class="text-lg font-medium text-gray-900">Log Time Entry</h3>
      </div>

      <form @submit.prevent="handleSubmit" class="p-6 space-y-4">
        <!-- Hours Field -->
        <div>
          <label
            for="hours"
            class="block text-sm font-medium text-gray-700 mb-1"
          >
            Hours Worked *
          </label>
          <input
            id="hours"
            v-model.number="form.hours"
            type="number"
            step="0.25"
            min="0.25"
            max="24"
            required
            class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            :class="{ 'border-red-300': errors.hours }"
            placeholder="2.5"
          />
          <p v-if="errors.hours" class="mt-1 text-sm text-red-600">
            {{ errors.hours }}
          </p>
          <p class="mt-1 text-xs text-gray-500">
            Enter in quarter-hour increments (0.25, 0.5, 0.75, 1.0)
          </p>
        </div>

        <!-- Date Field -->
        <div>
          <label
            for="date"
            class="block text-sm font-medium text-gray-700 mb-1"
          >
            Date *
          </label>
          <input
            id="date"
            v-model="form.date"
            type="date"
            required
            :max="today"
            class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            :class="{ 'border-red-300': errors.date }"
          />
          <p v-if="errors.date" class="mt-1 text-sm text-red-600">
            {{ errors.date }}
          </p>
        </div>

        <!-- Description Field -->
        <div>
          <label
            for="description"
            class="block text-sm font-medium text-gray-700 mb-1"
          >
            Work Description *
          </label>
          <textarea
            id="description"
            v-model="form.description"
            rows="3"
            required
            class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            :class="{ 'border-red-300': errors.description }"
            placeholder="Describe the work performed..."
          ></textarea>
          <p v-if="errors.description" class="mt-1 text-sm text-red-600">
            {{ errors.description }}
          </p>
        </div>

        <!-- Billable Checkbox -->
        <div class="flex items-center">
          <input
            id="billable"
            v-model="form.billable"
            type="checkbox"
            class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
          />
          <label for="billable" class="ml-2 block text-sm text-gray-700">
            This time is billable to the client
          </label>
        </div>

        <!-- Hourly Rate Field (if billable) -->
        <div v-if="form.billable">
          <label
            for="hourlyRate"
            class="block text-sm font-medium text-gray-700 mb-1"
          >
            Hourly Rate ($)
          </label>
          <input
            id="hourlyRate"
            v-model.number="form.hourlyRate"
            type="number"
            step="0.01"
            min="0"
            class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            :class="{ 'border-red-300': errors.hourlyRate }"
            placeholder="75.00"
          />
          <p v-if="errors.hourlyRate" class="mt-1 text-sm text-red-600">
            {{ errors.hourlyRate }}
          </p>
        </div>

        <!-- Total Calculation (if billable with rate) -->
        <div
          v-if="form.billable && form.hourlyRate && form.hours"
          class="bg-blue-50 rounded-lg p-3"
        >
          <div class="flex justify-between items-center">
            <span class="text-sm font-medium text-blue-900"
              >Estimated Total:</span
            >
            <span class="text-lg font-bold text-blue-900">
              ${{ (form.hours * form.hourlyRate).toFixed(2) }}
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
            <span>Log Time</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { useTicketsStore } from "../stores/tickets";
import type { CreateTimeEntryRequest } from "../types/ticket";

interface Props {
  ticketId: number;
}

interface Emits {
  (e: "close"): void;
  (e: "saved"): void;
}

const props = defineProps<Props>();
const emit = defineEmits<Emits>();

const ticketsStore = useTicketsStore();

// State
const loading = ref(false);
const form = ref({
  hours: null as number | null,
  date: new Date().toISOString().split("T")[0], // Today's date
  description: "",
  billable: true,
  hourlyRate: null as number | null,
});

const errors = ref<Record<string, string>>({});

// Computed
const today = computed(() => new Date().toISOString().split("T")[0]);

// Methods
const validateForm = (): boolean => {
  errors.value = {};

  if (!form.value.hours || form.value.hours <= 0) {
    errors.value.hours = "Please enter a valid number of hours";
  } else if (form.value.hours > 24) {
    errors.value.hours = "Hours cannot exceed 24 per day";
  } else if (form.value.hours % 0.25 !== 0) {
    errors.value.hours =
      "Please enter hours in quarter-hour increments (e.g., 0.25, 0.5, 1.75)";
  }

  if (!form.value.date) {
    errors.value.date = "Please select a date";
  } else if (new Date(form.value.date) > new Date()) {
    errors.value.date = "Date cannot be in the future";
  }

  if (!form.value.description.trim()) {
    errors.value.description = "Please describe the work performed";
  } else if (form.value.description.length > 1000) {
    errors.value.description = "Description must be less than 1000 characters";
  }

  if (
    form.value.billable &&
    form.value.hourlyRate !== null &&
    form.value.hourlyRate < 0
  ) {
    errors.value.hourlyRate = "Hourly rate cannot be negative";
  }

  return Object.keys(errors.value).length === 0;
};

const handleSubmit = async () => {
  if (!validateForm()) return;

  loading.value = true;

  try {
    const timeEntryData: CreateTimeEntryRequest = {
      ticketId: props.ticketId,
      hours: form.value.hours!,
      date: form.value.date,
      description: form.value.description.trim(),
      billable: form.value.billable,
      hourlyRate: form.value.billable
        ? form.value.hourlyRate || undefined
        : undefined,
    };

    const result = await ticketsStore.addTimeEntry(timeEntryData);

    if (result.success) {
      emit("saved");
    } else {
      errors.value.general = result.error || "Failed to log time entry";
    }
  } catch (error) {
    console.error("Time entry submission error:", error);
    errors.value.general = "An unexpected error occurred";
  } finally {
    loading.value = false;
  }
};

// Load default hourly rate (could come from user settings or client settings)
onMounted(() => {
  // Set default hourly rate - in a real app this might come from user preferences
  form.value.hourlyRate = 75.0;
});
</script>
