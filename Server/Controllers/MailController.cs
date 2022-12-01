using Microsoft.AspNetCore.Mvc;

using MailApp.Models;
using MailApp.Services;
using MailApp.Server.Models;

namespace MailApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class MailController : ControllerBase
{
    private readonly ILogger<MailController> _logger;
    private readonly MailService _service;

    public MailController(ILogger<MailController> logger, MailService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpPost("SaveMail")]
    public async Task<Mail> SaveMail([FromBody] SaveMessageDto saveMessageDto)
    {
        return await _service.SaveMail(saveMessageDto);
    }

    [HttpGet("ReceivedUserMail")]
    public async Task<List<MailFromUserToUser>> GetAllMailSendingToUser([FromQuery] Guid userId)
    {
        return await _service.AllMailSendingToUser(userId);
    }
}
