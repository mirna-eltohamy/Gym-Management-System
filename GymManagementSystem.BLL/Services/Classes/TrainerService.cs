using AutoMapper;
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

        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct)
        {
            var emailExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email, ct);
            var phoneExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone, ct);

            if (emailExists || phoneExists) return false;

            var trainer = _mapper.Map<Trainer>(model);

            _unitOfWork.GetRepository<Trainer>().Add(trainer);
            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count>0;
        }

        public async Task<bool> DeleteTrainerAsync(int trainerId, CancellationToken ct)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId, ct);

            var scheduledSession = await _unitOfWork.GetRepository<Session>().AnyAsync(s=>s.TrainerId==trainerId && s.StartDate > DateTime.Now, ct);
            if(scheduledSession) return false;

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

        public async Task<bool> UpdateTrainerAsync(TrainerToUpdateViewModel model, CancellationToken ct)
        {
            var emailExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email && t.Id!=model.id, ct);
            var phoneExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone && t.Id != model.id, ct);

            if (emailExists || phoneExists) return false;

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.id, ct);

            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Specialty = model.Specialty;
            trainer.Address.BuildingNumber = model.BuildingNumber;
            trainer.Address.Street = model.Street;
            trainer.Address.City = model.City;

            _unitOfWork.GetRepository<Trainer>().Update(trainer);
            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count > 0;
        }
    }
}
