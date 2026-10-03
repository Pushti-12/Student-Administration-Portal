# Student Administration Portal

A web-based student administration portal built using **ASP.NET Core MVC, Entity Framework Core, and SQL Server**. The application provides role-based access for Admin, Teacher, and Student users, along with authentication, student management, academic records, and REST API functionality.

## Features

- User registration and login
- JWT-based authentication
- Role-based access control
- Admin, Teacher, and Student roles
- Student profile management
- CRUD operations
- Student marks management
- REST API endpoints
- Input validation
- Search, filtering, and pagination
- Protected APIs and authorization
- API testing using Postman
- SQL Server database integration

## User Roles

### Admin
- Manage users
- View and manage student records
- Access the administrative dashboard
- Perform CRUD operations

### Teacher
- View student information
- Add and manage student marks
- View academic records

### Student
- Login securely
- View personal information
- View academic/marks information

## Technology Stack

- **Language:** C#
- **Framework:** ASP.NET Core MVC
- **ORM:** Entity Framework Core
- **Database:** SQL Server
- **Frontend:** Razor Views, HTML, CSS, JavaScript
- **Authentication:** JWT
- **API Testing:** Postman
- **Version Control:** Git & GitHub
- **IDE:** Visual Studio

## Authentication & Authorization

The application implements authentication and role-based authorization.

After successful login, a JWT token is generated with relevant claims. Protected endpoints use the token to authenticate requests and restrict access according to the user's role.

## CRUD Operations

The application implements CRUD functionality for managing student and application data:

- **Create** new records
- **Read** existing records
- **Update** records
- **Delete** records

Entity Framework Core is used for database operations between the ASP.NET Core application and SQL Server.

## API Testing

REST APIs are tested using **Postman** to verify:

- HTTP methods
- Request and response data
- HTTP status codes
- Authentication
- Authorization
- Input validation
- Error responses

## Database

The application uses **SQL Server** with **Entity Framework Core** for data persistence and database operations.

## Project Structure

```text
Student-Administration-Portal/
│
├── Registration/
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   ├── DTOs/
│   ├── Data/
│   ├── Views/
│   ├── wwwroot/
│   └── Program.cs
│
├── Registration.slnx
├── .gitignore
└── README.md
