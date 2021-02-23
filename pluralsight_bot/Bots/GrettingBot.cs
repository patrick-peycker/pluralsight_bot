using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using pluralsight_bot.Models;
using pluralsight_bot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace pluralsight_bot.Bots
{
	// Info : 01.00 - Create a Bot
	// Info : 01.01 - Create a class <GrettingBot> inherits from ActivityHandler (gestionnaire d'activités)
	public class GrettingBot : ActivityHandler
	{
		private readonly StateService stateService;

		// Info : 01.04 - Create a constructor with injection of StateService
		public GrettingBot(StateService stateService)
		{
			this.stateService = stateService ?? throw new ArgumentNullException($"{nameof(stateService)} in GrettingBot");
		}

		// Info : 01.02 - Override the async method <OnMessageActivityAsync>
		protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
		{
			await GetNameAsync(turnContext, cancellationToken);
		}

		// Info : 01.03 - Override a method <OnMemberAddedAsync>
		protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
		{
			foreach (var member in membersAdded)
			{
				if (member.Id != turnContext.Activity.Recipient.Id)
				{
					await GetNameAsync(turnContext, cancellationToken);
				}
			}
		}

		// Info : 01.05 - Create a method <GetName> with ITurnContext & CancellationToken in argument
		private async Task GetNameAsync(ITurnContext turnContext, CancellationToken cancellationToken)
		{
			UserProfile userProfile = await stateService.UserProfileAccessor.GetAsync(turnContext, () => new UserProfile());
			ConversationData conversationData = await stateService.ConversationDataAccessor.GetAsync(turnContext, () => new ConversationData());

			if (!string.IsNullOrEmpty(userProfile.Name))
            {
				await turnContext.SendActivityAsync(MessageFactory.Text(String.Format("Hi {0}. How can I help you today ?", userProfile.Name)), cancellationToken);
            }
			
			else
            {
				if (conversationData.PromptedUserForName)
                {
					// Set the name to what ther user provided
					userProfile.Name = turnContext.Activity.Text?.Trim();

					// Acknowledge that we got their name
					await turnContext.SendActivityAsync(MessageFactory.Text(String.Format("Hi {0}. How can I help you today ?", userProfile.Name)), cancellationToken);

					// Reset the flag to allow the bot to go through the cycle again.
					conversationData.PromptedUserForName = false;
				}

				else 
				{
					// Prompt the user for their name.
					await turnContext.SendActivityAsync(MessageFactory.Text($"What is your name ?"), cancellationToken);

					// Set the flag to true, so we don't prompt in the next turn
					conversationData.PromptedUserForName = true;
				}

				// Save any state changes that might have occured during the turn.
				await stateService.UserProfileAccessor.SetAsync(turnContext, userProfile);
				await stateService.ConversationDataAccessor.SetAsync(turnContext, conversationData);

				await stateService.UserState.SaveChangesAsync(turnContext);
				await stateService.ConversationState.SaveChangesAsync(turnContext);
			}
		}
	}
}
