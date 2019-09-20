using System;

namespace Website.Models
{
    public class RefreshToken
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public DateTime Expiration { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
