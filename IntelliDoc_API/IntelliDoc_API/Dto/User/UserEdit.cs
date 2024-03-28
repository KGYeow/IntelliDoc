using System.ComponentModel.DataAnnotations;

namespace IntelliDoc_API.Dto.User
{
    public class UserEdit
    {
        public string FullName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}