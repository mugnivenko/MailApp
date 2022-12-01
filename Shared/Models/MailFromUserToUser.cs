using System.ComponentModel.DataAnnotations;

namespace MailApp.Models;

public class MailFromUserToUser
{
    [Key]
    public Guid Id { get; set; }

    public Guid FromUserId { get; set; }
    public User FromUser { get; set; }
    public Guid ToUserId { get; set; }
    public User ToUser { get; set; }
    public Guid MailId { get; set; }
    public Mail Mail { get; set; }

    public override string ToString()
    {
        return string.Format("{0} {1} {2}", FromUserId, ToUserId, MailId);
    }
}