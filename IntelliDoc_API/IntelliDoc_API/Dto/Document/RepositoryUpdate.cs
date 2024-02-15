using System.ComponentModel.DataAnnotations;

namespace IntelliDoc_API.Dto.Document
{
    public class RepositoryUpdate
    {
        public string UpdateDegree { get; set; } = null!;
        public byte[] Attachment { get; set; } = null!;
        public string Type { get; set; } = null!;
    }
}