using Homy.Domin.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Application.Dtos.UserDtos
{
    public class UserFilterDto
    {
        public UserRole? Role { get; set; }           
        public bool? IsVerified { get; set; }         
        public bool? IsActive { get; set; }           
        public string SearchTerm { get; set; }        

        
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
