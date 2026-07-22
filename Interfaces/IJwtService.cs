using StudentAPI.Models;

namespace StudentAPI.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}