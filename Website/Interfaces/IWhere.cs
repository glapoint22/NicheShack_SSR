using System.Linq;

namespace Website.Interfaces
{
    public interface IWhere<T> where T: class
    {
        IQueryable<T> SetWhere(IQueryable<T> source);
    }
}
