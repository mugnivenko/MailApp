using Microsoft.AspNetCore.Mvc;
using MailApp.Services;
using MailApp.Models;

namespace MailApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly UserService _service;

    public UserController(ILogger<UserController> logger, UserService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet("GetUsers")]
    public async Task<IEnumerable<User>> GetUsers([FromQuery] string? username)
    {
        return await _service.GetUsers(username);
    }

    [HttpGet("GetUser")]
    public async Task<User> GetUser([FromQuery] string username)
    {
        return await _service.GetUserOrCreate(username);
    }
}
