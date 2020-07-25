using Core.Interfaces;
using Core.Models;
using Infrastructure.Auth;
using Infrastructure.Exceptions;
using Services.Models;
using Services.Managers.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Services.Managers
{
    public class AccountManager : UserManager<Account>, IAccountManager<Account>
    {
        private readonly IRepository<Account> _accountsRepo;
        private SignInManager<Account> SignInManager { get; }

        public AccountManager(
            // For base class
            IUserStore<Account> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<Account> passwordHasher,
            IEnumerable<IUserValidator<Account>> userValidators,
            IEnumerable<IPasswordValidator<Account>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<Account>> logger,
            // END For base class 
            IRepository<Account> accounts,
            SignInManager<Account> signInManager)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _accountsRepo = accounts;
            SignInManager = signInManager;
        }

        public async Task<IList<Account>> GetAll()
        {
            return await _accountsRepo.GetAll().ToListAsync();
        }

        public Task<Account> GetUserById(string id)
        {
            return Task.FromResult(_accountsRepo.GetAll().FirstOrDefault(x => x.Id == id));
        }

        public Task<bool> IsGlobalAdmin(string id)
        {
            return Task.FromResult(_accountsRepo.GetAll().FirstOrDefault(x => x.Id == id).IsMainAdmin);
        }

        public async Task<UserSignInResult<Account>> SignInAsync(string usernameOrEmail, string password, bool persistentSignIn = true)
        {
            var result = await SignInManager.PasswordSignInAsync(usernameOrEmail, password, persistentSignIn, true);
            var user = _accountsRepo.GetAll().FirstOrDefault(x => x.UserName == usernameOrEmail || x.Email == usernameOrEmail);
            return new UserSignInResult<Account>(result) { User = user };
        }

        public string GenerateToken(Account user)
        {
            ClaimsIdentity claims = GetIdentity(user);
            return claims.GenerateJwtToken();
        }

        public async Task<Account> CreateUserAsync(Account user, string password, bool signInAfter, bool persistentSignIn = true)
        {
            var result = await CreateAsync(user, password);
            if (!result.Succeeded)
                throw new IdentityCreateException("Auth exception");

            if (signInAfter)
            {
                await SignInManager.SignInAsync(user, persistentSignIn);
            }

            return await FindByIdAsync(user.Id);
        }

        public async Task<Account> UpdateAsync(string id, Account account)
        {
            var user = _accountsRepo.GetAll().FirstOrDefault(x => x.Id == id);
            if (user == null)
                throw new NotExistsException($"User {id} is not exists");

            user.FirstName = account.FirstName;
            user.LastName = account.LastName;
            user.UserName = account.UserName;
            user.PhoneNumber = account.PhoneNumber;
            user.Email = account.Email;

            var result = _accountsRepo.Update(user);
            await _accountsRepo.SaveAsync();
            return result;
        }

        public async Task DeleteAsync(string id)
        {
            var user = _accountsRepo.GetAll().FirstOrDefault(x => x.Id == id);
            if (user == null)
                throw new NotExistsException($"User {id} is not exists");

            _accountsRepo.Delete(user);
            await _accountsRepo.SaveAsync();
        }

        private ClaimsIdentity GetIdentity(Account user)
        {
            var roleClaim = user.IsMainAdmin ? "global_admin" : "";
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName)
            };
            if (!string.IsNullOrEmpty(roleClaim))
            {
                claims.Add(new Claim(ClaimsIdentity.DefaultNameClaimType, roleClaim));
            }
            ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}
