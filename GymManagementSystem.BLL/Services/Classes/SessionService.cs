using AutoMapper;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Sessions;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Models.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct)
        {
            if(model.EndDate<= model.StartDate) return false;
            if(model.StartDate<= DateTime.UtcNow) return false;
            if(model.Capacity<1 || model.Capacity >25) return false;

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId ,ct);
            if(trainer == null) return false;
            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId,ct);
            if (category is null) return false;

            var isValid = Enum.TryParse<Specialty>(category.CategoryName, out var categorySpecialty);
                if(!isValid || trainer.Specialty != categorySpecialty) return false;

            var session = _mapper.Map<Session>(model);

            _unitOfWork.GetRepository<Session>().Add(session);
            var count = await _unitOfWork.SaveChangesAsync(ct);
            return count>0;
        }

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct)
        {
            //Trainer & Category -> Navigational properties - not included by EFCore
            //-> Implement fn specific to Session - Session repo & include in UoW as property

            var sessions = await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategory(ct);

            if (sessions is null || !sessions.Any()) return null;

            var sessionsViewModel = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);

            foreach (var session in sessionsViewModel)
            {
                session.AvailableSlots = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);

            }

            return sessionsViewModel;
        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetAllTrainersForDropDownAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(false, ct);

            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(trainers);

        }
        public async Task<IEnumerable<CategorySelectViewModel>> GetAllCategoriesForDropDownAsync(CancellationToken ct = default)
        {
            var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(false, ct);

            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(categories);
        }
    }
}
