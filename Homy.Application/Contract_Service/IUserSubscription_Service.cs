using Homy.Application.Dtos.UserDtos.UserSubDTOs;

namespace Homy.Domin.Contract_Service
{
    public interface IUserSubscription_Service
    {
        Task<IEnumerable<UserSubReadDTO>> GetAllSubscriptionsAsync();
        Task<UserSubReadDTO?> GetUserSubscriptionAsync(Guid userId);
        Task<IEnumerable<UserSubReadDTO>> GetSubscriptionsByPackageAsync(long packageId);
        Task<IEnumerable<UserSubReadDTO>> GetExpiredSubscriptionsAsync();
        Task<IEnumerable<UserSubReadDTO>> GetExpiringIn7DaysAsync();

        Task CreateSubscriptionAsync(UserSubCreateDTO dto);
        Task<bool> UpdateSubscriptionAsync(Guid userId, UserSubUpdateDTO dto);
    }
}