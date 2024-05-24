using System.ComponentModel.DataAnnotations;

namespace IntelliDoc_API.Dto.Document
{
    public class RepositoryCreate
    {
        public string Name { get; set; } = null!;
        public byte[] Attachment { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int? MainDocId { get; set; }
        public List<RepositoryCreate>? RelatedDoc { get; set; }

    }
}