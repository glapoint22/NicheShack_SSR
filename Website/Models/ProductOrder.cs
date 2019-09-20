using System;
using System.Collections.Generic;

namespace Website.Models
{
    public class ProductOrder
    {
        public ProductOrder()
        {
            OrderProducts = new HashSet<OrderProduct>();
        }

        public string Id { get; set; }
        public string CustomerId { get; set; }
        public DateTime Date { get; set; }
        public int PaymentMethod { get; set; }
        public double Subtotal { get; set; }
        public double ShippingHandling { get; set; }
        public double Discount { get; set; }
        public double Tax { get; set; }
        public double Total { get; set; }
        public string ProductId { get; set; }



        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
