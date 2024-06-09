using BussinessObjects.Models;
using DAO;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implement
{
    public class TagRepository : ITagRepository
    {
        public async Task<IEnumerable<Tag>> GetAllTags()
        {
            return await TagDao.Instance.GetAllAsync();
        }
        public async Task<IEnumerable<Tag>> GetTagsForNewsArticle(string newsArticleId)
        {
            return await NewsDao.Instance.GetTagsForNewsArticle(newsArticleId);
        }
    }
}
