using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;
using TaskManagerAPI.Repositories;

namespace TaskManagerAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<List<TaskItemDto>> GetAllTasksAsync(int userId)
        {
            var tasks = await _taskRepository.GetAllByUserIdAsync(userId);
            return tasks.Select(MapToDto).ToList();
        }

        public async Task<TaskItemDto?> GetTaskByIdAsync(int id, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id, userId);
            return task == null ? null : MapToDto(task);
        }

        public async Task<TaskItemDto> CreateTaskAsync(CreateTaskDto createTaskDto, int userId)
        {
            var task = new TaskItem
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                UserId = userId
            };

            var createdTask = await _taskRepository.CreateAsync(task);
            return MapToDto(createdTask);
        }

        public async Task<TaskItemDto?> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id, userId);
            if (task == null)
                return null;

            if (!string.IsNullOrEmpty(updateTaskDto.Title))
                task.Title = updateTaskDto.Title;

            if (updateTaskDto.Description != null)
                task.Description = updateTaskDto.Description;

            if (updateTaskDto.IsCompleted.HasValue)
            {
                task.IsCompleted = updateTaskDto.IsCompleted.Value;
                task.CompletedAt = updateTaskDto.IsCompleted.Value ? DateTime.Now : null;
            }

            var updatedTask = await _taskRepository.UpdateAsync(task);
            return MapToDto(updatedTask);
        }

        public async Task<bool> DeleteTaskAsync(int id, int userId)
        {
            return await _taskRepository.DeleteAsync(id, userId);
        }

        private static TaskItemDto MapToDto(TaskItem task)
        {
            return new TaskItemDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                CreatedAt = task.CreatedAt,
                CompletedAt = task.CompletedAt,
                UserId = task.UserId
            };
        }
    }
}
