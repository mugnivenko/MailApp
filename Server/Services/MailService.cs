
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

using MailApp.Models;
using MailApp.Data;

using MailApp.Server.Models;

namespace MailApp.Services;

public class MailService
{
    private ApplicationDbContext _context;

    public MailService(ApplicationDbContext context)
    {
        _context = context;
    }

    private async Task SaveMailRelations(Guid senderId, Guid recipientId, Guid mailId)
    {
        var relation = new MailFromUserToUser()
        {
            FromUserId = senderId,
            ToUserId = recipientId,
            MailId = mailId,
        };
        await _context.MailFromUserToUser.AddAsync(relation);
        await _context.SaveChangesAsync();
    }

    public async Task<Mail> SaveMail(SaveMessageDto saveMessageDto)
    {
        var mail = new Mail()
        {
            Title = saveMessageDto.Title,
            Body = saveMessageDto.Body,
            CreatedAt = DateTime.UtcNow,
        };
        EntityEntry<Mail> savedMail = await _context.Mails.AddAsync(mail);
        await _context.SaveChangesAsync();
        await SaveMailRelations(saveMessageDto.SenderId, saveMessageDto.RecipientId, savedMail.Entity.Id);
        mail.Id = savedMail.Entity.Id;
        return mail;
    }

    public async Task<List<MailFromUserToUser>> AllMailSendingToUser(Guid userId)
    {
        List<MailFromUserToUser> relations = await _context.MailFromUserToUser
            .Include(i => i.FromUser)
            .Include(i => i.Mail)
            .Where((relation) => relation.ToUserId == userId)
            .ToListAsync();
        if (relations.Count() == 0)
        {
            return new List<MailFromUserToUser>();
        }
        return relations;
    }
}