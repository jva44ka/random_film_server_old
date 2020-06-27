using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using randomfilm_backend;
using randomfilm_backend.Models;
using randomfilm_backend.Models.Entities;

namespace randomfilm_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class AccountsController : ControllerBase
    {
        private readonly RandomFilmDBContext db;

        public AccountsController(RandomFilmDBContext context)
        {
            db = context;
        }

        // GET: api/Accounts
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return await db.Accounts.ToListAsync();
        }

        // GET: api/Accounts/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            var account = await db.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        // GET: api/Accounts/Self
        [HttpGet("Self")]
        [Authorize]
        public async Task<ActionResult<Account>> GetAccount()
        {
            Account account = await db.Accounts.
                FirstOrDefaultAsync(x => x.Login == this.HttpContext.User.Identity.Name);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        // POST: api/Accounts/Create
        [HttpPost("/Create")]
        public async Task<ActionResult> CreateAccount([FromBody] Account account)
        {
            // Валидация полей (в емейле собака и точка, в пароле заглавные и цифры и т.д.)

            // Создание нового аккаунта(user) на основе присланых данных (account)
            Account newAccount = new Account()
            {
                Email = account.Email,
                Login = account.Login,
                Password = account.Password,
                Role = await db.Roles.FirstOrDefaultAsync(x => x.Name == "user"),
            };
            db.Accounts.Add(newAccount);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AccountExists(newAccount.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return Ok();
        }

        // PUT: api/Accounts/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutAccount(int id, Account account)
        {
            if (id != account.Id)
            {
                return BadRequest();
            }

            db.Entry(account).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PUT: api/Accounts/Self
        [HttpPut("Self")]
        [Authorize]
        public async Task<IActionResult> PutAccount(Account account)
        {
            if (db.Accounts.
                FirstOrDefaultAsync(x => x.Login == this.HttpContext.User.Identity.Name) == null)
            {
                return BadRequest();
            }

            db.Entry(account).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                int id = db.Accounts.
                    FirstOrDefaultAsync(x => x.Login == this.HttpContext.User.Identity.Name).Result.Id;
                if (!AccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Accounts/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Account>> DeleteAccount(int id)
        {
            var account = await db.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            db.Accounts.Remove(account);
            await db.SaveChangesAsync();

            return account;
        }

        private bool AccountExists(int id)
        {
            return db.Accounts.Any(e => e.Id == id);
        }
    }
}