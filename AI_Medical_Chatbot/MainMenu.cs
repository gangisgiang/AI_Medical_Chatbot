using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public class MainMenu
    {
        private ChatbotInterface _chatbotInterface;

        public MainMenu(ChatbotInterface chatbotInterface)
        {
            _chatbotInterface = chatbotInterface;
        }

        public void Show()
        {
            while (true)
            {
                Console.WriteLine("Main Menu:");
                Console.WriteLine("1. Login as Admin");
                Console.WriteLine("2. Use Chatbot");
                Console.WriteLine("3. Exit");
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        LoginAsAdmin();
                        break;
                    case "2":
                        _chatbotInterface.Start(); // Start the chatbot interface
                        break;
                    case "3":
                        Console.WriteLine("Exiting the application.");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private void LoginAsAdmin()
        {
            Console.Write("Enter admin username: ");
            string username = Console.ReadLine();
            Console.Write("Enter admin password: ");
            string password = Console.ReadLine();

            // Hardcoded admin credentials for demonstration
            if (username == "ngheej" && password == "nghesi")
            {
                Console.WriteLine("Admin logged in successfully.");
                AdminInterface adminInterface = new AdminInterface();
                adminInterface.ShowMenu();
            }
            else
            {
                Console.WriteLine("Invalid admin credentials. Please try again.");
            }
        }
    }
}