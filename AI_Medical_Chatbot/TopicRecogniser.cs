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
            {"pulmonology", "respiratory"}
        };

        private string PreProcessInput(string input)
        {
            // Lowercase the input and remove special characters
            input = input.ToLower();
            input = Regex.Replace(input, "[^a-zA-Z0-9 ]", "");

            // Try to replace synonyms with broader terms
            foreach (var entry in topicMap)
            {
                if (input.Contains(entry.Key))
                {
                    input = input.Replace(entry.Key, entry.Value);
                }
            }

            return input;
        }

        // Recognise the and return the topic of the input
        public async Task<string> RecogniseandResponseTopic(string input)
        {
            // Preprocess the input using NLP techniques
            input = PreProcessInput(input);

            // Route the input to the appropriate AI service
            if (input.Contains("cardiovascular") || input.Contains("heart") || input.Contains("hypertension") || input.Contains("stroke"))
            {
                return await cardioAIService.GenerateResponse(input);
            }
            // else if (input.Contains("respiratory") || input.Contains("lung") || input.Contains("asthma") || input.Contains("pneumonia") || input.Contains("covid"))
            // {
            //     return await respAIService.GenerateResponse(input);
            // }

            return "Sorry, please provide more details.";
        }
    }
}