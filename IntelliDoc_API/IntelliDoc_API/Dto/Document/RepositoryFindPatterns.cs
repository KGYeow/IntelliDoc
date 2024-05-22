using System.ComponentModel.DataAnnotations;

namespace IntelliDoc_API.Dto.Document
{
    public class RepositoryFindPatterns
    {
        public string Name { get; set; } = null!;
        public byte[] Attachment { get; set; } = null!;
    }
}