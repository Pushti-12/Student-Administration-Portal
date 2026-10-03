using Registration.DTOs.Courses;

namespace Registration.Services
{
    public interface ICourseService
    {
        Task<List<CourseResponse>> GetAllAsync();
        Task<CourseResponse> CreateAsync(CreateCourseRequest request);
        Task<CourseResponse?> UpdateAsync(
            int id,
            UpdateCourseRequest request);
    }
}