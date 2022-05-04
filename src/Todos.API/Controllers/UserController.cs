using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todos.Core.Dtos.Users;
using Todos.Core.Services;
using Todos.Core.Services.Abstract;

namespace Todos.API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class UserController : Controller
{
    private readonly UserService _userService;

    private readonly IAuthService _authService;
    // GET
    public UserController(UserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    [HttpGet]
    public async Task<IEnumerable<UserDto>> Index()
    {
        return await _userService.GetAllUsers();
    }

    [HttpPost]
    public async Task<UserDto> Create([FromBody] CreateUserDto model)
    {
        return await _userService.Create(model);
    }

    [AllowAnonymous]
    [HttpPost("LogIn")]
    public async Task<IActionResult> LogIn([FromBody] LogInDto model)
    {
        var user = await _userService.LogIn(model);
        var token = _authService.GetUserToken(user);
        return Ok(new { user, token });
    }
}