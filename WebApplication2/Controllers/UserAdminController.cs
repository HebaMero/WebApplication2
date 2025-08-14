using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    public class UserAdminController
    {
    }
}
public class UsersAdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersAdminController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // Only admins can add roles to users
    [HttpPost("assign-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRole([FromQuery] string userEmail, [FromQuery] string role)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null) return NotFound("User not found");

        if (!await _userManager.IsInRoleAsync(user, role))
            await _userManager.AddToRoleAsync(user, role);

        return Ok("Role assigned");
    }
}
