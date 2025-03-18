using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManager.Models;
using TaskManager.Services;
using Task = TaskManager.Models.Task;

namespace TaskManager.Pages.Tasks;

public class EditModel : PageModel
{
    private readonly IDataService _dataService;

    [BindProperty]
    public Task Task { get; set; } = new Task { Name = string.Empty };

    public SelectList Users { get; set; } = default!;
    public SelectList States { get; set; } = default!;

    public EditModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var task = await _dataService.GetTaskByIdAsync(id.Value);
        if (task == null)
        {
            return NotFound();
        }

        Task = task;

        var users = await _dataService.GetUsersAsync();
        Users = new SelectList(users, "Id", "Name", Task.UserId);
        States = new SelectList(Enum.GetValues(typeof(TaskState)), Task.State);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            var users = await _dataService.GetUsersAsync();
            Users = new SelectList(users, "Id", "Name", Task.UserId);
            States = new SelectList(Enum.GetValues(typeof(TaskState)), Task.State);
            return Page();
        }

        var result = await _dataService.UpdateTaskAsync(Task);
        if (result == null)
        {
            return NotFound();
        }

        return RedirectToPage("./Index");
    }
}