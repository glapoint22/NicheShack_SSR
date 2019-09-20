using System.Collections.Generic;

namespace Website.Models
{
    public class List
    {
        public List()
        {
            Collaborators = new HashSet<ListCollaborator>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CollaborateId { get; set; }

        public virtual ICollection<ListCollaborator> Collaborators { get; set; }
    }
}
