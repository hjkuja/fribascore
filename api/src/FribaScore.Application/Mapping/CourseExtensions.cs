using FribaScore.Application.Models;
using FribaScore.Contracts.Responses;

namespace FribaScore.Application.Mapping;

public static class CourseExtensions
{
    public static CourseResponse ToResponse(this Course course) =>
        new(course.Id, course.Name, course.TotalPar, course.TotalLength,
            course.Holes.Select(h => h.ToResponse()).ToList());

    public static HoleResponse ToResponse(this Hole hole) =>
        new(hole.HoleNumber, hole.Par, hole.Length);
}
