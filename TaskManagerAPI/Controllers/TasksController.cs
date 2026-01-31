using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protege todas las rutas de este controlador
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// Obtener todas las tareas del usuario autenticado
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<TaskItemDto>>> GetTasks()
        {
            var userId = GetUserIdFromToken();
            var tasks = await _taskService.GetAllTasksAsync(userId);
            return Ok(tasks);
        }

        /// <summary>
        /// Obtener una tarea por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItemDto>> GetTask(int id)
        {
            var userId = GetUserIdFromToken();
            var task = await _taskService.GetTaskByIdAsync(id, userId);
            
            if (task == null)
            {
                return NotFound(new { message = "Tarea no encontrada" });
            }

            return Ok(task);
        }

        /// <summary>
        /// Crear una nueva tarea
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TaskItemDto>> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            var userId = GetUserIdFromToken();
            var task = await _taskService.CreateTaskAsync(createTaskDto, userId);
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        /// <summary>
        /// Actualizar una tarea
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TaskItemDto>> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            var userId = GetUserIdFromToken();
            var task = await _taskService.UpdateTaskAsync(id, updateTaskDto, userId);
            
            if (task == null)
            {
                return NotFound(new { message = "Tarea no encontrada" });
            }

            return Ok(task);
        }

        /// <summary>
        /// Eliminar una tarea
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = GetUserIdFromToken();
            var result = await _taskService.DeleteTaskAsync(id, userId);
            
            if (!result)
            {
                return NotFound(new { message = "Tarea no encontrada" });
            }

            return NoContent();
        }

        /// <summary>
        /// Obtener el ID del usuario desde el token JWT
        /// </summary>
        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}
