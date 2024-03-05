using System.ComponentModel.DataAnnotations;

namespace IntelliDoc_API.Dto.User
{
    public class Edit
    {
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}