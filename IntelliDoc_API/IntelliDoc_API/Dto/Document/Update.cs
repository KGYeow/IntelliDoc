using System.ComponentModel.DataAnnotations;

namespace IntelliDoc_API.Dto.Document
{
    public class Update
    {
        public byte[] Attachment { get; set; } = null!;
    }
}