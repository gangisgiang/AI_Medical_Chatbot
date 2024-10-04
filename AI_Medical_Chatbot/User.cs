using System;
using System.Collections.Generic;

namespace AI_Medical_Chatbot
{
	public class User
	{
		public string Username { get; set; }
		public int UserID { get; set; }
		public List<Message> MessagesHistory { get; set; } = new List<Message>();

		public User(string name, int userID)
		{
			Username = name;
			UserID = userID;
		}
	}
}