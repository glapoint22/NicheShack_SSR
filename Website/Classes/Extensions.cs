using System.Linq;
using Website.Interfaces;

namespace Website.Classes
{
    public static class Extensions
    {
        // ..................................................................................Sort By.....................................................................
        public static IOrderedQueryable<T> SortBy<T>(this IQueryable<T> source, ISort<T> dto) where T : class
        {
            return dto.SetSortOption(source);
        }


        // ..................................................................................Where.....................................................................
        public static IQueryable<T> Where<T>(this IQueryable<T> source, IWhere<T> dto) where T : class
        {
            return dto.SetWhere(source);
        }



        // ..................................................................................Select.....................................................................
        public static IQueryable<TOut> Select<T, TOut>(this IQueryable<T> source, ISelect<T, TOut> dto) where T : class where TOut : class
        {
            return source.Select(dto.SetSelect());
        }
    }
}
