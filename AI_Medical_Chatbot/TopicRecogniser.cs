using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Net.Http.Json;

namespace AI_Medical_Chatbot
{
    public class TopicRecogniser : IAIResponse
    {
        public readonly Dictionary<string, string> topicMap;
        public TopicRecogniser()
        {
            topicMap = LoadTopicMapFromFile("topicMap.json");
        }

        private Dictionary<string, string> LoadTopicMapFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string jsonContent = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent) ?? new Dictionary<string, string>();
            }

            Console.WriteLine("File not found. Using default topic map.");
            return new Dictionary<string, string>();
        }

        private (string, string) PreProcessInput(string input)
        {
            // Convert input to lowercase and remove special characters
            input = input.ToLower();
            input = Regex.Replace(input, "[^a-zA-Z0-9 ]", "");

            // Tokenize input to better identify keywords
            string[] tokens = input.Split(' ');

            // lowercase the tokens
            tokens = tokens.Select(token => token.ToLower()).ToArray();

            // Look for keywords in the input
            foreach (var entry in topicMap)
            {
                // Check if any token matches the keyword from the dictionary
                if (tokens.Any(token => token.Contains(entry.Key)))
                {
                    return (entry.Key, entry.Value);
                }
            }

            return (string.Empty, string.Empty);
        }

        public async Task<string> GenerateResponse(string input)
        {
            var (keyword, topic) = PreProcessInput(input);

            if (string.IsNullOrEmpty(topic))
            {
                return "Sorry, I don't have enough information on that. Can you try rephrasing your question?";
            }

            IAIResponse aiService = AIServiceFactory.CreateAIService(topic);

            // Generate the response using the selected AI service
            string response = await aiService.GenerateResponse(input);

            if (!string.IsNullOrEmpty(response))
            {
                return response;
            }

            return "Sorry, I don't have enough information on that. Can you try rephrasing your question?";
        }
    }
}