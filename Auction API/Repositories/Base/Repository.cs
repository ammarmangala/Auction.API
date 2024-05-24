using System.Linq.Expressions;
using Auction_API.Data;
using Auction_API.Entities;
using Auction_API.Entities.Base;

namespace Auction_API.Repositories.Base
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public T GetByCondition(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate).SingleOrDefault();
        }

        public async Task<bool> PlaceBidAsync(T entity, string userId)
        {
            if (entity is Bid bid)
            {
                bid.BidderId = userId;
                _context.Set<T>().Add(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public IEnumerable<T> GetAllByCondition(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate).ToList();
        }

        public T GetById(int id)
        {
            return _context.Set<T>().SingleOrDefault(x => x.Id == id);
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() > 0;
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }
    }
}
