using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Repositories.Interfaces
{
    public interface IPlanRepository 
    {
        Task<IEnumerable<Plan>> GetAllAsync(bool tracking = default, CancellationToken ct= default);
        Task<Plan?> GetByIdAsync(int id, CancellationToken ct= default);

        Task<int> AddAsync(Plan plan, CancellationToken ct= default);
        Task<int> UpdateAsync(Plan plan, CancellationToken ct= default);
        Task<int> DeleteAsync(Plan plan, CancellationToken ct= default);


    }
}
