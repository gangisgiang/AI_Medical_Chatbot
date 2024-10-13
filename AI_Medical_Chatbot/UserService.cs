using System;
namespace AI_Medical_Chatbot
{
	public class UserService : UserService_I
	{
		private readonly List<User> Users = new List<User>();
		private int nextUserID = 217;

		public void RegisterUser(string username)
		{
			// if the user is already registered, return
			foreach (User existingUser in Users)
			{
				if (existingUser.Username == username)
				{
					Console.WriteLine("User " + existingUser.Username + " is already registered.");
					return;
				}
			}

			User user = new User(username, nextUserID);
			Users.Add(user);
			Console.WriteLine("User " + user.Username + " has been registered.");

			nextUserID++;
		}

		public User GetUser(int userID)
		{
			foreach (User user in Users)
			{
				if (user.UserID == userID)
				{
					return user;
				}
			}
			return null;
		}
	}
}