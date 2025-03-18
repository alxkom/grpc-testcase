using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.Pages.Users;

public class IndexModel : PageModel
{
    private readonly IDataService _dataService;
    public List<User> Users { get; set; } = new();

    public IndexModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async System.Threading.Tasks.Task OnGetAsync()
    {
        Users = await _dataService.GetUsersAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int? userId)
    {
        if (userId != null)
        {
            var result = await _dataService.DeleteUserAsync(userId.Value);
            if (result != userId)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }

        return NotFound();
    }
}