import { defineStore } from 'pinia';
import { ref } from 'vue';

export interface Notification {
  id: string;
  type: 'success' | 'error' | 'warning' | 'info';
  message: string;
  duration?: number;
}

export const useNotificationStore = defineStore('notifications', () => {
  // State
  const notifications = ref<Notification[]>([]);

  // Actions
  const addNotification = (notification: Omit<Notification, 'id'>) => {
    const id = Math.random().toString(36).substr(2, 9);
    const newNotification: Notification = {
      ...notification,
      id,
      duration: notification.duration || 5000,
    };

    notifications.value.push(newNotification);

    // Auto-remove notification after duration
    setTimeout(() => {
      removeNotification(id);
    }, newNotification.duration);

    return id;
  };

  const removeNotification = (id: string) => {
    const index = notifications.value.findIndex((n) => n.id === id);
    if (index > -1) {
      notifications.value.splice(index, 1);
    }
  };

  const clearAll = () => {
    notifications.value = [];
  };

  // Convenience methods
  const success = (message: string, duration?: number) => {
    return addNotification({ type: 'success', message, duration });
  };

  const error = (message: string, duration?: number) => {
    return addNotification({ type: 'error', message, duration });
  };

  const warning = (message: string, duration?: number) => {
    return addNotification({ type: 'warning', message, duration });
  };

  const info = (message: string, duration?: number) => {
    return addNotification({ type: 'info', message, duration });
  };

  return {
    // State
    notifications,

    // Actions
    addNotification,
    removeNotification,
    clearAll,

    // Convenience methods
    success,
    error,
    warning,
    info,
  };
});
