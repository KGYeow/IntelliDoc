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
        protected readonly RegExService regExService;

        public RepositoryController(IConfiguration configuration, UserService userService, ModelService modelService, RegExService regExService, IntelliDocDBContext context) : base(configuration, userService, context)
        {
            this.regExService = regExService;
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
            var l = from doc in context.Documents.Include(a => a.ModifiedBy).Where(a => a.IsAllVersionsArchived == false && a.IsRelatedDoc == false)
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

        // Get the list of related documents of a repository document.
        [HttpGet]
        [Route("RelatedDocs/{DocId}")]
        public IActionResult GetRelatedDocuments(int docId)
        {
            var l = context.DocumentRelationships.Include(a => a.DocumentRelated)
                .Where(a => a.DocumentMainId == docId && a.DocumentRelated.IsAllVersionsArchived == false)
                .Select(x => new
                {
                    id = x.DocumentRelatedId,
                    name = x.DocumentRelated.Name,
                    description = x.DocumentRelated.Description,
                    currentVersion = x.DocumentRelated.CurrentVersion,
                    modifiedById = x.DocumentRelated.ModifiedById,
                    modifiedBy = x.DocumentRelated.ModifiedBy.FullName,
                    modifiedDate = x.DocumentRelated.ModifiedDate,
                    type = x.DocumentRelated.Type,
                }).ToList();
/*            var l = from doc in context.Documents.Include(a => a.ModifiedBy).Where(a => a.Id == docId)
                    join relation in context.DocumentRelationships.Where(a => a.DocumentMainId == docId)
                    on doc.Id equals relation.DocumentMainId into grouping
                    from relation in grouping.DefaultIfEmpty()
                    select new
                    {
                        id = doc.Id,
                        name = doc.Name,
                        description = doc.Description,
                        currentVersion = doc.CurrentVersion,
                        modifiedById = doc.ModifiedById,
                        modifiedBy = doc.ModifiedBy.FullName,
                        modifiedDate = doc.ModifiedDate,
                        type = doc.Type,
                    };*/
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
                IsAllVersionsArchived = false,
                IsRelatedDoc = false
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

            if (dto.RelatedDoc.Count != 0)
            {
                dto.RelatedDoc.ForEach(doc =>
                {
                    var relatedDocument = new Document
                    {
                        Name = doc.Name,
                        Category = "Others",
                        CurrentVersion = 1,
                        CreatedById = user.Id,
                        CreatedDate = DateTime.Now,
                        ModifiedById = user.Id,
                        ModifiedDate = DateTime.Now,
                        Type = doc.Type,
                        HaveArchivedDocVersion = false,
                        IsAllVersionsArchived = false,
                        IsRelatedDoc = true
                    };
                    context.Documents.Add(relatedDocument);
                    context.SaveChanges();

                    context.DocumentRelationships.Add(new DocumentRelationship
                    {
                        DocumentMainId = document.Id,
                        DocumentRelatedId = relatedDocument.Id,
                    });　
                    context.SaveChanges();

                    var versionHistory = new DocumentVersionHistory
                    {
                        DocumentId = relatedDocument.Id,
                        Version = 1,
                        ModifiedById = user.Id,
                        ModifiedDate = DateTime.Now,
                        Attachment = dto.Attachment,
                        IsArchived = false
                    };
                    context.DocumentVersionHistories.Add(versionHistory);
                    context.SaveChanges();
                });
            }

            return Ok(new Response { Status = "Success", Message = "New document created successfully" });
        }

        // Find the matched patterns in the document for adding related documents.
        [HttpPut]
        [Route("FindPatterns")]
        public IActionResult FindPatterns([FromBody] RepositoryFindPatterns dto)
        {
            var matchedPatternList = regExService.FindPattern(dto.Attachment, dto.Name);
            return Ok(matchedPatternList);
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
            var existingDocName = context.Documents.Where(d => d.Name == name && d.Id != docId).Any();

            if (existingDoc.Name == name)
                throw new Exception("The new document name is the same as the old");
            if (existingDocName)
                throw new Exception("The document name already exists");

            existingDoc.Name = name;
            context.Documents.Update(existingDoc);
            context.SaveChanges();

            return Ok(new Response { Status = "Success", Message = "Existing document renamed successfully" });
        }

        // Edit the description of existing document.
        [HttpPut]
        [Route("Edit/{DocId}")]
        public IActionResult Edit(int docId, [FromBody] RepositoryEdit dto)
        {
            var user = userService.GetUser(User);
            var existingDoc = context.Documents.Where(d => d.Id == docId).FirstOrDefault();
            var existingDocName = context.Documents.Where(d => d.Name == dto.Name && d.Id != docId).Any();
            
            if (existingDocName)
                throw new Exception("The document name already exists");

            existingDoc.Name = dto.Name;
            existingDoc.Category = dto.Category;
            existingDoc.Description = dto.Description;
            context.Documents.Update(existingDoc);
            context.SaveChanges();
            return Ok(new Response { Status = "Success", Message = "The document edited successfully" });
        }

        // Archive the existing document or its specific version.
        [HttpPut]
        [Route("Archive/{DocId}/{Version}")]
        public IActionResult Archive(int docId, int version)
        {
            var user = userService.GetUser(User);
            var existingDoc = context.Documents.Where(d => d.Id == docId).FirstOrDefault();
            existingDoc.HaveArchivedDocVersion = true;

            if (version == 0) // All versions.
            {
                var existingDocUserActions = context.DocumentUserActions.Where(d => d.DocumentId == docId).ToList();
                existingDocUserActions.ForEach(doc => { doc.IsFlagged = false; });
                context.DocumentUserActions.UpdateRange(existingDocUserActions);

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