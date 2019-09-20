using System;
using System.Collections.Generic;

namespace Website.Models
{
    public class ListCollaborator
    {
        public ListCollaborator()
        {
            ListProducts = new HashSet<ListProduct>();
        }


        public Guid Id { get; set; }
        public string CustomerId { get; set; }
        public string ListId { get; set; }
        public bool IsOwner { get; set; }


        public virtual Customer Customer { get; set; }
        public virtual List List { get; set; }
        public virtual ICollection<ListProduct> ListProducts { get; set; }
    }
}
