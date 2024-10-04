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

		public void DisplayConversation()
		{
			// Display the conversation
			foreach (Message message in Messages)
			{
				message.Sender();
			}
		}

		public void ChatbotResponse(string text)
		{
			// Send message to the user
			Message message = new Message(text, 0, 1)
			{
				SenderID = 0,
				ReceiverID = 1,
				Text = text,
				Time = DateTime.Now
			};

			Messages.Add(message);
		}
	}
}

