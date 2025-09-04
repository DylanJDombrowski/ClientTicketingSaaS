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
    return await apiClient.get<Ticket[]>('/tickets', filters);
  },

  // Get single ticket by ID with comments and time entries
  async getTicket(id: number): Promise<Ticket> {
    return await apiClient.get<Ticket>(`/tickets/${id}`);
  },

  // Create new ticket
  async createTicket(data: CreateTicketRequest): Promise<Ticket> {
    // Mock implementation for now - will be replaced with real API
    const mockTicket: Ticket = {
      id: Math.floor(Math.random() * 10000),
      ...data,
      status: 'open',
      clientName: 'Mock Client', // Will come from client lookup
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
      actualHours: 0,
      comments: [],
      timeEntries: [],
    };

    // Simulate API delay
    await new Promise((resolve) => setTimeout(resolve, 500));
    return mockTicket;
  },

  // Update existing ticket
  async updateTicket(id: number, data: UpdateTicketRequest): Promise<Ticket> {
    // Mock implementation - in real API this would be:
    // await apiClient.put<void>(`/tickets/${id}`, data)
    // return await this.getTicket(id)

    // For now, return mock updated ticket
    const mockTicket: Ticket = {
      id,
      title: data.title || 'Updated Ticket',
      description: data.description || 'Updated description',
      clientId: 1,
      clientName: 'Mock Client',
      status: data.status || 'open',
      priority: data.priority || 'medium',
      assignedTo: data.assignedTo,
      dueDate: data.dueDate,
      estimatedHours: data.estimatedHours,
      actualHours: 0,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
      tags: data.tags,
      comments: [],
      timeEntries: [],
    };

    await new Promise((resolve) => setTimeout(resolve, 300));
    return mockTicket;
  },

  // Delete ticket
  async deleteTicket(id: number): Promise<void> {
    // Mock implementation
    await new Promise((resolve) => setTimeout(resolve, 300));
    return;
  },

  // Add comment to ticket
  async addComment(
    ticketId: number,
    data: CreateCommentRequest
  ): Promise<TicketComment> {
    // Mock implementation
    const mockComment: TicketComment = {
      id: Math.floor(Math.random() * 10000),
      ticketId,
      ...data,
      authorName: 'Current User', // Will come from auth context
      createdAt: new Date().toISOString(),
    };

    await new Promise((resolve) => setTimeout(resolve, 300));
    return mockComment;
  },

  // Add time entry to ticket
  async addTimeEntry(data: CreateTimeEntryRequest): Promise<TimeEntry> {
    // Mock implementation
    const mockTimeEntry: TimeEntry = {
      id: Math.floor(Math.random() * 10000),
      ...data,
      createdBy: 'Current User', // Will come from auth context
      createdAt: new Date().toISOString(),
    };

    await new Promise((resolve) => setTimeout(resolve, 300));
    return mockTimeEntry;
  },

  // Get time entries for a ticket
  async getTimeEntries(ticketId: number): Promise<TimeEntry[]> {
    return await apiClient.get<TimeEntry[]>(
      `/tickets/${ticketId}/time-entries`
    );
  },

  // Update time entry
  async updateTimeEntry(
    id: number,
    data: Partial<TimeEntry>
  ): Promise<TimeEntry> {
    await apiClient.put<void>(`/time-entries/${id}`, data);
    return await apiClient.get<TimeEntry>(`/time-entries/${id}`);
  },

  // Delete time entry
  async deleteTimeEntry(id: number): Promise<void> {
    return await apiClient.delete<void>(`/time-entries/${id}`);
  },

  // Search tickets
  async searchTickets(query: string): Promise<Ticket[]> {
    return await apiClient.get<Ticket[]>('/tickets/search', { q: query });
  },
};
