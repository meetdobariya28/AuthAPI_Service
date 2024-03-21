using AngularAuthAPI.Context;
using AngularAuthAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AngularAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase //8. Create API Controller (Empty) .. //9. In VS CODE add Services 
    {
        private readonly AppDbContext _authContext; //Add this context to use it 
        //ctor + TAB for shortcut constructor creater
        public UserController(AppDbContext appDbContext)
        {
            _authContext = appDbContext;
        }
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userObj) //Login page to check the data from DB
        {
            if (userObj == null)
                return BadRequest(); // 400 error, if its empty

            var user = await _authContext.Users // if the username and password is same
                .FirstOrDefaultAsync(x => x.UserName == userObj.UserName && x.Password == userObj.Password);

            if (user == null) // If anyone is Not Matched
                return NotFound(new { Message = "User Not Found!" });

            user.Token = CreateJWT(user); //Adding Token method --Token-4
            return Ok(new
            {
                Token= user.Token, //--Token -5 --END 
                Message = "Login Success!" //if matached with DB
            });  
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userObj) //to take as data from front end side
        {
            if(userObj == null)
                return BadRequest(); //Error 404

            if (await CheckUserNameAsync(userObj.UserName)) //If this is true
                return BadRequest(new { Message = "Username is Already Exist!!" });

            if (await CheckEmailAsync(userObj.Email)) //If this is true
                return BadRequest(new { Message = "Email is Already Exist!!" });

            userObj.Role = "User"; //Role User
            userObj.Token = ""; // Empty JWT Token
            await _authContext.Users.AddAsync(userObj); //Add Data in DB
            await _authContext.SaveChangesAsync();  //Save Changes
                
            return Ok(new
                {
                   Message = "User Registered!"
            });
        }

        private  Task<bool> CheckUserNameAsync(string? userName) //new USername = DB Username
            => _authContext.Users.AnyAsync(x => x.UserName ==  userName);
        
        private  Task<bool> CheckEmailAsync(string? email) //new Email = DB email
            => _authContext.Users.AnyAsync(x => x.Email == email);

        private string CreateJWT(User user)//Jwt token made of handler, payload and signature --Token -2
        {
            var JwtTokenhandeler = new JwtSecurityTokenHandler(); // token handeler
            var key = Encoding.ASCII.GetBytes("your_secret_key_with_at_least_256_bits"); //key in bytes
            var identity = new ClaimsIdentity(new Claim[] //identity of role and full name to verify the user //payload
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor //Token Descriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1), //expiry time 
                SigningCredentials = credentials
            };
            var token = JwtTokenhandeler.CreateToken(tokenDescriptor); // Create token

            return JwtTokenhandeler.WriteToken(token); //token in encripted form
        }
        [Authorize] //this will protect api from getting data from DB to unauthoeize people 
        [HttpGet]
        public async Task<ActionResult<User>> GetAllUsers() //APi to Get all Users data form the Db to authenticate in angular side 
        {
            return Ok(await _authContext.Users.ToListAsync());
        }
    }
}
