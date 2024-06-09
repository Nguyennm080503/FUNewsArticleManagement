using BussinessObjects.Models;
using DTOS;

namespace Service.Interface
{
    public interface INewsArticleService
    {
        Task<IEnumerable<NewsArticle>> GetAllNewsArticle();
        Task CreateNewsArticle(NewsCreate newsCreate);
        Task DeleteNewsArticle(string newsID);
        Task UpdateNewsArticle(NewsUpdate newsUpdate);
        Task ChangeStatusNewsArticle(string newsID, int status);
    }
}
