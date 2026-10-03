using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registration.DTOs.Attendance;
using Registration.Services;

namespace Registration.Controllers
{
    [ApiController]
    [Route("api/attendance")]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IStudentService _studentService;

        public AttendanceController(
            IAttendanceService attendanceService,
            IStudentService studentService)
        {
            _attendanceService = attendanceService;
            _studentService = studentService;
        }


        [HttpPost]
        [Authorize(Roles = "ADMIN,TEACHER")]
        public async Task<IActionResult> Create(
            CreateAttendanceRequest request)
        {
            try
            {
                var attendance =
                    await _attendanceService
                        .CreateAsync(request);

                return Created("", attendance);
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


            var attendance =
                await _attendanceService
                    .GetByStudentIdAsync(id);


            return Ok(attendance);
        }
    }
}