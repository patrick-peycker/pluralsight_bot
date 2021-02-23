using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pluralsight_bot.Models
{
    // Info : 04.00 - Create a ConversationDataModel
    public class ConversationData
    {
        // Track wheter we have already asked the user's name
        public bool PromptedUserForName { get; set; } = false;
    }
}
