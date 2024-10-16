using System;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    class Program
    {
        private static readonly UserService userService = new UserService();

        public static TopicRecogniser topicRecogniser = new TopicRecogniser();
        public static async Task Main(string[] args)
        {
            
            // Assuming you have some mechanism for registering users
            Console.WriteLine("User Alice has been registered.");
            
            // Simulate user asking a question
            string userQuestion;
            Console.WriteLine($"Alice is asking a question:");
            userQuestion = Console.ReadLine();

            // Display the user’s question
            Console.WriteLine($"Displaying the conversation:");
            Console.WriteLine($"User: " + userQuestion);

            // Fetch and display the AI response
            string response = await topicRecogniser.GenerateResponse(userQuestion);

            // Display the AI's response in the conversation
            Console.WriteLine("AI Response: " + response);

            // Simulate user asking another question
            // Console.WriteLine($"Alice is asking another question:");
            // string userQuestion2 = "What are the symptoms of heart disease?";

            // // Display the user’s question
            // Console.WriteLine($"Displaying the conversation:");
            // Console.WriteLine($"User: " + userQuestion2);

            // // Fetch and display the AI response
            // string response2 = await topicRecogniser.RecogniseAndRespond(userQuestion2);

            // // Display the AI's response in the conversation
            // Console.WriteLine("AI Response: " + response2);

            // Initialize user service and chat component

            // Register a new user (Alice) and simulate a conversation
            userService.RegisterUser("Alice");
            User alice = userService.GetUser(1); // Assuming Alice gets UserID 1

            // if (alice != null)
            // {
            //     // Create a new conversation for Alice
            //     chatComponent.CreateNewConvo();
                
            //     // Start conversation loop
            //     bool continueChatting = true;

            //     while (continueChatting)
            //     {
            //         Console.WriteLine("\nAlice is asking a question (type 'exit' to stop): ");
            //         string question = Console.ReadLine(); // Take user input from the console

            //         // Check if the user wants to exit
            //         if (question.ToLower() == "exit")
            //         {
            //             continueChatting = false;
            //             Console.WriteLine("Ending conversation.");
            //         }
            //         else
            //         {
            //             // Ask the chatbot and process response
            //             chatComponent.AskChatbot(alice, question);

            //             // Display the conversation history
            //             chatComponent.DisplayConvo();
            //         }
            //     }
            // }
            // else
            // {
            //     Console.WriteLine("User Alice was not found.");
            // }
        }
    }
}
