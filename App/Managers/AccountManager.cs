using Core.Interfaces;
using Core.Models;
using Infrastructure.Exceptions;
using Infrastructure.Managers.Interfaces;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Managers
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
            this._accountsRepo = accounts;
            SignInManager = signInManager;
        }

        public async Task<UserSignInResult<Account>> SignInAsync(string username, string email, string password, bool persistentSignIn = true)
        {
            var result = await SignInManager.PasswordSignInAsync(username, password, persistentSignIn, true);
            var user = _accountsRepo.GetAll().FirstOrDefault(x => x.UserName == username || x.Email == email);
            return new UserSignInResult<Account>(result) { User = user };
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

        public Task<Account> GetUserById(string id)
        {
            return Task.FromResult(_accountsRepo.GetAll().FirstOrDefault(x => x.Id == id));
        }

        public Task<bool> IsGlobalAdmin(string id)
        {
            return Task.FromResult(_accountsRepo.GetAll().FirstOrDefault(x => x.Id == id).IsMainAdmin);
        }
    }
}
