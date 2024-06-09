using BussinessObjects.Models;
using DTOS;

namespace Repository.Interface
{
    public interface INewsArticleRepository
    {
        Task<IEnumerable<NewsArticle>> GetAllNewsArticle();
        Task CreateNewsArticle(NewsCreate newsCreate);
        Task DeleteNewsArticle(string newsID);
        Task UpdateNewsArticle(NewsUpdate newsUpdate);
        Task ChangeStatusNewsArticle(string newsID, int status);
    }
}
