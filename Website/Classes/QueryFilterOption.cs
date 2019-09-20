using Website.Interfaces;

namespace Website.Classes
{
    public struct QueryFilterOption : IQueryFilterOption
    {
        public string Id { get; set; }
        public string Label { get; set; }
    }
}
