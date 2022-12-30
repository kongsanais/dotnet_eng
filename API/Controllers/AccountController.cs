using System.Text;
using System.Security.Cryptography;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.DTOs;

namespace API.Controllers
{
    public class AccountController : BaseApiControll
    {
        private readonly DataContext _context;

        public AccountController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("register")]  // POST : api/account/register
        
        public async Task<ActionResult<AppUser>> Register([FromBody]RegisterDto registerDto){
            
            if(await UserExists(registerDto.Username)){
                return BadRequest("Username is taken");
            }

            using var hmac = new HMACSHA512();

            var user = new AppUser 
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            return user;

        }


        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AsQueryable().AnyAsync(x => x.UserName == username.ToLower());
        }

    }
}