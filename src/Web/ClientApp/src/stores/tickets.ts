import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import type {
  Ticket,
  CreateTicketRequest,
  UpdateTicketRequest,
  TicketFilters,
  TicketStatus,
  TicketPriority,
  TicketComment,
  CreateCommentRequest,
  TimeEntry,
  CreateTimeEntryRequest,
} from '../types/ticket';
import { ticketsApi } from '../services/api/tickets';

export const useTicketsStore = defineStore('tickets', () => {
  // State
  const tickets = ref<Ticket[]>([]);
  const selectedTicket = ref<Ticket | null>(null);
  const loading = ref(false);
  const error = ref<string | null>(null);
  const filters = ref<TicketFilters>({});

  // Computed
  const filteredTickets = computed(() => {
    let result = tickets.value;

    if (filters.value.status) {
      result = result.filter((t) => t.status === filters.value.status);
    }
    if (filters.value.priority) {
      result = result.filter((t) => t.priority === filters.value.priority);
    }
    if (filters.value.clientId) {
      result = result.filter((t) => t.clientId === filters.value.clientId);
    }
    if (filters.value.search) {
      const search = filters.value.search.toLowerCase();
      result = result.filter(
        (t) =>
          t.title.toLowerCase().includes(search) ||
          t.description.toLowerCase().includes(search) ||
          t.clientName.toLowerCase().includes(search)
      );
    }

    return result;
  });

  const ticketsByStatus = computed(() => ({
    open: tickets.value.filter((t) => t.status === 'open'),
    inProgress: tickets.value.filter((t) => t.status === 'in_progress'),
    completed: tickets.value.filter((t) => t.status === 'completed'),
  }));

  const ticketStats = computed(() => ({
    total: tickets.value.length,
    open: ticketsByStatus.value.open.length,
    inProgress: ticketsByStatus.value.inProgress.length,
    completed: ticketsByStatus.value.completed.length,
    overdue: tickets.value.filter(
      (t) =>
        t.dueDate &&
        new Date(t.dueDate) < new Date() &&
        t.status !== 'completed'
    ).length,
  }));

  // Actions
  const fetchTickets = async () => {
    loading.value = true;
    error.value = null;

    try {
      const response = await ticketsApi.getTickets();
      tickets.value = response;
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to fetch tickets';
      console.error('Fetch tickets error:', err);
    } finally {
      loading.value = false;
    }
  };

  const fetchTicket = async (id: number) => {
    loading.value = true;
    error.value = null;

    try {
      const ticket = await ticketsApi.getTicket(id);
      selectedTicket.value = ticket;

      // Update in list if it exists
      const index = tickets.value.findIndex((t) => t.id === id);
      if (index !== -1) {
        tickets.value[index] = ticket;
      }

      return ticket;
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to fetch ticket';
      console.error('Fetch ticket error:', err);
      return null;
    } finally {
      loading.value = false;
    }
  };

  const createTicket = async (ticketData: CreateTicketRequest) => {
    loading.value = true;
    error.value = null;

    try {
      const newTicket = await ticketsApi.createTicket(ticketData);

      // Add to local state
      tickets.value.unshift(newTicket);

      return { success: true, ticket: newTicket };
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to create ticket';
      console.error('Create ticket error:', err);
      return { success: false, error: error.value };
    } finally {
      loading.value = false;
    }
  };

  const updateTicket = async (id: number, ticketData: UpdateTicketRequest) => {
    loading.value = true;
    error.value = null;

    try {
      const updatedTicket = await ticketsApi.updateTicket(id, ticketData);

      // Update local state
      const index = tickets.value.findIndex((t) => t.id === id);
      if (index !== -1) {
        tickets.value[index] = updatedTicket;
      }

      if (selectedTicket.value?.id === id) {
        selectedTicket.value = updatedTicket;
      }

      return { success: true, ticket: updatedTicket };
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to update ticket';
      console.error('Update ticket error:', err);
      return { success: false, error: error.value };
    } finally {
      loading.value = false;
    }
  };

  const updateTicketStatus = async (id: number, status: TicketStatus) => {
    return updateTicket(id, { status });
  };

  const deleteTicket = async (id: number) => {
    loading.value = true;
    error.value = null;

    try {
      await ticketsApi.deleteTicket(id);

      // Remove from local state
      tickets.value = tickets.value.filter((t) => t.id !== id);

      if (selectedTicket.value?.id === id) {
        selectedTicket.value = null;
      }

      return { success: true };
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to delete ticket';
      console.error('Delete ticket error:', err);
      return { success: false, error: error.value };
    } finally {
      loading.value = false;
    }
  };

  // Comments
  const addComment = async (
    ticketId: number,
    commentData: CreateCommentRequest
  ) => {
    loading.value = true;
    error.value = null;

    try {
      const comment = await ticketsApi.addComment(ticketId, commentData);

      // Add to selected ticket if it's loaded
      if (selectedTicket.value?.id === ticketId) {
        if (!selectedTicket.value.comments) {
          selectedTicket.value.comments = [];
        }
        selectedTicket.value.comments.push(comment);
      }

      return { success: true, comment };
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to add comment';
      console.error('Add comment error:', err);
      return { success: false, error: error.value };
    } finally {
      loading.value = false;
    }
  };

  // Time Entries
  const addTimeEntry = async (timeEntryData: CreateTimeEntryRequest) => {
    loading.value = true;
    error.value = null;

    try {
      const timeEntry = await ticketsApi.addTimeEntry(timeEntryData);

      // Add to selected ticket if it's loaded
      if (selectedTicket.value?.id === timeEntryData.ticketId) {
        if (!selectedTicket.value.timeEntries) {
          selectedTicket.value.timeEntries = [];
        }
        selectedTicket.value.timeEntries.push(timeEntry);

        // Update actual hours
        const totalHours = selectedTicket.value.timeEntries.reduce(
          (sum, entry) => sum + entry.hours,
          0
        );
        selectedTicket.value.actualHours = totalHours;
      }

      return { success: true, timeEntry };
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to add time entry';
      console.error('Add time entry error:', err);
      return { success: false, error: error.value };
    } finally {
      loading.value = false;
    }
  };

  // Utility actions
  const setFilters = (newFilters: TicketFilters) => {
    filters.value = { ...filters.value, ...newFilters };
  };

  const clearFilters = () => {
    filters.value = {};
  };

  const clearError = () => {
    error.value = null;
  };

  const clearSelectedTicket = () => {
    selectedTicket.value = null;
  };

  return {
    // State
    tickets,
    selectedTicket,
    loading,
    error,
    filters,

    // Computed
    filteredTickets,
    ticketsByStatus,
    ticketStats,

    // Actions
    fetchTickets,
    fetchTicket,
    createTicket,
    updateTicket,
    updateTicketStatus,
    deleteTicket,
    addComment,
    addTimeEntry,
    setFilters,
    clearFilters,
    clearError,
    clearSelectedTicket,
  };
});
