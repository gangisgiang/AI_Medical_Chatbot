using System;
using System.Collections.Generic;

namespace AI_Medical_Chatbot
{
	public class User
	{
		public string Username { get; set; }
		public int UserID { get; set; }
		public List<Message> MessagesHistory { get; set; } = new List<Message>();
		public List<Conversation> ConversationList { get; set; } = new List<Conversation>();
		private static int convoCount = 0;

		public User(string name, int userID)
		{
			Username = name;
			UserID = userID;
			CreateNewConvo();
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
			// Add message to the latest conversation
			ConversationList[convoCount - 1].AddMessage(message);
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
			ConversationList[convoCount - 1].AddMessage(message);
		}

		public void CreateNewConvo()
		{
			if (convoCount == 0)
			{
				// Create the initial conversation
				Conversation initialConvo = new Conversation(convoCount, "Conversation");
				convoCount++;
				ConversationList.Add(initialConvo);
			}
			else
			{
				// Create a new conversation
				Conversation convo = new Conversation(convoCount, "Conversation " + "(" + convoCount + ")");
				convoCount++;
				ConversationList.Add(convo);
				Console.WriteLine(convo.ConvoName + " created.");
			}
		}

		public void RenameConvo(int convoIndex, string newName)
		{
			if (convoCount == 0)
			{
				Console.WriteLine("No conversation to rename.");
				return;
			}

			if (convoIndex < 0 || convoIndex >= convoCount)
			{
				Console.WriteLine("Invalid conversation index.");
				return;
			}

			// Rename the conversation
			ConversationList[convoCount - 1].ConvoName = newName;
		}

		public void DisplayConvo()
		{
			// Display the messages in the chosen conversation
			foreach (Conversation convo in ConversationList)
			{
				Console.WriteLine("--- " + convo.ConvoName + " ---");
				convo.DisplayMessages();
			}	
		}
	}
}

