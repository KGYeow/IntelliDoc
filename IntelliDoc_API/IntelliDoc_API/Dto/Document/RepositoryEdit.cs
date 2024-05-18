using System.ComponentModel.DataAnnotations;

namespace IntelliDoc_API.Dto.Document
{
    public class RepositoryEdit
    {
        public string? Name { set; get; }
        public string? Category { set; get; }
        public string? Description { get; set; }
    }
}