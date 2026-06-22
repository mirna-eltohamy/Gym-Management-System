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

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct)
        {
            if(model.EndDate<= model.StartDate) return Result.Validation("End Date must be after Start Date");
            if(model.StartDate<= DateTime.UtcNow) return Result.Validation("Start Date must be in the future");
            if(model.Capacity<1 || model.Capacity >25) return Result.Validation("Capacity must be between 1 and 25");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId ,ct);
            if(trainer == null) return Result.NotFound($"Trainer with Id {model.TrainerId} not found");
            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId,ct);
            if (category is null) return Result.NotFound($"Category with Id {model.CategoryId} not found");

            var isValid = Enum.TryParse<Specialty>(category.CategoryName, out var categorySpecialty);
                if(!isValid || trainer.Specialty != categorySpecialty) return Result.Validation("Failed to create session! Session category and trainer specialty unmatched");

            var session = _mapper.Map<Session>(model);

            _unitOfWork.GetRepository<Session>().Add(session);
            var count = await _unitOfWork.SaveChangesAsync(ct);
            return count>0? Result.OK(): Result.Fail("Failed to create session");
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

        public async Task<Result<SessionViewModel>> GetSessionByIdAsync(int sessionId, CancellationToken ct)
        {
            var session = await _unitOfWork.SessionRepository.GetSessionByIdWithTrainerAndCategoryAsync(sessionId, ct);

            if (session == null) return Result<SessionViewModel>.NotFound($"Session with Id {sessionId} not found");

            var mappedSession = _mapper.Map<SessionViewModel>(session);

            mappedSession.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);

            return Result<SessionViewModel>.OK(mappedSession);
        }

        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session == null) return Result<UpdateSessionViewModel>.NotFound($"Session with Id {sessionId} not found");

            if (session.StartDate <= DateTime.UtcNow) return Result<UpdateSessionViewModel>.Validation("Cannot edit an Ongoing or Completed session");

            var bookings = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(sessionId, ct);
            if (bookings > 0) return Result<UpdateSessionViewModel>.Fail("Cannot update session that already has bookings");

            var model = _mapper.Map<UpdateSessionViewModel>(session);

            return Result<UpdateSessionViewModel>.OK(model);

        }

        public async Task<Result> UpdateSessionAsync(int sessionId, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.FirstOrDefaultAsync(s=>s.Id ==sessionId, ct);
            if (session == null) return Result.NotFound($"Session with Id {sessionId} not found");

            if (session.StartDate <= DateTime.UtcNow) return Result.Fail("Cannot edit an Ongoing or Completed session");

            if (model.StartDate <= DateTime.UtcNow) return Result.Validation("Start date must be in the future");

            if(model.EndDate <= model.StartDate) return Result.Validation("End date must be after start date");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId, ct);
            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId, ct);

            var isValid = Enum.TryParse<Specialty>(category.CategoryName, out var categorySpecialty);
            if (!isValid || trainer.Specialty != categorySpecialty) return Result.Validation("Session category and trainer specialty unmatched");

            //if (trainer.Specialty.ToString() != session.Category.CategoryName) return Result.Validation();

            var bookings = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(sessionId, ct);
            if (bookings > 0) return Result.Fail("Cannot update session that already has bookings");

            session.StartDate = model.StartDate;
            session.EndDate=model.EndDate;
            session.Description = model.Description;
            session.TrainerId = model.TrainerId;
            session.UpdatedAt = DateTime.Now;

            _unitOfWork.SessionRepository.Update(session);

            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count>0? Result.OK() : Result.Fail("Failed to update session");
        }

        public async Task<Result> DeleteSessionAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session is null) return Result.NotFound($"Session with Id {sessionId} not found");

            if (session.EndDate > DateTime.Now && session.StartDate <= DateTime.Now) return Result.Fail("Cannot remove ongoing session");

            var bookings = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(sessionId, ct);
            if (bookings > 0) return Result.Fail("Cannot remove sessions that already have bookings");

             _unitOfWork.SessionRepository.Delete(session);
            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count>0? Result.OK() : Result.Fail("Failed to remove session");

        }
    }
}
