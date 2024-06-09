using BussinessObjects.Models;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implement
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _repository;
        public TagService(ITagRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Tag>> GetAllTags()
        {
            return await _repository.GetAllTags();
        }

        public async Task<IEnumerable<Tag>> GetTagsForNewsArticle(string newsArticleId)
        {
            return await _repository.GetTagsForNewsArticle(newsArticleId);
        }
    }
}
