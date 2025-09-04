export interface User {
  id: string;
  email: string;
  role: string;
  tenantId: string;
  tenantName: string;
  firstName?: string;
  lastName?: string;
  isActive: boolean;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: User;
}

export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
  tenantName: string;
  firstName?: string;
  lastName?: string;
}

export interface RegisterResponse {
  token: string;
  user: User;
}

export interface RefreshTokenResponse {
  token: string;
}
