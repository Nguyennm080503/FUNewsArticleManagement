using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOS
{
    public class NewsUpdate
    {
        public string NewsArticleId { get; set; } = null!;

        public string? NewsTitle { get; set; }

        public string? NewsContent { get; set; }

        public short? CategoryId { get; set; }

        public short? CreatedById { get; set; }


        public virtual ICollection<int> Tags { get; set; } = new List<int>();
    }
}
