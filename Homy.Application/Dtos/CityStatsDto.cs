using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homy.Application.Dtos
{
    public class CityStatsDto
    {
        public long CityId { get; set; }
        public int PropertiesCount { get; set; }
        public int ProjectsCount { get; set; }
        public int DistrictsCount { get; set; }
    }
}
