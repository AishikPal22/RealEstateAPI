﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstateApplication.Data;
using RealEstateApplication.Models;
using System.Security.Claims;

namespace RealEstateApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        ApplicationDbContext _appdb = new ApplicationDbContext();

        [HttpGet("PropertyList")]
        [Authorize]
        public IActionResult GetProperties(int categoryId) 
        {
            var propertiesResult = _appdb.Properties.Where(c => c.CategoryId == categoryId);
            if (propertiesResult == null) { return NotFound(); }
            return Ok(propertiesResult);
        }

        [HttpGet("PropertyDetail")]
        [Authorize]
        public IActionResult GetPropertyDetail(int id)
        {
            var propertiesResult = _appdb.Properties.FirstOrDefault(c => c.Id == id);
            if (propertiesResult == null) { return NotFound(); }
            return Ok(propertiesResult);
        }

        [HttpGet("TrendingProperties")]
        [Authorize]
        public IActionResult GetTrendingProperties()
        {
            var propertiesResult = _appdb.Properties.Where(c => c.IsTrending == true);
            if (propertiesResult == null) { return NotFound(); }
            return Ok(propertiesResult);
        }

        [HttpGet("SearchProperties")]
        [Authorize]
        public IActionResult GetSearchProperties(string address)
        {
            var propertiesResult = _appdb.Properties.Where(c => c.Address.Contains(address));
            if (propertiesResult == null) { return NotFound(); }
            return Ok(propertiesResult);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody] Property property)
        {
            if (property == null) { return NoContent(); }
            else
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var user = _appdb.Users.FirstOrDefault(u => u.Email == userEmail);
                if(user == null) { return NotFound(); }
                property.IsTrending = false;
                property.UserId = user.Id;
                _appdb.Properties.Add(property);
                _appdb.SaveChanges();
                return StatusCode(StatusCodes.Status201Created);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Put(int id, [FromBody] Property property)
        {
            var propertyResult = _appdb.Properties.FirstOrDefault(p => p.Id == id);
            if (propertyResult == null) { return NotFound(); }
            else
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var user = _appdb.Users.FirstOrDefault(u => u.Email == userEmail);
                if (user == null) { return NotFound(); }
                if(propertyResult.UserId == user.Id)
                {
                    propertyResult.Name = property.Name;
                    propertyResult.Detail = property.Detail;
                    propertyResult.Price = property.Price;
                    propertyResult.Address = property.Address;
                    property.IsTrending = false;
                    property.UserId = user.Id;
                    _appdb.SaveChanges();
                    return Ok("Record updated successfully!");
                }
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var propertyResult = _appdb.Properties.FirstOrDefault(p => p.Id == id);
            if (propertyResult == null) { return NotFound(); }
            else
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var user = _appdb.Users.FirstOrDefault(u => u.Email == userEmail);
                if (user == null) { return NotFound(); }
                if (propertyResult.UserId == user.Id)
                {
                    _appdb.Properties.Remove(propertyResult);
                    _appdb.SaveChanges();
                    return Ok("Record deleted successfully!");
                }
                return BadRequest();
            }
        }
    }
}
