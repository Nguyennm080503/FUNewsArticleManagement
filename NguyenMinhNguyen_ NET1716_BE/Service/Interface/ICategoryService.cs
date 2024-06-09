using BussinessObjects.Models;
using DTOS;

namespace Service.Interface
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategory();
        Task CreateCategory(CategoryCreate categoryCreate);
        Task<bool> DeleteCategory(int categoryID);
        Task UpdateCategory(CategoryUpdate categoryUpdate);
    }
}
