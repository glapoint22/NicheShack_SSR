using System;
using System.Linq;
using System.Linq.Expressions;
using Website.Interfaces;
using Website.Models;

namespace Website.Classes
{
    public class CategoryDetailDTO : CategoryDTO, ISelect<Category, CategoryDetailDTO>
    {
        public string Icon { get; set; }


        // ..................................................................................Set Select.....................................................................
        public Expression<Func<Category, CategoryDetailDTO>> SetSelect()
        {
            return x => new CategoryDetailDTO
            {
                Id = x.Id,
                Name = x.Name,
                Icon = x.Icon,
                Niches = x.Niches
                    .OrderBy(y => y.Name)
                    .Select(y => new NicheDetailDTO
                    {
                        Id = y.Id,
                        Name = y.Name,
                        Icon = y.Icon
                    })
            };
        }
    }
}
