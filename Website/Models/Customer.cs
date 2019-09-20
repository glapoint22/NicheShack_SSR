using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Website.Models
{
    public class Customer : IdentityUser<string>
    {
        public Customer()
        {
            ListCollaborators = new HashSet<ListCollaborator>();
            RefreshTokens = new HashSet<RefreshToken>();
            ProductOrders = new HashSet<ProductOrder>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }


        public virtual ICollection<ListCollaborator> ListCollaborators { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
    }
}
