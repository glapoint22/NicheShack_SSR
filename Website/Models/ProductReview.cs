using System;

namespace Website.Models
{
    public class ProductReview
    {
        public int Id { get; set; }
        public string ProductId { get; set; }
        public string Title { get; set; }
        public double Rating { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public bool IsVerified { get; set; }
        public string Text { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }


        public virtual Product Product { get; set; }
    }
}
