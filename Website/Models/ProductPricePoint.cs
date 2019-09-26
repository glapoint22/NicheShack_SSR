﻿namespace Website.Models
{
    public class ProductPricePoint
    {
        public int Id { get; set; }
        public string ProductId { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }

        public virtual Product Product { get; set; }
    }
}
