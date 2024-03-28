using System.ComponentModel.DataAnnotations;

namespace IntelliDoc_API.Dto.Document
{
    public class RepositoryUpdate
    {
        public byte[] Attachment { get; set; } = null!;
    }
}