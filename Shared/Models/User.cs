using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MailApp.Models;

public class User
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public string UserName { get; set; }

    [JsonIgnore]
    public ICollection<MailFromUserToUser> MailFromUsersUser { get; set; }
    [JsonIgnore]
    public ICollection<MailFromUserToUser> MailUsersToUser { get; set; }

    public override string ToString()
    {
        return string.Format("{0} {1} {2} {3}", Id, UserName, MailFromUsersUser, MailUsersToUser);
    }

}