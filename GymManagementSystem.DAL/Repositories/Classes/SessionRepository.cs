using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Repositories.Classes
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymDbContext _context;

        public SessionRepository(GymDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategory(CancellationToken ct = default)
            => await _context.Sessions.AsNoTracking().Include(s => s.Trainer).Include(s => s.Category).ToArrayAsync(ct);

        public async Task<int> GetCountOfBookedSlotsAsync(int sessionId, CancellationToken ct = default)
            => await _context.Bookings.AsNoTracking().CountAsync(b => b.SessionId == sessionId, ct);

        public async Task<Session?> GetSessionByIdWithTrainerAndCategoryAsync(int sessionId, CancellationToken ct = default)
            => await _context.Sessions.AsNoTracking().Include(s => s.Trainer).Include(s => s.Category).FirstOrDefaultAsync(s => s.Id == sessionId, ct);
    
        
    
    }
}
