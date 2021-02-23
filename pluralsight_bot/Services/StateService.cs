using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using pluralsight_bot.Models;
using System;

namespace pluralsight_bot.Services
{
	// Info : 03.01 - Create a State Service (Bot State Class - UserState)
	// Info : 03.02 - Create a class <StateService> (UserState, StateProperty, StatePropertyAccessor)

	public class StateService
	{
		// Info : 03.03 - Create a UserState & ConversationState property
		public UserState UserState { get; }
		public ConversationState ConversationState { get; set; }


		// Info : 03.05 - Create a property <UserProfileId> & <ConversationDataId>
		public static string UserProfileId { get; } = $"{nameof(StateService)}.UserProfile";
		public static string ConversationDataId { get; } = $"{nameof(StateService)}.ConversationData";
		public static string DialogStateId { get; } = $"{nameof(StateService)}.DialogState";


		// Info : 03.06 - Create a accessor for UserProfile & ConversationData (Get, Set, Delete)
		public IStatePropertyAccessor<UserProfile> UserProfileAccessor { get; set; }
		public IStatePropertyAccessor<ConversationData> ConversationDataAccessor { get; set; }
		public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }

		// Info : 03.04 - Create a constructor with injection of UserState & ConversationState
		public StateService(UserState UserState, ConversationState ConversationState)
		{
			this.UserState = UserState ?? throw new ArgumentNullException(nameof(UserState));
			this.ConversationState = ConversationState ?? throw new ArgumentNullException(nameof(ConversationState));

			// Info : 03.08 - Call <InitializeAccessors>  
			InitializeAccessors();
		}

		// Info : 03.07 - Create a method to initialize the accessor
		public void InitializeAccessors()
		{
			// Initialize Conversation State Accessors
			ConversationDataAccessor = ConversationState.CreateProperty<ConversationData>(ConversationDataId);
			DialogStateAccessor = ConversationState.CreateProperty<DialogState>(DialogStateId);

			// Initialize User State
			UserProfileAccessor = UserState.CreateProperty<UserProfile>(UserProfileId);
		}


	}
}
