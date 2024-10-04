using System;
namespace AI_Medical_Chatbot
{
	public class User
	{
		public string Username { get; set; }
		public int UserID { get; set; }
		public int convoID = 1;
		public List<Message> MessagesHistory { get; set; } = new List<Message>();
		public Conversation ConversationList { get; set; }
		private static int convoCount = 0;

		public User(string name, int userID)
		{
			Username = name;
			UserID = userID;
			ConversationList = new Conversation(convoID);
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
			ConversationList.AddMessage(message);
		}

		public void ChatbotAnswer(string text)
		{
			// Receive message from the chatbot
			Message message = new Message(text, 0, UserID)
			{
				SenderID = 0,
				ReceiverID = this.UserID,
				Text = text,
				Time = DateTime.Now
			};

			MessagesHistory.Add(message);
			ConversationList.AddMessage(message);
		}
	}
}

