using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL
{
    public interface IUnitOfWork
    {
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity , new();

        Task<int> SaveChangesAsync(CancellationToken ct = default);

        public ISessionRepository SessionRepository { get; }
    }
}
