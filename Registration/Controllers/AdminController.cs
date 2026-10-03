using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registration.DTOs.Admin;
using Registration.Services;

namespace Registration.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "ADMIN")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;


        public AdminController(
            IAdminService adminService)
        {
            _adminService = adminService;
        }


        // =========================================
        // GET CURRENT ADMIN
        // GET: /api/admin/me
        // =========================================

        [HttpGet("me")]
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


            var admin =
                await _adminService
                    .GetByUserIdAsync(userId);


            if (admin == null)
            {
                return NotFound(new
                {
                    message =
                        "Admin profile not found."
                });
            }


            return Ok(admin);
        }


        // =========================================
        // UPDATE CURRENT ADMIN
        // PUT: /api/admin/me
        // =========================================

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe(
            UpdateAdminRequest request)
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
                var admin =
                    await _adminService
                        .UpdateAsync(
                            userId,
                            request);


                if (admin == null)
                {
                    return NotFound(new
                    {
                        message =
                            "Admin profile not found."
                    });
                }


                return Ok(admin);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message =
                        ex.Message
                });
            }
        }
    }
}