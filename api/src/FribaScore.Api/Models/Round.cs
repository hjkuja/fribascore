namespace FribaScore.Api.Models;

public class Round
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CourseId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public List<ScoreEntry> Scores { get; set; } = [];
}

public class ScoreEntry
{
    public Guid PlayerId { get; set; }
    public int HoleNumber { get; set; }
    public int Score { get; set; }
}
