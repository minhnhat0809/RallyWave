using UserManagement.DTOs.UserDto.ViewDto;

namespace UserManagement.DTOs.FriendDto;

public class FriendShipViewDto
{
    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public int? Level { get; set; }

    public sbyte? Status { get; set; }

    public string? ReceiverName { get; set; }
    public string? SenderName { get; set; }
    

    //public virtual UserViewDto Receiver { get; set; } = null!;

    //public virtual UserViewDto Sender { get; set; } = null!;
}