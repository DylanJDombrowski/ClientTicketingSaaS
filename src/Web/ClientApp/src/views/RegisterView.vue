<template>
  <div
    class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8"
  >
    <div class="max-w-md w-full space-y-8">
      <!-- Header -->
      <div>
        <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
          Create Your Account
        </h2>
        <p class="mt-2 text-center text-sm text-gray-600">
          Or
          <router-link
            to="/login"
            class="font-medium text-blue-600 hover:text-blue-500"
          >
            sign in to your existing account
          </router-link>
        </p>
      </div>

      <!-- Progress Steps -->
      <div class="flex items-center justify-center space-x-4">
        <div
          v-for="(step, index) in steps"
          :key="index"
          class="flex items-center"
        >
          <div
            class="flex items-center justify-center w-8 h-8 rounded-full text-sm font-medium"
            :class="{
              'bg-blue-600 text-white': currentStep > index,
              'bg-blue-100 text-blue-600 ring-2 ring-blue-600':
                currentStep === index,
              'bg-gray-200 text-gray-600': currentStep < index,
            }"
          >
            <CheckIcon v-if="currentStep > index" class="h-4 w-4" />
            <span v-else>{{ index + 1 }}</span>
          </div>
          <div
            v-if="index < steps.length - 1"
            class="w-8 h-0.5 mx-2"
            :class="{
              'bg-blue-600': currentStep > index,
              'bg-gray-200': currentStep <= index,
            }"
          ></div>
        </div>
      </div>

      <!-- Step Content -->
      <form @submit.prevent="handleSubmit" class="mt-8 space-y-6">
        <!-- Step 1: Account Info -->
        <div v-show="currentStep === 0" class="space-y-4">
          <div>
            <label for="email" class="block text-sm font-medium text-gray-700">
              Email Address
            </label>
            <input
              id="email"
              v-model="form.email"
              type="email"
              required
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-lg focus:outline-none focus:ring-blue-500 focus:border-blue-500 focus:z-10"
              :class="{ 'border-red-300': errors.email }"
              placeholder="your@email.com"
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
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-lg focus:outline-none focus:ring-blue-500 focus:border-blue-500"
              :class="{ 'border-red-300': errors.password }"
              placeholder="Choose a strong password"
            />
            <p v-if="errors.password" class="mt-1 text-sm text-red-600">
              {{ errors.password }}
            </p>

            <!-- Password Strength Indicator -->
            <div v-if="form.password" class="mt-2">
              <div class="flex items-center space-x-2">
                <div class="flex-1 bg-gray-200 rounded-full h-2">
                  <div
                    class="h-2 rounded-full transition-all duration-300"
                    :class="passwordStrength.colorClass"
                    :style="{ width: passwordStrength.percentage + '%' }"
                  ></div>
                </div>
                <span
                  class="text-xs font-medium"
                  :class="passwordStrength.colorClass"
                >
                  {{ passwordStrength.label }}
                </span>
              </div>
              <ul class="mt-2 text-xs text-gray-600 space-y-1">
                <li class="flex items-center space-x-2">
                  <CheckIcon
                    v-if="passwordChecks.length"
                    class="h-3 w-3 text-green-500"
                  />
                  <XMarkIcon v-else class="h-3 w-3 text-gray-400" />
                  <span>At least 8 characters</span>
                </li>
                <li class="flex items-center space-x-2">
                  <CheckIcon
                    v-if="passwordChecks.uppercase"
                    class="h-3 w-3 text-green-500"
                  />
                  <XMarkIcon v-else class="h-3 w-3 text-gray-400" />
                  <span>One uppercase letter</span>
                </li>
                <li class="flex items-center space-x-2">
                  <CheckIcon
                    v-if="passwordChecks.lowercase"
                    class="h-3 w-3 text-green-500"
                  />
                  <XMarkIcon v-else class="h-3 w-3 text-gray-400" />
                  <span>One lowercase letter</span>
                </li>
                <li class="flex items-center space-x-2">
                  <CheckIcon
                    v-if="passwordChecks.number"
                    class="h-3 w-3 text-green-500"
                  />
                  <XMarkIcon v-else class="h-3 w-3 text-gray-400" />
                  <span>One number</span>
                </li>
              </ul>
            </div>
          </div>

          <div>
            <label
              for="confirmPassword"
              class="block text-sm font-medium text-gray-700"
            >
              Confirm Password
            </label>
            <input
              id="confirmPassword"
              v-model="form.confirmPassword"
              type="password"
              required
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-lg focus:outline-none focus:ring-blue-500 focus:border-blue-500"
              :class="{ 'border-red-300': errors.confirmPassword }"
              placeholder="Confirm your password"
            />
            <p v-if="errors.confirmPassword" class="mt-1 text-sm text-red-600">
              {{ errors.confirmPassword }}
            </p>
          </div>
        </div>

        <!-- Step 2: Personal Info -->
        <div v-show="currentStep === 1" class="space-y-4">
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label
                for="firstName"
                class="block text-sm font-medium text-gray-700"
              >
                First Name
              </label>
              <input
                id="firstName"
                v-model="form.firstName"
                type="text"
                class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-lg focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                :class="{ 'border-red-300': errors.firstName }"
                placeholder="John"
              />
              <p v-if="errors.firstName" class="mt-1 text-sm text-red-600">
                {{ errors.firstName }}
              </p>
            </div>

            <div>
              <label
                for="lastName"
                class="block text-sm font-medium text-gray-700"
              >
                Last Name
              </label>
              <input
                id="lastName"
                v-model="form.lastName"
                type="text"
                class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-lg focus:outline-none focus:ring-blue-500 focus:border-blue-500"
                :class="{ 'border-red-300': errors.lastName }"
                placeholder="Doe"
              />
              <p v-if="errors.lastName" class="mt-1 text-sm text-red-600">
                {{ errors.lastName }}
              </p>
            </div>
          </div>
        </div>

        <!-- Step 3: Business Info -->
        <div v-show="currentStep === 2" class="space-y-4">
          <div>
            <label
              for="tenantName"
              class="block text-sm font-medium text-gray-700"
            >
              Business/Organization Name
            </label>
            <input
              id="tenantName"
              v-model="form.tenantName"
              type="text"
              required
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-lg focus:outline-none focus:ring-blue-500 focus:border-blue-500"
              :class="{ 'border-red-300': errors.tenantName }"
              placeholder="Acme Consulting LLC"
            />
            <p v-if="errors.tenantName" class="mt-1 text-sm text-red-600">
              {{ errors.tenantName }}
            </p>
            <p class="mt-1 text-xs text-gray-500">
              This will be your organization name in the system
            </p>
          </div>

          <div>
            <label
              for="businessType"
              class="block text-sm font-medium text-gray-700"
            >
              Business Type
            </label>
            <select
              id="businessType"
              v-model="form.businessType"
              class="mt-1 block w-full px-3 py-2 border border-gray-300 bg-white rounded-lg focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            >
              <option value="">Select your business type</option>
              <option value="consulting">Consulting</option>
              <option value="agency">Digital Agency</option>
              <option value="freelancer">Freelancer</option>
              <option value="software">Software Development</option>
              <option value="support">IT Support</option>
              <option value="other">Other</option>
            </select>
          </div>
        </div>

        <!-- Step 4: Confirmation -->
        <div v-show="currentStep === 3" class="space-y-4">
          <div class="bg-blue-50 rounded-lg p-4">
            <h3 class="text-lg font-medium text-blue-900 mb-4">
              Review Your Information
            </h3>
            <dl class="space-y-2">
              <div class="flex justify-between">
                <dt class="text-sm font-medium text-blue-700">Email:</dt>
                <dd class="text-sm text-blue-900">{{ form.email }}</dd>
              </div>
              <div
                v-if="form.firstName || form.lastName"
                class="flex justify-between"
              >
                <dt class="text-sm font-medium text-blue-700">Name:</dt>
                <dd class="text-sm text-blue-900">
                  {{ form.firstName }} {{ form.lastName }}
                </dd>
              </div>
              <div class="flex justify-between">
                <dt class="text-sm font-medium text-blue-700">Organization:</dt>
                <dd class="text-sm text-blue-900">{{ form.tenantName }}</dd>
              </div>
              <div v-if="form.businessType" class="flex justify-between">
                <dt class="text-sm font-medium text-blue-700">
                  Business Type:
                </dt>
                <dd class="text-sm text-blue-900">
                  {{ businessTypeLabels[form.businessType] }}
                </dd>
              </div>
            </dl>
          </div>

          <div class="flex items-center">
            <input
              id="agreeTerms"
              v-model="form.agreeTerms"
              type="checkbox"
              class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
            />
            <label for="agreeTerms" class="ml-2 block text-sm text-gray-700">
              I agree to the
              <a href="#" class="text-blue-600 hover:text-blue-500"
                >Terms of Service</a
              >
              and
              <a href="#" class="text-blue-600 hover:text-blue-500"
                >Privacy Policy</a
              >
            </label>
          </div>
          <p v-if="errors.agreeTerms" class="text-sm text-red-600">
            {{ errors.agreeTerms }}
          </p>
        </div>

        <!-- General Error -->
        <div
          v-if="errors.general"
          class="bg-red-50 border border-red-200 rounded-lg p-3"
        >
          <p class="text-sm text-red-600">{{ errors.general }}</p>
        </div>

        <!-- Navigation Buttons -->
        <div class="flex justify-between pt-6">
          <button
            v-if="currentStep > 0"
            type="button"
            @click="previousStep"
            class="flex items-center px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50"
          >
            <ArrowLeftIcon class="h-4 w-4 mr-2" />
            Previous
          </button>
          <div v-else></div>

          <button
            v-if="currentStep < steps.length - 1"
            type="button"
            @click="nextStep"
            class="flex items-center px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700"
          >
            Next
            <ArrowRightIcon class="h-4 w-4 ml-2" />
          </button>
          <button
            v-else
            type="submit"
            :disabled="loading || !canSubmit"
            class="flex items-center px-6 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <div
              v-if="loading"
              class="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"
            ></div>
            {{ loading ? "Creating Account..." : "Create Account" }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "../stores/auth";
import { useNotificationStore } from "../stores/notifications";
import {
  CheckIcon,
  XMarkIcon,
  ArrowLeftIcon,
  ArrowRightIcon,
} from "@heroicons/vue/24/outline";

const router = useRouter();
const authStore = useAuthStore();
const notificationStore = useNotificationStore();

// State
const currentStep = ref(0);
const loading = ref(false);
const form = ref({
  email: "",
  password: "",
  confirmPassword: "",
  firstName: "",
  lastName: "",
  tenantName: "",
  businessType: "",
  agreeTerms: false,
});

const errors = ref<Record<string, string>>({});

const steps = ["Account", "Personal", "Business", "Review"];

const businessTypeLabels: Record<string, string> = {
  consulting: "Consulting",
  agency: "Digital Agency",
  freelancer: "Freelancer",
  software: "Software Development",
  support: "IT Support",
  other: "Other",
};

// Computed
const passwordChecks = computed(() => ({
  length: form.value.password.length >= 8,
  uppercase: /[A-Z]/.test(form.value.password),
  lowercase: /[a-z]/.test(form.value.password),
  number: /\d/.test(form.value.password),
}));

const passwordStrength = computed(() => {
  const checks = Object.values(passwordChecks.value);
  const score = checks.filter(Boolean).length;

  if (score === 0) return { percentage: 0, label: "", colorClass: "" };
  if (score === 1)
    return {
      percentage: 25,
      label: "Weak",
      colorClass: "bg-red-500 text-red-600",
    };
  if (score === 2)
    return {
      percentage: 50,
      label: "Fair",
      colorClass: "bg-yellow-500 text-yellow-600",
    };
  if (score === 3)
    return {
      percentage: 75,
      label: "Good",
      colorClass: "bg-blue-500 text-blue-600",
    };
  return {
    percentage: 100,
    label: "Strong",
    colorClass: "bg-green-500 text-green-600",
  };
});

const canSubmit = computed(() => {
  return (
    form.value.agreeTerms && Object.values(passwordChecks.value).every(Boolean)
  );
});

// Methods
const validateCurrentStep = (): boolean => {
  errors.value = {};

  if (currentStep.value === 0) {
    // Account validation
    if (!form.value.email.trim()) {
      errors.value.email = "Email is required";
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.value.email)) {
      errors.value.email = "Please enter a valid email address";
    }

    if (!form.value.password) {
      errors.value.password = "Password is required";
    } else if (!Object.values(passwordChecks.value).every(Boolean)) {
      errors.value.password = "Password does not meet requirements";
    }

    if (!form.value.confirmPassword) {
      errors.value.confirmPassword = "Please confirm your password";
    } else if (form.value.password !== form.value.confirmPassword) {
      errors.value.confirmPassword = "Passwords do not match";
    }
  } else if (currentStep.value === 2) {
    // Business validation
    if (!form.value.tenantName.trim()) {
      errors.value.tenantName = "Business name is required";
    } else if (form.value.tenantName.length > 100) {
      errors.value.tenantName =
        "Business name must be less than 100 characters";
    }
  } else if (currentStep.value === 3) {
    // Final validation
    if (!form.value.agreeTerms) {
      errors.value.agreeTerms = "You must agree to the terms and conditions";
    }
  }

  return Object.keys(errors.value).length === 0;
};

const nextStep = () => {
  if (validateCurrentStep() && currentStep.value < steps.length - 1) {
    currentStep.value++;
  }
};

const previousStep = () => {
  if (currentStep.value > 0) {
    currentStep.value--;
  }
};

const handleSubmit = async () => {
  if (!validateCurrentStep() || !canSubmit.value) return;

  loading.value = true;
  errors.value = {};

  try {
    const result = await authStore.register({
      email: form.value.email,
      password: form.value.password,
      confirmPassword: form.value.confirmPassword,
      tenantName: form.value.tenantName,
      firstName: form.value.firstName || undefined,
      lastName: form.value.lastName || undefined,
    });

    if (result.success) {
      notificationStore.success(
        "Account created successfully! Welcome to TicketSaaS!"
      );
      router.push("/dashboard");
    } else {
      errors.value.general = result.error || "Registration failed";
    }
  } catch (error) {
    console.error("Registration error:", error);
    errors.value.general = "An unexpected error occurred";
  } finally {
    loading.value = false;
  }
};
</script>
