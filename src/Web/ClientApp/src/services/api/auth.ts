import { apiClient } from './client';
import type {
  User,
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  RegisterResponse,
  RefreshTokenResponse,
} from '../../types/auth';

export const authApi = {
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    return await apiClient.post<LoginResponse>('/auth/login', credentials);
  },

  async register(userData: RegisterRequest): Promise<RegisterResponse> {
    return await apiClient.post<RegisterResponse>('/auth/register', userData);
  },

  // Logout user
  async logout(): Promise<void> {
    return await apiClient.post<void>('/auth/logout');
  },

  // Get current user profile
  async getCurrentUser(): Promise<User> {
    return await apiClient.get<User>('/auth/me');
  },

  // Refresh access token
  async refreshToken(): Promise<RefreshTokenResponse> {
    return await apiClient.post<RefreshTokenResponse>('/auth/refresh');
  },

  // Update user profile
  async updateProfile(data: Partial<User>): Promise<User> {
    return await apiClient.put<User>('/auth/profile', data);
  },

  // Change password
  async changePassword(data: {
    currentPassword: string;
    newPassword: string;
    confirmPassword: string;
  }): Promise<void> {
    return await apiClient.post<void>('/auth/change-password', data);
  },
};
