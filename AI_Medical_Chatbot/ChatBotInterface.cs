using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public class ChatbotInterface
    {
        private readonly DatabaseUserService _userService;
        private readonly ChatComponent _chatComponent;
        private readonly EmailService _emailService;
        private User _currentUser;

        public ChatbotInterface(DatabaseUserService userService, ChatComponent chatComponent, EmailService emailService)
        {
            _userService = userService;
            _chatComponent = chatComponent;
            _emailService = emailService;
        }

        public void Start()
        {
            while (true)
            {
                DisplayMainMenu();
                string choice = GetUserInput("Select an option: ");

                switch (choice)
                {
                    case "1":
                        Register();
                        break;
                    case "2":
                        Login();
                        if (_currentUser != null)
                        {
                            ShowUserMenu();
                        }
                        break;
                    case "3":
                        ResetPassword();
                        break;
                    case "4":
                        Exit();
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private void DisplayMainMenu()
        {
            Console.WriteLine("Main Menu:");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Reset Password");
            Console.WriteLine("4. Exit");
        }

        private void Exit()
        {
            Console.WriteLine("Goodbye!");
        }

        private void Register()
        {
            string username = GetUserInput("Enter a username: ");
            string password = GetUserInput("Enter a password: ");
            string email = GetUserInput("Enter your email: ");
            
            _userService.RegisterUser(username, password, email);
        }

        private void Login()
        {
            string username = GetUserInput("Enter your username: ");
            string password = GetUserInput("Enter your password: ");

            _currentUser = _userService.LoginUser(username, password);
            if (_currentUser == null)
            {
                Console.WriteLine("Login failed. Invalid credentials.");
            }
        }

        // Password Reset Function
        private void ResetPassword()
        {
            string email = GetUserInput("Enter your email to reset your password: ");
            
            // Step 1: Send reset code to email
            _userService.ResetPassword(email, _emailService);

            // Step 2: Ask the user for the reset code
            string enteredCode = GetUserInput("Enter the reset code sent to your email: ");

            // Step 3: Verify the reset code
            if (_userService.VerifyResetCode(email, enteredCode))
            {
                Console.WriteLine("Reset code verified.");

                // Step 4: Allow the user to set a new password
                string newPassword = GetUserInput("Enter your new password: ");

                // Step 5: Update password in the database
                _userService.SetNewPassword(email, newPassword);

                // Step 6: Log in with the new password
                Console.WriteLine("Your password has been reset. You can now log in with your new password.");
            }
            else
            {
                Console.WriteLine("Invalid reset code. Please try again.");
            }
        }

        private void ShowUserMenu()
        {
            bool loggedIn = true;
            while (loggedIn)
            {
                DisplayUserMenu();
                string choice = GetUserInput("Select an option: ");

                switch (choice)
                {
                    case "1":
                        StartNewConversation();
                        break;
                    case "2":
                        _chatComponent.DisplayConvo();
                        break;
                    case "3":
                        RenameConversation();
                        break;
                    case "4":
                        DeleteConversation();
                        break;
                    case "5":
                        loggedIn = false;
                        _currentUser = null;
                        Console.WriteLine("Logged out successfully.");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private void DisplayUserMenu()
        {
            Console.WriteLine("User Menu:");
            Console.WriteLine("1. Start a new conversation");
            Console.WriteLine("2. View conversation history");
            Console.WriteLine("3. Rename a conversation");
            Console.WriteLine("4. Delete a conversation");
            Console.WriteLine("5. Logout");
        }

        private void StartNewConversation()
        {
            _chatComponent.CreateNewConvo();
            while (true)
            {
                string question = GetUserInput("Ask your question (or type 'exit' to end): ");
                
                if (question.ToLower() == "exit")
                {
                    Console.WriteLine("Ending conversation.");
                    break;
                }

                _chatComponent.AskChatbot(_currentUser, question);
                _chatComponent.DisplayConvo();
            }
        }

        private void RenameConversation()
        {
            string convoIdInput = GetUserInput("Enter the conversation ID to rename: ");
            if (Guid.TryParse(convoIdInput, out Guid convoId))
            {
                string newName = GetUserInput("Enter the new name for the conversation: ");
                ExecuteCommand(new RenameConvoCmd(_chatComponent, convoId, newName));
            }
            else
            {
                Console.WriteLine("Invalid conversation ID format.");
            }
        }

        private void DeleteConversation()
        {
            string convoIdInput = GetUserInput("Enter the conversation ID to delete: ");
            if (Guid.TryParse(convoIdInput, out Guid convoId))
            {
                ExecuteCommand(new DeleteConvoCmd(_chatComponent, convoId));
            }
            else
            {
                Console.WriteLine("Invalid conversation ID format.");
            }
        }

        private void ExecuteCommand(Command command)
        {
            command.Execute();
        }

        private string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}