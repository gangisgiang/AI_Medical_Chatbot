using System;
namespace AI_Medical_Chatbot
{
	public class UserService : IUserService
	{
		private readonly List<User> Users = new List<User>();

		public void RegisterUser(string username, int userID)
		{
			// if the user is already registered, return
			foreach (User existingUser in Users)
			{
				if (existingUser.UserID == userID)
				{
					Console.WriteLine("User " + existingUser.Username + " is already registered.");
					return;
				}
			}

			User user = new User(username, userID);
			Users.Add(user);
			Console.WriteLine("User " + user.Username + " has been registered.");
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