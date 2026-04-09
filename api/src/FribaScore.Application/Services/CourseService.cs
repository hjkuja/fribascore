using FribaScore.Application.Database;
using FribaScore.Application.Mapping;
using FribaScore.Application.Models;
using FribaScore.Application.Services.Interfaces;
using FribaScore.Contracts.Exceptions;
using FribaScore.Contracts.Requests.Courses;
using FribaScore.Contracts.Responses;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace FribaScore.Application.Services;

public class CourseService(AppDbContext db) : ICourseService
{
    public async Task<Result<IEnumerable<CourseResponse>>> GetAllAsync()
    {
        try
        {
            var courses = await db.Courses.ToListAsync();
            return new Result<IEnumerable<CourseResponse>>(courses.Select(c => c.ToResponse()));
        }
        catch (Exception ex)
        {
            return new Result<IEnumerable<CourseResponse>>(ex);
        }
    }

    public async Task<Result<CourseResponse>> GetByIdAsync(Guid id)
    {
        try
        {
            var course = await db.Courses.FindAsync(id);
            if (course is null)
                return new Result<CourseResponse>(new NotFoundException(nameof(Course)));
            return new Result<CourseResponse>(course.ToResponse());
        }
        catch (Exception ex)
        {
            return new Result<CourseResponse>(ex);
        }
    }

    public async Task<Result<CourseResponse>> CreateAsync(CreateCourseRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return new Result<CourseResponse>(new BadRequestException("Course name is required."));

            var course = new Course
            {
                Name = request.Name,
                TotalPar = request.TotalPar,
                TotalLength = request.TotalLength,
                Holes = request.Holes.Select(h => new Hole
                {
                    HoleNumber = h.HoleNumber,
                    Par = h.Par,
                    Length = h.Length
                }).ToList()
            };

            db.Courses.Add(course);
            await db.SaveChangesAsync();
            return new Result<CourseResponse>(course.ToResponse());
        }
        catch (Exception ex)
        {
            return new Result<CourseResponse>(ex);
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var course = await db.Courses.FindAsync(id);
            if (course is null)
                return new Result<bool>(new NotFoundException(nameof(Course)));
            db.Courses.Remove(course);
            await db.SaveChangesAsync();
            return new Result<bool>(true);
        }
        catch (Exception ex)
        {
            return new Result<bool>(ex);
        }
    }
}
