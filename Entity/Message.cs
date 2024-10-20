namespace Entity;

public partial class Message
{
    public int MessageId { get; set; }

    public int? ConservationId { get; set; }

    public int Sender { get; set; }

    public string Content { get; set; } = null!;

    public DateTime DateTime { get; set; }

    public sbyte Status { get; set; }

    public virtual Conservation? Conservation { get; set; }

    public virtual User SenderNavigation { get; set; } = null!;
}
