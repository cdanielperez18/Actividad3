using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs
{
    public class CreateTaskDto
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateTaskDto
    {
        [StringLength(200, MinimumLength = 1)]
        public string? Title { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool? IsCompleted { get; set; }
    }

    public class TaskItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int UserId { get; set; }
    }
}
