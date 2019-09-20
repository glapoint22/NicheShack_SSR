using System;
using System.Linq.Expressions;
using Website.Interfaces;
using Website.Models;
using static Website.Classes.Enums;

namespace Website.Classes
{
    public class ProductMediaDTO : ISelect<ProductMedia, ProductMediaDTO>
    {
        public string Url { get; set; }
        public string Thumbnail { get; set; }
        public string Type { get; set; }



        // ..................................................................................Set Select.....................................................................
        public Expression<Func<ProductMedia, ProductMediaDTO>> SetSelect()
        {
            return x => new ProductMediaDTO
            {
                Url = x.Url,
                Thumbnail = x.Thumbnail,
                Type = ((ProductMediaType)x.Type).ToString("g")
            };
        }
    }
}
