using System;
using Microsoft.AspNetCore.Mvc;

namespace Studio1BTask.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost("[action]")]
        public string GetToken()
        {
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            return token;
        }
    }
}