using BussinessObjects.Models;
using DTOS;

namespace Repository.Interface
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategory();
        Task CreateCategory(CategoryCreate categoryCreate);
        Task<bool> DeleteCategory(int categoryID);
        Task UpdateCategory(CategoryUpdate categoryUpdate);
    }
}
