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
    public class FlagController : BaseController
    {
        protected readonly ModelService modelService;

        public FlagController(IConfiguration configuration, UserService userService, ModelService modelService, IntelliDocDBContext context) : base(configuration, userService, context)
        {
            this.modelService = modelService;
        }

        // Get the options for filters.
        [HttpGet]
        [Route("FilterOption")]
        public IActionResult GetFlagFilterOption()
        {
            var user = userService.GetUser(User);
            var docNameList = context.DocumentUserActions
                .Include(a => a.Document).Where(a => a.UserId == user.Id && a.IsFlagged == true)
                .ToList().OrderBy(a => a.Document.Name)
                .Select(x => new { id = x.DocumentId, name = x.Document.Name, type = x.Document.Type });
            var docCategoryList = modelService.GetCategoryList().ToList().OrderBy(a => a);

            return Ok(new { docNameList, docCategoryList });
        }

        // Get the filtered flagged document list.
        [HttpGet]
        [Route("Filter")]
        public IActionResult GetFilteredFlaggedDocuments([FromQuery] RepositoryFilter dto)
        {
            var user = userService.GetUser(User);
            var l = context.DocumentUserActions
                .Include(a => a.Document).Include(a => a.Document.ModifiedBy)
                .Where(a => a.UserId == user.Id && a.IsFlagged == true)
                .Select(x => new
                {
                    id = x.DocumentId,
                    name = x.Document.Name,
                    description = x.Document.Description,
                    category = x.Document.Category,
                    currentVersion = x.Document.CurrentVersion,
                    modifiedById = x.Document.ModifiedById,
                    modifiedBy = x.Document.ModifiedBy.FullName,
                    modifiedDate = x.Document.ModifiedDate,
                    type = x.Document.Type,
                    isFlagged = x.IsFlagged
                });

            if (dto.DocId != null)
                l = l.Where(a => a.id == dto.DocId);
            if (dto.Category != null)
                l = l.Where(a => a.category.Contains(dto.Category));
            if (dto.Type != null)
                l = l.Where(a => a.type == dto.Type);
            l.ToList();

            return Ok(l);
        }

        // Flag/mark the existing document.
        [HttpPut]
        [Route("{DocId}")]
        public IActionResult Flag(int docId)
        {
            var user = userService.GetUser(User);
            var existingDoc = context.DocumentUserActions.Where(d => d.DocumentId == docId && d.UserId == user.Id).FirstOrDefault();
            string msg;

            if (existingDoc == null)
            {
                var document = new DocumentUserAction
                {
                    DocumentId = docId,
                    UserId = user.Id,
                    IsFlagged = true
                };
                context.DocumentUserActions.Add(document);
                context.SaveChanges();
                msg = "Document has been flagged successfully";
            }
            else
            {
                existingDoc.IsFlagged = !existingDoc.IsFlagged;
                context.DocumentUserActions.Update(existingDoc);
                context.SaveChanges();

                if (existingDoc.IsFlagged)
                    msg = "Document has been flagged successfully";
                else
                    msg = "Document has been unflagged successfully";
            }
            return Ok(new Response { Status = "Success", Message = msg });
        }   
    }
}