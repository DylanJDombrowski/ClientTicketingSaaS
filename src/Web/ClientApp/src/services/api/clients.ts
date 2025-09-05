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

  // Get single client by ID - NOW IMPLEMENTED
  async getClient(id: number): Promise<Client> {
    return await apiClient.get<Client>(`/clients/${id}`);
  },

  // Create new client - FIXED
  async createClient(data: CreateClientRequest): Promise<Client> {
    const response = await apiClient.post<{ id: number }>('/clients', data);
    // Now we can fetch the created client since GET single client works
    return await this.getClient(response.id);
  },

  // Update existing client - NOW IMPLEMENTED
  async updateClient(id: number, data: UpdateClientRequest): Promise<Client> {
    // Your API expects the ID in the body for validation
    const updateData = { ...data, id };
    await apiClient.put<void>(`/clients/${id}`, updateData);
    return await this.getClient(id);
  },

  // Delete client - NOW IMPLEMENTED
  async deleteClient(id: number): Promise<void> {
    return await apiClient.delete<void>(`/clients/${id}`);
  },

  // Search clients (future enhancement)
  async searchClients(query: string): Promise<Client[]> {
    throw new Error('Search clients not implemented yet');
  },
};
