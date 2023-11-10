using IntelliDoc_API.Models;
using IntelliDoc_API.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IntelliDoc_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : BaseController
    {
        public DocumentController(IConfiguration configuration, UserService userService, IntelliDocDBContext context) : base(configuration, userService, context)
        {
        }

        [HttpGet]
        [Route("FilterOption")]
        public IActionResult GetFilterOption()
        {
            var record = context.Documents.Where(a => a.IsAllVersionsArchived == false).ToList();
            var documentName = record.OrderBy(a => a.Name).GroupBy(a => a.Name).Select(a => a.Key);
            return Ok(new { documentName });
        }

        [HttpGet]
        [Route("Result")]
        public IActionResult Get([FromQuery] string? documentName = null, [FromQuery] string? category = null, [FromQuery] string? type = null)
        {
            var l = context.Documents.Include(a => a.Category).Where(a => a.IsAllVersionsArchived == false).ToList()
                .Select(x => new { x.Id, name = x.Name, version = x.Version, categoryId = x.CategoryId, category = x.Category.Name, createdBy = x.CreatedBy, createdDate = x.CreatedDate, updatedDate = x.UpdatedDate, type = x.Type });

            if (documentName != null)
                l = l.Where(a => a.name == documentName);
            if (category != null)
                l = l.Where(a => a.category == category);
            if (type != null)
                l = l.Where(a => a.type == type);
            l.ToList();

            return Ok(l);
        }

        [HttpGet]
        [Route("GetAttachment/{Id}")]
        public IActionResult GetAttachment(int id)
        {
            var document = context.Documents.Where(d => d.Id == id).FirstOrDefault();
            if (document.Attachment == null)
                throw new Exception("The document is not found in the system!");

            return Ok(document.Attachment);
        }

        [HttpPost]
        [Route("")]
        public IActionResult Create([FromBody] DocumentDto dto)
        {
            var user = userService.GetUser(User);
            var existingDoc = context.Documents.Where(a => a.Name == dto.DocumentName).Any();

            if (existingDoc)
                throw new Exception("Document Name already exist");

            var category = context.DocumentCategories.Where(a => a.Name == dto.Category).FirstOrDefault();

            var document = new Document();
            document.Name = dto.DocumentName;
            document.Version = "1.0";
            document.CategoryId = category.Id;
            document.CreatedBy = user.FullName;
            document.CreatedDate = DateTime.Now;
            document.Attachment = dto.Attachment;
            document.Type = dto.Type;
            document.HaveArchivedDocVersion = false;
            document.IsAllVersionsArchived = false;
            context.Documents.Add(document);
            context.SaveChanges();

            var versionHistory = new DocumentVersionHistory();
            versionHistory.DocumentId = document.Id;
            versionHistory.Version = document.Version;
            versionHistory.CategoryId = document.CategoryId;
            versionHistory.CreatedBy = user.FullName;
            versionHistory.CreatedDate = document.CreatedDate;
            versionHistory.Attachment = document.Attachment;
            versionHistory.Type = document.Type;
            versionHistory.IsArchived = false;
            context.DocumentVersionHistories.Add(versionHistory);
            context.SaveChanges();

            return Ok(document);
        }

        [HttpPut]
        [Route("{Id}")]
        public IActionResult Update(int id, [FromBody] DocumentEditDto dto)
        {
            var user = userService.GetUser(User);
            var data = context.Documents.Where(d => d.Id == id).FirstOrDefault();

            var newVersion = data.Version;
            string[] versionComponents = newVersion.Split('.');
            int major = int.Parse(versionComponents[0]);
            int minor = int.Parse(versionComponents[1]);

            if (dto.VersionUpdate == "Major")
                newVersion = $"{++major}.0";
            else
                newVersion = $"{major}.{++minor}";

            var newCategory = context.DocumentCategories.Where(a => a.Name == dto.Category).FirstOrDefault();
            var oldCategory = context.DocumentCategories.Where(a => a.Id == data.CategoryId).FirstOrDefault();

            var datachanges = "Document Name: " + data.Name;
            if (data.Version != newVersion)
            {
                datachanges += ", Version: [" + data.Version + " > " + newVersion + "]";
            }
            if (oldCategory.Id != newCategory.Id)
            {
                datachanges += ", Category: [" + oldCategory.Name + " > " + newCategory.Name + "]";
            }

            data.Version = newVersion;
            data.CategoryId = newCategory.Id;
            data.UpdatedBy = user.FullName;
            data.UpdatedDate = DateTime.Now;
            data.Attachment = dto.Attachment;
            data.Type = dto.Type;
            context.Documents.Update(data);
            context.SaveChanges();

            var versionHistory = new DocumentVersionHistory();
            versionHistory.DocumentId = data.Id;
            versionHistory.Version = newVersion;
            versionHistory.CategoryId = data.CategoryId;
            versionHistory.CreatedBy = data.CreatedBy;
            versionHistory.CreatedDate = data.CreatedDate;
            versionHistory.UpdatedBy = data.UpdatedBy;
            versionHistory.UpdatedDate = data.UpdatedDate;
            versionHistory.Attachment = data.Attachment;
            versionHistory.Type = data.Type;
            versionHistory.IsArchived = false;
            context.DocumentVersionHistories.Add(versionHistory);
            context.SaveChanges();

            return Ok(data);
        }

        [HttpPut]
        [Route("Archive/{Id}")]
        public IActionResult Archive(int id)
        {
            var user = userService.GetUser(User);
            var data = context.Documents.Where(a => a.Id == id).FirstOrDefault();
            var dataVersionHistory = context.DocumentVersionHistories.Where(a => a.DocumentId == id).ToList();

            var archivedDate = DateTime.Now;
            foreach (var versionHistory in dataVersionHistory)
            {
                versionHistory.LatestArchivedDate = archivedDate;
                versionHistory.IsArchived = true;
                context.DocumentVersionHistories.Update(versionHistory);
                context.SaveChanges();
            }

            data.LatestArchivedDate = archivedDate;
            data.HaveArchivedDocVersion = true;
            data.IsAllVersionsArchived = true;
            context.Documents.Update(data);
            context.SaveChanges();

            return Ok(data);
        }

        public class DocumentDto
        {
            [Required(ErrorMessage = "Document Name required")]
            public string? DocumentName { get; set; }
            [Required(ErrorMessage = "Description required")]
            public string? Description { get; set; }
            [Required(ErrorMessage = "Category required")]
            public string? Category { get; set; }
            [Required(ErrorMessage = "Attachment required")]
            public byte[]? Attachment { get; set; }
            [Required(ErrorMessage = "Document Type required")]
            public string? Type { get; set; }
        }

        public class DocumentEditDto
        {
            [Required(ErrorMessage = "Version Update required")]
            public string? VersionUpdate { get; set; }
            [Required(ErrorMessage = "Category required")]
            public string? Category { get; set; }
            [Required(ErrorMessage = "Attachment required")]
            public byte[]? Attachment { get; set; }
            [Required(ErrorMessage = "Document Type required")]
            public string? Type { get; set; }
        }
    }
}