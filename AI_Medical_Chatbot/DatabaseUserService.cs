// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Text.Json;
// using System.Text.Json.Serialization;

// namespace AI_Medical_Chatbot
// {
//     public class DatabaseUserService : UserService_I
//     {
//         private readonly string UsersPath = "users.json";
// 		private List<User> Users = new List<User>();
		
// 		public DatabaseUserService()
// 		{
// 			// Load users from the database
// 			LoadUsers();    
// 		}

// 		public void RegisterUser(string username)
// 		{
// 			// if the user is already registered, return
// 			foreach (User existingUser in Users)
// 			{
// 				if (existingUser.Username == username)
// 				{
// 					Console.WriteLine("User " + existingUser.Username + " is already registered.");
// 					return;
// 				}
// 			}

// 			User user = new User(username, );
// 			Users.Add(user);
// 			Console.WriteLine("User " + user.Username + " has been registered.");
// 			SaveUsers();
// 		}
        
// 		private void SaveUsers()
// 		{
// 			string json = JsonSerializer.Serialize(Users);
// 			File.WriteAllText(UsersPath, json);
// 		}

//         public User GetUser(int userID)
//         {
//             foreach (User user in Users)
//             {
//                 if (user.UserID == userID)
//                 {
//                     return user;
//                 }
//             }
//             return null;
//         }

//         private void LoadUsers()
//         {
//             if (File.Exists(UsersPath))
//             {
//                 string json = File.ReadAllText(UsersPath);
//                 Users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
//             }
//             else
//             {
//                 Users = new List<User>();
//             }
//         }
//     }
// }