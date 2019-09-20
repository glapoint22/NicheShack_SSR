using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Website.Interfaces;
using Website.Models;

namespace Website.Classes
{
    public class ProductDTO : ISelect<Product, ProductDTO>, ISort<Product>, IWhere<Product>
    {
        private readonly QueryParams queryParams;
        private readonly IEnumerable<FilteredProduct> filteredProducts;

        public string Id { get; set; }
        public string Title { get; set; }
        public string UrlTitle { get; set; }
        public double Rating { get; set; }
        public int TotalReviews { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public string Image { get; set; }
        public List<KeyValuePair<string, string>> NumProductsPerPageOptions => new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("24", "24"),
            new KeyValuePair<string, string>("48", "48"),
            new KeyValuePair<string, string>("72", "72"),
            new KeyValuePair<string, string>("96", "96")
        };

        public List<KeyValuePair<string, string>> BrowseSortOptions => new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Price: Low to High", "price-asc"),
            new KeyValuePair<string, string>("Price: High to Low", "price-desc"),
            new KeyValuePair<string, string>("Highest Rating", "rating")
        };


        public List<KeyValuePair<string, string>> SearchSortOptions
        {
            get
            {
                List<KeyValuePair<string, string>> options = new List<KeyValuePair<string, string>>();
                options.Add(new KeyValuePair<string, string>("Best Match", "best-match"));
                options.AddRange(BrowseSortOptions);

                return options;
            }
        }


        // Constructors
        public ProductDTO() { }

        public ProductDTO(QueryParams queryParams, IEnumerable<FilteredProduct> filteredProducts)
        {
            this.queryParams = queryParams;
            this.filteredProducts = filteredProducts;
        }






        // ..................................................................................Set Select.....................................................................
        public Expression<Func<Product, ProductDTO>> SetSelect()
        {
            return x => new ProductDTO
            {
                Id = x.Id,
                Title = x.Title,
                UrlTitle = x.UrlTitle,
                Rating = x.Rating,
                TotalReviews = x.TotalReviews,
                MinPrice = x.MinPrice,
                MaxPrice = x.MaxPrice,
                Image = x.Image
            };
        }





        // .............................................................................Set Sort Option.....................................................................
        public IOrderedQueryable<Product> SetSortOption(IQueryable<Product> source)
        {
            IOrderedQueryable<Product> sortOption = null;


            switch (queryParams.Sort)
            {
                case "price-asc":
                    sortOption = source.OrderBy(x => x.MinPrice);
                    break;
                case "price-desc":
                    sortOption = source.OrderByDescending(x => x.MinPrice);
                    break;
                case "rating":
                    sortOption = source.OrderByDescending(x => x.Rating);
                    break;
                default:
                    if (queryParams.SearchWords != string.Empty)
                    {
                        // Best Match
                        sortOption = source.OrderBy(x => x.Title.StartsWith(queryParams.SearchWords) ? (x.Title == queryParams.SearchWords ? 0 : 1) : 2);
                    }
                    else
                    {
                        // Price: Low to High
                        sortOption = source.OrderBy(x => x.MinPrice);
                    }

                    break;
            }

            return sortOption;
        }






        // ..................................................................................Set Where.....................................................................
        public IQueryable<Product> SetWhere(IQueryable<Product> source)
        {
            //Search words
            if (queryParams.SearchWords != string.Empty)
            {
                string[] searchWordsArray = queryParams.SearchWords.Split(' ');
                source = source.Where(x => searchWordsArray.Any(z => x.Title.ToLower().Contains(z.ToLower())));
            }


            //Category
            if (queryParams.CategoryId >= 0)
            {
                source = source.Where(x => x.Niche.CategoryId == queryParams.CategoryId);
            }


            //Niche
            if (queryParams.NicheId >= 0)
            {
                source = source.Where(x => x.NicheId == queryParams.NicheId);
            }


            //Filters
            if (queryParams.Filters.Count > 0)
            {

                //Price Filter
                if (queryParams.Filters.Any(x => x.Key == "Price"))
                {
                    PriceFilterOption priceRange = queryParams.GetPriceRange();

                    if (priceRange.Min == priceRange.Max)
                    {
                        source = source.Where(x =>
                            (x.MaxPrice > x.MinPrice && priceRange.Min >= x.MinPrice && priceRange.Min <= x.MaxPrice) ||
                            (x.MaxPrice <= x.MinPrice && x.MinPrice == priceRange.Min)
                        );
                    }
                    else
                    {
                        source = source.Where(x =>
                            (x.MaxPrice > x.MinPrice && priceRange.Min >= x.MinPrice && priceRange.Min <= x.MaxPrice) ||
                            (x.MaxPrice > x.MinPrice && priceRange.Max >= x.MinPrice && priceRange.Max <= x.MaxPrice) ||
                            (x.MaxPrice > x.MinPrice && priceRange.Min <= x.MinPrice && priceRange.Max >= x.MaxPrice) ||
                            (x.MaxPrice <= x.MinPrice && x.MinPrice >= priceRange.Min && x.MinPrice <= priceRange.Max)
                        );
                    }
                }


                // Customer Rating filter
                if (queryParams.Filters.Any(x => x.Key == "Customer Rating"))
                {
                    int rating = queryParams.GetMinRating();
                    source = source.Where(x => x.Rating >= rating);
                }


                // Custom filters
                if (filteredProducts.Count() > 0)
                {
                    // Group the filtered products into their respective filters outputting only product ids
                    List<List<string>> productIds = filteredProducts
                    .GroupBy(x => x.FilterId)
                    .Select(x => x
                        .Where(a => a.FilterId == x
                            .Select(b => b.FilterId)
                            .FirstOrDefault())
                        .Select(a => a.ProductId)
                        .ToList())
                    .ToList();

                    // Set the where clause for each group of product ids
                    for (int i = 0; i < productIds.Count; i++)
                    {
                        var ids = productIds[i];
                        source = source.Where(x => ids.Contains(x.Id));
                    }
                }
            }
            return source;

        }
    }
}
