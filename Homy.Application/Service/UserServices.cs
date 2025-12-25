using Homy.Application.Contract_Service;
using Homy.Application.Dtos.UserDtos;
using Homy.Domin.Contract_Service;
using Homy.Domin.models;
using Homy.Infurastructure.Unitofworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Homy.Application.Service
{
    
    public class User_Service : IUser_Service
    {
        private readonly IUnitofwork _unitOfWork;

        public User_Service(IUnitofwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResponse<UserDto>> GetAllUsersAsync(UserFilterDto filter)
        {
            var response = new PaginatedResponse<UserDto>();
            try
            {
                // جلب كل المستخدمين حسب الفلترة
                var allUsers = await _unitOfWork.UserRepo.GetAllUsersAsync(
                    role: filter.Role,
                    isVerified: filter.IsVerified,
                    isActive: filter.IsActive,
                    searchTerm: filter.SearchTerm
                );

                // حساب العدد الإجمالي
                response.TotalCount = allUsers.Count();
                response.CurrentPage = filter.PageNumber;
                response.PageSize = filter.PageSize;

                // تطبيق الـ Pagination
                var pagedUsersList = allUsers
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();

                // Map to DTOs
                var userDtos = new List<UserDto>();
                foreach (var user in pagedUsersList)
                {
                    userDtos.Add(MapUserToDto(user));
                }

                response.Data = userDtos;
                response.Success = true;
                response.Message = $"تم جلب {userDtos.Count} من {response.TotalCount} مستخدم";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "حدث خطأ أثناء جلب المستخدمين";
                response.Errors.Add(ex.Message);
                
                if (ex.InnerException != null)
                {
                    response.Errors.Add($"Inner: {ex.InnerException.Message}");
                }
            }
            return response;
        }


        public async Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid userId)
        {
            var response = new ApiResponse<UserDto>();

            try
            {
                var user = await _unitOfWork.UserRepo.GetUserByIdAsync(userId);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "المستخدم غير موجود";
                    return response;
                }

                response.Data = MapUserToDto(user);
                response.Success = true;
                response.Message = "تم جلب بيانات المستخدم بنجاح";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "حدث خطأ أثناء جلب بيانات المستخدم";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

       
        public async Task<ApiResponse<IEnumerable<UserDto>>> GetUnverifiedAgentsAsync()
        {
            var response = new ApiResponse<IEnumerable<UserDto>>();

            try
            {
                var agents = await _unitOfWork.UserRepo.GetUnverifiedAgentsAsync();

                response.Data = agents.Select(MapUserToDto).ToList();
                response.Success = true;
                response.Message = $"يوجد {agents.Count()} سمسار في انتظار التوثيق";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "حدث خطأ أثناء جلب السماسرة";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<ApiResponse<bool>> VerifyAgentAsync(VerificationRequestDto request)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var user = await _unitOfWork.UserRepo.GetUserByIdAsync(request.UserId);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "المستخدم غير موجود";
                    return response;
                }

                if (user.Role != UserRole.Agent)
                {
                    response.Success = false;
                    response.Message = "المستخدم ليس سمسار";
                    return response;
                }

                
                var result = await _unitOfWork.UserRepo.UpdateVerificationStatusAsync(
                    request.UserId,
                    request.IsApproved,
                    request.Reason
                );

                if (result)
                {
                    await _unitOfWork.Save();

                    // Create notification for user
                    var notification = new Notification
                    {
                        UserId = request.UserId,
                        Title = request.IsApproved 
                            ? "تم توثيق حسابك بنجاح! ✅" 
                            : "تم رفض طلب التوثيق ❌",
                        Message = request.IsApproved 
                            ? "مبروك! تم توثيق حسابك بنجاح. يمكنك الآن الاستفادة من جميع مميزات الحساب الموثق." 
                            : $"عذراً، تم رفض طلب التوثيق. السبب: {request.Reason}. يمكنك تقديم طلب جديد بعد تصحيح المشكلة.",
                        Type = request.IsApproved 
                            ? NotificationType.VerificationApproved 
                            : NotificationType.VerificationRejected,
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false
                    };

                    await _unitOfWork.NotificationRepo.AddAsync(notification);
                    await _unitOfWork.Save();

                    

                    response.Success = true;
                    response.Data = true;
                    response.Message = request.IsApproved
                        ? "تم توثيق السمسار بنجاح"
                        : $"تم رفض طلب التوثيق. السبب: {request.Reason}";
                }
                else
                {
                    response.Success = false;
                    response.Message = "فشل تحديث حالة التوثيق";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "حدث خطأ أثناء التحقق من السمسار";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<ApiResponse<bool>> UpdateUserActiveStatusAsync(UpdateUserStatusDto request)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _unitOfWork.UserRepo.UpdateActiveStatusAsync(
                    request.UserId,
                    request.IsActive
                    );

                if (result)
                {
                    await _unitOfWork.Save();
                    response.Success = true;
                    response.Data = true;
                    response.Message = request.IsActive 
                        ? "تم تفعيل المستخدم بنجاح"
                        : "تم إلغاء تفعيل المستخدم بنجاح";
                }
                else
                {
                    response.Success = false;
                    response.Message = "المستخدم غير موجود";
                }
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = "حدث خطأ أثناء تحديث حالة المستخدم";
                response.Errors.Add(ex.Message);
            }

            return response;
        }



        public async Task<ApiResponse<bool>> DeleteUserAsync(Guid userId)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var user = await _unitOfWork.UserRepo.GetUserByIdAsync(userId);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "المستخدم غير موجود";
                    return response;
                }

                
                if (user.Role == UserRole.Admin)
                {
                    response.Success = false;
                    response.Message = "لا يمكن حذف حساب الأدمن";
                    return response;
                }

                var result = await _unitOfWork.UserRepo.DeleteUserAsync(userId);

                if (result)
                {
                    await _unitOfWork.Save();

                    response.Success = true;
                    response.Data = true;
                    response.Message = "تم حذف المستخدم بنجاح";
                }
                else
                {
                    response.Success = false;
                    response.Message = "فشل حذف المستخدم";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "حدث خطأ أثناء حذف المستخدم";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        
        public async Task<ApiResponse<UserStatisticsDto>> GetUserStatisticsAsync()
        {
            var response = new ApiResponse<UserStatisticsDto>();

            try
            {
                var stats = await _unitOfWork.UserRepo.GetUserStatisticsAsync();

                response.Data = new UserStatisticsDto
                {
                    TotalUsers = stats["TotalUsers"],
                    ActiveUsers = stats["ActiveUsers"],
                    TotalOwners = stats["TotalOwners"],
                    TotalAgents = stats["TotalAgents"],
                    VerifiedAgents = stats["VerifiedAgents"],
                    UnverifiedAgents = stats["UnverifiedAgents"],
                    Admins = stats["Admins"]
                };

                response.Success = true;
                response.Message = "تم جلب الإحصائيات بنجاح";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "حدث خطأ أثناء جلب الإحصائيات";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        
        public async Task<ApiResponse<IEnumerable<UserDto>>> SearchUsersAsync(string searchTerm)
        {
            var response = new ApiResponse<IEnumerable<UserDto>>();

            try
            {
                var users = await _unitOfWork.UserRepo.GetAllUsersAsync(
                    searchTerm: searchTerm
                );

                response.Data = users.Select(MapUserToDto).ToList();
                response.Success = true;
                response.Message = $"تم العثور على {users.Count()} مستخدم";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "حدث خطأ أثناء البحث";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        
        private UserDto MapUserToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                WhatsAppNumber = user.WhatsAppNumber,
                Email = user.Email,
                ProfileImageUrl = user.ProfileImageUrl,
                Role = user.Role,
                RoleText = GetRoleText(user.Role),
                IsVerified = user.IsVerified,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                PropertiesCount = user.Properties?.Count ?? 0,
                SavedPropertiesCount = user.SavedProperties?.Count ?? 0,
                HasActiveSubscription = user.Subscriptions?.Any(s => s.IsActive) ?? false,
                
                // Verification Documents
                IdCardFrontUrl = user.IdCardFrontUrl,
                IdCardBackUrl = user.IdCardBackUrl,
                SelfieWithIdUrl = user.SelfieWithIdUrl,
                VerificationRejectReason = user.VerificationRejectReason
            };
        }

        
        private string GetRoleText(UserRole role)
        {
            return role switch
            {
                UserRole.Owner => "مالك",
                UserRole.Agent => "سمسار",
                UserRole.Admin => "أدمن",
                _ => "غير محدد"
            };
        }
    }
}