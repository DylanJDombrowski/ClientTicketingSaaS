export interface Ticket {
  id: number;
  title: string;
  description: string;
  clientId: number;
  clientName: string;
  status: TicketStatus;
  priority: TicketPriority;
  assignedTo?: string;
  createdAt: string;
  updatedAt: string;
  dueDate?: string;
  estimatedHours?: number;
  actualHours?: number;
  tags?: string[];
  comments?: TicketComment[];
  timeEntries?: TimeEntry[];
}

export interface CreateTicketRequest {
  title: string;
  description: string;
  clientId: number;
  priority: TicketPriority;
  dueDate?: string;
  estimatedHours?: number;
  tags?: string[];
}

export interface UpdateTicketRequest {
  title?: string;
  description?: string;
  status?: TicketStatus;
  priority?: TicketPriority;
  assignedTo?: string;
  dueDate?: string;
  estimatedHours?: number;
  tags?: string[];
}

export interface TicketComment {
  id: number;
  ticketId: number;
  content: string;
  authorName: string;
  createdAt: string;
  isInternal: boolean;
}

export interface CreateCommentRequest {
  content: string;
  isInternal: boolean;
}

export interface TimeEntry {
  id: number;
  ticketId: number;
  description: string;
  hours: number;
  date: string;
  billable: boolean;
  hourlyRate?: number;
  createdBy: string;
  createdAt: string;
}

export interface CreateTimeEntryRequest {
  ticketId: number;
  description: string;
  hours: number;
  date: string;
  billable: boolean;
  hourlyRate?: number;
}

export type TicketStatus = 'open' | 'in_progress' | 'completed';
export type TicketPriority = 'low' | 'medium' | 'high';

export interface TicketFilters {
  status?: TicketStatus;
  priority?: TicketPriority;
  clientId?: number;
  assignedTo?: string;
  search?: string;
  dateFrom?: string;
  dateTo?: string;
}

export interface TicketStats {
  total: number;
  open: number;
  inProgress: number;
  completed: number;
  overdue: number;
}
