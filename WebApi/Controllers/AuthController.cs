using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using randomfilm_backend.Models;
using randomfilm_backend.Models.Entities;

namespace randomfilm_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class AuthController : ControllerBase
    {
        private readonly RandomFilmDBContext db;

        public AuthController(RandomFilmDBContext context)
        {
            db = context;
        }

        // POST: api/Auth
        [HttpPost("token")]
        public async Task<ActionResult<string>> Post(Account account)
        {
            var username = account.Login;
            var password = account.Password;

            ClaimsIdentity identity = await GetIdentity(username, password, db);
            if (identity == null)
            {
                return NotFound();
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            string jwtToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Ok(jwtToken);
        }

        private async Task<ClaimsIdentity> GetIdentity(string username, string password, RandomFilmDBContext db)
        {
            Account person = await db.Accounts
                                    .Include(x => x.Role)
                                    .FirstOrDefaultAsync(x => x.Login == username && x.Password == password);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role.Name)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }
    }
}