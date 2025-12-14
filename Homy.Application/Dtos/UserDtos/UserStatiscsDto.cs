using System;

namespace Homy.Application.Dtos.UserDtos {
	public class UserStatisticsDto
	{
		public int TotalUsers { get; set; }
		public int ActiveUsers { get; set; }
		public int TotalOwners { get; set; }
		public int TotalAgents { get; set; }
		public int VerifiedAgents { get; set; }
		public int UnverifiedAgents { get; set; }
		public int Admins { get; set; }
	}
}