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
    public class RepositoryController : BaseController
    {
        protected readonly ModelService modelService;

        public RepositoryController(IConfiguration configuration, UserService userService, ModelService modelService, IntelliDocDBContext context) : base(configuration, userService, context)
        {
            this.modelService = modelService;
        }

        // Get the options for filters.
        [HttpGet]
        [Route("FilterOption")]
        public IActionResult GetRepositoryFilterOption()
        {
            var docNameList = context.Documents.Where(a => a.IsAllVersionsArchived == false).ToList()
                .OrderBy(a => a.Name).Select(x => new { id = x.Id, name = x.Name, type = x.Type });
            var docCategoryList = modelService.GetCategoryList().ToList().OrderBy(a => a);

            return Ok(new { docNameList, docCategoryList });
        }

        // Get the filtered repository list.
        [HttpGet]
        [Route("Filter")]                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        
        public IActionResult GetFilteredRepository([FromQuery] RepositoryFilter dto)
        {
            var user = userService.GetUser(User);
            var l = from doc in context.Documents.Include(a => a.ModifiedBy).Where(a => a.IsAllVersionsArchived == false)
                    join act in context.DocumentUserActions.Where(a => a.UserId == user.Id)
                    on doc.Id equals act.DocumentId into grouping
                    from act in grouping.DefaultIfEmpty()
                    select new
                    {
                        id = doc.Id,
                        name = doc.Name,
                        description = doc.Description,
                        category = doc.Category,
                        currentVersion = doc.CurrentVersion,
                        modifiedById = doc.ModifiedById,
                        modifiedBy = doc.ModifiedBy.FullName,
                        modifiedDate = doc.ModifiedDate,
                        type = doc.Type,
                        isFlagged = act != null ? act.IsFlagged : false
                    };

            if (dto.DocId != null)
                l = l.Where(a => a.id == dto.DocId);
            if (dto.Category != null)
                l = l.Where(a => a.category.Contains(dto.Category));
            if (dto.Type != null)
                l = l.Where(a => a.type == dto.Type);
            l.ToList();

            return Ok(l);
        }

        // Get the list of version history of a repository document.
        [HttpGet]
        [Route("VersionHistory/{DocId}")]
        public IActionResult GetDocumentVersionHistory(int docId)
        {
            var l = context.DocumentVersionHistories.Include(a => a.ModifiedBy)
                .Where(d => d.DocumentId == docId && d.IsArchived == false)
                .Select(x => new
                {
                    id = x.Id,
                    version = x.Version,
                    modifiedById = x.ModifiedById,
                    modifiedBy = x.ModifiedBy.FullName,
                    modifiedDate = x.ModifiedDate,
                }).ToList();
            return Ok(l);
        }

        // Get the document attachment.
        [HttpGet]
        [Route("GetAttachment/{DocId}/{Version}")]
        public IActionResult GetDocumentAttachment(int docId, int version)
        {
            var document = new DocumentVersionHistory();
            var documentVersions = context.DocumentVersionHistories.Where(d => d.DocumentId == docId && d.IsArchived == false).ToList();

            if (version == 0) // Get the latest version
                document = documentVersions.OrderByDescending(d => d.Id).FirstOrDefault();
            else
                document = documentVersions.Where(d => d.Version == version).FirstOrDefault();

            if (document == null)
                throw new Exception("The document is not found in the system!");

            return Ok(document.Attachment);
        }

        // Classify the document.
        [HttpPost]
        [Route("Classify")]
        public IActionResult PredictDocumentClasses([FromBody] RepositoryCreate dto)
        {
            var classification = modelService.Classify(dto.Attachment, dto.Name);
            return Ok(classification);
        }

        // Upload and create the new document.
        [HttpPost]
        [Route("")]
        public IActionResult Create([FromBody] RepositoryCreate dto)
        {
            var user = userService.GetUser(User);
            var existingDoc = context.Documents.Where(a => a.Name == dto.Name).Any();

            if (existingDoc)
                throw new Exception("The document name already exists");

            var classification = modelService.Classify(dto.Attachment, dto.Name);
            string assignedCategories = string.Join(", ", classification.OrderBy(a => a));

            var document = new Document
            {
                Name = dto.Name,
                Category = assignedCategories == "" ? "Others" : assignedCategories,
                CurrentVersion = 1,
                CreatedById = user.Id,
                CreatedDate = DateTime.Now,
                ModifiedById = user.Id,
                ModifiedDate = DateTime.Now,
                Type = dto.Type,
                HaveArchivedDocVersion = false, 
                IsAllVersionsArchived = false
            };
            context.Documents.Add(document);
            context.SaveChanges();

            var versionHistory = new DocumentVersionHistory
            {
                DocumentId = document.Id,
                Version = 1,
                ModifiedById = user.Id,
                ModifiedDate = DateTime.Now,
                Attachment = dto.Attachment,
                IsArchived = false
            };
            context.DocumentVersionHistories.Add(versionHistory);
            context.SaveChanges();

            return Ok(new Response { Status = "Success", Message = "New document created successfully" });
        }

        // Edit and update the existing document.
        [HttpPut]
        [Route("{DocId}")]
        public IActionResult Update(int docId, [FromBody] RepositoryUpdate dto)
        {
            var user = userService.GetUser(User);
            var previousDocVersion = context.DocumentVersionHistories.Where(d => d.DocumentId == docId).OrderByDescending(d => d.Id).FirstOrDefault();

            var existingDoc = context.Documents.Where(d => d.Id == docId).FirstOrDefault();
            existingDoc.CurrentVersion = previousDocVersion.Version + 1;
            existingDoc.ModifiedById = user.Id;
            existingDoc.ModifiedDate = DateTime.Now;
            context.Documents.Update(existingDoc);
            context.SaveChanges();

            var versionHistory = new DocumentVersionHistory
            {
                DocumentId = previousDocVersion.DocumentId,
                Version = previousDocVersion.Version + 1,
                ModifiedById = user.Id,
                ModifiedDate = DateTime.Now,
                Attachment = dto.Attachment,
                IsArchived = false
            };
            context.DocumentVersionHistories.Add(versionHistory);
            context.SaveChanges();

            return Ok(new Response { Status = "Success", Message = "Existing document updated successfully" });
        }

        // Rename the existing document.
        [HttpPut]
        [Route("Rename/{DocId}/{Name}")]
        public IActionResult Rename(int docId, string name)
        {
            var user = userService.GetUser(User);
            var existingDoc = context.Documents.Where(d => d.Id == docId).FirstOrDefault();

            if (existingDoc.Name == name)
                throw new Exception("The new document name is the same as the old");
            else
            {
                existingDoc.Name = name;
                context.Documents.Update(existingDoc);
                context.SaveChanges();

                return Ok(new Response { Status = "Success", Message = "Existing document renamed successfully" });
            }
        }

        // Edit the description of existing document.
        [HttpPut]
        [Route("Description/{DocId}")]
        public IActionResult EditDescription(int docId, [FromBody] RepositoryDescription dto)
        {
            var user = userService.GetUser(User);
            var existingDoc = context.Documents.Where(d => d.Id == docId).FirstOrDefault();
            existingDoc.Description = dto.Description;
            context.Documents.Update(existingDoc);
            context.SaveChanges();
            return Ok(new Response { Status = "Success", Message = "Description edited successfully" });
        }

        // Archive the existing document or its specific version.
        [HttpPut]
        [Route("Archive/{DocId}/{Version}")]
        public IActionResult Archive(int docId, int version)
        {
            var user = userService.GetUser(User);
            var existingDoc = context.Documents.Where(d => d.Id == docId).FirstOrDefault();
            existingDoc.HaveArchivedDocVersion = true;

/*            var existingDocUserActions = context.DocumentUserActions.Where(d => d.DocumentId == docId).ToList();
            existingDocUserActions.ForEach(doc => { doc.IsFlagged = false; });
            context.DocumentUserActions.UpdateRange(existingDocUserActions);*/

            if (version == 0) // All versions.
            {
                existingDoc.IsAllVersionsArchived = true;
                context.Documents.Update(existingDoc);
                context.SaveChanges();

                var existingVersionHistories = context.DocumentVersionHistories.Where(d => d.DocumentId == docId).ToList();
                var archivedDate = DateTime.Now;
                foreach (var versionHistory in existingVersionHistories)
                {
                    versionHistory.ArchivedDate = archivedDate;
                    versionHistory.IsArchived = true;
                    context.DocumentVersionHistories.Update(versionHistory);
                    context.SaveChanges();
                }

                return Ok(new Response { Status = "Success", Message = "Existing document archived successfully" });
            }
            else
            {
                var existingVersionHistory = context.DocumentVersionHistories.Where(d => d.DocumentId == docId && d.Version == version).FirstOrDefault();
                existingVersionHistory.ArchivedDate = DateTime.Now;
                existingVersionHistory.IsArchived = true;
                context.DocumentVersionHistories.Update(existingVersionHistory);
                context.SaveChanges();

                var latestDocVersion = context.DocumentVersionHistories.Where(d => d.DocumentId == docId && d.IsArchived == false)
                    .OrderByDescending(d => d.Id)
                    .FirstOrDefault();

                existingDoc.CurrentVersion = latestDocVersion.Version;
                existingDoc.ModifiedById = latestDocVersion.ModifiedById;
                existingDoc.ModifiedDate = latestDocVersion.ModifiedDate;

                // If the all the versions are archived.
                var isNoArchivedVersionExist = context.DocumentVersionHistories.Where(d => d.DocumentId == docId && d.IsArchived == false).Any();
                if (!isNoArchivedVersionExist)
                    existingDoc.IsAllVersionsArchived = true;
                context.Documents.Update(existingDoc);
                context.SaveChanges();

                return Ok(new Response { Status = "Success", Message = "Specific document version archived successfully" });
            }
        }
    }
}