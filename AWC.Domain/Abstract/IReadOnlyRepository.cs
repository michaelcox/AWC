using System;
using System.Linq;

namespace AWC.Domain.Abstract
{
    interface IReadOnlyRepository
    {
        T Single<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class, new();
        IQueryable<T> All<T>() where T : class, new();
    }
}
