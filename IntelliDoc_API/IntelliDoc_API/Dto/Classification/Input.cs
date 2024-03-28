using System.ComponentModel.DataAnnotations;

namespace IntelliDoc_API.Dto.Classification
{
    public class Input
    {
        public string DocPath { get; set; } = null!;
        public string DocName { get; set; } = null!;
    }
}