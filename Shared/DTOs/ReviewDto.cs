namespace Trofi.io.Shared;

public class ReviewDto
{
    public Guid ReviewId { get; set; }
    public string? Title { get; set; }
    public string? Review { get; set; }
    public string? WrittenBy { get; set; }
    public float Rating { get; set; }
    public int UpVotes { get; set; }
    public DateTime WrittenOn { get; set; }
    public DateTime? EditedOn { get; set; }
}
