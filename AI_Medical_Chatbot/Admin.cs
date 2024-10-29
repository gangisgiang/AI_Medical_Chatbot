using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace AI_Medical_Chatbot
{
    public class Admin
    {
        private static Admin? _instance;
        private List<User> _users = new List<User>();
        private const string UsersFilePath = "users.json";

        private Admin()
        {
            LoadUsersFromFile();
        }

        public static Admin GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Admin();
            }
            return _instance;
        }

        private void LoadUsersFromFile()
        {
            if (File.Exists(UsersFilePath))
            {
                try
                {
                    string json = File.ReadAllText(UsersFilePath);
                    _users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to load users from file: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Users file not found. No users loaded.");
            }
        }

        public void ViewAllUsers()
        {
            Console.WriteLine("Registered Users:"   );
            foreach (User user in _users)
            {
                Console.WriteLine("UserId: " + user.UserID + "Username: " + user.Username + ", Email: " + user.Email);
            }
        }

        public void DeleteUser(string username)
        {
            User deletedUser = _users.FirstOrDefault(u => u.Username == username);
            if (deletedUser != null)
            {
                _users.Remove(deletedUser);
                Console.WriteLine("User deleted successfully.");
            }
            else
            {
                Console.WriteLine("User not found.");
            }
        }

        public void AddUser(string username, string password, string email)
        {
            if (_users.Any(existingUser => existingUser.Username == username))
            {
                Console.WriteLine("Username already exists. Please choose another.");
                return;
            }

            User newUser = new User(_users.Count + 1, username, password, email);
            _users.Add(newUser);
            Console.WriteLine("User registered successfully.");
        }
    }
}