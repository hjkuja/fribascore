using System.ComponentModel.DataAnnotations;

namespace FribaScore.Bui.Models;

/// <summary>
/// Represents a disc golf course with a collection of <see cref="Hole"/>s.
/// </summary>
public sealed class Course
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Identifier for the course, typically name shortened and lowercased.
    /// </summary>
    public required string Identifier { get; set; }

    [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
    public required string Name { get; set; }

    public required List<Hole> Holes { get; set; }

    public int TotalPar => Holes.Sum(h => h.Par);

    public decimal TotalLength => Holes.Sum(h => h.Length);
}

/// <summary>
/// Represents a hole on a disc golf <see cref="Course"/>.
/// </summary>
public sealed class Hole
{
    public required int Number { get; set; }

    public required int Par { get; set; }

    public decimal Length { get; set; } = 0;
}
