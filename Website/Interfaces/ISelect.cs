using System;
using System.Linq.Expressions;

namespace Website.Interfaces
{
    public interface ISelect<T, TOut> where T : class where TOut : class
    {
        Expression<Func<T, TOut>> SetSelect();
    }
}
