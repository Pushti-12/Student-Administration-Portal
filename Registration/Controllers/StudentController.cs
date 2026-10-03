using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registration.DTOs.Students;
using Registration.Services;

namespace Registration.Controllers
{
    [ApiController]
    [Route("api/students")]
    [Authorize]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(
            IStudentService studentService)
        {
            _studentService = studentService;
        }


        // =========================================
        // GET ALL STUDENTS
        // ADMIN / TEACHER ONLY
        // =========================================

        [HttpGet]
        [Authorize(Roles = "ADMIN,TEACHER")]
        public async Task<IActionResult> GetAll()
        {
            var students =
                await _studentService.GetAllAsync();

            return Ok(students);
        }


        // =========================================
        // GET CURRENT LOGGED-IN STUDENT
        // STUDENT ONLY
        // =========================================

        [HttpGet("me")]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> GetMe()
        {
            var userIdClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier);


            if (
                userIdClaim == null ||
                !int.TryParse(
                    userIdClaim.Value,
                    out var userId)
            )
            {
                return Unauthorized(new
                {
                    message =
                        "Invalid user identity."
                });
            }


            var student =
                await _studentService
                    .GetByUserIdAsync(userId);


            if (student == null)
            {
                return NotFound(new
                {
                    message =
                        "Student profile not found."
                });
            }


            return Ok(student);
        }


        // =========================================
        // UPDATE CURRENT LOGGED-IN STUDENT
        // STUDENT ONLY
        // =========================================

        [HttpPut("me")]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> UpdateMe(
            UpdateStudentRequest request)
        {
            var userIdClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier);


            if (
                userIdClaim == null ||
                !int.TryParse(
                    userIdClaim.Value,
                    out var userId)
            )
            {
                return Unauthorized(new
                {
                    message =
                        "Invalid user identity."
                });
            }


            try
            {
                var student =
                    await _studentService
                        .GetByUserIdAsync(userId);


                if (student == null)
                {
                    return NotFound(new
                    {
                        message =
                            "Student profile not found."
                    });
                }


                var updatedStudent =
                    await _studentService
                        .UpdateAsync(
                            student.Id,
                            request);


                if (updatedStudent == null)
                {
                    return NotFound(new
                    {
                        message =
                            "Student profile not found."
                    });
                }


                return Ok(updatedStudent);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
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


        // =========================================
        // GET STUDENT BY ID
        // =========================================

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var student =
                await _studentService
                    .GetByIdAsync(id);


            if (student == null)
            {
                return NotFound(new
                {
                    message =
                        "Student not found."
                });
            }


            // STUDENT CAN ONLY VIEW OWN RECORD

            if (User.IsInRole("STUDENT"))
            {
                var userIdClaim =
                    User.FindFirst(
                        ClaimTypes.NameIdentifier);


                if (
                    userIdClaim == null ||
                    !int.TryParse(
                        userIdClaim.Value,
                        out var userId) ||
                    student.UserId != userId
                )
                {
                    return Forbid();
                }
            }


            return Ok(student);
        }


        // =========================================
        // CREATE STUDENT
        // ADMIN ONLY
        // =========================================

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Create(
            CreateStudentRequest request)
        {
            try
            {
                var student =
                    await _studentService
                        .CreateAsync(request);


                return CreatedAtAction(
                    nameof(GetById),
                    new { id = student.Id },
                    student);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
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


        // =========================================
        // UPDATE STUDENT
        // ADMIN ONLY
        // =========================================

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Update(
            int id,
            UpdateStudentRequest request)
        {
            try
            {
                var student =
                    await _studentService
                        .UpdateAsync(
                            id,
                            request);


                if (student == null)
                {
                    return NotFound(new
                    {
                        message =
                            "Student not found."
                    });
                }


                return Ok(student);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
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


        // =========================================
        // DELETE STUDENT
        // ADMIN ONLY
        // =========================================

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted =
                    await _studentService
                        .DeleteAsync(id);


                if (!deleted)
                {
                    return NotFound(new
                    {
                        message =
                            "Student not found."
                    });
                }


                return Ok(new
                {
                    message =
                        "Student deleted successfully."
                });
            }
            catch
            {
                return StatusCode(
                    500,
                    new
                    {
                        message =
                            "Unable to delete student."
                    });
            }
        }
    }
}