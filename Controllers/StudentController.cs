using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAPI.Data;
using StudentAPI.DTOs;
using StudentAPI.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace StudentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class StudentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public StudentController(
    ApplicationDbContext context,
    IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpPost("profile")]
        public async Task<IActionResult> CreateProfile(StudentFormDto dto)
        {
            // Get logged-in user id
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Check if profile already exists
            bool exists = await _context.StudentForms
                .AnyAsync(x => x.UserId == userId);

            if (exists)
            {
                return BadRequest(new
                {
                    message = "Profile already exists."
                });
            }

            var profile = new StudentForm
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                UserId = userId
            };

            _context.StudentForms.Add(profile);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile created successfully."
            });
        }


        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var profile = await _context.StudentForms
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (profile == null)
            {
                return NotFound(new
                {
                    message = "Profile not found."
                });
            }

            return Ok(profile);
        }


        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(StudentFormDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var profile = await _context.StudentForms
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (profile == null)
            {
                return NotFound(new
                {
                    message = "Profile not found."
                });
            }

            profile.FirstName = dto.FirstName;
            profile.LastName = dto.LastName;
            profile.Gender = dto.Gender;
            profile.DateOfBirth = dto.DateOfBirth;
            profile.PhoneNumber = dto.PhoneNumber;
            profile.Address = dto.Address;
            profile.City = dto.City;
            profile.State = dto.State;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile updated successfully."
            });
        }

        [HttpPost("upload-photo")]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new
                {
                    Message = "Please select an image."
                });
            }

            // Allow only images
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new
                {
                    Message = "Only JPG, JPEG and PNG files are allowed."
                });
            }

            // Max size = 2 MB
            if (file.Length > 2 * 1024 * 1024)
            {
                return BadRequest(new
                {
                    Message = "Maximum file size is 2 MB."
                });
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var student = await _context.StudentForms
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (student == null)
            {
                return NotFound(new
                {
                    Message = "Student profile not found."
                });
            }

            string uploadsFolder = Path.Combine(_environment.WebRootPath, "Uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid() + extension;

            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            student.ProfilePhoto = "/Uploads/" + uniqueFileName;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Photo uploaded successfully.",
                PhotoUrl = student.ProfilePhoto
            });
        }
    }
}