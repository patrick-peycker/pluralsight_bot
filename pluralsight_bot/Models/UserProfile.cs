using System;

namespace pluralsight_bot.Models
{
	// Info : 02.00 - Create a User Profile
	// Info : 02.01 - Create a class <UserProfile>
	// Info : 02.02 - Create a property <Name>

	public class UserProfile
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime CallbackTime { get; set; }
		public string PhoneNumber { get; set; }
		public string Bug { get; set; }
	}
}
