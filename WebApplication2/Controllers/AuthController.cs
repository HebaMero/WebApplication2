using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    public class AuthController
    {
    }
}
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtService;

    public AuthController(UserManager<ApplicationUser> userManager, IJwtTokenService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // basic server-side validation example
        if (string.IsNullOrWhiteSpace(model.Password) || model.Password.Length < 8)
            return BadRequest("Password must be at least 8 characters.");

        var user = new ApplicationUser
        {
            UserName = model.Email.Split('@')[0],
            Email = model.Email,
            DisplayName = model.DisplayName
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // By default, give "User" role
        await _userManager.AddToRoleAsync(user, "User");

        return Ok(new { message = "User created" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return Unauthorized("Invalid credentials");

        var valid = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!valid) return Unauthorized("Invalid credentials");

        var token = await _jwtService.CreateTokenAsync(user);
        return Ok(new AuthResponseDto { Token = token, ExpiresAt = DateTime.UtcNow.AddMinutes(60) });
    }