using System.ComponentModel.DataAnnotations;

namespace IntelliDoc_API.Dto.Document
{
    public class RepositoryFilter
    {
        public int? DocId { get; set; }
        public string? Category { get; set; }
        public string? Type { get; set;}
    }
}