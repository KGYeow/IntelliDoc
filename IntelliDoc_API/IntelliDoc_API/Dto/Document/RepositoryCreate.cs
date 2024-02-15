using System.ComponentModel.DataAnnotations;

namespace IntelliDoc_API.Dto.Document
{
    public class RepositoryCreate
    {
        public string DocName { get; set; } = null!;
        public string Category { get; set; } = null!;
        public byte[] Attachment { get; set; } = null!;
        public string Type { get; set; } = null!;
    }
}