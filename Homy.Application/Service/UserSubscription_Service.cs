using Homy.Application.Dtos.UserDtos.UserSubDTOs;
using Homy.Domin.Contract_Repo;
using Homy.Domin.Contract_Service;
using Homy.Domin.models;
using Homy.Infurastructure.Repository;
using Homy.Infurastructure.Unitofworks;

namespace Homy.Infurastructure.Service
{
    public class UserSubscription_Service : IUserSubscription_Service
    {
        private readonly IUnitofwork _unitOfWork;

        public UserSubscription_Service(IUnitofwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ================= READ =================

        public async Task<IEnumerable<UserSubReadDTO>> GetAllSubscriptionsAsync()
        {
            var subs = await _unitOfWork.UserSubscriptionRepo.GetAllAsync();
            return subs.Select(MapToReadDTO);
        }

        public async Task<UserSubReadDTO?> GetUserSubscriptionAsync(Guid userId)
        {
            var sub = await _unitOfWork.UserSubscriptionRepo.GetByUserIdAsync(userId);
            if (sub == null) return null;

            return MapToReadDTO(sub);
        }

        public async Task<IEnumerable<UserSubReadDTO>> GetSubscriptionsByPackageAsync(long packageId)
        {
            var subs = await _unitOfWork.UserSubscriptionRepo.GetByPackageIdAsync(packageId);
            return subs.Select(MapToReadDTO);
        }

        public async Task<IEnumerable<UserSubReadDTO>> GetExpiredSubscriptionsAsync()
        {
            var subs = await _unitOfWork.UserSubscriptionRepo.GetExpiredAsync();
            return subs.Select(MapToReadDTO);
        }

        public async Task<IEnumerable<UserSubReadDTO>> GetExpiringIn7DaysAsync()
        {
            var subs = await _unitOfWork.UserSubscriptionRepo.GetExpiringIn7DaysAsync();
            return subs.Select(MapToReadDTO);
        }

        // ================= WRITE =================

        public async Task CreateSubscriptionAsync(UserSubCreateDTO dto)
        {
            var entity = new UserSubscription
            {
                UserId = dto.UserId,
                PackageId = dto.PackageId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                PaymentMethod = dto.PaymentMethod,
                AmountPaid = dto.AmountPaid,
                IsActive = true
            };

            await _unitOfWork.UserSubscriptionRepo.AddAsync(entity);
            await _unitOfWork.Save(); 
    }

        public async Task<bool> UpdateSubscriptionAsync(Guid userId, UserSubUpdateDTO dto)
        {
            var sub = await _unitOfWork.UserSubscriptionRepo.GetByUserIdAsync(userId);
            if (sub == null) return false;

            sub.EndDate = dto.EndDate;
            sub.IsActive = dto.IsActive;

            await _unitOfWork.Save(); 
        return true;
        }

        // ================= MAPPING =================

        private static UserSubReadDTO MapToReadDTO(UserSubscription sub)
        {
            return new UserSubReadDTO
            {
                UserId = sub.UserId,
                UserName = sub.User?.FullName,
                PackageId = sub.PackageId,
                StartDate = sub.StartDate,
                EndDate = sub.EndDate,
                IsActive = sub.IsActive,
                PaymentMethod = sub.PaymentMethod,
                AmountPaid = sub.AmountPaid
            };
        }
    }

}