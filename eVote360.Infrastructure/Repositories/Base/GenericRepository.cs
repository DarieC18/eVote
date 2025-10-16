using EVote360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using AppRepos = EVote360.Application.Abstractions.Repositories;

namespace EVote360.Infrastructure.Repositories.Base
{
    public class GenericRepository<T> : AppRepos.IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _ctx;
        private readonly DbSet<T> _set;

        public GenericRepository(AppDbContext ctx)
        {
            _ctx = ctx;
            _set = _ctx.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _set.FindAsync(new object?[] { id }, ct);

        public async Task<IEnumerable<T>> ListAsync(
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Expression<Func<T, bool>>? predicate = null,
            string[]? includes = null,
            CancellationToken ct = default)
        {
            IQueryable<T> q = _set.AsQueryable();

            if (predicate is not null) q = q.Where(predicate);

            if (includes is not null)
                foreach (var inc in includes)
                    q = q.Include(inc);

            if (orderBy is not null) q = orderBy(q);

            return await q.AsNoTracking().ToListAsync(ct);
        }

        public async Task<int> CountAsync(
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken ct = default)
        {
            IQueryable<T> q = _set.AsQueryable();
            if (predicate is not null) q = q.Where(predicate);
            return await q.CountAsync(ct);
        }

        public async Task AddAsync(T entity, CancellationToken ct = default)
        {
            await _set.AddAsync(entity, ct);
            await _ctx.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            _set.Update(entity);
            await _ctx.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(T entity, CancellationToken ct = default)
        {
            _set.Remove(entity);
            await _ctx.SaveChangesAsync(ct);
        }
    }
}
