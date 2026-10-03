using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registration.DTOs.Marks;
using Registration.Services;

namespace Registration.Controllers
{
    [ApiController]
    [Route("api/marks")]
    [Authorize]
    public class MarksController : ControllerBase
    {
        private readonly IMarksService _marksService;
        private readonly IStudentService _studentService;

        public MarksController(
            IMarksService marksService,
            IStudentService studentService)
        {
            _marksService = marksService;
            _studentService = studentService;
        }


        [HttpPost]
        [Authorize(Roles = "ADMIN,TEACHER")]
        public async Task<IActionResult> Create(
            CreateMarksRequest request)
        {
            try
            {
                var marks =
                    await _marksService
                        .CreateAsync(request);

                return Created("", marks);
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


        [HttpGet("student/{id}")]
        [Authorize(Roles = "ADMIN,TEACHER,STUDENT")]
        public async Task<IActionResult> GetByStudent(int id)
        {
            if (User.IsInRole("STUDENT"))
            {
                var claim =
                    User.FindFirst(
                        ClaimTypes.NameIdentifier);

                if (
                    claim == null ||
                    !int.TryParse(
                        claim.Value,
                        out var userId)
                )
                {
                    return Unauthorized();
                }


                var student =
                    await _studentService
                        .GetByIdAsync(id);


                if (
                    student == null ||
                    student.UserId != userId
                )
                {
                    return Forbid();
                }
            }


            var marks =
                await _marksService
                    .GetByStudentIdAsync(id);


            return Ok(marks);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN,TEACHER")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _marksService.DeleteAsync(id);

                return Ok(new
                {
                    message = "Marks deleted successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }
    }
}