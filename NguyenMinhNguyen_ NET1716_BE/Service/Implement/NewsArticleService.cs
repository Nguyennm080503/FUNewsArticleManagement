using BussinessObjects.Models;
using DTOS;
using Repository.Interface;
using Service.Interface;

namespace Service.Implement
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly INewsArticleRepository _newsArticleRepository;

        public NewsArticleService(INewsArticleRepository newsArticleRepository)
        {
            _newsArticleRepository = newsArticleRepository;
        }

        public async Task ChangeStatusNewsArticle(string newsID, int status)
        {
            await _newsArticleRepository.ChangeStatusNewsArticle(newsID, status);
        }

        public async Task CreateNewsArticle(NewsCreate newsCreate)
        {
            await _newsArticleRepository.CreateNewsArticle(newsCreate);
        }

        public async Task DeleteNewsArticle(string newsID)
        {
            await _newsArticleRepository.DeleteNewsArticle(newsID);
        }

        public async Task<IEnumerable<NewsArticle>> GetAllNewsArticle()
        {
            return await _newsArticleRepository.GetAllNewsArticle();
        }

        public async Task UpdateNewsArticle(NewsUpdate newsUpdate)
        {
            await _newsArticleRepository.UpdateNewsArticle(newsUpdate);
        }
    }
}
