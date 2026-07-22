using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAPI.Data;
using StudentAPI.DTOs;
using StudentAPI.Models;
using System.Security.Claims;

namespace StudentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin Dashboard Statistics
        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var totalStudents = await _context.StudentForms.CountAsync();

            var maleStudents = await _context.StudentForms
                .CountAsync(x => x.Gender.ToLower() == "male");

            var femaleStudents = await _context.StudentForms
                .CountAsync(x => x.Gender.ToLower() == "female");

            var studentsWithPhoto = await _context.StudentForms
                .CountAsync(x => !string.IsNullOrEmpty(x.ProfilePhoto));

            var studentsWithoutPhoto = totalStudents - studentsWithPhoto;

            var totalUsers = await _context.Users.CountAsync();

            var totalAdmins = await _context.Users
                .CountAsync(x => x.Role == UserRole.Admin);

            var totalNormalUsers = await _context.Users
                .CountAsync(x => x.Role == UserRole.User);

            return Ok(new
            {
                TotalUsers = totalUsers,
                TotalAdmins = totalAdmins,
                TotalStudents = totalStudents,
                TotalNormalUsers = totalNormalUsers,
                MaleStudents = maleStudents,
                FemaleStudents = femaleStudents,
                StudentsWithPhoto = studentsWithPhoto,
                StudentsWithoutPhoto = studentsWithoutPhoto
            });
        }
        // Get All Students with Search + Pagination + Sorting
        [HttpGet("students")]
        public async Task<IActionResult> GetAllStudents(

            string? search,
            string? sortBy = "FirstName",
            bool ascending = true,
            int page = 1,
            int pageSize = 5)
        {
            var query = _context.StudentForms
                .Include(x => x.User)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                query = query.Where(x =>
                    x.FirstName.ToLower().Contains(search) ||
                    x.LastName.ToLower().Contains(search) ||
                    x.City.ToLower().Contains(search) ||
                    x.User!.Email.ToLower().Contains(search));
            }

            // Sorting
            query = sortBy?.ToLower() switch
            {
                "lastname" => ascending
                    ? query.OrderBy(x => x.LastName)
                    : query.OrderByDescending(x => x.LastName),

                "city" => ascending
                    ? query.OrderBy(x => x.City)
                    : query.OrderByDescending(x => x.City),

                "state" => ascending
                    ? query.OrderBy(x => x.State)
                    : query.OrderByDescending(x => x.State),

                _ => ascending
                    ? query.OrderBy(x => x.FirstName)
                    : query.OrderByDescending(x => x.FirstName)
            };

            // Total records
            var totalRecords = await query.CountAsync();

            // Pagination
            var students = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.Gender,
                    x.PhoneNumber,
                    x.City,
                    x.State,
                    x.ProfilePhoto,
                    Email = x.User!.Email,
                    FullName = x.User.FullName
                })
                .ToListAsync();

            return Ok(new
            {
                TotalRecords = totalRecords,
                Page = page,
                PageSize = pageSize,
                Data = students
            });
        }

        // Get Student By Id
        [HttpGet("students/{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var student = await _context.StudentForms
                .Include(s => s.User)
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    s.Id,
                    s.FirstName,
                    s.LastName,
                    s.Gender,
                    s.DateOfBirth,
                    s.PhoneNumber,
                    s.Address,
                    s.City,
                    s.State,
                    s.ProfilePhoto,
                    UserId = s.UserId,
                    Email = s.User!.Email,
                    FullName = s.User.FullName
                })
                .FirstOrDefaultAsync();

            if (student == null)
            {
                return NotFound(new
                {
                    Message = "Student not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Student fetched successfully.",
                Data = student
            });
        }

        // Update Student
        [HttpPut("students/{id}")]
        public async Task<IActionResult> UpdateStudent(int id, StudentFormDto dto)
        {
            var student = await _context.StudentForms.FindAsync(id);

            if (student == null)
            {
                return NotFound(new
                {
                    Message = "Student not found."
                });
            }

            student.FirstName = dto.FirstName;
            student.LastName = dto.LastName;
            student.Gender = dto.Gender;
            student.DateOfBirth = dto.DateOfBirth;
            student.PhoneNumber = dto.PhoneNumber;
            student.Address = dto.Address;
            student.City = dto.City;
            student.State = dto.State;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Student updated successfully."
            });
        }

        // Delete Student
        [HttpDelete("students/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.StudentForms.FindAsync(id);

            if (student == null)
            {
                return NotFound(new
                {
                    Message = "Student not found."
                });
            }

            _context.StudentForms.Remove(student);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Student deleted successfully."
            });
        }



        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .OrderBy(x => x.FullName)
                .Select(x => new UserDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email,
                    ProfilePhoto = x.ProfilePhoto,
                    Role = x.Role.ToString(),
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt,
                    LastLogin = x.LastLogin
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<UserDto>>
            {
                Success = true,
                Message = "Users fetched successfully.",
                Data = users
            });
        }


        // Get User By Id
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users
                .Where(x => x.Id == id)
                .Select(x => new UserDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email,
                    ProfilePhoto = x.ProfilePhoto,
                    Role = x.Role.ToString(),
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt,
                    LastLogin = x.LastLogin
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found."
                });
            }

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Message = "User fetched successfully.",
                Data = user
            });
        }


        // Update User
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found."
                });
            }

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.Role = dto.Role;
            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "User updated successfully."
            });
        }


        // Delete User
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found."
                });
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "User deleted successfully."
            });
        }

        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> ChangeRole(int id, ChangeRoleDto dto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.Role = dto.Role;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Role updated successfully."
            });
        }


        [HttpPut("users/{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, ChangeStatusDto dto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Status updated successfully."
            });
        }


        [HttpGet("reports")]
        public async Task<IActionResult> Reports()
        {
            var dto = new ReportsDto
            {
                TotalStudents = await _context.StudentForms.CountAsync(),

                TotalUsers = await _context.Users.CountAsync(),

                TotalAdmins = await _context.Users.CountAsync(x => x.Role == UserRole.Admin),

                TotalNormalUsers = await _context.Users.CountAsync(x => x.Role == UserRole.User),

                MaleStudents = await _context.StudentForms.CountAsync(x => x.Gender == "Male"),

                FemaleStudents = await _context.StudentForms.CountAsync(x => x.Gender == "Female"),

                StudentsWithPhoto = await _context.StudentForms.CountAsync(x => x.ProfilePhoto != null),

                StudentsWithoutPhoto = await _context.StudentForms.CountAsync(x => x.ProfilePhoto == null)
            };

            // ===========================
            // City Wise Report
            // ===========================

            dto.CityWise = await _context.StudentForms
                .GroupBy(x => x.City)
                .Select(x => new CityReportDto
                {
                    City = x.Key,
                    Count = x.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            // ===========================
            // State Wise Report
            // ===========================

            dto.StateWise = await _context.StudentForms
                .GroupBy(x => x.State)
                .Select(x => new StateReportDto
                {
                    State = x.Key,
                    Count = x.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            // ===========================
            // Monthly Registrations
            // ===========================

            var monthlyData = await _context.Users
                .GroupBy(x => new
                {
                    x.CreatedAt.Year,
                    x.CreatedAt.Month
                })
                .Select(x => new
                {
                    Year = x.Key.Year,
                    Month = x.Key.Month,
                    Count = x.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            dto.MonthlyRegistrations = monthlyData
                .Select(x => new MonthlyReportDto
                {
                    Month = new DateTime(x.Year, x.Month, 1).ToString("MMM yyyy"),
                    Count = x.Count
                })
                .ToList();

            return Ok(dto);
        }




        [AllowAnonymous]
        [HttpGet("db-test")]
        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                await _context.Database.OpenConnectionAsync();
                await _context.Database.CloseConnectionAsync();

                return Ok("Database Connected Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }


        // Get Admin Settings
        [HttpGet("settings")]
        public async Task<IActionResult> GetSettings()
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var admin = await _context.Users.FindAsync(adminId);

            if (admin == null)
                return NotFound();

            var dto = new AdminSettingsDto
            {
                FullName = admin.FullName,
                Email = admin.Email,
                Role = admin.Role.ToString(),
                IsActive = admin.IsActive,
                LastLogin = admin.LastLogin,
                ContactEmail = admin.Email
            };

            return Ok(dto);
        }

        // Update Admin Settings
        [HttpPut("settings")]
        public async Task<IActionResult> UpdateSettings(AdminSettingsDto dto)
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var admin = await _context.Users.FindAsync(adminId);

            if (admin == null)
                return NotFound();

            admin.FullName = dto.FullName;
            admin.Email = dto.Email;
            admin.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Settings updated successfully."
            });
        }
    }
}