using System.Linq;

namespace Website.Interfaces
{
    public interface ISort<T> where T : class
    {
        IOrderedQueryable<T> SetSortOption(IQueryable<T> source);
    }
}
