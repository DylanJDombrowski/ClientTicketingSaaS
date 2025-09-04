<template>
  <div
    class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center p-4 z-50"
  >
    <div
      class="bg-white rounded-lg shadow-xl max-w-md w-full max-h-screen overflow-y-auto"
    >
      <div class="px-6 py-4 border-b border-gray-200">
        <h3 class="text-lg font-medium text-gray-900">
          {{ isEdit ? "Edit Client" : "New Client" }}
        </h3>
      </div>

      <form @submit.prevent="handleSubmit" class="p-6 space-y-4">
        <!-- Name Field -->
        <div>
          <label
            for="name"
            class="block text-sm font-medium text-gray-700 mb-1"
          >
            Name *
          </label>
          <input
            id="name"
            v-model="form.name"
            type="text"
            required
            class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            :class="{ 'border-red-300': errors.name }"
            placeholder="Enter client name"
          />
          <p v-if="errors.name" class="mt-1 text-sm text-red-600">
            {{ errors.name }}
          </p>
        </div>

        <!-- Email Field -->
        <div>
          <label
            for="email"
            class="block text-sm font-medium text-gray-700 mb-1"
          >
            Email *
          </label>
          <input
            id="email"
            v-model="form.email"
            type="email"
            required
            class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            :class="{ 'border-red-300': errors.email }"
            placeholder="client@example.com"
          />
          <p v-if="errors.email" class="mt-1 text-sm text-red-600">
            {{ errors.email }}
          </p>
        </div>

        <!-- Phone Field -->
        <div>
          <label
            for="phone"
            class="block text-sm font-medium text-gray-700 mb-1"
          >
            Phone
          </label>
          <input
            id="phone"
            v-model="form.phone"
            type="tel"
            class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            :class="{ 'border-red-300': errors.phone }"
            placeholder="+1 (555) 123-4567"
          />
          <p v-if="errors.phone" class="mt-1 text-sm text-red-600">
            {{ errors.phone }}
          </p>
        </div>

        <!-- Company Field -->
        <div>
          <label
            for="company"
            class="block text-sm font-medium text-gray-700 mb-1"
          >
            Company
          </label>
          <input
            id="company"
            v-model="form.company"
            type="text"
            class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            :class="{ 'border-red-300': errors.company }"
            placeholder="Company name"
          />
          <p v-if="errors.company" class="mt-1 text-sm text-red-600">
            {{ errors.company }}
          </p>
        </div>

        <!-- Active Status (only for edit) -->
        <div v-if="isEdit" class="flex items-center">
          <input
            id="isActive"
            v-model="form.isActive"
            type="checkbox"
            class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
          />
          <label for="isActive" class="ml-2 block text-sm text-gray-700">
            Active client
          </label>
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
            <span>{{ isEdit ? "Update" : "Create" }} Client</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from "vue";
import { useClientsStore } from "../stores/clients";
import type { Client, CreateClientRequest } from "../types/client";

interface Props {
  client?: Client | null;
}

interface Emits {
  (e: "close"): void;
  (e: "saved"): void;
}

const props = withDefaults(defineProps<Props>(), {
  client: null,
});

const emit = defineEmits<Emits>();

const clientsStore = useClientsStore();

// State
const loading = ref(false);
const form = ref({
  name: "",
  email: "",
  phone: "",
  company: "",
  isActive: true,
});

const errors = ref<Record<string, string>>({});

// Computed
const isEdit = computed(() => !!props.client);

// Watch for prop changes to populate form
watch(
  () => props.client,
  (client) => {
    if (client) {
      form.value = {
        name: client.name,
        email: client.email,
        phone: client.phone || "",
        company: client.company || "",
        isActive: client.isActive,
      };
    } else {
      // Reset form for new client
      form.value = {
        name: "",
        email: "",
        phone: "",
        company: "",
        isActive: true,
      };
    }
    errors.value = {};
  },
  { immediate: true }
);

// Methods
const validateForm = (): boolean => {
  errors.value = {};

  if (!form.value.name.trim()) {
    errors.value.name = "Name is required";
  } else if (form.value.name.length > 200) {
    errors.value.name = "Name must be less than 200 characters";
  }

  if (!form.value.email.trim()) {
    errors.value.email = "Email is required";
  } else if (form.value.email.length > 254) {
    errors.value.email = "Email must be less than 254 characters";
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.value.email)) {
    errors.value.email = "Please enter a valid email address";
  }

  if (form.value.phone && form.value.phone.length > 50) {
    errors.value.phone = "Phone must be less than 50 characters";
  }

  if (form.value.company && form.value.company.length > 200) {
    errors.value.company = "Company name must be less than 200 characters";
  }

  return Object.keys(errors.value).length === 0;
};

const handleSubmit = async () => {
  if (!validateForm()) return;

  loading.value = true;

  try {
    let result;

    if (isEdit.value && props.client) {
      // Update existing client
      result = await clientsStore.updateClient(props.client.id, {
        name: form.value.name,
        email: form.value.email,
        phone: form.value.phone || undefined,
        company: form.value.company || undefined,
        isActive: form.value.isActive,
      });
    } else {
      // Create new client
      const createData: CreateClientRequest = {
        name: form.value.name,
        email: form.value.email,
        phone: form.value.phone || undefined,
        company: form.value.company || undefined,
      };
      result = await clientsStore.createClient(createData);
    }

    if (result.success) {
      emit("saved");
    } else {
      // Handle validation errors from server
      if (result.error?.includes("unique")) {
        errors.value.email = "This email address is already in use";
      } else {
        errors.value.general = result.error || "An error occurred";
      }
    }
  } catch (error) {
    console.error("Form submission error:", error);
    errors.value.general = "An unexpected error occurred";
  } finally {
    loading.value = false;
  }
};
</script>
