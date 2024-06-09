using BussinessObjects.Models;
using DAO;
using DTOS;
using Repository.Interface;

namespace Repository.Implement
{
    public class SystemAccountRepository : ISystemAccountRepository
    {

        public async Task CreateAccount(AccountCreate accountCreate)
        {
            var id = await SystemAccountDao.Instance.GetMaxID();
            var account = new SystemAccount()
            {
                AccountId = (short)(id + 1),
                AccountEmail = accountCreate.AccountEmail,
                AccountName = accountCreate.AccountName,
                AccountPassword = accountCreate.AccountPassword,
                AccountRole = accountCreate.AccountRole,
            };
            await SystemAccountDao.Instance.CreateAsync(account);
        }

        public async Task<bool> DeleteAccounts(int accountID)
        {
            bool check = await SystemAccountDao.Instance.GetAccountExisted(accountID);
            if (check)
            {
                return false;
            }
            else
            {
                await SystemAccountDao.Instance.RemoveAccountAsync(accountID);
                return true;
            }
        }

        public async Task<SystemAccount> GetAccountByEmail(string email)
        {
            var account = await SystemAccountDao.Instance.GetAccountByEmail(email);
            return account;
        }

        public async Task<SystemAccount> GetAccountByID(int accountID)
        {
            return await SystemAccountDao.Instance.GetDetail(accountID);
        }

        public async Task<IEnumerable<SystemAccount>> GetAllAccount()
        {
            return await SystemAccountDao.Instance.GetAllAsync();
        }

        public async Task UpdateAccount(AccountUpdate accountUpdate)
        {
            var account = await SystemAccountDao.Instance.GetDetail(accountUpdate.AccountId);
            account.AccountName = accountUpdate.AccountName;
            account.AccountPassword = accountUpdate.AccountPassword;
            account.AccountRole = accountUpdate.AccountRole;
            account.AccountEmail = accountUpdate.AccountEmail;
            await SystemAccountDao.Instance.UpdateAsync(account);
        }
    }
}
