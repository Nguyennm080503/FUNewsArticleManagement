﻿using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ITagService
    {
        Task<IEnumerable<Tag>> GetAllTags();
        Task<IEnumerable<Tag>> GetTagsForNewsArticle(string newsArticleId);
    }
}
