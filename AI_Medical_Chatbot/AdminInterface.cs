using System;

namespace AI_Medical_Chatbot
{
    public class AdminInterface
    {
        private Admin _admin;

        public AdminInterface()
        {
            _admin = Admin.GetInstance();
        }

        public void ShowMenu()
        {
            Console.WriteLine("Admin Menu:");
            Console.WriteLine("1. View All Users");
            Console.WriteLine("2. Delete a User");
            Console.WriteLine("3. Add a New User");
            Console.WriteLine("4. Log Out");
            Console.Write("Select an option: ");

            string choice = Console.ReadLine();
            HandleChoice(choice);
        }

        private void HandleChoice(string choice)
        {
            switch (choice)
            {
                case "1":
                    _admin.ViewAllUsers();
                    break;
                case "2":
                    string deleteUsername = GetUserInput("Enter the username of the user to delete: ");
                    _admin.DeleteUser(deleteUsername);
                    break;
                case "3":
                    string username = GetUserInput("Enter new username: ");
                    string password = GetUserInput("Enter password: ");
                    string email = GetUserInput("Enter email: ");
                    _admin.AddUser(username, password, email);
                    break;
                case "4":
                    Console.WriteLine("Logging out.");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }

            ShowMenu();
        }

        private string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}