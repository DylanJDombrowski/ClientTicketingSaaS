<template>
  <div class="space-y-6">
    <!-- Header -->
    <div>
      <h1 class="text-3xl font-bold text-gray-900">Profile Settings</h1>
      <p class="mt-2 text-sm text-gray-600">
        Manage your account information and preferences
      </p>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Sidebar Navigation -->
      <div class="lg:col-span-1">
        <nav class="bg-white rounded-lg shadow p-4 space-y-1">
          <button
            v-for="tab in tabs"
            :key="tab.id"
            @click="activeTab = tab.id"
            class="w-full flex items-center space-x-3 px-3 py-2 text-left text-sm font-medium rounded-lg transition-colors"
            :class="{
              'bg-blue-100 text-blue-700': activeTab === tab.id,
              'text-gray-600 hover:bg-gray-100 hover:text-gray-900':
                activeTab !== tab.id,
            }"
          >
            <component :is="tab.icon" class="h-5 w-5" />
            <span>{{ tab.name }}</span>
          </button>
        </nav>
      </div>

      <!-- Main Content -->
      <div class="lg:col-span-2">
        <!-- Profile Information Tab -->
        <div
          v-show="activeTab === 'profile'"
          class="bg-white rounded-lg shadow"
        >
          <div class="px-6 py-4 border-b border-gray-200">
            <h2 class="text-lg font-medium text-gray-900">
              Profile Information
            </h2>
            <p class="text-sm text-gray-500">
              Update your personal details and contact information
            </p>
          </div>

          <form @submit.prevent="updateProfile" class="p-6 space-y-6">
            <!-- Avatar Upload -->
            <div class="flex items-center space-x-6">
              <div class="relative">
                <div
                  class="w-20 h-20 bg-gray-200 rounded-full flex items-center justify-center"
                >
                  <UserIcon
                    v-if="!profileForm.avatar"
                    class="h-10 w-10 text-gray-400"
                  />
                  <img
                    v-else
                    :src="profileForm.avatar"
                    alt="Avatar"
                    class="w-20 h-20 rounded-full object-cover"
                  />
                </div>
                <label
                  class="absolute -bottom-2 -right-2 bg-blue-600 hover:bg-blue-700 text-white rounded-full p-2 cursor-pointer"
                >
                  <CameraIcon class="h-4 w-4" />
                  <input
                    type="file"
                    accept="image/*"
                    class="hidden"
                    @change="handleAvatarChange"
                  />
                </label>
              </div>
              <div>
                <h3 class="text-sm font-medium text-gray-900">Profile Photo</h3>
                <p class="text-sm text-gray-500">JPG, GIF or PNG. 2MB max.</p>
              </div>
            </div>

            <!-- Name Fields -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label
                  for="firstName"
                  class="block text-sm font-medium text-gray-700 mb-1"
                >
                  First Name
                </label>
                <input
                  id="firstName"
                  v-model="profileForm.firstName"
                  type="text"
                  class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  :class="{ 'border-red-300': profileErrors.firstName }"
                />
                <p
                  v-if="profileErrors.firstName"
                  class="mt-1 text-sm text-red-600"
                >
                  {{ profileErrors.firstName }}
                </p>
              </div>

              <div>
                <label
                  for="lastName"
                  class="block text-sm font-medium text-gray-700 mb-1"
                >
                  Last Name
                </label>
                <input
                  id="lastName"
                  v-model="profileForm.lastName"
                  type="text"
                  class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  :class="{ 'border-red-300': profileErrors.lastName }"
                />
                <p
                  v-if="profileErrors.lastName"
                  class="mt-1 text-sm text-red-600"
                >
                  {{ profileErrors.lastName }}
                </p>
              </div>
            </div>

            <!-- Email Field -->
            <div>
              <label
                for="email"
                class="block text-sm font-medium text-gray-700 mb-1"
              >
                Email Address
              </label>
              <input
                id="email"
                v-model="profileForm.email"
                type="email"
                class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                :class="{ 'border-red-300': profileErrors.email }"
              />
              <p v-if="profileErrors.email" class="mt-1 text-sm text-red-600">
                {{ profileErrors.email }}
              </p>
            </div>

            <!-- Timezone -->
            <div>
              <label
                for="timezone"
                class="block text-sm font-medium text-gray-700 mb-1"
              >
                Timezone
              </label>
              <select
                id="timezone"
                v-model="profileForm.timezone"
                class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="America/New_York">Eastern Time (UTC-5)</option>
                <option value="America/Chicago">Central Time (UTC-6)</option>
                <option value="America/Denver">Mountain Time (UTC-7)</option>
                <option value="America/Los_Angeles">
                  Pacific Time (UTC-8)
                </option>
                <option value="Europe/London">London (UTC+0)</option>
                <option value="Europe/Paris">Paris (UTC+1)</option>
                <option value="Asia/Tokyo">Tokyo (UTC+9)</option>
              </select>
            </div>

            <div class="flex justify-end">
              <button
                type="submit"
                :disabled="profileLoading"
                class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg disabled:opacity-50 disabled:cursor-not-allowed flex items-center space-x-2"
              >
                <div
                  v-if="profileLoading"
                  class="animate-spin rounded-full h-4 w-4 border-b-2 border-white"
                ></div>
                <span>{{ profileLoading ? "Saving..." : "Save Changes" }}</span>
              </button>
            </div>
          </form>
        </div>

        <!-- Security Tab -->
        <div
          v-show="activeTab === 'security'"
          class="bg-white rounded-lg shadow"
        >
          <div class="px-6 py-4 border-b border-gray-200">
            <h2 class="text-lg font-medium text-gray-900">Security Settings</h2>
            <p class="text-sm text-gray-500">
              Manage your password and security preferences
            </p>
          </div>

          <form @submit.prevent="changePassword" class="p-6 space-y-6">
            <div>
              <label
                for="currentPassword"
                class="block text-sm font-medium text-gray-700 mb-1"
              >
                Current Password
              </label>
              <input
                id="currentPassword"
                v-model="passwordForm.currentPassword"
                type="password"
                required
                class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                :class="{ 'border-red-300': passwordErrors.currentPassword }"
              />
              <p
                v-if="passwordErrors.currentPassword"
                class="mt-1 text-sm text-red-600"
              >
                {{ passwordErrors.currentPassword }}
              </p>
            </div>

            <div>
              <label
                for="newPassword"
                class="block text-sm font-medium text-gray-700 mb-1"
              >
                New Password
              </label>
              <input
                id="newPassword"
                v-model="passwordForm.newPassword"
                type="password"
                required
                class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                :class="{ 'border-red-300': passwordErrors.newPassword }"
              />
              <p
                v-if="passwordErrors.newPassword"
                class="mt-1 text-sm text-red-600"
              >
                {{ passwordErrors.newPassword }}
              </p>
            </div>

            <div>
              <label
                for="confirmNewPassword"
                class="block text-sm font-medium text-gray-700 mb-1"
              >
                Confirm New Password
              </label>
              <input
                id="confirmNewPassword"
                v-model="passwordForm.confirmNewPassword"
                type="password"
                required
                class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                :class="{ 'border-red-300': passwordErrors.confirmNewPassword }"
              />
              <p
                v-if="passwordErrors.confirmNewPassword"
                class="mt-1 text-sm text-red-600"
              >
                {{ passwordErrors.confirmNewPassword }}
              </p>
            </div>

            <div class="flex justify-end">
              <button
                type="submit"
                :disabled="passwordLoading"
                class="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-lg disabled:opacity-50 disabled:cursor-not-allowed flex items-center space-x-2"
              >
                <div
                  v-if="passwordLoading"
                  class="animate-spin rounded-full h-4 w-4 border-b-2 border-white"
                ></div>
                <span>{{
                  passwordLoading ? "Updating..." : "Update Password"
                }}</span>
              </button>
            </div>
          </form>
        </div>

        <!-- Organization Tab -->
        <div
          v-show="activeTab === 'organization'"
          class="bg-white rounded-lg shadow"
        >
          <div class="px-6 py-4 border-b border-gray-200">
            <h2 class="text-lg font-medium text-gray-900">
              Organization Settings
            </h2>
            <p class="text-sm text-gray-500">
              Manage your organization details and billing
            </p>
          </div>

          <form @submit.prevent="updateOrganization" class="p-6 space-y-6">
            <div>
              <label
                for="organizationName"
                class="block text-sm font-medium text-gray-700 mb-1"
              >
                Organization Name
              </label>
              <input
                id="organizationName"
                v-model="orgForm.name"
                type="text"
                required
                class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                :class="{ 'border-red-300': orgErrors.name }"
              />
              <p v-if="orgErrors.name" class="mt-1 text-sm text-red-600">
                {{ orgErrors.name }}
              </p>
            </div>

            <div>
              <label
                for="businessType"
                class="block text-sm font-medium text-gray-700 mb-1"
              >
                Business Type
              </label>
              <select
                id="businessType"
                v-model="orgForm.businessType"
                class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="consulting">Consulting</option>
                <option value="agency">Digital Agency</option>
                <option value="freelancer">Freelancer</option>
                <option value="software">Software Development</option>
                <option value="support">IT Support</option>
                <option value="other">Other</option>
              </select>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label
                  for="defaultHourlyRate"
                  class="block text-sm font-medium text-gray-700 mb-1"
                >
                  Default Hourly Rate ($)
                </label>
                <input
                  id="defaultHourlyRate"
                  v-model.number="orgForm.defaultHourlyRate"
                  type="number"
                  step="0.01"
                  min="0"
                  class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  placeholder="75.00"
                />
              </div>

              <div>
                <label
                  for="currency"
                  class="block text-sm font-medium text-gray-700 mb-1"
                >
                  Currency
                </label>
                <select
                  id="currency"
                  v-model="orgForm.currency"
                  class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                >
                  <option value="USD">USD - US Dollar</option>
                  <option value="EUR">EUR - Euro</option>
                  <option value="GBP">GBP - British Pound</option>
                  <option value="CAD">CAD - Canadian Dollar</option>
                </select>
              </div>
            </div>

            <!-- Current Plan Info -->
            <div class="bg-gray-50 rounded-lg p-4">
              <h3 class="text-sm font-medium text-gray-900 mb-2">
                Current Plan
              </h3>
              <div class="flex items-center justify-between">
                <div>
                  <p class="text-sm text-gray-600">Professional Plan</p>
                  <p class="text-xs text-gray-500">
                    Up to 100 clients, unlimited tickets
                  </p>
                </div>
                <button
                  type="button"
                  class="bg-blue-600 hover:bg-blue-700 text-white px-3 py-1 rounded text-sm"
                >
                  Upgrade
                </button>
              </div>
            </div>

            <div class="flex justify-end">
              <button
                type="submit"
                :disabled="orgLoading"
                class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg disabled:opacity-50 disabled:cursor-not-allowed flex items-center space-x-2"
              >
                <div
                  v-if="orgLoading"
                  class="animate-spin rounded-full h-4 w-4 border-b-2 border-white"
                ></div>
                <span>{{ orgLoading ? "Saving..." : "Save Changes" }}</span>
              </button>
            </div>
          </form>
        </div>

        <!-- Notifications Tab -->
        <div
          v-show="activeTab === 'notifications'"
          class="bg-white rounded-lg shadow"
        >
          <div class="px-6 py-4 border-b border-gray-200">
            <h2 class="text-lg font-medium text-gray-900">
              Notification Preferences
            </h2>
            <p class="text-sm text-gray-500">
              Choose how you want to be notified about activities
            </p>
          </div>

          <form @submit.prevent="updateNotifications" class="p-6 space-y-6">
            <!-- Email Notifications -->
            <div>
              <h3 class="text-sm font-medium text-gray-900 mb-4">
                Email Notifications
              </h3>
              <div class="space-y-3">
                <div class="flex items-center justify-between">
                  <div>
                    <p class="text-sm font-medium text-gray-700">New Tickets</p>
                    <p class="text-xs text-gray-500">
                      When a new support ticket is created
                    </p>
                  </div>
                  <input
                    v-model="notificationForm.emailNewTickets"
                    type="checkbox"
                    class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                  />
                </div>

                <div class="flex items-center justify-between">
                  <div>
                    <p class="text-sm font-medium text-gray-700">
                      Ticket Comments
                    </p>
                    <p class="text-xs text-gray-500">
                      When someone comments on a ticket
                    </p>
                  </div>
                  <input
                    v-model="notificationForm.emailTicketComments"
                    type="checkbox"
                    class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                  />
                </div>

                <div class="flex items-center justify-between">
                  <div>
                    <p class="text-sm font-medium text-gray-700">
                      Overdue Tickets
                    </p>
                    <p class="text-xs text-gray-500">
                      Daily digest of overdue tickets
                    </p>
                  </div>
                  <input
                    v-model="notificationForm.emailOverdueTickets"
                    type="checkbox"
                    class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                  />
                </div>
              </div>
            </div>

            <!-- Browser Notifications -->
            <div>
              <h3 class="text-sm font-medium text-gray-900 mb-4">
                Browser Notifications
              </h3>
              <div class="space-y-3">
                <div class="flex items-center justify-between">
                  <div>
                    <p class="text-sm font-medium text-gray-700">
                      Real-time Updates
                    </p>
                    <p class="text-xs text-gray-500">
                      Show browser notifications for updates
                    </p>
                  </div>
                  <input
                    v-model="notificationForm.browserNotifications"
                    type="checkbox"
                    class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                  />
                </div>
              </div>
            </div>

            <div class="flex justify-end">
              <button
                type="submit"
                :disabled="notificationLoading"
                class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg disabled:opacity-50 disabled:cursor-not-allowed flex items-center space-x-2"
              >
                <div
                  v-if="notificationLoading"
                  class="animate-spin rounded-full h-4 w-4 border-b-2 border-white"
                ></div>
                <span>{{
                  notificationLoading ? "Saving..." : "Save Preferences"
                }}</span>
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useAuthStore } from "../stores/auth";
import { useNotificationStore } from "../stores/notifications";
import {
  UserIcon,
  CameraIcon,
  ShieldCheckIcon,
  BuildingOfficeIcon,
  BellIcon,
} from "@heroicons/vue/24/outline";

const authStore = useAuthStore();
const notificationStore = useNotificationStore();

// State
const activeTab = ref("profile");
const profileLoading = ref(false);
const passwordLoading = ref(false);
const orgLoading = ref(false);
const notificationLoading = ref(false);

const tabs = [
  { id: "profile", name: "Profile", icon: UserIcon },
  { id: "security", name: "Security", icon: ShieldCheckIcon },
  { id: "organization", name: "Organization", icon: BuildingOfficeIcon },
  { id: "notifications", name: "Notifications", icon: BellIcon },
];

// Forms
const profileForm = ref({
  firstName: "",
  lastName: "",
  email: "",
  avatar: "",
  timezone: "America/New_York",
});

const passwordForm = ref({
  currentPassword: "",
  newPassword: "",
  confirmNewPassword: "",
});

const orgForm = ref({
  name: "",
  businessType: "consulting",
  defaultHourlyRate: 75,
  currency: "USD",
});

const notificationForm = ref({
  emailNewTickets: true,
  emailTicketComments: true,
  emailOverdueTickets: true,
  browserNotifications: false,
});

// Errors
const profileErrors = ref<Record<string, string>>({});
const passwordErrors = ref<Record<string, string>>({});
const orgErrors = ref<Record<string, string>>({});

// Methods
const handleAvatarChange = (event: Event) => {
  const file = (event.target as HTMLInputElement).files?.[0];
  if (file) {
    // In a real app, you'd upload this to a server
    const reader = new FileReader();
    reader.onload = (e) => {
      profileForm.value.avatar = e.target?.result as string;
    };
    reader.readAsDataURL(file);
  }
};

const updateProfile = async () => {
  profileLoading.value = true;
  profileErrors.value = {};

  try {
    // Mock update - replace with real API call
    await new Promise((resolve) => setTimeout(resolve, 1000));

    notificationStore.success("Profile updated successfully!");
  } catch (error) {
    notificationStore.error("Failed to update profile");
  } finally {
    profileLoading.value = false;
  }
};

const changePassword = async () => {
  passwordLoading.value = true;
  passwordErrors.value = {};

  // Validate passwords match
  if (
    passwordForm.value.newPassword !== passwordForm.value.confirmNewPassword
  ) {
    passwordErrors.value.confirmNewPassword = "Passwords do not match";
    passwordLoading.value = false;
    return;
  }

  try {
    // Mock password change - replace with real API call
    await new Promise((resolve) => setTimeout(resolve, 1000));

    // Reset form
    passwordForm.value = {
      currentPassword: "",
      newPassword: "",
      confirmNewPassword: "",
    };

    notificationStore.success("Password updated successfully!");
  } catch (error) {
    notificationStore.error("Failed to update password");
  } finally {
    passwordLoading.value = false;
  }
};

const updateOrganization = async () => {
  orgLoading.value = true;
  orgErrors.value = {};

  try {
    // Mock update - replace with real API call
    await new Promise((resolve) => setTimeout(resolve, 1000));

    notificationStore.success("Organization settings updated!");
  } catch (error) {
    notificationStore.error("Failed to update organization");
  } finally {
    orgLoading.value = false;
  }
};

const updateNotifications = async () => {
  notificationLoading.value = true;

  try {
    // Mock update - replace with real API call
    await new Promise((resolve) => setTimeout(resolve, 500));

    notificationStore.success("Notification preferences saved!");
  } catch (error) {
    notificationStore.error("Failed to update preferences");
  } finally {
    notificationLoading.value = false;
  }
};

// Load initial data
onMounted(() => {
  if (authStore.currentUser) {
    profileForm.value = {
      firstName: authStore.currentUser.firstName || "",
      lastName: authStore.currentUser.lastName || "",
      email: authStore.currentUser.email,
      avatar: "",
      timezone: "America/New_York",
    };

    orgForm.value = {
      name: authStore.currentUser.tenantName,
      businessType: "consulting",
      defaultHourlyRate: 75,
      currency: "USD",
    };
  }
});
</script>
