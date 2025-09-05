export interface Client {
  id: number;
  name: string;
  email: string;
  phone?: string;
  company?: string;
  isActive: boolean;
  createdAt?: string;
  updatedAt?: string;
}

export interface CreateClientRequest {
  name: string;
  email: string;
  phone?: string;
  company?: string;
}

export interface UpdateClientRequest {
  name?: string;
  email?: string;
  phone?: string;
  company?: string;
  isActive?: boolean;
}

// Add this for validation errors
export interface ValidationError {
  propertyName: string;
  errorMessage: string;
  errorCode?: string;
}
