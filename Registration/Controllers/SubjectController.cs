using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registration.DTOs.Subjects;
using Registration.Services;

namespace Registration.Controllers
{
    [ApiController]
    [Route("api/subjects")]
    [Authorize]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(
            ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        // GET /api/subjects/course/{courseId}
        [HttpGet("course/{courseId}")]
        [Authorize(Roles = "ADMIN,TEACHER,STUDENT")]
        public async Task<IActionResult>
            GetByCourse(int courseId)
        {
            var subjects =
                await _subjectService
                    .GetByCourseIdAsync(courseId);

            return Ok(subjects);
        }

        // POST /api/subjects
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult>
            Create(
                CreateSubjectRequest request)
        {
            try
            {
                var subject =
                    await _subjectService
                        .CreateAsync(request);

                return Created(
                    "",
                    subject);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(
                    new
                    {
                        message = ex.Message
                    });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(
                    new
                    {
                        message = ex.Message
                    });
            }
        }

        // PUT /api/subjects/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult>
            Update(
                int id,
                CreateSubjectRequest request)
        {
            try
            {
                var subject =
                    await _subjectService
                        .UpdateAsync(
                            id,
                            request);

                if (subject == null)
                {
                    return NotFound(
                        new
                        {
                            message =
                                "Subject not found."
                        });
                }

                return Ok(subject);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(
                    new
                    {
                        message = ex.Message
                    });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(
                    new
                    {
                        message = ex.Message
                    });
            }
        }

        // DELETE /api/subjects/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult>
            Delete(int id)
        {
            try
            {
                var deleted =
                    await _subjectService
                        .DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(
                        new
                        {
                            message =
                                "Subject not found."
                        });
                }

                return Ok(
                    new
                    {
                        message =
                            "Subject deleted successfully."
                    });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(
                    new
                    {
                        message = ex.Message
                    });
            }
        }
    }
}