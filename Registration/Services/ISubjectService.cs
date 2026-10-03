using Registration.DTOs.Subjects;

namespace Registration.Services
{
    public interface ISubjectService
    {
        Task<List<SubjectResponse>> GetByCourseIdAsync(
            int courseId);

        Task<SubjectResponse> CreateAsync(
            CreateSubjectRequest request);

        Task<SubjectResponse?> UpdateAsync(
            int id,
            CreateSubjectRequest request);

        Task<bool> DeleteAsync(int id);
    }
}