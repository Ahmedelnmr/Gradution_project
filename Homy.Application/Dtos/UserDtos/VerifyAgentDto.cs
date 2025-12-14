using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Application.Dtos.UserDtos
{
    public class VerifyAgentDto
    {
        public Guid UserId { get; set; }
        public bool Approve { get; set; }
        public string? RejectionReason { get; set; }
    }
}
