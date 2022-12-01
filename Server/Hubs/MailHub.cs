using Microsoft.AspNetCore.SignalR;

namespace MailApp.Server.Hubs
{
    public class MailHub : Hub
    {
        public async Task SendMail(string senderName, Guid recipientId, string title)
        {
            await Clients.All.SendAsync("ReceiveMessage", senderName, recipientId, title);
        }
    }
}