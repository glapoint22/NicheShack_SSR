using System;

namespace Website.Classes
{
    public struct Collaborator
    {
        public Guid Id { get; set; }
        public string CustomerId { get; set; }
        public string ListId { get; set; }
        public bool IsOwner { get; set; }
        public string Name { get; set; }
    }
}
