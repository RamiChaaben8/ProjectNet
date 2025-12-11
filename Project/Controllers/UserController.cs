using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTO;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await userManager.Users.ToListAsync();
            var userDTOs = new List<UserDTO>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                userDTOs.Add(new UserDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = roles.FirstOrDefault() ?? "No Role"
                });
            }

            return Ok(userDTOs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var roles = await userManager.GetRolesAsync(user);
            var userDTO = new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault() ?? "No Role"
            };

            return Ok(userDTO);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] SignupDTO signupDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser
            {
                UserName = signupDTO.Name,
                Email = signupDTO.Email,
                PhoneNumber = signupDTO.PhoneNumber
            };

            var result = await userManager.CreateAsync(user, signupDTO.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors });
            }

            // Assign role if provided
            if (!string.IsNullOrEmpty(signupDTO.Role))
            {
                await userManager.AddToRoleAsync(user, signupDTO.Role);
            }

            return Ok(new { message = "User created successfully", userId = user.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Update user properties
            user.UserName = userDTO.UserName;
            user.Email = userDTO.Email;
            user.PhoneNumber = userDTO.PhoneNumber;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors });
            }

            // Update role if provided
            if (!string.IsNullOrEmpty(userDTO.Role))
            {
                var currentRoles = await userManager.GetRolesAsync(user);
                await userManager.RemoveFromRolesAsync(user, currentRoles);
                await userManager.AddToRoleAsync(user, userDTO.Role);
            }

            return Ok(new { message = "User updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(new { message = "User deleted successfully" });
        }
    }
}
