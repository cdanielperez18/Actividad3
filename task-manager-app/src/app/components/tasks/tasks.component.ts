import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskService } from '../../services/task.service';
import { Task, CreateTaskRequest, UpdateTaskRequest } from '../../models/task.model';


@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './tasks.component.html',
  styleUrl: './tasks.component.css'
})
export class TasksComponent implements OnInit {
  tasks: Task[] = [];
  isLoading = false;
  errorMessage = '';

  showModal = false;
  isEditing = false;
  currentTaskId: number | null = null;
  taskTitle = '';
  taskDescription = '';

  showDeleteModal = false;
  taskToDelete: Task | null = null;

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.taskService.getAllTasks().subscribe({
      next: (tasks) => {
        this.tasks = tasks;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error al cargar tareas:', error);
        this.errorMessage = 'Error al cargar las tareas';
        this.isLoading = false;
      }
    });
  }

  openCreateModal(): void {
    this.isEditing = false;
    this.currentTaskId = null;
    this.taskTitle = '';
    this.taskDescription = '';
    this.showModal = true;
  }

  openEditModal(task: Task): void {
    this.isEditing = true;
    this.currentTaskId = task.id;
    this.taskTitle = task.title;
    this.taskDescription = task.description;
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.taskTitle = '';
    this.taskDescription = '';
    this.currentTaskId = null;
  }

  saveTask(): void {
    if (!this.taskTitle.trim()) {
      alert('El título es obligatorio');
      return;
    }

    if (this.isEditing && this.currentTaskId) {
      this.updateTask();
    } else {
      this.createTask();
    }
  }

  createTask(): void {
    const newTask: CreateTaskRequest = {
      title: this.taskTitle,
      description: this.taskDescription
    };

    this.taskService.createTask(newTask).subscribe({
      next: (task) => {
        this.tasks.unshift(task);
        this.closeModal();
      },
      error: (error) => {
        console.error('Error al crear tarea:', error);
        alert('Error al crear la tarea');
      }
    });
  }

  updateTask(): void {
    if (!this.currentTaskId) return;

    const updateData: UpdateTaskRequest = {
      title: this.taskTitle,
      description: this.taskDescription
    };

    this.taskService.updateTask(this.currentTaskId, updateData).subscribe({
      next: (updatedTask) => {
        const index = this.tasks.findIndex(t => t.id === this.currentTaskId);
        if (index !== -1) {
          this.tasks[index] = updatedTask;
        }
        this.closeModal();
      },
      error: (error) => {
        console.error('Error al actualizar tarea:', error);
        alert('Error al actualizar la tarea');
      }
    });
  }

  toggleTaskStatus(task: Task): void {
    const updateData: UpdateTaskRequest = {
      isCompleted: !task.isCompleted
    };

    this.taskService.updateTask(task.id, updateData).subscribe({
      next: (updatedTask) => {
        const index = this.tasks.findIndex(t => t.id === task.id);
        if (index !== -1) {
          this.tasks[index] = updatedTask;
        }
      },
      error: (error) => {
        console.error('Error al actualizar estado:', error);
        alert('Error al actualizar el estado de la tarea');
      }
    });
  }

  openDeleteModal(task: Task): void {
    this.taskToDelete = task;
    this.showDeleteModal = true;
  }

  confirmDelete(): void {
    if (!this.taskToDelete) return;

    this.taskService.deleteTask(this.taskToDelete.id).subscribe({
      next: () => {
        this.tasks = this.tasks.filter(t => t.id !== this.taskToDelete!.id);
        this.closeDeleteModal();
      },
      error: (error) => {
        console.error('Error al eliminar tarea:', error);
        alert('Error al eliminar la tarea');
      }
    });
  }

  closeDeleteModal(): void {
    this.showDeleteModal = false;
    this.taskToDelete = null;
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('es-ES', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  get completedTasks(): number {
    return this.tasks.filter(t => t.isCompleted).length;
  }

  get pendingTasks(): number {
    return this.tasks.filter(t => !t.isCompleted).length;
  }
}
