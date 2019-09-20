using System.Collections.Generic;

namespace Website.Models
{
    public class Product
    {
        public Product()
        {
            ProductFilters = new HashSet<ProductFilter>();
            ProductMedia = new HashSet<ProductMedia>();
            ProductContent = new HashSet<ProductContent>();
            ProductPricePoints = new HashSet<ProductPricePoint>();
            ProductReviews = new HashSet<ProductReview>();
            ListProducts = new HashSet<ListProduct>();
            ProductOrders = new HashSet<ProductOrder>();
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string UrlTitle { get; set; }
        public int NicheId { get; set; }
        public string Hoplink { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string ShareImage { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public bool Featured { get; set; }
        public int TotalReviews { get; set; }
        public double Rating { get; set; }
        public double OneStar { get; set; }
        public double TwoStars { get; set; }
        public double ThreeStars { get; set; }
        public double FourStars { get; set; }
        public double FiveStars { get; set; }


        public virtual Niche Niche { get; set; }
        public virtual ICollection<ProductFilter> ProductFilters { get; set; }
        public virtual ICollection<ProductMedia> ProductMedia { get; set; }
        public virtual ICollection<ProductContent> ProductContent { get; set; }
        public virtual ICollection<ProductPricePoint> ProductPricePoints { get; set; }
        public virtual ICollection<ProductReview> ProductReviews { get; set; }
        public virtual ICollection<ListProduct> ListProducts { get; set; }
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
    }
}
