// Replace src/Web/ClientApp/src/services/api/tickets.ts with this:

import { apiClient } from './client';
import type {
  Ticket,
  CreateTicketRequest,
  UpdateTicketRequest,
  TicketComment,
  CreateCommentRequest,
  TimeEntry,
  CreateTimeEntryRequest,
  TicketFilters,
} from '../../types/ticket';

export const ticketsApi = {
  // Get all tickets for current tenant
  async getTickets(filters?: TicketFilters): Promise<Ticket[]> {
    const params = new URLSearchParams();

    if (filters?.status) params.append('status', filters.status.toString());
    if (filters?.priority)
      params.append('priority', filters.priority.toString());
    if (filters?.clientId)
      params.append('clientId', filters.clientId.toString());
    if (filters?.assignedTo) params.append('assignedToId', filters.assignedTo);

    const queryString = params.toString();
    const url = queryString ? `/tickets?${queryString}` : '/tickets';

    return await apiClient.get<Ticket[]>(url);
  },

  // Get single ticket by ID with comments and time entries
  async getTicket(id: number): Promise<Ticket> {
    return await apiClient.get<Ticket>(`/tickets/${id}`);
  },

  // Create new ticket
  async createTicket(data: CreateTicketRequest): Promise<Ticket> {
    const response = await apiClient.post<{ id: number }>('/tickets', data);
    // Fetch the created ticket to return full data
    return await this.getTicket(response.id);
  },

  // Update existing ticket
  async updateTicket(id: number, data: UpdateTicketRequest): Promise<Ticket> {
    await apiClient.put<void>(`/tickets/${id}`, data);
    return await this.getTicket(id);
  },

  // Update just the ticket status
  async updateTicketStatus(id: number, status: number): Promise<Ticket> {
    await apiClient.patch<void>(`/tickets/${id}/status`, { status });
    return await this.getTicket(id);
  },

  // Delete ticket
  async deleteTicket(id: number): Promise<void> {
    return await apiClient.delete<void>(`/tickets/${id}`);
  },

  // Add comment to ticket
  async addComment(
    ticketId: number,
    data: CreateCommentRequest
  ): Promise<TicketComment> {
    // Note: This endpoint doesn't exist yet in your backend
    throw new Error('Add comment endpoint not implemented yet');
  },

  // Add time entry to ticket
  async addTimeEntry(data: CreateTimeEntryRequest): Promise<TimeEntry> {
    const response = await apiClient.post<{ id: number }>(
      '/time-entries',
      data
    );

    // Return properly typed object matching TimeEntry interface
    return {
      id: response.id,
      description: data.description,
      hours: data.hours,
      startTime: data.startTime,
      endTime: data.endTime,
      isBillable: data.isBillable,
      created: new Date().toISOString(), // Fixed: use 'created' not 'createdAt'
      createdBy: 'Current User',
    };
  },

  // Get time entries for a ticket
  async getTimeEntries(ticketId: number): Promise<TimeEntry[]> {
    return await apiClient.get<TimeEntry[]>(`/time-entries/ticket/${ticketId}`);
  },

  // Search tickets
  async searchTickets(query: string): Promise<Ticket[]> {
    return await apiClient.get<Ticket[]>('/tickets', { search: query });
  },
};
