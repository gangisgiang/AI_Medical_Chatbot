using System;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize the EmailService
            EmailService emailService = new EmailService();

            // Initialize the user service with email service
            DatabaseUserService userService = new DatabaseUserService(emailService);

            // Initialize the conversation database service and chat component
            DatabaseConvoService convoService = new DatabaseConvoService();
            ChatComponent chatComponent = ChatComponent.GetInstance(convoService);

            // Initialize the chatbot interface with all necessary services
            ChatbotInterface chatbotInterface = new ChatbotInterface(userService, chatComponent, emailService);

            // Start the chatbot interface
            Console.WriteLine("Welcome to the AI Medical Chatbot System!");
            MainMenu mainMenu = new MainMenu(chatbotInterface);
            mainMenu.Show();
        }
    }
}

