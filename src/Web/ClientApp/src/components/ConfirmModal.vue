<template>
  <div
    class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center p-4 z-50"
  >
    <div class="bg-white rounded-lg shadow-xl max-w-md w-full">
      <div class="p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <ExclamationTriangleIcon
              class="h-6 w-6"
              :class="{
                'text-red-600': confirmStyle === 'danger',
                'text-yellow-600': confirmStyle === 'warning',
                'text-blue-600': confirmStyle === 'info',
              }"
            />
          </div>
          <div class="ml-3">
            <h3 class="text-lg font-medium text-gray-900">
              {{ title }}
            </h3>
            <div class="mt-2">
              <p class="text-sm text-gray-500">
                {{ message }}
              </p>
            </div>
          </div>
        </div>
      </div>

      <div
        class="bg-gray-50 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse rounded-b-lg"
      >
        <button
          type="button"
          @click="$emit('confirm')"
          class="w-full inline-flex justify-center rounded-lg border border-transparent shadow-sm px-4 py-2 text-base font-medium text-white focus:outline-none focus:ring-2 focus:ring-offset-2 sm:ml-3 sm:w-auto sm:text-sm"
          :class="{
            'bg-red-600 hover:bg-red-700 focus:ring-red-500':
              confirmStyle === 'danger',
            'bg-yellow-600 hover:bg-yellow-700 focus:ring-yellow-500':
              confirmStyle === 'warning',
            'bg-blue-600 hover:bg-blue-700 focus:ring-blue-500':
              confirmStyle === 'info',
          }"
        >
          {{ confirmText }}
        </button>
        <button
          type="button"
          @click="$emit('cancel')"
          class="mt-3 w-full inline-flex justify-center rounded-lg border border-gray-300 shadow-sm px-4 py-2 bg-white text-base font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 sm:mt-0 sm:ml-3 sm:w-auto sm:text-sm"
        >
          {{ cancelText }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ExclamationTriangleIcon } from "@heroicons/vue/24/outline";

interface Props {
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  confirmStyle?: "danger" | "warning" | "info";
}

interface Emits {
  (e: "confirm"): void;
  (e: "cancel"): void;
}

withDefaults(defineProps<Props>(), {
  confirmText: "Confirm",
  cancelText: "Cancel",
  confirmStyle: "info",
});

defineEmits<Emits>();
</script>
