using System;
using System.Collections.Generic;

namespace AI_Medical_Chatbot
{
    public class Conversation
    {
        public Guid ConvoID { get; } // Unique identifier for each conversation
        public string ConvoName { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();

        public Conversation(Guid convoID, string convoName)
        {
            ConvoID = convoID;
            ConvoName = convoName;
        }

        public void AddMessage(Message message)
        {
            Messages.Add(message);
        }

        public void DisplayMessages()
        {
            foreach (var message in Messages)
            {
                message.Sender();
            }
        }
    }
}
