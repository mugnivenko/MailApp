using System.ComponentModel.DataAnnotations;

namespace MailApp.Server.Models;

public class SaveMessageDto
{
    [Required]
    public Guid SenderId { get; set; }
    [Required]
    public Guid RecipientId { get; set; }
    [Required]
    [DataType(DataType.Text)]
    public string Title { get; set; }
    [Required]
    [DataType(DataType.Text)]
    public string Body { get; set; }
}