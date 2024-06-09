using BussinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class SystemAccountDao : BaseDao<SystemAccount>
    {
        private static SystemAccountDao instance = null;
        private static readonly object instacelock = new object();

        private SystemAccountDao()
        {

        }

        public static SystemAccountDao Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SystemAccountDao();
                }
                return instance;
            }
        }

        public async Task<SystemAccount> GetDetail(int accountID)
        {
            var datacontext = new FunewsManagementDbContext();
            return await datacontext.SystemAccounts.FirstOrDefaultAsync(x => x.AccountId == accountID);
        }

        public async Task<bool> GetAccountExisted(int accountID)
        {
            var datacontext = new FunewsManagementDbContext();
            var newsExisted = await datacontext.NewsArticles.Where(x => x.CreatedById == accountID).AnyAsync();
            return newsExisted;
        }

        public async Task<bool> RemoveAccountAsync(int accountID)
        {
            try
            {
                var _context = new FunewsManagementDbContext();
                var account = await GetDetail(accountID);
                _context.SystemAccounts.Remove(account);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<SystemAccount> GetAccountByEmail(string email)
        {
            var _context = new FunewsManagementDbContext();
            return await _context.SystemAccounts.FirstOrDefaultAsync(x => x.AccountEmail == email);
        }

        public async Task<int> GetMaxID()
        {
            var _context = new FunewsManagementDbContext();
            return _context.SystemAccounts.Max(x => x.AccountId);
        }
    }
}
