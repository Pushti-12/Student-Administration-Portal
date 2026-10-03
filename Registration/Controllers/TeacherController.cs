using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registration.DTOs.Teachers;
using Registration.Services;

namespace Registration.Controllers
{
    [ApiController]
    [Route("api/teachers")]
    [Authorize]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        // =========================
        // GET ALL
        // =========================
        [HttpGet]
        [Authorize(Roles = "ADMIN,TEACHER")]
        public async Task<IActionResult> GetAll()
        {
            var teachers =
                await _teacherService.GetAllAsync();

            return Ok(teachers);
        }

        // =========================
        // GET LOGGED-IN TEACHER
        // =========================
        [HttpGet("me")]
        [Authorize(Roles = "TEACHER")]
        public async Task<IActionResult> GetMe()
        {
            var claim =
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null ||
                !int.TryParse(claim.Value, out var userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid user identity."
                });
            }

            var teacher =
                await _teacherService.GetByUserIdAsync(userId);

            if (teacher == null)
            {
                return NotFound(new
                {
                    message = "Teacher profile not found."
                });
            }

            return Ok(teacher);
        }

        // =========================
        // UPDATE LOGGED-IN TEACHER
        // =========================
        [HttpPut("me")]
        [Authorize(Roles = "TEACHER")]
        public async Task<IActionResult> UpdateMe(
            UpdateTeacherRequest request)
        {
            var claim =
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null ||
                !int.TryParse(claim.Value, out var userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid user identity."
                });
            }

            try
            {
                var teacher =
                    await _teacherService.GetByUserIdAsync(userId);

                if (teacher == null)
                {
                    return NotFound(new
                    {
                        message = "Teacher profile not found."
                    });
                }

                var updatedTeacher =
                    await _teacherService.UpdateAsync(
                        teacher.Id,
                        request);

                if (updatedTeacher == null)
                {
                    return NotFound(new
                    {
                        message = "Teacher profile not found."
                    });
                }

                return Ok(updatedTeacher);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        // =========================
        // CREATE
        // =========================
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Create(
            CreateTeacherRequest request)
        {
            try
            {
                var teacher =
                    await _teacherService.CreateAsync(request);

                return Created("", teacher);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        // =========================
        // UPDATE
        // =========================
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Update(
            int id,
            UpdateTeacherRequest request)
        {
            try
            {
                var teacher =
                    await _teacherService.UpdateAsync(
                        id,
                        request);

                if (teacher == null)
                {
                    return NotFound(new
                    {
                        message = "Teacher not found."
                    });
                }

                return Ok(teacher);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted =
                    await _teacherService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = "Teacher not found."
                    });
                }

                return Ok(new
                {
                    message =
                        "Teacher deleted successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }
    }
}