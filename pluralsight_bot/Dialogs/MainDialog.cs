using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Configuration;
using pluralsight_bot.Services;

namespace pluralsight_bot.Dialogs
{
	public class MainDialog : ComponentDialog
	{
		private readonly StateService stateService;

		public MainDialog(StateService stateService) : base(nameof(MainDialog))
		{
			this.stateService = stateService ?? throw new ArgumentNullException(nameof(stateService));

			InitializeWaterfallDialog();
		}

		private void InitializeWaterfallDialog()
		{
			/**
             * We are going to use a waterfall dialog to frame-up our conversation.
             * The great thing about the waterfall dialog is that it gives us a good
             * back and forth template to utilize for our conversation.
             * 
             * The first thing that we are going to do set-up our waterfall dialog
             * is to establish all the steps by creating an instance of the waterfall 
             * steps below. In the list are the detailed methods that are going to be
             * called and on what order in the waterfall flow. The order is very important
             * here since this will be called in the order based on the list.
             */
			var waterfallSteps = new WaterfallStep[]
			{
				InitialStepAsync,
				FinalStepAsync
			};

			// Add Named Dialogs
			AddDialog(new GreetingDialog($"{nameof(MainDialog)}.greeting", stateService));
			AddDialog(new BugReportDialog($"{nameof(MainDialog)}.bugReport", stateService));

			AddDialog(new WaterfallDialog($"{nameof(MainDialog)}.mainFlow", waterfallSteps));

			// Set the starting dialog
			InitialDialogId = $"{nameof(MainDialog)}.mainFlow";
		}

		private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			/**
             * Our bot runs in a LOOP. If on the first loop you input "hi", then 
             * the greeting dialog will be activated.  If not, then the bug report
             * dialog will be activated.
             */
			if (Regex.Match(stepContext.Context.Activity.Text.ToLower(), "hi").Success)
			{
				return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.greeting", null, cancellationToken);
			}

			else
			{
				return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.bugReport", null, cancellationToken);
			}

			/**
             * First, we use the dispatch model to determine which 
             * cognitive service (LUIS or QnA) to use
             */
			//var recognizerResult = await botService.Dispatch.RecognizeAsync(stepContext.Context, cancellationToken);
			/**
             * top intent tell us which cognitive service to use
             */
			//var topIntent = recognizerResult.GetTopScoringIntent();

			//switch (topIntent.intent)
			//{
			//	case "GreetingIntent":
			//		return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.greeting", null, cancellationToken);
			//	case "NewBugReportIntent":
			//		return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.bugReport", null, cancellationToken);
			//	case "QueryBugTypeIntent":
			//		return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.bugType", null, cancellationToken);
			//	default:
			//		await stepContext.Context.SendActivityAsync(MessageFactory.Text($"I'm sorry I don't know what you mean."), cancellationToken);
			//		break;
			//}

			//return await stepContext.NextAsync(null, cancellationToken);
		}

		private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			return await stepContext.EndDialogAsync(null, cancellationToken);
		}
	}
}
