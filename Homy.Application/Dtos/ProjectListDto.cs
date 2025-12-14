using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Application.Dtos
{
    public class ProjectListDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? LogoUrl { get; set; }
        public string CityName { get; set; } = null!;
        public string? DistrictName { get; set; }
        public bool IsActive { get; set; }
        public int PropertiesCount { get; set; }
        public decimal? MinPrice { get; set; }
    }
}
