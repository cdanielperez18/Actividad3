using TaskManagerAPI.DTOs;

namespace TaskManagerAPI.Services
{
    public interface ITaskService
    {
        Task<List<TaskItemDto>> GetAllTasksAsync(int userId);
        Task<TaskItemDto?> GetTaskByIdAsync(int id, int userId);
        Task<TaskItemDto> CreateTaskAsync(CreateTaskDto createTaskDto, int userId);
        Task<TaskItemDto?> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto, int userId);
        Task<bool> DeleteTaskAsync(int id, int userId);
    }
}
