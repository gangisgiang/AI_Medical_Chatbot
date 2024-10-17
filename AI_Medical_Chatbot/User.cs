using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public class User
	{
		public int UserID { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
		public List<Message> MessagesHistory { get; set; } = new List<Message>();

		public User(int userId, string username, string password, string email)
		{
			UserID = userId;
			Username = username;
			Password = password;
			Email = email;
		}
	}
}