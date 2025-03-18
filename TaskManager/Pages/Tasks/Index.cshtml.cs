using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManager.Models;
using TaskManager.Services;
using Task = TaskManager.Models.Task;

namespace TaskManager.Pages.Tasks;

public class IndexModel : PageModel
{
    private readonly IDataService _dataService;
    public List<Task> Tasks { get; set; } = new();
    public SelectList Users { get; set; } = default!;
    public int? SelectedUserId { get; set; }

    public IndexModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async System.Threading.Tasks.Task OnGetAsync(int? userId)
    {
        var users = await _dataService.GetUsersAsync();
        Users = new SelectList(users, "Id", "Name", userId);  // Pass selected value to maintain selection
        SelectedUserId = userId;

        // Handle filtering
        if (userId.HasValue)
        {
            Tasks = await _dataService.GetUserTasksAsync(userId.Value);
        }
        else
        {
            Tasks = await _dataService.GetTasksAsync();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int? taskId)
    {
        if (taskId != null)
        {
            var result = await _dataService.DeleteTaskAsync(taskId.Value);
            if (result != taskId)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }

        return NotFound();
    }
}