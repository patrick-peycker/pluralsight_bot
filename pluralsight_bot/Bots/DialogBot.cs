using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using pluralsight_bot.Helpers;
using pluralsight_bot.Services;

namespace pluralsight_bot.Bots
{
	// Info : 05.00 - Create a new Bot - Dialog Bot
	public class DialogBot<T> : ActivityHandler where T : Dialog
	{
		protected readonly Dialog dialog;
		protected readonly StateService stateService;
		protected readonly ILogger logger;

		public DialogBot(T dialog, StateService stateService, ILogger<DialogBot<T>> logger)
		{
			this.dialog = dialog ?? throw new ArgumentNullException($"{nameof(dialog)} in Dialog Bot");
			this.stateService = stateService ?? throw new ArgumentNullException($"{nameof(stateService)} in Dialog Bot");
			this.logger = logger ?? throw new ArgumentNullException($"{nameof(logger)} in Dialog Bot");
		}

		/*
		* This is another hook of the base class of the ActivityHandler.
		* This is called anytime a bot gets any activity at all regardless of the message type.
		* So, if the bot receives a message, this OnTurnAsync method and OnMessageActivityAsync will be called.
		*/

		public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
		{
			await base.OnTurnAsync(turnContext, cancellationToken);

			// Save any state changes that might have occured during the turn
			await stateService.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
			await stateService.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
		}

		protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
		{
			logger.LogInformation("Running dialog with MessageActivity.");

			// Run the dialog with the new message Activity.
			await dialog.Run(turnContext, stateService.DialogStateAccessor, cancellationToken);
		}

	}
}
