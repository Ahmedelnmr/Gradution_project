using System;
namespace Homy.Application.Dtos.UserDtos
{
    public class UpdateUserStatusDto
    {
        public Guid UserId { get; set; }
        public bool IsActive { get; set; }
    }
}