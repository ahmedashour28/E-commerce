using ECommerce.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.FullName,
                    Roles = _userManager.GetRolesAsync(u).Result
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole(Guid userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound("User not found");

            if (!await _roleManager.RoleExistsAsync(role))
                return BadRequest("Role does not exist");

            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded) return BadRequest("Failed to assign role");

            return Ok($"Role '{role}' assigned to user '{user.UserName}'");
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);
            return NoContent();
        }
    }
}