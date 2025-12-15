using Homy.Domin.models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Application.Dtos.UserDtos.UserSubDTOs
{
    public class UserSubUpdateDTO
    {
        

        
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

       
    }
}
