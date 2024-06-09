
using BussinessObjects.Models;
using DTOS;

namespace Repository.Interface
{
    public interface ISystemAccountRepository
    {
        Task CreateAccount(AccountCreate accountCreate);
        Task<bool> DeleteAccounts(int accountID);
        Task<IEnumerable<SystemAccount>> GetAllAccount();
        Task UpdateAccount(AccountUpdate accountUpdate);
        Task<SystemAccount> GetAccountByEmail(string email);
        Task<SystemAccount> GetAccountByID(int accountID);
    }
}
