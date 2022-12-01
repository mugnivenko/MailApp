using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MailApp.Models;
public class Mail
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Body { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime CreatedAt { get; set; }

    [JsonIgnore]
    public virtual MailFromUserToUser MailFromUserToUser { get; set; }

    public override string ToString()
    {
        return string.Format("{0} {1} {2} {3} {4}", Id, Title, Body, CreatedAt, MailFromUserToUser);
    }
}