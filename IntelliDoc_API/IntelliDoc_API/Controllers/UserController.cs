using IntelliDoc_API.Authentication;
using IntelliDoc_API.Dto.User;
using IntelliDoc_API.Models;
using IntelliDoc_API.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntelliDoc_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        public UserController(IConfiguration configuration, UserService userService, IntelliDocDBContext context) : base(configuration, userService, context)
        {
        }

        // Get the options for filters.
        [HttpGet]
        [Route("FilterOption")]
        public IActionResult GetUserFilterOption()
        {
            var userNameList = context.Users.ToList().Select(x => new { id = x.Id, fullName = x.FullName });
            return Ok(userNameList);
        }

        // Get the filtered list of user accounts.
        [HttpGet]
        [Route("Filter")]
        public IActionResult GetFilteredUser([FromQuery] UserFilter dto)
        {
            var l = context.Users.Include(a => a.UserRole).ToList()
                .Select(x => new { id = x.Id, username = x.Username, fullName = x.FullName, email = x.Email, roleId = x.UserRoleId, role = x.UserRole?.Name, isActive = x.IsActive });

            if (dto.UserId != null)
                l = l.Where(a => a.id == dto.UserId);
            l.ToList();

            return Ok(l);
        }

        // Get the list of user role.
        [HttpGet]
        [Route("RoleList")]
        public IActionResult RoleList()
        {
            var roleList = context.UserRoles.ToList();
            return Ok(roleList);
        }

        // Get the user's role.
        [HttpGet]
        [Route("Role/{RoleId}")]
        public IActionResult Role(int roleId)
        {
            var role = context.UserRoles.Where(a => a.Id == roleId).FirstOrDefault();
            return Ok(role.Name);
        }

        // Get the self user information.
        [HttpGet]
        [Route("Me")]
        public IActionResult Me()
        {
            var user = userService.GetUser(User);
            return Ok(user);
        }

        // Create a new user account.
        [HttpPost]
        [Route("")]
        public IActionResult Create([FromBody] Create dto)
        {
            return Ok(new Response { Status = "Success", Message = "The user account has been created successfully" });
        }

        // Update the current logged-in user account information.
        [HttpPut]
        [Route("Me")]
        public IActionResult MeUpdate([FromBody] ProfileEdit dto)
        {
            var user = userService.GetUser(User);
            user.Username = dto.Username;
            user.FullName = dto.FullName;
            user.Email = dto.Email;

            context.Users.Update(user);
            context.SaveChanges();

            return Ok(new Response { Status = "Success", Message = "Your user profile has been edited successfully" });
        }

        // Update the current logged-in user account profile photo.
        [HttpPut]
        [Route("Me/ProfilePhoto")]
        public IActionResult MeUpdateProfilePhoto([FromBody] ProfilePhotoEdit dto)
        {
            var user = userService.GetUser(User);
            user.ProfilePhoto = dto.ProfilePhoto;

            context.Users.Update(user);
            context.SaveChanges();

            return Ok(new Response { Status = "Success", Message = "Your profile photo has been updated successfully" });
        }

        // Delete the current logged-in user account profile photo.
        [HttpPut]
        [Route("Me/ProfilePhoto/Delete")]
        public IActionResult MeDeleteProfilePhoto()
        {
            var user = userService.GetUser(User);
            user.ProfilePhoto = null;

            context.Users.Update(user);
            context.SaveChanges();

            return Ok(new Response { Status = "Success", Message = "Your profile photo has been deleted successfully" });
        }

        // Change the password of the current logged-in user.
        [HttpPut]
        [Route("Me/Password/{NewPassword}")]
        public IActionResult MeUpdatePassword(string newPassword)
        {
            var user = userService.GetUser(User);

            if (!string.IsNullOrEmpty(newPassword))
            {
                var encryptedPassword = AppStatic.Encrypt(newPassword);

                if (user.Password != encryptedPassword)
                {
                    user.Password = encryptedPassword;
                    context.Users.Update(user);
                    context.SaveChanges();
                }
                else
                    throw new Exception("The new password is the same as the old password");
            }

            return Ok(new Response { Status = "Success", Message = "Your password updated successfully" });
        }

        // Get the specific user's account information.
        [HttpGet]
        [Route("Info/{UserId}")]
        public IActionResult GetUserInfo(int userId)
        {
            var user = context.Users.Where(a => a.Id == userId).FirstOrDefault();
            var userInfo = context.Users.Include(a => a.UserRole).Where(a => a.Id == userId)
                .Select(x => new { id = x.Id, roleId = x.UserRoleId, role = x.UserRole.Name, userId, fullName = x.FullName, username = x.Username, email = x.Email, profilePhoto = x.ProfilePhoto })
                .FirstOrDefault();
            return Ok(userInfo);
        }

        // Update the specific user's account information.
        [HttpPut]
        [Route("Info/{UserId}/Edit")]
        public IActionResult UserInfoUpdate(int userId, [FromBody] Edit dto)
        {
            var role = context.UserRoles.Where(a => a.Name == dto.Role).FirstOrDefault();
            var userInfo = context.Users.Where(a => a.Id == userId).FirstOrDefault();

            userInfo.FullName = dto.FullName;
            userInfo.Username = dto.Username;
            userInfo.Email = dto.Email;
            userInfo.UserRoleId = role.Id;

            context.Users.Update(userInfo);
            context.SaveChanges();

            return Ok(new Response { Status = "Success", Message = "The user account information has been updated successfully" });
        }

        // Delete the user account.
        [HttpDelete]
        [Route("Delete/{UserId}")]
        public IActionResult UserDelete(int userId)
        {
            var userInfo = context.Users.Where(a => a.Id == userId).FirstOrDefault();
            context.Users.Remove(userInfo);
            context.SaveChanges();　

            return Ok(new Response { Status = "Success", Message = "User account deleted successfully" });
        }
    }
}