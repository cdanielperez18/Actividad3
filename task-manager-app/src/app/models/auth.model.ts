export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface AuthResponse {
  userId: number;
  username: string;
  email: string;
  token: string;
}

export interface User {
  userId: number;
  username: string;
  email: string;
}
