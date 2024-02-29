using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace App1.API.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        
        [Authorize(Policy = "AgePolicy")]
        [Authorize(Policy = "Sivaslilar")]
        [HttpGet]
        public IActionResult GetStock()
        {
            var userName = HttpContext.User.Identity.Name;
            var userId = User.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.NameIdentifier)?.Value;
            //
             //Stok ile ilgili işlemler yapılır.
            ////
            
            return Ok($"{userName} - {userId}--Stock service result");
        }
    }
}
