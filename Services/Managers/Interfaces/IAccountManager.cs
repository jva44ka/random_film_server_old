using Core.Models;
using Microsoft.AspNetCore.Identity;
using Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Managers.Interfaces
{
    public interface IAccountManager<TUser> where TUser : class
    {
        Task<IdentityResult> CreateUserAsync(Account user, string password, bool signInAfter, bool persistentSignIn = true);
        Task<IList<Account>> GetAll();
        Task<Account> GetUserById(string id);
        Task<bool> IsGlobalAdmin(string id);
        Task<UserSignInResult<Account>> SignInAsync(string usernameOrEmail, string password, bool persistentSignIn = true);
        string GenerateToken(Account user);
        Task<Account> UpdateAsync(string id, Account account);
        Task DeleteAsync(string id);
    }
}
