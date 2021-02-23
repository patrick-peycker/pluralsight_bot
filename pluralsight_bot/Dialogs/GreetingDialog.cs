﻿using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using pluralsight_bot.Models;
using pluralsight_bot.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace pluralsight_bot.Dialogs
{
	public class GreetingDialog : ComponentDialog
	{
		private readonly StateService stateService;

		public GreetingDialog(string dialogId, StateService stateService) : base(dialogId)
		{
			this.stateService = stateService ?? throw new ArgumentNullException(nameof(stateService));

			InitializeWaterfallDialog();
		}

		private void InitializeWaterfallDialog()
		{
			// Create Waterfall Steps
			var waterfallSteps = new WaterfallStep[]
			{
				InitialStepAsync,
				FinalStepAsync
			};

			// Add Named Dialogs
			AddDialog(new WaterfallDialog($"{nameof(GreetingDialog)}.mainFlow", waterfallSteps));
			AddDialog(new TextPrompt($"{nameof(GreetingDialog)}.name"));

			// Set the starting Dialog
			InitialDialogId = $"{nameof(GreetingDialog)}.mainFlow";
		}

		private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			UserProfile userProfile = await stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());

			if (string.IsNullOrEmpty(userProfile.Name))
			{
				/*
				* If we don't have the name, then we will kick start the text prompt dialog
				* that we have just added in the code with the ID:
				* 
				* AddDialog(new TextPrompt($"{nameof(GreetingDialog)}.name"));
				* 
				 */

				return await stepContext.PromptAsync(
					$"{nameof(GreetingDialog)}.name",
					new PromptOptions { Prompt = MessageFactory.Text("What is your name ? ") },
					cancellationToken);
			}

			else
			{
				return await stepContext.NextAsync(null, cancellationToken);
			}
		}

		private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			UserProfile userProfile = await stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());

			if (string.IsNullOrEmpty(userProfile.Name))
			{
				// Set the name
				userProfile.Name = (string)stepContext.Result;

				// Save any state changes that might have occured during the turn
				await stateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);
			}

			await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("Hi {0}.  How can I Help you today ?", userProfile.Name)), cancellationToken);
			return await stepContext.EndDialogAsync(null, cancellationToken);
		}
	}
}