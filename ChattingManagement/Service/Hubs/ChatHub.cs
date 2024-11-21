using Microsoft.AspNetCore.SignalR;

namespace ChattingManagement.Service.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(int conservationId, int senderId, string senderName, string content)
    {
        await Clients.Group(conservationId.ToString()).SendAsync("ReceiveMessage", conservationId, senderId, senderName, content);
    }
    
    // Thêm user vào một nhóm chat (conservation)
    public async Task JoinConservation(int conservationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, conservationId.ToString());
    }

    // Rời nhóm chat
    public async Task LeaveConservation(int conservationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, conservationId.ToString());
    }
}