using System;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize user service and chat component
            UserService_I userService = new DatabaseUserService();
            ChatComponent chatComponent = ChatComponent.GetInstance(new DatabaseConvoService());

            // Register a new user (Alice) and simulate a conversation
            userService.RegisterUser("Alice");
            User alice = userService.GetUser(1); // Assuming Alice gets UserID 1

            if (alice != null)
            {
                // Create a new conversation for Alice
                chatComponent.CreateNewConvo();
                
                // Start conversation loop
                bool continueChatting = true;

                while (continueChatting)
                {
                    Console.WriteLine("\nAlice is asking a question (type 'exit' to stop): ");
                    string question = Console.ReadLine(); // Take user input from the console

                    // Check if the user wants to exit
                    if (question.ToLower() == "exit")
                    {
                        continueChatting = false;
                        Console.WriteLine("Ending conversation.");
                    }
                    else
                    {
                        // Ask the chatbot and process response
                        chatComponent.AskChatbot(alice, question);

                        // Display the conversation history
                        chatComponent.DisplayConvo();
                    }
                }
            }
            else
            {
                Console.WriteLine("User Alice was not found.");
            }
        }
    }
}
