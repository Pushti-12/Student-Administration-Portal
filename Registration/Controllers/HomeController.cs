using Microsoft.AspNetCore.Mvc;
using Registration.Models;
using System.Diagnostics;

namespace Registration.Controllers
{
    public class HomeController : Controller
    {
        // =========================================
        // DEFAULT PAGE
        // =========================================

        public IActionResult Index()
        {
            return View(
                "~/Views/Auth/Login.cshtml"
            );
        }


        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId =
                        Activity.Current?.Id ??
                        HttpContext.TraceIdentifier
                }
            );
        }


        // =========================================
        // ADMIN
        // =========================================

        [HttpGet("/Admin/Dashboard")]
        public IActionResult AdminDashboard()
        {
            return View(
                "~/Views/Admin/Dashboard.cshtml"
            );
        }


        [HttpGet("/Admin/Profile")]
        public IActionResult AdminProfile()
        {
            return View(
                "~/Views/Admin/Profile.cshtml"
            );
        }


        [HttpGet("/Admin/Students")]
        public IActionResult AdminStudents()
        {
            return View(
                "~/Views/Admin/Students.cshtml"
            );
        }


        [HttpGet("/Admin/Teachers")]
        public IActionResult AdminTeachers()
        {
            return View(
                "~/Views/Admin/Teachers.cshtml"
            );
        }


        [HttpGet("/Admin/Courses")]
        public IActionResult AdminCourses()
        {
            return View(
                "~/Views/Admin/Courses.cshtml"
            );
        }


        [HttpGet("/Admin/Attendance")]
        public IActionResult AdminAttendance()
        {
            return View(
                "~/Views/Admin/Attendance.cshtml"
            );
        }


        [HttpGet("/Admin/Marks")]
        public IActionResult AdminMarks()
        {
            return View(
                "~/Views/Admin/Marks.cshtml"
            );
        }


        [HttpGet("/Admin/Fees")]
        public IActionResult AdminFees()
        {
            return View(
                "~/Views/Admin/Fees.cshtml"
            );
        }


        // =========================================
        // TEACHER
        // =========================================

        [HttpGet("/Teacher/Dashboard")]
        public IActionResult TeacherDashboard()
        {
            return View(
                "~/Views/Teacher/Dashboard.cshtml"
            );
        }


        [HttpGet("/Teacher/Courses")]
        public IActionResult TeacherCourses()
        {
            return View(
                "~/Views/Teacher/Courses.cshtml"
            );
        }


        [HttpGet("/Teacher/Students")]
        public IActionResult TeacherStudents()
        {
            return View(
                "~/Views/Teacher/Students.cshtml"
            );
        }


        [HttpGet("/Teacher/Attendance")]
        public IActionResult TeacherAttendance()
        {
            return View(
                "~/Views/Teacher/Attendance.cshtml"
            );
        }


        [HttpGet("/Teacher/Marks")]
        public IActionResult TeacherMarks()
        {
            return View(
                "~/Views/Teacher/Marks.cshtml"
            );
        }


        [HttpGet("/Teacher/Profile")]
        public IActionResult TeacherProfile()
        {
            return View(
                "~/Views/Teacher/Profile.cshtml"
            );
        }


        // =========================================
        // STUDENT
        // =========================================

        [HttpGet("/Student/Dashboard")]
        public IActionResult StudentDashboard()
        {
            return View(
                "~/Views/Student/Dashboard.cshtml"
            );
        }


        [HttpGet("/Student/Profile")]
        public IActionResult StudentProfile()
        {
            return View(
                "~/Views/Student/Profile.cshtml"
            );
        }


        [HttpGet("/Student/Courses")]
        public IActionResult StudentCourses()
        {
            return View(
                "~/Views/Student/Courses.cshtml"
            );
        }


        [HttpGet("/Student/Attendance")]
        public IActionResult StudentAttendance()
        {
            return View(
                "~/Views/Student/Attendance.cshtml"
            );
        }


        [HttpGet("/Student/Marks")]
        public IActionResult StudentMarks()
        {
            return View(
                "~/Views/Student/Marks.cshtml"
            );
        }


        [HttpGet("/Student/Fees")]
        public IActionResult StudentFees()
        {
            return View(
                "~/Views/Student/Fees.cshtml"
            );
        }


        // =========================================
        // LOGIN
        // =========================================

        [HttpGet("/Auth/Login")]
        public IActionResult Login()
        {
            return View(
                "~/Views/Auth/Login.cshtml"
            );
        }


        // =========================================
        // REGISTER
        // =========================================

        [HttpGet("/Auth/Register")]
        public IActionResult Register()
        {
            return View(
                "~/Views/Auth/Register.cshtml"
            );
        }
    }
}