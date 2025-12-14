using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Application.Dtos.UserDtos
{
    public class UserListDto
    {
        public UserStatsDto Stats { get; set; } = null;
        //public IPagedList<UserDto> Users { get; set; } = null;
    }
}
