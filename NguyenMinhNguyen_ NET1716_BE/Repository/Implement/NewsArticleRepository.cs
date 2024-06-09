using Azure;
using BussinessObjects.Models;
using DAO;
using DTOS;
using Repository.Interface;

namespace Repository.Implement
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        public async Task<IEnumerable<NewsArticle>> GetAllNewsArticle()
        {
            return await NewsDao.Instance.GetAllNewsArticle();
        }

        public async Task CreateNewsArticle(NewsCreate newsCreate)
        {
            var id = await NewsDao.Instance.GetMaxID();
            var tags = new List<Tag>();
            if(newsCreate.Tags != null)
            {
                foreach (var tag in newsCreate.Tags)
                {
                    var t = await TagDao.Instance.GetTagByID(tag);
                    tags.Add(t);
                }
                NewsArticle newsArticle = new NewsArticle()
                {
                    NewsArticleId = (id + 1).ToString(),
                    CategoryId = newsCreate.CategoryId,
                    CreatedById = newsCreate.CreatedById,
                    CreatedDate = DateTime.Now,
                    NewsContent = newsCreate.NewsContent,
                    NewsTitle = newsCreate.NewsTitle,
                    NewsStatus = true,
                };
                await NewsDao.Instance.CreateAsync(newsArticle);
                await NewsDao.Instance.CreateNewsTag(newsArticle.NewsArticleId, tags);
            }
            else
            {
                NewsArticle newsArticle = new NewsArticle()
                {
                    NewsArticleId = (id + 1).ToString(),
                    CategoryId = newsCreate.CategoryId,
                    CreatedById = newsCreate.CreatedById,
                    CreatedDate = DateTime.Now,
                    NewsContent = newsCreate.NewsContent,
                    NewsTitle = newsCreate.NewsTitle,
                    NewsStatus = true,
                };
            }
        }

        public async Task DeleteNewsArticle(string newsID)
        {
            await NewsDao.Instance.RemoveNewsTag(newsID);
            await NewsDao.Instance.RemoveAsync(newsID);
        }

        public async Task UpdateNewsArticle(NewsUpdate newsUpdate)
        {
            var news = await NewsDao.Instance.GetDetail(newsUpdate.NewsArticleId);
            news.NewsTitle = newsUpdate.NewsTitle;
            news.NewsContent = newsUpdate.NewsContent;
            news.CategoryId = newsUpdate.CategoryId;
            news.CreatedById = newsUpdate.CreatedById;
            news.ModifiedDate = DateTime.Now;
            var tags = new List<Tag>();
            if(newsUpdate.Tags != null)
            {
                foreach (var tag in newsUpdate.Tags)
                {
                    var t = await TagDao.Instance.GetTagByID(tag);
                    tags.Add(t);
                }

                await NewsDao.Instance.UpdateAsync(news);
                await NewsDao.Instance.UpdateNewsTag(newsUpdate.NewsArticleId, tags);
            }
            else
            {
                await NewsDao.Instance.UpdateAsync(news);
            }
            
        }

        public async Task ChangeStatusNewsArticle(string newsID, int status)
        {
            var news = await NewsDao.Instance.GetDetail(newsID);
            bool newStatus = status == 1 ? true : false;
            news.NewsStatus = newStatus;
            news.ModifiedDate = DateTime.Now;
            await NewsDao.Instance.UpdateAsync(news);

        }
    }
}
