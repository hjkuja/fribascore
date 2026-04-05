namespace FribaScore.Application.Models;

public class Course
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public int TotalPar { get; set; }
    public int TotalLength { get; set; }
    public List<Hole> Holes { get; set; } = [];
}

public class Hole
{
    public int HoleNumber { get; set; }
    public int Par { get; set; }
    public int? Length { get; set; }
}
