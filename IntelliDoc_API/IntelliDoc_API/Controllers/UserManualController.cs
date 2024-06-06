using DocumentFormat.OpenXml.Spreadsheet;
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
    public class UserManualController : BaseController
    {
        public UserManualController(IConfiguration configuration, UserService userService, IntelliDocDBContext context) : base(configuration, userService, context)
        {
        }

        // Get the user manual document list.
        [HttpGet]
        [Route("")]
        public IActionResult GetManualDocumentList()
        {
            var l = context.UserManualDocuments.ToList();
            return Ok(l);
        }

        // Get the user manual document.
        [HttpGet]
        [Route("GetManualDocument/{DocName}")]
        public IActionResult GetManualDocument(string docName)
        {
            var document = context.UserManualDocuments.Where(d => d.Name == docName).FirstOrDefault();
            if (document == null)
                throw new Exception("The document, " + docName + ", is not found in the system!");
            return Ok(document);
        }

        // Add the user manual document.
        [HttpPost]
        [Route("")]
        public IActionResult AddManualDocument([FromBody] RepositoryCreate dto)
        {
            var existingDoc = context.UserManualDocuments.Where(a => a.Name == dto.Name).Any();

            if (existingDoc)
                throw new Exception("The document name already exists");

            var document = new UserManualDocument
            {
                Name = dto.Name,
                Type = dto.Type,
                Attachment = dto.Attachment
            };
            context.UserManualDocuments.Add(document);
            context.SaveChanges();

            return Ok(new Response { Status = "Success", Message = "New user manual document created successfully" });
        }
    }
}