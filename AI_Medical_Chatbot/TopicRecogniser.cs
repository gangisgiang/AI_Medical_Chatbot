using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AI_Medical_Chatbot
{
    public class TopicRecogniser
    {
        private readonly CardioAIService cardioAIService = new CardioAIService();
        // private readonly RespAIService respAIService = new RespAIService();

        public readonly Dictionary<string, string> topicMap = new Dictionary<string, string>()
        {
            {"cardio", "cardiovascular"},
            {"heart", "cardiovascular"},
            {"hypertension", "cardiovascular"},
            {"blood pressure", "cardiovascular"},
            {"stroke", "cardiovascular"},
            {"cardiovascular", "cardiovascular"},
            {"heart disease", "cardiovascular"},
            {"respiratory", "respiratory"},
            {"lung", "respiratory"},
            {"asthma", "respiratory"},
            {"bronchitis", "respiratory"},
            {"chronic obstructive pulmonary disease", "respiratory"},
            {"emphysema", "respiratory"},
            {"pneumonia", "respiratory"},
            {"tuberculosis", "respiratory"},
            {"covid", "respiratory"},
            {"coronavirus", "respiratory"},
            {"covid-19", "respiratory"},
            {"sars-cov-2", "respiratory"},
            {"sars", "respiratory"},
            {"mers", "respiratory"},
            {"mers-cov", "respiratory"},
            {"influenza", "respiratory"},
            {"flu", "respiratory"},
            {"common cold", "respiratory"},
            {"pulmonary", "respiratory"},
            {"pulmonology", "respiratory"},
            {"neuro", "neurology"},
            {"neuron", "neurology"},
            {"brain", "neurology"},
            {"nervous system", "neurology"},
            {"brainstem", "neurology"},
            {"cerebellum", "neurology"},
            {"myelin", "neurology"},
            {"axon", "neurology"},
        };

        private (string, string) PreProcessInput(string input)
        {
            // Convert input to lowercase and remove special characters
            input = input.ToLower();
            input = Regex.Replace(input, "[^a-zA-Z0-9 ]", "");

            // Tokenize input to better identify keywords
            string[] tokens = input.Split(' ');

            // Look for keywords in the input
            foreach (var entry in topicMap)
            {
                // Check if any token matches the keyword from the dictionary
                if (tokens.Any(token => token.Contains(entry.Key)))
                {
                    Console.WriteLine($"Matched keyword: {entry.Key} -> Topic: {entry.Value}");
                    return (entry.Key, entry.Value);
                }
            }

            return (string.Empty, string.Empty); // Return empty if no match is found
        }

        // Recognise the and return the topic of the input
        public async Task<string> RecogniseAndRespond(string input)
        {
            var (keyword, topic) = PreProcessInput(input);

            if (string.IsNullOrEmpty(topic))
            {
                return "Sorry, I don't have enough information on that. Can you try rephrasing your question?";

            }

            Console.WriteLine("Processing input for topic: " + topic);

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