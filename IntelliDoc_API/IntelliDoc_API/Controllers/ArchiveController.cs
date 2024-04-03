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
        protected readonly ModelService modelService;

        public ArchiveController(IConfiguration configuration, UserService userService, ModelService modelService, IntelliDocDBContext context) : base(configuration, userService, context)
        {
            this.modelService = modelService;
        }

        // Get the options for filters.
        [HttpGet]
        [Route("FilterOption")]
        public IActionResult GetArchiveFilterOption()
        {
            var docNameList = context.Documents.Where(a => a.HaveArchivedDocVersion == true).ToList()
                .OrderBy(a => a.Name).Select(x => new { id = x.Id, name = x.Name, type = x.Type });
            var docCategoryList = modelService.GetCategoryList().ToList().OrderBy(a => a);

            return Ok(new { docNameList, docCategoryList });
        }

        // Get the filtered archive list.
        [HttpGet]
        [Route("Filter")]
        public IActionResult GetFilteredArchive([FromQuery] RepositoryFilter dto)
        {
            var l = context.Documents.Where(a => a.HaveArchivedDocVersion == true).OrderBy(a => a.Name)
                .Select(x => new { id = x.Id, name = x.Name, category = x.Category, type = x.Type });

            if (dto.DocId != null)
                l = l.Where(a => a.id == dto.DocId);
            if (dto.Category != null)
                l = l.Where(a => a.category.Contains(dto.Category));
            if (dto.Type != null)
                l = l.Where(a => a.type == dto.Type);
            l.ToList();

            return Ok(l);
        }

        // Get the list of version history of a archived document.
        [HttpGet]
        [Route("VersionHistory/{DocId}")]
        public IActionResult GetDocumentVersionHistory(int docId)
        {
            var l = context.DocumentVersionHistories
                .Where(d => d.DocumentId == docId && d.IsArchived == true)
                .Select(x => new
                {
                    id = x.Id,
                    version = x.Version,
                    archivedDate = x.ArchivedDate,
                }).ToList();
            return Ok(l);
        }

        // Restore the archived document or its specific version.
        [HttpPut]
        [Route("Restore/{DocId}/{Version}")]
        public IActionResult Restore(int docId, int version)
        {
            var user = userService.GetUser(User);
            var archivedDoc = context.Documents.Where(d => d.Id == docId).FirstOrDefault();
            archivedDoc.IsAllVersionsArchived = false;

            if (version == 0) // All versions.
            {
                var archivedVersionHistories = context.DocumentVersionHistories.Where(d => d.DocumentId == docId && d.IsArchived == true).ToList();
                foreach (var archivedDocVersion in archivedVersionHistories)
                {
                    archivedDocVersion.ArchivedDate = null;
                    archivedDocVersion.IsArchived = false;
                    context.DocumentVersionHistories.Update(archivedDocVersion);
                    context.SaveChanges();
                }

                var latestDocVersion = context.DocumentVersionHistories.Where(d => d.DocumentId == docId && d.IsArchived == false)
                    .OrderByDescending(d => d.Id)
                    .FirstOrDefault();

                archivedDoc.HaveArchivedDocVersion = false;
                archivedDoc.ModifiedById = latestDocVersion.ModifiedById;
                archivedDoc.ModifiedDate = latestDocVersion.ModifiedDate;
                context.Documents.Update(archivedDoc);
                context.SaveChanges();

                return Ok(new Response { Status = "Success", Message = "Archived document restored successfully" });
            }
            else
            {
                var archivedDocVersion = context.DocumentVersionHistories.Where(d => d.DocumentId == docId && d.Version == version).FirstOrDefault();
                archivedDocVersion.ArchivedDate = null;
                archivedDocVersion.IsArchived = false;
                context.DocumentVersionHistories.Update(archivedDocVersion);
                context.SaveChanges();

                // If the all the versions are restored.
                var isArchivedVersionExist = context.DocumentVersionHistories.Where(d => d.DocumentId == docId && d.IsArchived == true).Any();
                if (!isArchivedVersionExist)
                    archivedDoc.HaveArchivedDocVersion = false;

                var latestDocVersion = context.DocumentVersionHistories.Where(d => d.DocumentId == docId && d.IsArchived == false)
                    .OrderByDescending(d => d.Id)
                    .FirstOrDefault();

                archivedDoc.ModifiedById = latestDocVersion.ModifiedById;
                archivedDoc.ModifiedDate = latestDocVersion.ModifiedDate;
                context.Documents.Update(archivedDoc);
                context.SaveChanges();

                return Ok(new Response { Status = "Success", Message = "Specific document version restored successfully" });
            }
        }

        // Delete the archived document or its specific version permanently.
        [HttpDelete]
        [Route("Delete/{DocId}/{Version}")]
        public IActionResult DeletePermanently(int docId, int version)
        {
            var user = userService.GetUser(User);
            var existingDoc = context.Documents.Where(d => d.Id == docId).FirstOrDefault();
            var archivedVersionHistories = context.DocumentVersionHistories.Where(d => d.DocumentId == docId && d.IsArchived == true).ToList();
            string msg;

            if (version == 0) // All archived versions.
            {
                context.DocumentVersionHistories.RemoveRange(archivedVersionHistories);
                context.SaveChanges();

                existingDoc.HaveArchivedDocVersion = false;
                existingDoc.IsAllVersionsArchived = false;
                context.Documents.Update(existingDoc);
                context.SaveChanges();

                msg = "Archived document delete forever successfully";
            }
            else
            {
                var archivedDocVersion = archivedVersionHistories.Where(d => d.Version == version).FirstOrDefault();
                context.DocumentVersionHistories.Remove(archivedDocVersion);
                context.SaveChanges();

                var isArchivedDocVersionExist = context.DocumentVersionHistories.Where(d => d.DocumentId == docId && d.IsArchived == true).Any();
                if (!isArchivedDocVersionExist)
                {
                    existingDoc.HaveArchivedDocVersion = false;
                    existingDoc.IsAllVersionsArchived = false;
                    context.Documents.Update(existingDoc);
                    context.SaveChanges();
                }

                msg = "Specific archived document version delete forever successfully";
            }

            var isDocVersionExist = context.DocumentVersionHistories.Where(d => d.DocumentId == docId).Any();
            if (!isDocVersionExist)
            {
                existingDoc = context.Documents.Where(d => d.Id == docId).FirstOrDefault();
                context.Documents.Remove(existingDoc);
                context.SaveChanges();
            }

            return Ok(new Response { Status = "Success", Message = msg });
        }
    }
}