using System.Linq;
using Website.Interfaces;
using Website.Models;

namespace Website.Classes
{
    public class ProductDetailDTO : ISelect<Product, ProductDetailDTO>
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string UrlTitle { get; set; }
        public double Rating { get; set; }
        public int TotalReviews { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Hoplink { get; set; }
        public string ShareImage { get; set; }
        public double OneStar { get; set; }
        public double TwoStars { get; set; }
        public double ThreeStars { get; set; }
        public double FourStars { get; set; }
        public double FiveStars { get; set; }



        // ..................................................................................Set Select.....................................................................
        public IQueryable<ProductDetailDTO> SetSelect(IQueryable<Product> source)
        {
            return source.Select(x => new ProductDetailDTO
            {
                Id = x.Id,
                Title = x.Title,
                UrlTitle = x.UrlTitle,
                Rating = x.Rating,
                TotalReviews = x.TotalReviews,
                MinPrice = x.MinPrice,
                MaxPrice = x.MaxPrice,
                Image = x.Image,
                Hoplink = x.Hoplink,
                Description = x.Description,
                ShareImage = x.ShareImage,
                OneStar = x.OneStar,
                TwoStars = x.TwoStars,
                ThreeStars = x.ThreeStars,
                FourStars = x.FourStars,
                FiveStars = x.FiveStars
            });
        }
    }
}
