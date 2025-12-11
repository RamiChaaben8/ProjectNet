using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Project.Data;
using Project.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Project.Controllers
{
    [Route("api/[controller]")]
   // [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;
        private readonly Project.Repositories.IClientRepository clientRepository;

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, Project.Repositories.IClientRepository clientRepository)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
            this.clientRepository = clientRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> signup([FromBody] SignupDTO signupDTO)
        {
            try
            {
                if (await userManager.FindByEmailAsync(signupDTO.Email) != null)
                {
                    return BadRequest("User exist!!!");
                }
                var applicationUser = new ApplicationUser()
                {
                    Email = signupDTO.Email,
                    UserName = signupDTO.Email,  // Use email as username for consistency
                };
                var result = await userManager.CreateAsync(applicationUser, signupDTO.Password);
                if(result.Succeeded)
                {
                    var role = signupDTO.Role ?? "User";
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                    await userManager.AddToRoleAsync(applicationUser, role);

                    // Create Client entity
                    if (await clientRepository.GetClientByEmail(signupDTO.Email) == null)
                    {
                        var newClient = new DataBase.Client
                        {
                            Name = signupDTO.Name,
                            Email = signupDTO.Email,
                            Password = "Hidden", // Don't store actual password here
                            Phone = signupDTO.PhoneNumber ?? "",
                            Adresse = "", // Default empty
                            Role = role
                        };
                        await clientRepository.CreateNewClient(newClient);
                    }

                    return Ok("user created");
                }
                return BadRequest("erreur");
            }
            catch (Exception ex)
            {
                return BadRequest("problem");
            }
        }
        [HttpPost("client-register")]
        public async Task<IActionResult> ClientRegister([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                // Validate input
                if (registerDTO == null)
                {
                    return BadRequest("Registration data is null");
                }

                if (string.IsNullOrEmpty(registerDTO.Email))
                {
                    return BadRequest("Email is required");
                }

                if (string.IsNullOrEmpty(registerDTO.Name))
                {
                    return BadRequest("Name is required");
                }

                if (string.IsNullOrEmpty(registerDTO.Password))
                {
                    return BadRequest("Password is required");
                }

                if (await userManager.FindByEmailAsync(registerDTO.Email) != null)
                {
                    return BadRequest("User already exists!");
                }
                
                var applicationUser = new ApplicationUser()
                {
                    Email = registerDTO.Email,
                    UserName = registerDTO.Email,  // Use email as username for consistency
                    PhoneNumber = registerDTO.PhoneNumber
                };
                
                var result = await userManager.CreateAsync(applicationUser, registerDTO.Password);
                if (result.Succeeded)
                {
                    // Assign default "Client" role for clients
                    var role = "Client";
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                    await userManager.AddToRoleAsync(applicationUser, role);

                     // Create Client entity
                    if (await clientRepository.GetClientByEmail(registerDTO.Email) == null)
                    {
                        var newClient = new DataBase.Client
                        {
                            Name = registerDTO.Name,
                            Email = registerDTO.Email,
                            Password = "Hidden",
                            Phone = registerDTO.PhoneNumber ?? "",
                            Adresse = registerDTO.Address ?? "",
                            Role = role
                        };
                        await clientRepository.CreateNewClient(newClient);
                    }

                    return Ok("User registered successfully");
                }
                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                // Validate input
                if (loginDTO == null)
                {
                    return BadRequest("Login data is null");
                }

                if (string.IsNullOrEmpty(loginDTO.Email))
                {
                    return BadRequest("Email is required");
                }

                if (string.IsNullOrEmpty(loginDTO.Password))
                {
                    return BadRequest("Password is required");
                }

                var user = await userManager.FindByEmailAsync(loginDTO.Email);
                if (user == null)
                {
                    return BadRequest("invalide");
                }
                if (await userManager.CheckPasswordAsync(user, loginDTO.Password))
                {
                    var userRoles = await userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

                    var token = new JwtSecurityToken(
                        issuer: configuration["JWT:ValidIssuer"],
                        audience: configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                        );

                    return Ok(new LoginResultDTO
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        Role = userRoles.FirstOrDefault()
                    });
                }
                return BadRequest("invalid password");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
