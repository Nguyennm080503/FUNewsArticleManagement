using BussinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class TagDao : BaseDao<Tag>
    {
        private static TagDao instance = null;
        private static readonly object instacelock = new object();

        private TagDao()
        {

        }

        public static TagDao Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TagDao();
                }
                return instance;
            }
        }

        public string GetNameTag(int tagID)
        {
            var context = new FunewsManagementDbContext();
            return context.Tags.FirstOrDefaultAsync(x => x.TagId == tagID).Result.TagName;
        }

        public async Task<Tag> GetTagByID(int tag)
        {
            var context = new FunewsManagementDbContext();
            return await context.Tags.FirstOrDefaultAsync(x => x.TagId == tag);
        }
    }
}
