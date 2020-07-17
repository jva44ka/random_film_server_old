using Core.Models;
using Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Managers.Interfaces
{
    public interface IAccountManager<TUser> where TUser : class
    {
        Task<Account> CreateUserAsync(Account user, string password, bool signInAfter, bool persistentSignIn = true);
        Task<IList<Account>> GetAll();
        Task<Account> GetUserById(string id);
        Task<bool> IsGlobalAdmin(string id);
        Task<UserSignInResult<Account>> SignInAsync(string usernameOrEmail, string password, bool persistentSignIn = true);
        string GenerateToken(Account user);
        Task<Account> UpdateAsync(string id, Account account);
        Task DeleteAsync(string id);
    }
}
