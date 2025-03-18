using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManager.Models;
using TaskManager.Services;
using Task = TaskManager.Models.Task;

namespace TaskManager.Pages.Tasks;

public class CreateModel : PageModel
{
    private readonly IDataService _dataService;

    [BindProperty]
    public Task Task { get; set; } = new Task { Name = string.Empty };

    public SelectList Users { get; set; } = default!;
    public SelectList States { get; set; } = default!;

    public CreateModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var users = await _dataService.GetUsersAsync();
        Users = new SelectList(users, "Id", "Name");
        States = new SelectList(Enum.GetValues(typeof(TaskState)));

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            var users = await _dataService.GetUsersAsync();
            Users = new SelectList(users, "Id", "Name");
            States = new SelectList(Enum.GetValues(typeof(TaskState)));
            return Page();
        }

        await _dataService.CreateTaskAsync(Task);
        return RedirectToPage("./Index");
    }
}