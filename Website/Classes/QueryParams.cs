using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Website.Classes
{
    public class QueryParams
    {
        public string SearchWords { get; private set; }
        public string Sort { get; private set; }
        public int CategoryId { get; private set; }
        public int NicheId { get; private set; }
        public List<int> CustomFilterOptions;
        public List<KeyValuePair<string, string>> Filters = new List<KeyValuePair<string, string>>();
        public List<KeyValuePair<string, string>> CustomFilters = new List<KeyValuePair<string, string>>();



        public QueryParams(string searchWords, string sort, int categoryId, int nicheId, string filters)
        {
            SearchWords = searchWords;
            Sort = sort;
            CategoryId = categoryId;
            NicheId = nicheId;
            SetFilters(filters);
        }



        // ..................................................................................Set Filters....................................................................
        private void SetFilters(string filterString)
        {
            // Get the filters from the filter string
            var matches = Regex.Matches(filterString, @"([a-zA-Z\s]+)\|([0-9\^\-]+)\|");

            // Create a key value pair for each filter (filter name, filer value)
            foreach (Match match in matches)
            {
                Filters.Add(new KeyValuePair<string, string>(match.Groups[1].Value, match.Groups[2].Value));
            }

            // Get the custom filters from filters
            CustomFilters = Filters.Where(x => x.Key != "Price" && x.Key != "Customer Rating").ToList();


            // Get the filter options from the custom filters and assign it to CustomFilterOptions property
            SetCustomFilterOptions();
        }






        // ..................................................................................Get Price Range................................................................
        public PriceFilterOption GetPriceRange()
        {
            string priceFilter = Filters.Find(x => x.Key == "Price").Value;

            Match result = Regex.Match(priceFilter, @"(\d+\.?(?:\d+)?)-(\d+\.?(?:\d+)?)");
            return new PriceFilterOption
            {
                Min = float.Parse(result.Groups[1].Value),
                Max = float.Parse(result.Groups[2].Value)
            };
        }





        // ........................................................................Set Custom Filter Options................................................................
        public void SetCustomFilterOptions(string exclude = "")
        {
            List<string> options = CustomFilters
                .Where(x => x.Key != exclude)
                .Select(x => x.Value)
                .ToList();

            CustomFilterOptions = new List<int>();

            foreach (string option in options)
            {
                CustomFilterOptions
                    .AddRange(Regex.Matches(option, @"(\d+)")
                        .Select(x => int.Parse(x.Groups[1].Value))
                        .ToList());
            };
        }






        // .................................................................................Get Min Rating................................................................
        public int GetMinRating()
        {
            // Get the customer rating filter
            string ratingString = Filters
                .Where(x => x.Key == "Customer Rating")
                .Select(x => x.Value)
                .Single();

            // Return the lowest rating
            return Regex.Matches(ratingString, @"(\d+)")
                .Select(x => int.Parse(x.Groups[1].Value))
                .ToList()
                .Min();
        }
    }
}