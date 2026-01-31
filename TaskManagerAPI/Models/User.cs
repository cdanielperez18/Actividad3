namespace TaskManagerAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Relación con tareas
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
