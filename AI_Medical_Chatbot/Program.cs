using System;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    class Program
    {
        private static readonly UserService userService = new UserService();
        public static async Task Main(string[] args)
        {
            // Assuming you have some mechanism for registering users
            Console.WriteLine("User Alice has been registered.");
            
            // Simulate user asking a question
            Console.WriteLine($"Alice is asking a question:");
            string userQuestion = "What is heart disease?";

            // Display the user’s question
            Console.WriteLine($"Displaying the conversation:");
            Console.WriteLine($"User: " + userQuestion);

            // Fetch and display the AI response
            TopicRecogniser topicRecogniser = new TopicRecogniser();
            string response = await topicRecogniser.RecogniseAndRespond(userQuestion);

            // Display the AI's response in the conversation
            Console.WriteLine("AI Response: " + response);
        }
    }
}
