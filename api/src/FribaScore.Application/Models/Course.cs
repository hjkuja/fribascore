namespace FribaScore.Application.Models;

/// <summary>
/// Represents a golf course with a collection of holes.
/// </summary>
public class Course
{
    /// <summary>
    /// Gets or sets the unique identifier for the course.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the name of the course.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the total par for the course.
    /// </summary>
    public int TotalPar { get; set; }

    /// <summary>
    /// Gets or sets the total length of the course in meters.
    /// </summary>
    public int TotalLength { get; set; }

    /// <summary>
    /// Gets or sets the collection of holes for this course.
    /// </summary>
    public List<Hole> Holes { get; set; } = [];
}

/// <summary>
/// Represents a single hole on a golf course.
/// </summary>
public class Hole
{
    /// <summary>
    /// Gets or sets the hole number.
    /// </summary>
    public int HoleNumber { get; set; }

    /// <summary>
    /// Gets or sets the par for this hole.
    /// </summary>
    public int Par { get; set; }

    /// <summary>
    /// Gets or sets the length of this hole in meters.
    /// </summary>
    public int? Length { get; set; }
}
