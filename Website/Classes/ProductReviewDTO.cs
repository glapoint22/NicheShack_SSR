using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Website.Interfaces;
using Website.Models;

namespace Website.Classes
{
    public class ProductReviewDTO : ISelect<ProductReview, ProductReviewDTO>, ISort<ProductReview>
    {
        private readonly string sortBy;

        public int Id { get; set; }
        public string Title { get; set; }
        public double Rating { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public bool IsVerified { get; set; }
        public string Text { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public string ProductId { get; set; }
        public KeyValuePair<string, string>[] SortOptions => new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>("High to Low Rating", "high-low-rating"),
            new KeyValuePair<string, string>("Low to High Rating", "low-high-rating"),
            new KeyValuePair<string, string>("Newest to Oldest", "new-old"),
            new KeyValuePair<string, string>("Oldest to Newest", "old-new"),
            new KeyValuePair<string, string>("Most helpful", "most-helpful")
        };

        public int ReviewsPerPage => 10;



        // Constructors
        public ProductReviewDTO() { }

        public ProductReviewDTO(string sortBy)
        {
            this.sortBy = sortBy;
        }



        // ..................................................................................Set Select.....................................................................
        public Expression<Func<ProductReview, ProductReviewDTO>> SetSelect()
        {
            return x => new ProductReviewDTO
            {
                Id = x.Id,
                Title = x.Title,
                ProductId = x.ProductId,
                Rating = x.Rating,
                Username = x.Customer.ReviewName,
                Date = x.Date,
                IsVerified = x.IsVerified,
                Text = x.Text,
                Likes = x.Likes,
                Dislikes = x.Dislikes
            };
        }




        // .............................................................................Set Sort Option.....................................................................
        public IOrderedQueryable<ProductReview> SetSortOption(IQueryable<ProductReview> source)
        {
            IOrderedQueryable<ProductReview> sortOption = null;


            switch (sortBy)
            {
                case "low-high-rating":
                    sortOption = source.OrderBy(x => x.Rating);
                    break;

                case "new-old":
                    sortOption = source.OrderByDescending(x => x.Date);
                    break;

                case "old-new":
                    sortOption = source.OrderBy(x => x.Date);
                    break;

                case "most-helpful":
                    sortOption = source.OrderByDescending(x => x.Likes);
                    break;

                default:
                    // High to low rating
                    sortOption = source.OrderByDescending(x => x.Rating);
                    break;
            }

            return sortOption;
        }
    }
}
