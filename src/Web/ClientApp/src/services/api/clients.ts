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

  // Get single client by ID (not implemented yet)
  async getClient(id: number): Promise<Client> {
    throw new Error('Get single client not implemented yet');
  },

  // Create new client - Fixed to work with current API
  async createClient(data: CreateClientRequest): Promise<Client> {
    const response = await apiClient.post<{ id: number }>('/clients', data);

    // Return a client object with the new ID and submitted data
    return {
      id: response.id,
      name: data.name,
      email: data.email,
      phone: data.phone,
      company: data.company,
      isActive: true,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    };
  },

  // Update existing client (not implemented yet)
  async updateClient(id: number, data: UpdateClientRequest): Promise<Client> {
    throw new Error('Update client not implemented yet');
  },

  // Delete client (not implemented yet)
  async deleteClient(id: number): Promise<void> {
    throw new Error('Delete client not implemented yet');
  },

  // Search clients (not implemented yet)
  async searchClients(query: string): Promise<Client[]> {
    throw new Error('Search clients not implemented yet');
  },
};
