// Replace your entire src/Web/ClientApp/src/types/ticket.ts with this:

export interface Ticket {
  id: number;
  title: string;
  description: string;
  clientId: number;
  clientName: string;
  clientCompany?: string;
  status: number; // Backend sends integers
  priority: number; // Backend sends integers
  assignedToId?: string;
  created: string; // Backend uses 'created', not 'createdAt'
  lastModified: string; // Backend uses 'lastModified', not 'updatedAt'
  dueDate?: string;
  estimatedHours: number;
  actualHours: number;
  comments?: TicketComment[];
  timeEntries?: TimeEntry[];
}

export interface CreateTicketRequest {
  title: string;
  description: string;
  clientId: number;
  priority: number; // Send as integer
  dueDate?: string;
  estimatedHours?: number;
}

export interface UpdateTicketRequest {
  title: string;
  description: string;
  status: number; // Send as integer
  priority: number; // Send as integer
  assignedToId?: string;
  dueDate?: string;
  estimatedHours: number;
}

export interface TicketComment {
  id: number;
  comment: string; // Backend uses 'comment', not 'content'
  isInternal: boolean;
  created: string; // Backend uses 'created'
  createdBy?: string;
}

export interface CreateCommentRequest {
  comment: string; // Backend expects 'comment'
  isInternal: boolean;
}

export interface TimeEntry {
  id: number;
  description: string;
  hours: number;
  startTime: string;
  endTime?: string;
  isBillable: boolean;
  created: string; // Backend uses 'created'
  createdBy: string;
}

export interface CreateTimeEntryRequest {
  ticketId: number;
  description: string;
  hours: number;
  startTime: string;
  endTime?: string;
  isBillable: boolean;
}

// Keep these enums for frontend display purposes
export enum TicketStatus {
  Open = 1,
  InProgress = 2,
  PendingReview = 3,
  Resolved = 4,
  Closed = 5,
  Cancelled = 6,
}

export enum TicketPriority {
  Low = 1,
  Medium = 2,
  High = 3,
  Urgent = 4,
}

export interface TicketFilters {
  status?: number; // Send as integer
  priority?: number; // Send as integer
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
