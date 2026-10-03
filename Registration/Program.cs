using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Registration.Data;
using Registration.Models;
using Registration.Services;

var builder = WebApplication.CreateBuilder(args);


// =========================================
// MVC + JSON ENUM SUPPORT
// =========================================

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });


// =========================================
// DATABASE
// =========================================

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration
                .GetConnectionString(
                    "DefaultConnection"
                )
        )
);


// =========================================
// PASSWORD HASHER
// =========================================

builder.Services.AddScoped<
    IPasswordHasher<User>,
    PasswordHasher<User>
>();


// =========================================
// SERVICES
// =========================================

builder.Services.AddScoped<
    IAuthService,
    AuthService
>();

builder.Services.AddScoped<
    IAdminService,
    AdminService
>();

builder.Services.AddScoped<
    IStudentService,
    StudentService
>();

builder.Services.AddScoped<
    ITeacherService,
    TeacherService
>();

builder.Services.AddScoped<
    ICourseService,
    CourseService
>();

builder.Services.AddScoped<
    ISubjectService,
    SubjectService
>();

builder.Services.AddScoped<
    IAttendanceService,
    AttendanceService
>();

builder.Services.AddScoped<
    IMarksService,
    MarksService
>();

builder.Services.AddScoped<
    IFeeService,
    FeeService
>();

builder.Services.AddScoped<
    IEmailService,
    EmailService
>();


// =========================================
// JWT
// =========================================

var jwtKey =
    builder.Configuration["Jwt:Key"];


if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "JWT key is not configured in appsettings.json."
    );
}


var key =
    new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(jwtKey)
    );


builder.Services.AddAuthentication(
    options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    }
)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = key,

            ValidateIssuer = false,

            ValidateAudience = false,

            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero
        };
});


builder.Services.AddAuthorization();


var app = builder.Build();


// =========================================
// MIDDLEWARE
// =========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Home/Error"
    );

    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();


// =========================================
// API CONTROLLERS
// =========================================

app.MapControllers();


app.MapControllerRoute(
    name: "default",
    pattern:
        "{controller=Home}/{action=Index}/{id?}"
);


// =========================================
// SEED DEFAULT ADMIN
// =========================================

using (var scope =
       app.Services.CreateScope())
{
    var db =
        scope.ServiceProvider
            .GetRequiredService<
                ApplicationDbContext
            >();


    var passwordHasher =
        scope.ServiceProvider
            .GetRequiredService<
                IPasswordHasher<User>
            >();


    if (!db.Users.Any(
        u => u.Role == UserRole.ADMIN))
    {
        var admin =
            new User
            {
                Name =
                    "System Administrator",

                Email =
                    "admin@gmail.com",

                Role =
                    UserRole.ADMIN
            };


        admin.Password =
            passwordHasher.HashPassword(
                admin,
                "Admin@123"
            );


        db.Users.Add(admin);


        db.SaveChanges();
    }
}


app.Run();