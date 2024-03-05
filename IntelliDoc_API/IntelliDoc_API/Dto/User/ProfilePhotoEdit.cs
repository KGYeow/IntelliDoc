using System.ComponentModel.DataAnnotations;

namespace IntelliDoc_API.Dto.User
{
    public class ProfilePhotoEdit
    {
        public byte[] ProfilePhoto { get; set; } = null!;
    }
}