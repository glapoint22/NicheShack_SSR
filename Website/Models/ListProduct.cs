using System;

namespace Website.Models
{
    public class ListProduct
    {
        public string ProductId { get; set; }
        public Guid CollaboratorId { get; set; }
        public DateTime DateAdded { get; set; }

        public virtual Product Product { get; set; }
        public virtual ListCollaborator Collaborator { get; set; }
    }
}
