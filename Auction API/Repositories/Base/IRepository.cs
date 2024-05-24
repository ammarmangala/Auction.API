using System.Linq.Expressions;
using Auction_API.Entities.Base;

namespace Auction_API.Repositories.Base
{

    public interface IRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAllByCondition(Expression<Func<T, bool>> predicate);
        T GetById(int id);
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
        bool SaveChanges();
        T GetByCondition(Expression<Func<T, bool>> predicate);
        Task<bool> PlaceBidAsync(T entity, string userId);
    }
}
