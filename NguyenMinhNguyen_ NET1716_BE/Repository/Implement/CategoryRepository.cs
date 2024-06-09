using BussinessObjects.Models;
using DAO;
using DTOS;
using Repository.Interface;

namespace Repository.Implement
{
    public class CategoryRepository : ICategoryRepository
    {
        public async Task CreateCategory(CategoryCreate categoryCreate)
        {
            //var id = await CategoryDao.Instance.GetMaxID();
            var category = new Category();
            category.CategoryName = categoryCreate.CategoryName;
            category.CategoryDesciption = categoryCreate.CategoryDesciption;
            await CategoryDao.Instance.CreateAsync(category);
        }

        public async Task<bool> DeleteCategory(int categoryID)
        {
            bool check = await CategoryDao.Instance.GetCategoryExisted(categoryID);
            if (check)
            {
                return false;
            }
            else
            {
                await CategoryDao.Instance.RemoveCategoryAsync(categoryID);
                return true;
            }
        }

        public async Task<IEnumerable<Category>> GetAllCategory()
        {
            return await CategoryDao.Instance.GetAllAsync();
        }

        public async Task UpdateCategory(CategoryUpdate categoryUpdate)
        {
            var category = await CategoryDao.Instance.GetDetail(categoryUpdate.CategoryId);
            category.CategoryName = categoryUpdate.CategoryName;
            category.CategoryDesciption = categoryUpdate.CategoryDesciption;
            await CategoryDao.Instance.UpdateAsync(category);
        }
    }
}
