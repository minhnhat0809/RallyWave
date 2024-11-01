using UserManagement.DTOs.UserDto.ViewDto;

namespace UserManagement.DTOs.FriendDto;

public class FriendshipViewDto
{
    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public int? Level { get; set; }

    public sbyte? Status { get; set; }

    public virtual UserViewDto Receiver { get; set; } = null!;

    public virtual UserViewDto Sender { get; set; } = null!;
}