using AutoMapper;
using AutoMapper.Execution;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.MemberViewModels;
using GymManagementSystem.BLL.ViewModels.Trainers;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TrainerService(IUnitOfWork unitOfWork
            ,IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }   

        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct)
        {
            var emailExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email, ct);
            if (emailExists) return Result.Validation("Failed to add trainer! Email already exists");

            var phoneExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone, ct);
            if (phoneExists) return Result.Validation("Failed to add trainer! Phone Number already exists");


            var trainer = _mapper.Map<Trainer>(model);

            _unitOfWork.GetRepository<Trainer>().Add(trainer);
            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count>0? Result.OK() : Result.Fail("Failed to add trainer");
        }

        public async Task<bool> DeleteTrainerAsync(int trainerId, CancellationToken ct)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId, ct);

            var scheduledSession = await _unitOfWork.GetRepository<Session>().AnyAsync(s=>s.TrainerId==trainerId && s.StartDate > DateTime.Now, ct);
            if (scheduledSession) return false;

            _unitOfWork.GetRepository<Trainer>().Delete(trainer);
            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count>0;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);

            var trainersViewModel = _mapper.Map<IEnumerable<TrainerViewModel>>(trainers);

            return trainersViewModel;
        }

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId, ct);

            var trainerModel = _mapper.Map<TrainerViewModel>(trainer);

            return trainerModel;

        }

        public async Task<TrainerToUpdateViewModel> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId, ct);

            var model = _mapper.Map<TrainerToUpdateViewModel>(trainer);

            return model;
        }

        public async Task<Result> UpdateTrainerAsync(int trainerId, TrainerToUpdateViewModel model, CancellationToken ct)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId, ct);
            if (trainer is null) return Result.NotFound($"Member with Id {trainerId} not found");


            var emailExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email && t.Id != trainerId, ct);
            if (emailExists) return Result.Validation("Failed to update trainer! Email already exists");

            var phoneExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone && t.Id != trainerId, ct);
            if (phoneExists) return Result.Validation("Failed to update trainer! Phone Number already exists");


            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Address.BuildingNumber = model.BuildingNumber;
            trainer.Address.Street = model.Street;
            trainer.Address.City = model.City;
            trainer.Specialty = model.Specialty;

            _unitOfWork.GetRepository<Trainer>().Update(trainer);
            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count > 0 ? Result.OK() : Result.Fail("Failed to update trainer");
        }
    }
}
