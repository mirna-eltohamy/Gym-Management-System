using AutoMapper;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Analytics;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class AnalyticService : IAnalyticService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AnalyticService(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<AnalyticsViewModel> GetAnalyticsAsync(CancellationToken ct)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetCountAsync(ct:ct);            
            var activeMembers = await _unitOfWork.GetRepository<Membership>().GetCountAsync(ct:ct);
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetCountAsync(ct:ct);
            var upcomigSessions = await _unitOfWork.SessionRepository.GetCountAsync(s=> s.StartDate>DateTime.Now);
            var ongoingSessions = await _unitOfWork.SessionRepository.GetCountAsync(s=> s.StartDate<=DateTime.Now && s.EndDate >= DateTime.Now);
            var completedSessions = await _unitOfWork.SessionRepository.GetCountAsync(s=> s.EndDate < DateTime.Now);

            var model = new AnalyticsViewModel()
            {
                Members = members,
                ActiveMembers = activeMembers,
                Trainers = trainers,
                UpcomingSessions = upcomigSessions,
                CompletedSessions = completedSessions,
                OngoingSessions = ongoingSessions
            };

            return model;

        }
    }
}
