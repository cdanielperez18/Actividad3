export interface Task {
  id: number;
  title: string;
  description: string;
  isCompleted: boolean;
  createdAt: string;
  completedAt: string | null;
  userId: number;
}

export interface CreateTaskRequest {
  title: string;
  description: string;
}

export interface UpdateTaskRequest {
  title?: string;
  description?: string;
  isCompleted?: boolean;
}
