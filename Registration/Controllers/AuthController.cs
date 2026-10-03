using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Registration.DTOs.Auth;
using Registration.Services;

namespace Registration.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        // =====================================================
        // FORGOT PASSWORD PAGE
        // GET: /Auth/ForgotPassword
        // =====================================================

        [HttpGet("/Auth/ForgotPassword")]
        [AllowAnonymous]
        public IActionResult ForgotPasswordPage()
        {
            return View("ForgotPassword");
        }


        // =====================================================
        // RESET PASSWORD PAGE
        // GET: /Auth/ResetPassword?token=XXXX
        // =====================================================

        [HttpGet("/Auth/ResetPassword")]
        [AllowAnonymous]
        public IActionResult ResetPasswordPage(
            [FromQuery] string token)
        {
            ViewBag.Token = token;

            return View("ResetPassword");
        }


        // =====================================================
        // LOGIN API
        // POST: /api/auth/login
        // =====================================================

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            var result =
                await _authService.LoginAsync(request);

            if (result == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            return Ok(result);
        }


        // =====================================================
        // REGISTER API
        // POST: /api/auth/register
        // =====================================================

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var result =
                await _authService.RegisterAsync(request);


            if (!result)
            {
                return BadRequest(new
                {
                    message =
                        "Registration failed. Please check the entered details."
                });
            }


            return Ok(new
            {
                message = "Registration successful."
            });
        }


        // =====================================================
        // LOGOUT API
        // POST: /api/auth/logout
        // =====================================================

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();

            return Ok(new
            {
                message = "Logout successful."
            });
        }


        // =====================================================
        // FORGOT PASSWORD API
        // POST: /api/auth/forgot-password
        // =====================================================

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var resetBaseUrl =
                $"{Request.Scheme}://{Request.Host}/Auth/ResetPassword";


            await _authService.ForgotPasswordAsync(
                request,
                resetBaseUrl);


            return Ok(new
            {
                message =
                    "If this email is registered, a password reset link has been sent."
            });
        }


        // =====================================================
        // RESET PASSWORD API
        // POST: /api/auth/reset-password
        // =====================================================

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var result =
                await _authService.ResetPasswordAsync(request);


            if (!result)
            {
                return BadRequest(new
                {
                    message =
                        "Invalid or expired password reset link."
                });
            }


            return Ok(new
            {
                message =
                    "Password has been reset successfully."
            });
        }
    }
}