using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApii_2.Models;
using WebApii_2.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebApii_2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public readonly UserManager<applicationUser> userManager;

        public IConfiguration Config { get; }

        public AccountController(UserManager<applicationUser> userManager,IConfiguration config)
        {
            this.userManager = userManager;
            Config = config;
        }

        // create new user
        [HttpPost("Register")]
        public async Task<IActionResult> Registration(RgisterUserDTO rgisterUserDTO)
        {
            if (ModelState.IsValid)
            {
                // save
                applicationUser user = new applicationUser();
                user.UserName = rgisterUserDTO.UserName;
                user.Email = rgisterUserDTO.Email;
                IdentityResult result = await userManager.CreateAsync(user, rgisterUserDTO.Password);
                if (result.Succeeded)
                {
                    return Ok("account added successfuly");
                }
                return BadRequest("userName or password is wrong");
            }
            return BadRequest();

        }



        [HttpPost("login")]
        public async Task<IActionResult> Login(loginUserDTO UserDTO)
        {
            if (ModelState.IsValid)
            {
                applicationUser user = await userManager.FindByNameAsync(UserDTO.UserName);
                if (user != null)
                {
                    bool found = await userManager.CheckPasswordAsync(user, UserDTO.Password);
                    if (found)
                    {
                        // claims 
                        
                        var claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                        claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

                        // get roles
                        var roles = await userManager.GetRolesAsync(user);
                        foreach (var itemRoles in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, itemRoles));
                        }
                        SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["jwtBearer:SignInKey"]));
                        SigningCredentials sincred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); 

                        // create token
                        JwtSecurityToken mytoken = new JwtSecurityToken(

                            issuer: "http://localhost:15574/",
                            audience: "http://localhost:4200/",
                            claims: claims,
                            expires: DateTime.Now.AddHours(1),
                            signingCredentials:sincred
                            );

                        return Ok (new
                        {
                            token =new JwtSecurityTokenHandler().WriteToken(mytoken),
                            expiration = mytoken.ValidTo
                        }
                            
                            );




                    }
                }
            }
            return Unauthorized();

        }
    }
};
