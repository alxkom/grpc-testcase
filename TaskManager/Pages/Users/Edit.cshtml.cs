using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.Pages.Users;

public class EditModel : PageModel
{
    private readonly IDataService _dataService;

    [BindProperty]
    public User UserModel { get; set; } = new User { Name = string.Empty };

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

        var user = await _dataService.GetUserByIdAsync(id.Value);
        if (user == null)
        {
            return NotFound();
        }

        UserModel = user;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _dataService.UpdateUserAsync(UserModel);
        if (result == null)
        {
            return NotFound();
        }

        return RedirectToPage("./Index");
    }
}