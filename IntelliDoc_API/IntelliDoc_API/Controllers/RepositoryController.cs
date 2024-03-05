﻿using IntelliDoc_API.Authentication;
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
        public RepositoryController(IConfiguration configuration, UserService userService, IntelliDocDBContext context) : base(configuration, userService, context)
        {
        }

        // Get the options for filters.
        [HttpGet]
        [Route("FilterOption")]
        public IActionResult GetRepositoryFilterOption()
        {
            var docNameList = context.Documents.Where(a => a.IsAllVersionsArchived == false).ToList()
                .OrderBy(a => a.Name).GroupBy(a => a.Name).Select(a => a.Key);
            var docCategoryList = context.DocumentCategories.ToList()
                .OrderBy(a => a.Name).GroupBy(a => a.Name).Select(a => a.Key);

            return Ok(new { docNameList, docCategoryList });
        }

        // Get the filtered repository list.
        [HttpGet]
        [Route("Filter")]                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        
        public IActionResult GetFilteredRepository([FromQuery] RepositoryFilter dto)
        {
            var l = context.Documents
                .Include(a => a.Category).Include(a => a.ModifiedBy)
                .Where(a => a.IsAllVersionsArchived == false)
                .Select(x => new
                {
                    id = x.Id,
                    name = x.Name,
                    category = x.Category.Name,
                    modifiedById = x.ModifiedById,
                    modifiedBy = x.ModifiedBy.FullName,
                    modifiedDate = x.ModifiedDate,
                });

            if (dto.DocId != null)
                l = l.Where(a => a.id == dto.DocId);
            if (dto.Category != null)
                l = l.Where(a => a.category == dto.Category);
            l.ToList();

            return Ok(l);
        }

        // Get the list of version history of a repository document.
        [HttpGet]
        [Route("VersionHistory/{DocId}")]
        public IActionResult GetDocumentVersionHistory(int docId)
        {
            var l = context.DocumentVersionHistories.Where(d => d.DocumentId == docId && d.IsArchived == false).ToList();
            return Ok(l);
        }

        // Get the document attachment.
        [HttpGet]
        [Route("GetAttachment/{DocId}/{Version}")]
        public IActionResult GetDocumentAttachment(int docId, string version)
        {
            var document = new DocumentVersionHistory();

            if (version == "Latest")
                document = context.DocumentVersionHistories.Where(d => d.DocumentId == docId).OrderByDescending(d => d.Id).FirstOrDefault();
            else
                document = context.DocumentVersionHistories.Where(d => d.DocumentId == docId && d.Version == version).FirstOrDefault();

            if (document == null)
                throw new Exception("The document is not found in the system!");

            return Ok(new { document.Attachment, document.Type });
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

            var document = new Document
            {
                Name = dto.Name,
                CategoryId = 1,
                CreatedById = user.Id,
                CreatedDate = DateTime.Now,
                ModifiedById = user.Id,
                ModifiedDate = DateTime.Now,
                HaveArchivedDocVersion = false, 
                IsAllVersionsArchived = false
            };
            context.Documents.Add(document);
            context.SaveChanges();

            var versionHistory = new DocumentVersionHistory
            {
                DocumentId = document.Id,
                Version = "1.0",
                Attachment = dto.Attachment,
                Type = dto.Type,
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

            string[] versionComponents = previousDocVersion.Version.Split('.');
            int major = int.Parse(versionComponents[0]);
            int minor = int.Parse(versionComponents[1]);

            var newVersion = "";
            if (dto.UpdateDegree == "Major")
                newVersion = $"{++major}.0";
            else
                newVersion = $"{major}.{++minor}";

            var existingDoc = context.Documents.Where(d => d.Id == docId).FirstOrDefault();
            existingDoc.ModifiedById = user.Id;
            existingDoc.ModifiedDate = DateTime.Now;
            context.Documents.Update(existingDoc);
            context.SaveChanges();

            var versionHistory = new DocumentVersionHistory
            {
                DocumentId = previousDocVersion.DocumentId,
                Version = newVersion,
                UpdatedById = user.Id,
                UpdatedDate = DateTime.Now,
                Attachment = dto.Attachment,
                Type = dto.Type,
                IsArchived = false
            };
            context.DocumentVersionHistories.Add(versionHistory);
            context.SaveChanges();

            return Ok(new Response { Status = "Success", Message = "Existing document updated successfully" });
        }

        // Archive the existing document or its specific version.
        [HttpPut]
        [Route("Archive/{DocId}/{Version}")]
        public IActionResult Archive(int docId, string version)
        {
            var user = userService.GetUser(User);
            var existingDoc = context.Documents.Where(d => d.Id == docId).FirstOrDefault();
            existingDoc.HaveArchivedDocVersion = true;

            if (version == "All")
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

                context.Documents.Update(existingDoc);
                context.SaveChanges();

                var existingVersionHistory = context.DocumentVersionHistories.Where(d => d.DocumentId == docId && d.Version == version).FirstOrDefault();
                existingVersionHistory.ArchivedDate = DateTime.Now;
                existingVersionHistory.IsArchived = true;
                context.DocumentVersionHistories.Update(existingVersionHistory);
                context.SaveChanges();

                return Ok(new Response { Status = "Success", Message = "Specific document version archived successfully" });
            }
        }
    }
}