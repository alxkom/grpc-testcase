using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.Pages.Users;

public class CreateModel : PageModel
{
    private readonly IDataService _dataService;

    [BindProperty]
    public User UserModel { get; set; } = new User { Name = string.Empty };

    public CreateModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _dataService.CreateUserAsync(UserModel);
        return RedirectToPage("./Index");
    }
}