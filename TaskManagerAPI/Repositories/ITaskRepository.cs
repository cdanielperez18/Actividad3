using TaskManagerAPI.Models;

namespace TaskManagerAPI.Repositories
{
    public interface ITaskRepository
    {
        Task<List<TaskItem>> GetAllByUserIdAsync(int userId);
        Task<TaskItem?> GetByIdAsync(int id, int userId);
        Task<TaskItem> CreateAsync(TaskItem task);
        Task<TaskItem> UpdateAsync(TaskItem task);
        Task<bool> DeleteAsync(int id, int userId);
    }
}
