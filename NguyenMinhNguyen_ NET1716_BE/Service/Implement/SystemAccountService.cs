using BussinessObjects.Models;
using DTOS;
using Microsoft.Extensions.Configuration;
using Repository.Interface;
using Service.Interface;

namespace Service.Implement
{
    public class SystemAccountService : ISystemAccountService
    {
        private readonly ISystemAccountRepository _repository;
        private readonly ITokenService _tokenService;

        public SystemAccountService(ISystemAccountRepository repository, ITokenService tokenService)
        {
            _repository = repository;
            _tokenService = tokenService;
        }

        public async Task CreateAccount(AccountCreate accountCreate)
        {
            await _repository.CreateAccount(accountCreate);
        }

        public async Task<bool> DeleteAccounts(int accountID)
        {
            return await _repository.DeleteAccounts(accountID);
        }

        public async Task<SystemAccount> EmailExisted(string email)
        {
            return await _repository.GetAccountByEmail(email);
        }

        public async Task<SystemAccount> GetAccountProfile(int accountID)
        {
            return await _repository.GetAccountByID(accountID);
        }

        public async Task<IEnumerable<SystemAccount>> GetAllAccount()
        {
            return await _repository.GetAllAccount();
        }

        public async Task<LoginTokenDto> LoginAccount(LoginDto loginDto)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json", true, true)
                       .Build();
            string email = config["User:AccountEmail"];
            string pwd = config["User:AccountPassword"];
            var accountAdmin = new SystemAccount();
            if(loginDto.AccountEmail == email && loginDto.AccountPassword == pwd)
            {
                accountAdmin.AccountPassword = pwd;
                accountAdmin.AccountEmail = email;
                accountAdmin.AccountRole = 0;
                LoginTokenDto loginTokenDto = new LoginTokenDto();
                loginTokenDto.Token = _tokenService.CreateToken(accountAdmin);
                return loginTokenDto;
            }
            else
            {
                var account = await _repository.GetAccountByEmail(loginDto.AccountEmail);
                if (account == null)
                {
                    return null;
                }
                else
                {
                    if (account.AccountPassword == loginDto.AccountPassword)
                    {
                        LoginTokenDto loginTokenDto = new LoginTokenDto();
                        loginTokenDto.Token = _tokenService.CreateToken(account);
                        return loginTokenDto;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public async Task UpdateAccount(AccountUpdate accountUpdate)
        {
            await _repository.UpdateAccount(accountUpdate);
        }

    }
}
