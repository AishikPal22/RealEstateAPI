﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RealEstateApplication.Data;
using RealEstateApplication.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealEstateApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        ApplicationDbContext _appdb = new ApplicationDbContext();

        private IConfiguration _config;
        public UsersController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("[action]")]
        public IActionResult Register([FromBody] User user)
        {
            var userExists = _appdb.Users.FirstOrDefault(u => u.Email == user.Email);
            if (userExists != null) 
            {
                return BadRequest("User with same email id already exists");    
            }
            _appdb.Users.Add(user);
            _appdb.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost("[action]")]
        public IActionResult Login([FromBody] User user) 
        {
            var currentUser = _appdb.Users.FirstOrDefault(x => x.Email == user.Email && x.Password == user.Password);
            
            if(currentUser == null) { return NotFound(); }
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            var claims = new[] { new Claim(ClaimTypes.Email, user.Email) };
            
            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims : claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(jwt);
        }
    }
}
