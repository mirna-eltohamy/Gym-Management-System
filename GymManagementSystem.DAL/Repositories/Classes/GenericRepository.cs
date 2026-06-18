using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GymManagementSystem.DAL.Repositories.Classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly DbSet<TEntity> _dbSet;
        private readonly GymDbContext _context;

        public GenericRepository(GymDbContext context) 
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default)
            =>tracking? await _dbSet.ToListAsync(ct): await _dbSet.AsNoTracking().ToListAsync(ct) ;

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _dbSet.FindAsync(id, ct);

        public void Add(TEntity entity)
            => _dbSet.AddAsync(entity);
       
        public void Update(TEntity entity)
            => _dbSet.Update(entity);

        public void Delete(TEntity entity)
            => _dbSet.Remove(entity);
       

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
            => await _dbSet.AnyAsync(predicate,ct);

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
            => await _dbSet.FirstOrDefaultAsync(predicate, ct);


    }
}
