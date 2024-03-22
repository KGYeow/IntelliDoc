using IntelliDoc_API.Authentication;
using IntelliDoc_API.Models;
using IntelliDoc_API.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntelliDoc_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : BaseController
    {
        public DashboardController(IConfiguration configuration, UserService userService, IntelliDocDBContext context) : base(configuration, userService, context)
        {
        }

        // Get the Dashboard data.
        [HttpGet]
        [Route("DashboardData")]
        public IActionResult DashboardData()
        {
            var docCategory = context.DocumentCategories.Select(d => d.Name).ToList();

            var storedDocNum = new List<int>();
            for (int i = 1; i <= docCategory.Count; i++)
                storedDocNum.Add(context.Documents.Where(d => d.IsAllVersionsArchived == false && d.CategoryId == i).Count());

            var archivedDocNum = new List<int>();
            for (int i = 1; i <= docCategory.Count; i++)
                archivedDocNum.Add(context.Documents.Where(d => d.HaveArchivedDocVersion == true && d.CategoryId == i).Count());

            var yAxisMax = storedDocNum.Concat(archivedDocNum).Max() < 10 ? 10 : storedDocNum.Concat(archivedDocNum).Max();

            var totalStoredDoc = context.Documents.Where(d => d.IsAllVersionsArchived == false).Count();

            var storedDoc = new List<StoredDoc>();
            for (int i = 0; i < docCategory.Count; i++)
                storedDoc.Add(new StoredDoc { Category = docCategory[i], Frequency = storedDocNum[i] });
            var top3Categories = storedDoc.OrderByDescending(d => d.Frequency).Take(3).ToList();
            var top3CategoriesName = top3Categories.Select(d => d.Category).ToList();
            var top3CategoriesFrequency = top3Categories.Select(d => d.Frequency).ToList();

            return Ok(new { docCategory, storedDocNum, archivedDocNum, yAxisMax, totalStoredDoc, top3CategoriesName, top3CategoriesFrequency });
        }

        public class StoredDoc
        {
            public string Category { get; set; } = null!;
            public int Frequency { get; set; }
        }
    }
}