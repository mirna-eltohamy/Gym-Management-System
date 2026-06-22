using GymManagementSystem.BLL.ViewModels.Analytics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IAnalyticService
    {
        Task<AnalyticsViewModel> GetAnalyticsAsync(CancellationToken ct);
    }
}
