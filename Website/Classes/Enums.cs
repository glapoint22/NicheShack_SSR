namespace Website.Classes
{
    public struct Enums
    {
        public enum ProductMediaType
        {
            Video,
            Image
        }

        public enum ProductContentType
        {
            PDF,
            DVD
        }

        public enum PricePointFrequency
        {
            SinglePayment,
            Monthly,
            Quarterly,
            Yearly
        }


        public enum OrderProductTypes
        {
            Physical,
            Digital,
            Recurring
        }
    }
}
