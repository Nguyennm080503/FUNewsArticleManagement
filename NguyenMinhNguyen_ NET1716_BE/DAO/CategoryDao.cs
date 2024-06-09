using BussinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class CategoryDao : BaseDao<Category>
    {
        private static CategoryDao instance = null;
        private static readonly object instacelock = new object();

        private CategoryDao()
        {

        }

        public static CategoryDao Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CategoryDao();
                }
                return instance;
            }
        }

        public async Task<Category> GetDetail(int categoryID)
        {
            var datacontext = new FunewsManagementDbContext();
            return await datacontext.Categories.FirstOrDefaultAsync(x => x.CategoryId == categoryID);
        }

        public async Task<bool> GetCategoryExisted(int categoryID)
        {
            var datacontext = new FunewsManagementDbContext();
            var newsExisted = await datacontext.NewsArticles.Where(x => x.CategoryId == categoryID).AnyAsync();
            return newsExisted;
        }

        public async Task<bool> RemoveCategoryAsync(int categoryID)
        {
            try
            {
                var _context = new FunewsManagementDbContext();
                var category = await GetDetail(categoryID);
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<int> GetMaxID()
        {
            var _context = new FunewsManagementDbContext();
            return _context.Categories.Max(x => x.CategoryId);
        }
    }
}
