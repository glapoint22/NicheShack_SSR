using Website.Interfaces;

namespace Website.Classes
{
    public struct PriceFilterOption : IQueryFilterOption
    {
        public string Label { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
    }
}
