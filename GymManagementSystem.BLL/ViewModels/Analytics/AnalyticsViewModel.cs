
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.ViewModels.Analytics
{
    public class AnalyticsViewModel
    {
        public int Members { get; set; }
        public int ActiveMembers { get; set; }
        public int Trainers { get; set; }
        public int UpcomingSessions { get; set; }
        public int OngoingSessions { get; set; }
        public int CompletedSessions { get; set; }

    }
}
