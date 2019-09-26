using System.Linq;

namespace Website.Interfaces
{
    public interface ISelect<T, TOut> where T : class where TOut : class
    {
        IQueryable<TOut> SetSelect(IQueryable<T> source);
    }
}
