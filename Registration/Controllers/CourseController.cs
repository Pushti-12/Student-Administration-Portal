using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registration.DTOs.Courses;
using Registration.Services;

namespace Registration.Controllers
{
    [ApiController]
    [Route("api/courses")]
    [Authorize]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(
            ICourseService courseService)
        {
            _courseService = courseService;
        }


        // =========================
        // GET ALL
        // Public because registration
        // page needs available courses.
        // =========================

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var courses =
                await _courseService.GetAllAsync();

            return Ok(courses);
        }


        // =========================
        // CREATE
        // ADMIN ONLY
        // =========================

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Create(
            CreateCourseRequest request)
        {
            try
            {
                var course =
                    await _courseService
                        .CreateAsync(request);

                return Created(
                    "",
                    course);
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


        // =========================
        // UPDATE
        // ADMIN ONLY
        // =========================

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Update(
            int id,
            UpdateCourseRequest request)
        {
            try
            {
                var course =
                    await _courseService
                        .UpdateAsync(
                            id,
                            request);


                if (course == null)
                {
                    return NotFound(
                        new
                        {
                            message =
                                "Course not found."
                        });
                }


                return Ok(course);
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
    }
}