using System.Collections.Generic;
using Website.Interfaces;

namespace Website.Classes
{
    public struct FilterData
    {
        public string Type { get; set; }
        public string Caption { get; set; }
        public IEnumerable<IQueryFilterOption> Options { get; set; }
    }
}
