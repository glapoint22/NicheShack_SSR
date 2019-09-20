namespace Website.Models
{
    public class OrderProduct
    {
        public string Id { get; set; } // This will be the itemNo from the instant notification
        public string Title { get; set; }
        public int Type { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string OrderId { get; set; }
        public bool IsMain { get; set; } // lineItemType from the instant notification will determine if this field is true or false. If the value is "ORIGINAL", then it will be true


        public virtual ProductOrder ProductOrder { get; set; }
    }
}
