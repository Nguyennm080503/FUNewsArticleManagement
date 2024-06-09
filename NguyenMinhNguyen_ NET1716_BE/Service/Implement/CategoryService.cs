using BussinessObjects.Models;
using DTOS;
using Repository.Interface;
using Service.Interface;

namespace Service.Implement
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task CreateCategory(CategoryCreate categoryCreate)
        {
            await _categoryRepository.CreateCategory(categoryCreate);
        }

        public async Task<bool> DeleteCategory(int categoryID)
        {
            return await _categoryRepository.DeleteCategory(categoryID);
        }

        public async Task<IEnumerable<Category>> GetAllCategory()
        {
            return await _categoryRepository.GetAllCategory();
        }

        public async Task UpdateCategory(CategoryUpdate categoryUpdate)
        {
            await _categoryRepository.UpdateCategory(categoryUpdate);
        }
    }
}
