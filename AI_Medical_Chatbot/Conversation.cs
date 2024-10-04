using System;
namespace AI_Medical_Chatbot
{
	public class Conversation
	{
		public int ConvoID { get; set; }
		public List<Message> Messages { get; set; } = new List<Message>();

		public Conversation(int convoID)
		{
			ConvoID = convoID;
		}

		public void AddMessage(Message message)
		{
			// Add message to the conversation
			Messages.Add(message);
		}

		// Display the message in the conversation
		public void DisplayMessages()
		{
			foreach (Message message in Messages)
			{
				message.Sender();
			}
		}

		// Increase the conversation ID
		public void NewConvo()
		{
			ConvoID++;
		}
	}
}

