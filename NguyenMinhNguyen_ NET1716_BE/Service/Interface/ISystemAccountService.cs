
using BussinessObjects.Models;
using DTOS;

namespace Service.Interface
{
    public interface ISystemAccountService
    {
        Task CreateAccount(AccountCreate accountCreate);
        Task<bool> DeleteAccounts(int accountID);
        Task<IEnumerable<SystemAccount>> GetAllAccount();
        Task<LoginTokenDto> LoginAccount(LoginDto loginDto);
        Task UpdateAccount(AccountUpdate accountUpdate);
        Task<SystemAccount> EmailExisted(string email);
        Task<SystemAccount> GetAccountProfile(int accountID);
    }
}
