import { defineStore } from 'pinia';
import { ref } from 'vue';
import type { Client, CreateClientRequest } from '../types/client';
import { clientsApi } from '../services/api/clients';

export const useClientsStore = defineStore('clients', () => {
  // State
  const clients = ref<Client[]>([]);
  const selectedClient = ref<Client | null>(null);
  const loading = ref(false);
  const error = ref<string | null>(null);

  // Actions
  const fetchClients = async () => {
    loading.value = true;
    error.value = null;

    try {
      const response = await clientsApi.getClients();
      clients.value = response;
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to fetch clients';
      console.error('Fetch clients error:', err);
    } finally {
      loading.value = false;
    }
  };

  const fetchClient = async (id: number) => {
    loading.value = true;
    error.value = null;

    try {
      const client = await clientsApi.getClient(id);
      selectedClient.value = client;
      return client;
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to fetch client';
      console.error('Fetch client error:', err);
      return null;
    } finally {
      loading.value = false;
    }
  };

  const createClient = async (clientData: CreateClientRequest) => {
    loading.value = true;
    error.value = null;

    try {
      const newClient = await clientsApi.createClient(clientData);

      // Add to local state
      clients.value.push(newClient);

      return { success: true, client: newClient };
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to create client';
      console.error('Create client error:', err);
      return { success: false, error: error.value };
    } finally {
      loading.value = false;
    }
  };

  const updateClient = async (id: number, clientData: Partial<Client>) => {
    loading.value = true;
    error.value = null;

    try {
      const updatedClient = await clientsApi.updateClient(id, clientData);

      // Update local state
      const index = clients.value.findIndex((c) => c.id === id);
      if (index !== -1) {
        clients.value[index] = updatedClient;
      }

      if (selectedClient.value?.id === id) {
        selectedClient.value = updatedClient;
      }

      return { success: true, client: updatedClient };
    } catch (err: any) {
      if (
        err.response?.status === 400 &&
        err.response?.data?.includes('ID mismatch')
      ) {
        error.value = 'Invalid client ID';
      } else {
        error.value = err.response?.data?.message || 'Failed to update client';
      }
      console.error('Update client error:', err);
      return { success: false, error: error.value };
    } finally {
      loading.value = false;
    }
  };

  const deleteClient = async (id: number) => {
    loading.value = true;
    error.value = null;

    try {
      await clientsApi.deleteClient(id);

      // Remove from local state
      clients.value = clients.value.filter((c) => c.id !== id);

      if (selectedClient.value?.id === id) {
        selectedClient.value = null;
      }

      return { success: true };
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to delete client';
      console.error('Delete client error:', err);
      return { success: false, error: error.value };
    } finally {
      loading.value = false;
    }
  };

  const clearError = () => {
    error.value = null;
  };

  const clearSelectedClient = () => {
    selectedClient.value = null;
  };

  return {
    // State
    clients,
    selectedClient,
    loading,
    error,

    // Actions
    fetchClients,
    fetchClient,
    createClient,
    updateClient,
    deleteClient,
    clearError,
    clearSelectedClient,
  };
});
