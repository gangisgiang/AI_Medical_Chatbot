using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AI_Medical_Chatbot
{
    public class DatabaseUserService
    {
        private const string UsersPath = "users.json";
        private List<User> Users = new List<User>();
        private EmailService _emailService;
        private Dictionary<string, string> resetCodeMap = new Dictionary<string, string>();

        public DatabaseUserService(EmailService emailService)
        {
            _emailService = emailService;
            LoadUsers();
        }

        public void RegisterUser(string username, string password, string email)
        {
            if (!IsValidEmail(email))
            {
                Console.WriteLine("Invalid email format.");
                return;
            }

            foreach (User existingUser in Users)
            {
                if (existingUser.Username == username)
                {
                    Console.WriteLine("Username already exists. Please choose another.");
                    return;
                }
            }

            // Create new user with Username, Password, and Email
            User newUser = new User(Users.Count + 1, username, password, email);
            Users.Add(newUser);
            SaveUsers();
            Console.WriteLine("User registered successfully.");
        }

        public User LoginUser(string username, string password)
        {
            foreach (User user in Users)
            {
                if (user.Username == username && user.Password == password)
                {
                    return user;
                }
            }
            Console.WriteLine("Invalid username or password.");
            return null;
        }

        public bool ResetPassword(string email, EmailService emailService)
        {
            // Find the user by email
            User user = Users.Find(u => u.Email == email);
            if (user == null)
            {
                Console.WriteLine("Email not found.");
                return false;
            }

            // Generate a reset code
            string resetCode = GenerateResetCode();

            // Store the reset code temporarily
            if (resetCodeMap.ContainsKey(email))
            {
                resetCodeMap[email] = resetCode;
            }
            else
            {
                resetCodeMap.Add(email, resetCode);
            }

            // Send the reset code to the user's email
            emailService.SendEmail(email, "Password Reset Code", "Your password reset code is: " + resetCode);
            Console.WriteLine("A reset code has been sent to your email.");
            return true;
        }

        public bool VerifyResetCode(string email, string enteredCode)
        {
            // Check if the entered code matches the stored code
            if (resetCodeMap.ContainsKey(email) && resetCodeMap[email] == enteredCode)
            {
                resetCodeMap.Remove(email); // Remove code after it's used
                return true;
            }
            return false;
        }

        private string GenerateResetCode()
        {
            Random random = new Random();
            return random.Next(210705, 999999).ToString(); // Generates a 6-digit reset code.
        }

        public void SetNewPassword(string email, string newPassword)
        {
            // Find the user and update their password
            User user = Users.Find(u => u.Email == email);
            if (user != null)
            {
                user.Password = newPassword;
                SaveUsers();
                Console.WriteLine("Password updated successfully.");
            }
            else
            {
                Console.WriteLine("User not found.");
            }
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private void SaveUsers()
        {
            string json = JsonSerializer.Serialize(Users);
            File.WriteAllText(UsersPath, json);
        }

        private void LoadUsers()
        {
            if (File.Exists(UsersPath))
            {
                string json = File.ReadAllText(UsersPath);
                Users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            }
        }
    }
}
