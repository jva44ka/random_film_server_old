using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Models;
using WebApi.ViewModels.RequestModels;
using WebApi.ViewModels.ResultModels;
using Infrastructure.Exceptions;
using Services.Managers.Interfaces;
using AutoMapper;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountManager<Account> _accountManager;
        private readonly IMapper _mapper;

        public AccountsController(IAccountManager<Account> accountManager, IMapper mapper)
        {
            _accountManager = accountManager;
            _mapper = mapper;
        }

        // GET: api/Accounts
        [HttpGet]
        [Authorize]
        public async Task<IList<AccountViewModel>> GetAccounts()
        {
            var accounts = await _accountManager.GetAll();
            return _mapper.Map<IList<Account>, IList<AccountViewModel>>(accounts);
        }

        // GET: api/Accounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(string id)
        {
            var account = await _accountManager.GetUserById(id);

            if (account == null)
                return NotFound();

            return account;
        }

        // POST: api/Accounts/login
        [HttpPost("login")]
        public async Task<LoginResult> Login([FromBody]LoginRequest request)
        {
            if (string.IsNullOrEmpty(request?.UsernameOrEmail) || string.IsNullOrEmpty(request?.Password))
                throw new MissingParametersException("request is null or contains empty param");

            var signInResult = await _accountManager.SignInAsync(request.UsernameOrEmail, request.Password);
            var result = new LoginResult();
            if (signInResult?.User != null && signInResult.SignInResult.Succeeded)
            {
                result.LoggedIn = true;
                result.Token = _accountManager.GenerateToken(signInResult.User);
                result.UserId = signInResult.User.Id;
            }
            else if (signInResult?.SignInResult.IsLockedOut == true)
                throw new LoginException("User is banned");

            else
            {
                LoginException ex = new LoginException("Wrong username/password");
                if (signInResult?.User != null)
                    ex.AccessFailedCount = signInResult.User.AccessFailedCount;

                throw ex;
            }
            return result;
        }

        // POST: api/Accounts/Create
        [HttpPost]
        public async Task<AccountViewModel> CreateAccount([FromBody]CreateAccountRequest request)
        {
            var newAccount = _mapper.Map<CreateAccountRequest, Account>(request);
            var createdUser = await _accountManager.CreateUserAsync(newAccount, request.Password, request.SignInAfter);
            var result = _mapper.Map<Account, AccountViewModel>(createdUser);
            return result;
        }

        // PUT: api/Accounts/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<AccountViewModel>> PutAccount(string id, AccountViewModel accountVM)
        {
            var account = _mapper.Map<AccountViewModel, Account>(accountVM);
            var updatedUser = await _accountManager.UpdateAsync(id, account);
            var result = _mapper.Map<Account, AccountViewModel>(updatedUser);
            return result;
        }

        // DELETE: api/Accounts/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteAccount(string id)
        {
            await _accountManager.DeleteAsync(id);
            return Ok();
        }
    }
}