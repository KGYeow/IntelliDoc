﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IntelliDoc_API.Service;
using IntelliDoc_API.Dto.Authentication;
using IntelliDoc_API.Authentication;
using IntelliDoc_API.Models;
using IntelliDoc_API;

namespace RnD_Traceability_System_API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        IConfiguration configuration;
        UserService userService;
        private readonly IntelliDocDBContext context;
        public AuthenticateController(IConfiguration configuration, UserService userService, IntelliDocDBContext context)
        {
            this.configuration = configuration;
            this.userService = userService;
            this.context = context;
        }

        [HttpPost]
        [Route("LoginInfo")]
        public IActionResult GetLoginInfo([FromBody] LoginModel model)
        {
            var user = userService.GetUser(model.Username);
            if (user != null)
            {
                if (!userService.CheckPassword(user, model.Password))
                    throw new Exception("Incorrect password");
            }
            return Ok(new Response { Status = "Success", Message = "User login successfully" });
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var user = userService.GetUser(model.Username);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ?? ""),
                new Claim(ClaimTypes.Name, user.Username ?? ""),
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: configuration["JWT:ValidIssuer"],
                audience: configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(30),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), expiration = token.ValidTo });
        }

        [HttpGet]
        [Route("Logout")]
        public IActionResult Logout()
        {
            return Ok(new Response { Status = "Success", Message = "User logout successfully" });
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            var existingUser = context.Users.Where(a => a.Username == model.Username).FirstOrDefault();
            var existingEmail = context.Users.Where(a => a.Email == model.Email).FirstOrDefault();

            if (existingUser != null)
                throw new Exception("Username has been used");
            if (existingEmail != null)
                throw new Exception("Email has been used");

            var role = context.UserRoles.Where(a => a.Name == model.Role).FirstOrDefault();
            var newUser = new User
            {
                UserRoleId = role.Id,
                FullName = model.Username,
                Username = model.Username,
                Email = model.Email,
                IsActive = true,
            };
            var createUser = userService.Create(newUser, model.Password);

            if (createUser != null)
                return Ok(new { Error = false, Message = "User register successfully" });
            else
                return Ok(new { Error = true, Message = "User register unsuccessful" });
        }

        [HttpGet]
        [Route("me")]
        [Authorize]
        public IActionResult Me()
        {
            var user = userService.GetUser(User);
            return Ok(user);
        }

        [HttpGet]
        [Route("GenerateEncryptedPassword")]
        public IActionResult GenerateEncryptedPassword(string password)
        {
            return Ok(AppStatic.Encrypt(password));
        }
    }
}