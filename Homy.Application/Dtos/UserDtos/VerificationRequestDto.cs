using System;
namespace Homy.Application.Dtos.UserDtos
{
    public class VerificationRequestDto
    {
            public Guid UserId { get; set; }
            public bool IsApproved { get; set; }        
            public string Reason { get; set; }
    }
}


