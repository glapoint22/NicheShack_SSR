using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Website.Interfaces;
using Website.Models;

namespace Website.Classes
{
    public class ProductOrderDTO : IWhere<ProductOrder>
    {
        private readonly string customerId;
        private readonly string filter;
        private readonly string searchWords;

        public string OrderNumber { get; set; }
        public string Date { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentMethodImg { get; set; }
        public double Subtotal { get; set; }
        public double ShippingHandling { get; set; }
        public double Discount { get; set; }
        public double Tax { get; set; }
        public double Total { get; set; }
        public string ProductUrlTitle { get; set; }
        public string Hoplink { get; set; }
        public IEnumerable<OrderProductInfoDTO> Products { get; set; }



        // Constructors
        public ProductOrderDTO() { }


        public ProductOrderDTO(string customerId, string filter, string searchWords)
        {
            this.customerId = customerId;
            this.filter = filter;
            this.searchWords = searchWords;
        }




        // ..................................................................................Set Where.....................................................................
        public IQueryable<ProductOrder> SetWhere(IQueryable<ProductOrder> source)
        {
            // Customer Id
            source = source.Where(x => x.CustomerId == customerId);

            // If there are search words
            if (searchWords != string.Empty)
            {
                source = source.Where(x => x.Id == searchWords);
            }
            else
            {
                // Get orders in a given time frame
                switch (filter)
                {
                    case "last30":
                        source = source.Where(x => x.Date <= DateTime.UtcNow && x.Date > DateTime.UtcNow.AddDays(-30));
                        break;
                    case "6-months":
                        source = source.Where(x => x.Date <= DateTime.UtcNow && x.Date > DateTime.UtcNow.AddMonths(-6));
                        break;
                    default:
                        // Match the year (ex. year-2019)
                        Match match = Regex.Match(filter, @"(\d+)");

                        // Get all orders in a certain year
                        int year = int.Parse(match.Groups[1].Value);
                        source = source.Where(x => x.Date.Year == year);
                        break;
                }
            }

            return source;
        }
    }
}
