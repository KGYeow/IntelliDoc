using IntelliDoc_API.Authentication;
using IntelliDoc_API.Dto.Document;
using IntelliDoc_API.Models;
using IntelliDoc_API.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntelliDoc_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArchiveController : BaseController
    {
        public ArchiveController(IConfiguration configuration, UserService userService, IntelliDocDBContext context) : base(configuration, userService, context)
        {
        }

        // Get the options for filters.
        [HttpGet]
        [Route("FilterOption")]
        public IActionResult GetArchiveFilterOption()
        {
            var docNameList = context.Documents.Where(a => a.HaveArchivedDocVersion == true).ToList()
                .OrderBy(a => a.Name).GroupBy(a => a.Name).Select(a => a.Key);
            var docCategoryList = context.DocumentCategories.ToList()
                .OrderBy(a => a.Name).GroupBy(a => a.Name).Select(a => a.Key);

            return Ok(new { docNameList, docCategoryList });
        }

        // Get the filtered archive list.
        [HttpGet]
        [Route("Filter")]
        public IActionResult GetFilteredArchive([FromQuery] RepositoryFilter dto)
        {
            var l = context.Documents.Include(a => a.Category).Where(a => a.HaveArchivedDocVersion == false).OrderBy(a => a.Name)
                .Select(x => new { id = x.Id, name = x.Name, category = x.Category.Name });

            if (dto.DocId != null)
                l = l.Where(a => a.id == dto.DocId);
            if (dto.Category != null)
                l = l.Where(a => a.category == dto.Category);
            l.ToList();

            return Ok(l);
        }

        // Get the list of version history of a archived document.
        [HttpGet]
        [Route("VersionHistory/{DocId}")]
        public IActionResult GetDocumentVersionHistory(int docId)
        {
            var l = context.DocumentVersionHistories.Where(d => d.DocumentId == docId && d.IsArchived == true).ToList();
            return Ok(l);
        }

        // Restore the archived document or its specific version.
        [HttpPut]
        [Route("Restore/{DocId}/{Version}")]
        public IActionResult Restore(int docId, string version)
        {
            var user = userService.GetUser(User);
            var archivedDoc = context.Documents.Where(d => d.Id == docId).FirstOrDefault();
            
            if (version == "All")
            {
                archivedDoc.HaveArchivedDocVersion = false;
                archivedDoc.IsAllVersionsArchived = false;
                context.Documents.Update(archivedDoc);
                context.SaveChanges();

                var archivedVersionHistories = context.DocumentVersionHistories.Where(d => d.DocumentId == docId && d.IsArchived == true).ToList();
                foreach (var versionHistory in archivedVersionHistories)
                {
                    versionHistory.ArchivedDate = null;
                    versionHistory.IsArchived = false;
                    context.DocumentVersionHistories.Update(versionHistory);
                    context.SaveChanges();
                }

                return Ok(new Response { Status = "Success", Message = "Archived document restored successfully" });
            }
            else
            {
                var archivedVersionHistory = context.DocumentVersionHistories.Where(d => d.DocumentId == docId && d.Version == version).FirstOrDefault();
                archivedVersionHistory.ArchivedDate = null;
                archivedVersionHistory.IsArchived = false;
                context.DocumentVersionHistories.Update(archivedVersionHistory);
                context.SaveChanges();

                // If the all the versions are restored.
                var isArchivedVersionExist = context.DocumentVersionHistories.Where(d => d.DocumentId == docId && d.IsArchived == true).Any();
                if (!isArchivedVersionExist)
                {
                    archivedDoc.HaveArchivedDocVersion = false;
                    archivedDoc.IsAllVersionsArchived = false;
                    context.Documents.Update(archivedDoc);
                    context.SaveChanges();
                }

                return Ok(new Response { Status = "Success", Message = "Specific document version restored successfully" });
            }
        }
    }
}