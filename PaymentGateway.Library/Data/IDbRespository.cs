using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PaymentGateway.Data
{
    public interface IDbRespository <T> where T : class
    {
        Task AddNewItem(T item);
        Task<T> GetItem(Expression<Func<T, bool>> predicate);
        Task UpdateItem(T item);
    }
}