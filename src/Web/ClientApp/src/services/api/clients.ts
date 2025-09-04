import { apiClient } from './client';
import type {
  Client,
  CreateClientRequest,
  UpdateClientRequest,
} from '../../types/client';

export const clientsApi = {
  // Get all clients for current tenant
  async getClients(): Promise<Client[]> {
    return await apiClient.get<Client[]>('/clients');
  },

  // Get single client by ID
  async getClient(id: number): Promise<Client> {
    return await apiClient.get<Client>(`/clients/${id}`);
  },

  // Create new client
  async createClient(data: CreateClientRequest): Promise<Client> {
    // The API returns the ID, but we need to fetch the full client
    const id = await apiClient.post<number>('/clients', data);
    return await this.getClient(id);
  },

  // Update existing client
  async updateClient(id: number, data: UpdateClientRequest): Promise<Client> {
    await apiClient.put<void>(`/clients/${id}`, data);
    return await this.getClient(id);
  },

  // Delete client
  async deleteClient(id: number): Promise<void> {
    return await apiClient.delete<void>(`/clients/${id}`);
  },

  // Search clients (future enhancement)
  async searchClients(query: string): Promise<Client[]> {
    return await apiClient.get<Client[]>('/clients/search', { q: query });
  },
};
