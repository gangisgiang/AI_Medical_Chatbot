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

        public void ResetPassword(string email, EmailService emailService)
        {
            User user = Users.Find(u => u.Email == email);

            if (user == null)
            {
                Console.WriteLine("Email not found.");
                return;
            }

            string resetCode = GenerateResetCode();
            emailService.SendEmail(email, "Password Reset Code", $"Your password reset code is: {resetCode}");
            Console.WriteLine("A reset code has been sent to your email. Please enter the code to proceed.");
        }

        private string GenerateResetCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString(); // Generates a 6-digit reset code.
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public void SendEmail(string email, string subject, string body)
        {
            try
            {
                // For demo purposes, just display the email content
                Console.WriteLine($"Sending email to {email}: \nSubject: {subject}\nBody: {body}");
                // In a real system, you'd use an SMTP client to send an email
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email. Error: {ex.Message}");
            }
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
