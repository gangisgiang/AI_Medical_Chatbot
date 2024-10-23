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
                        UseAsGuest();
                        break;
                    case "4":
                        ResetPassword();
                        break;
                    case "5":
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
            Console.WriteLine("3. Use as Guest");
            Console.WriteLine("4. Reset Password");
            Console.WriteLine("5. Exit");
        }

        private void Exit()
        {
            return;
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
            else
            {
                _chatComponent.SetCurrentUser(_currentUser);
            }
        }

        private void UseAsGuest()
        {
            Console.WriteLine("You are now using the chatbot as a guest. Your conversations will not be saved.");
            _chatComponent.SetGuestMode(true);
            StartGuestConversation();
            _chatComponent.SetGuestMode(false);
        }

        private void StartGuestConversation()
        {
            Console.Write("Ask your question (or type 'exit' to end): ");
            while (true)
            {
                string question = Console.ReadLine();
                if (question.ToLower() == "exit")
                {
                    Console.WriteLine("Ending guest session.");
                    break;
                }
                // Let the ChatComponent handle the chatbot response for the guest
                _chatComponent.AskChatbot(question);
            }
        }

        // Password Reset Function
        private void ResetPassword()
        {
            string email = GetUserInput("Enter your email to reset your password: ");
            
            // Send reset code to email
            bool emailFound = _userService.ResetPassword(email);

            // If the email is not found, return
            if (!emailFound)
            {
                Console.WriteLine("Email not found. Please try again.");
                return;
            }

            // Ask the user to enter the reset code
            string enteredCode = GetUserInput("Enter the reset code sent to your email: ");

            // Verify the reset code and get the username
            string username = _userService.VerifyResetCode(email, enteredCode);
            if (!string.IsNullOrEmpty(username))
            {
                Console.WriteLine($"Reset code verified. The username associated with this email is: {username}");

                // Allow the user to set a new password
                string newPassword = GetUserInput("Enter your new password: ");
                _userService.SetNewPassword(email, newPassword);

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
                        ShowMessageHistory();
                        break;
                    case "3":
                        ContinueConversation();
                        break;
                    case "4":
                        RenameConversation();
                        break;
                    case "5":
                        DeleteConversation();
                        break;
                    case "6":
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
            Console.WriteLine("3. Continue a conversation");
            Console.WriteLine("4. Rename a conversation");
            Console.WriteLine("5. Delete a conversation");
            Console.WriteLine("6. Logout");
        }

        private void StartNewConversation()
        {
            _chatComponent.CreateNewConvo();

            Console.Write("Ask your question (or type 'exit' to end): ");

            while (true)
            {
                string question = Console.ReadLine();

                if (question.ToLower() == "exit")
                {
                    Console.WriteLine("Ending conversation.");
                    break;
                }

                _chatComponent.AskChatbot(question);
            }
        }

        public void ContinueConversation()
        {
            Console.WriteLine("Continue a conversation:");
            _chatComponent.DisplayConvoList();

            string convoIdInput = GetUserInput("Enter the conversation ID to continue: ");
            if (Guid.TryParse(convoIdInput, out Guid convoId))
            {
                bool selected = _chatComponent.ContinueConversation(convoId);
                if (selected)
                {
                    Console.WriteLine("Continuing conversation: " + convoId);
                    Console.WriteLine("Ask your question (or type 'exit' to end): ");
                    while (true)
                    {
                        string question = Console.ReadLine();
                        
                        if (question.ToLower() == "exit")
                        {
                            Console.WriteLine("Ending conversation.");
                            break;
                        }

                        _chatComponent.AskChatbot(question);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid conversation ID.");
                }
            }
            else
            {
                Console.WriteLine("Invalid conversation ID format.");
            }
        }

        private void ShowMessageHistory()
        {
            _chatComponent.DisplayConvoList();

            string convoIdInput = GetUserInput("Enter the conversation ID to view message history: ");
            if (convoIdInput.ToLower() == "exit")
            {
                return;
            }

            if (Guid.TryParse(convoIdInput, out Guid convoId))
            {
                _chatComponent.DisplayConvoMessages(convoId);
            }
            else
            {
                Console.WriteLine("Invalid conversation ID format.");
            }
        }

        private void RenameConversation()
        {
            _chatComponent.DisplayConvoList();

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
            _chatComponent.DisplayConvoList();

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