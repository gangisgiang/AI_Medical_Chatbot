using System;
namespace AI_Medical_Chatbot
{
	public class User
	{
		public string Username { get; set; }
		public int UserID { get; set; }
		public List<Message> MessagesHistory { get; set; } = new List<Message>();
		public Conversation conversation { get; set; }

		public User(string name, int userID)
		{
			Username = name;
			UserID = userID;
		}

		public void AskChatbot(string text)
		{
			// Send message to the chatbot
			Message message = new Message(text, UserID, 0)
			{
				SenderID = this.UserID,
				ReceiverID = 0,
				Text = text,
				Time = DateTime.Now
			};

			MessagesHistory.Add(message);
			conversation.AddMessage(message);
		}
	}
}

