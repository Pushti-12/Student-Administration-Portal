using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registration.DTOs.Fees;
using Registration.Services;

namespace Registration.Controllers
{
    [ApiController]
    [Route("api/fees")]
    [Authorize]
    public class FeeController : ControllerBase
    {
        private readonly IFeeService _feeService;
        private readonly IStudentService _studentService;

        public FeeController(
            IFeeService feeService,
            IStudentService studentService)
        {
            _feeService = feeService;
            _studentService = studentService;
        }


        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Create(
            CreateFeeRequest request)
        {
            try
            {
                var fee =
                    await _feeService
                        .CreateAsync(request);

                return Created("", fee);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(
                    new
                    {
                        message = ex.Message
                    });
            }
        }


        [HttpGet("student/{id}")]
        [Authorize(Roles = "ADMIN,STUDENT")]
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


            var fees =
                await _feeService
                    .GetByStudentIdAsync(id);


            return Ok(fees);
        }
    }
}