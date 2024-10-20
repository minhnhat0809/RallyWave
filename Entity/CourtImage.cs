namespace Entity;

public partial class CourtImage
{
    public int ImageId { get; set; }

    public int CourtId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public virtual Court Court { get; set; } = null!;
}
