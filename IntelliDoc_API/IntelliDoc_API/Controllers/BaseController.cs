using IntelliDoc_API.Models;
using IntelliDoc_API.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntelliDoc_API.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly IConfiguration configuration;
        protected readonly UserService userService;
        protected readonly IntelliDocDBContext context;

        public BaseController(IConfiguration configuration, UserService userService, IntelliDocDBContext context)
        {
            this.configuration = configuration;
            this.userService = userService;
            this.context = context;
        }
    }
}