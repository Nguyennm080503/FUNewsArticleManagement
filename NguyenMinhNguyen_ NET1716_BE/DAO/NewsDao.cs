using BussinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace DAO
{
    public class NewsDao : BaseDao<NewsArticle>
    {
        private static NewsDao instance = null;
        private static readonly object instacelock = new object();

        private NewsDao()
        {

        }

        public static NewsDao Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NewsDao();
                }
                return instance;
            }
        }

        public async Task<IEnumerable<NewsArticle>> GetAllNewsArticle()
        {
            var datacontext = new FunewsManagementDbContext();
            var news = await datacontext.NewsArticles.Include(x => x.Category).Include(x => x.CreatedBy).OrderByDescending(x => x.CreatedDate).ToListAsync();
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };

            return news;
        }

        public async Task<NewsArticle> GetDetail(string newsID)
        {
            var datacontext = new FunewsManagementDbContext();
            return await datacontext.NewsArticles.FirstOrDefaultAsync(x => x.NewsArticleId == newsID);
        }

        public async Task<int> GetMaxID()
        {
            var _context = new FunewsManagementDbContext();
            return int.Parse(_context.NewsArticles.Max(x => x.NewsArticleId));
        }

        public async Task<IEnumerable<Tag>> GetTagsForNewsArticle(string newsArticleId)
        {
            var _context = new FunewsManagementDbContext();
            var tags =  _context.NewsArticles
                                .Where(n => n.NewsArticleId == newsArticleId)
                                .SelectMany(n => n.Tags)
                                .ToList();
            return tags;
        }

        public async Task CreateNewsTag(string newID, List<Tag> tags)
        {
            var _context = new FunewsManagementDbContext();
            foreach (var tag in tags)
            {
                var newsTag = new Dictionary<string, object>
                {
                    ["NewsArticleId"] = newID,
                    ["TagId"] = tag.TagId
                };
                _context.Set<Dictionary<string, object>>("NewsTag").Add(newsTag);
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateNewsTag(string newID, List<Tag> tags)
        {
            var _context = new FunewsManagementDbContext();
            var existingNewsTags = _context.Set<Dictionary<string, object>>("NewsTag")
            .Where(nt => nt["NewsArticleId"].ToString() == newID);

            _context.Set<Dictionary<string, object>>("NewsTag").RemoveRange(existingNewsTags);
            await _context.SaveChangesAsync();

            foreach (var tag in tags)
            {
                var newsTag = new Dictionary<string, object>
                {
                    ["NewsArticleId"] = newID,
                    ["TagId"] = tag.TagId
                };
                _context.Set<Dictionary<string, object>>("NewsTag").Add(newsTag);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveNewsTag(string newID)
        {
            var _context = new FunewsManagementDbContext();
            var existingNewsTags = _context.Set<Dictionary<string, object>>("NewsTag")
            .Where(nt => nt["NewsArticleId"].ToString() == newID);

            _context.Set<Dictionary<string, object>>("NewsTag").RemoveRange(existingNewsTags);

            await _context.SaveChangesAsync();
        }
    }
}
