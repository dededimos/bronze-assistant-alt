using System;
using System.Collections.Generic;

namespace BronzeArtWebApplication.Shared.Helpers
{
    public class VariousHelpers
    {
        /// <summary>
        /// Constructs a string containing "br" tags to seperate Messages in a MudSnackbar into Lines
        /// </summary>
        /// <param name="messages">The List of Messages</param>
        /// <returns>A Final String containing all Messages to be shown in a MudSnackBar</returns>
        public static string GetSnackBarMultilineMessage(List<string> messages,string firstLineMessage = "")
        {
            string finalString = string.Empty;
            List<string> messagesList = new();

            //If there is no first Message then treat the first message in the Message List as First (does not contain <br/>)
            if (firstLineMessage is "")
            {
                finalString = messages.Count > 0 ? messages[0] : "";
                //Construct a new list and remove the first element 
                if (messages.Count > 0)
                {
                    messagesList.AddRange(messages);
                    messagesList.Remove(messagesList[0]);
                }
                foreach (var msg in messagesList)
                {
                    finalString += $"<br/>{msg}";
                }
            }
            else
            {
                finalString = firstLineMessage;
                foreach (var msg in messages)
                {
                    finalString += $"<br/>{msg}";
                }
            }

            return finalString;
        }

        /// <summary>
        /// Logs Messages to Console
        /// </summary>
        /// <param name="messages">The Collection of Messages</param>
        /// <param name="firstMessage">The First Message if not empty ,otherwise omitted</param>
        public static void LogToConsole(IEnumerable<string> messages , string firstMessage = "")
        {
            if (!string.IsNullOrEmpty(firstMessage))
            {
                Console.WriteLine(firstMessage);
            }

            foreach (var message in messages)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    Console.WriteLine(message);
                }
            }
        }
    }
}
