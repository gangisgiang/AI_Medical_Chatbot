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
            {"pulmonology", "respiratory"}
        };

        private (string, int) PreProcessInput(string input)
        {
            // Lowercase the input and remove special characters
            input = input.ToLower();
            input = Regex.Replace(input, "[^a-zA-Z0-9 ]", "");
            // input = input.Replace("what is", "").Trim();

            // Try to replace synonyms with broader terms
            foreach (var entry in topicMap)
            {
                int keywordIndex = input.IndexOf(entry.Key);
                if (keywordIndex != -1)
                {
                    input = input.Replace(entry.Key, entry.Value);
                    return (input, keywordIndex);
                }
            }

            return (input, -1);
        }

        // Recognise the and return the topic of the input
        public async Task<string> RecogniseAndRespond(string input)
        {
            var (processedInput, keywordIndex) = PreProcessInput(input);

            if (keywordIndex == -1)
            {
                return "Sorry, I don't have enough information on that. Can you try rephrasing your question?";
            }

            string response = string.Empty;

            if (input.Contains("cardiovascular"))
            {
                response = await cardioAIService.GenerateResponse(input);
            }
            
            // Display the response if something is fetched
            if (!string.IsNullOrEmpty(response))
            {
                return response;
            }

            // Fallback response if no specific topic is matched
            return "Sorry, I don't have enough information on that. Can you try rephrasing your question?";
        }
    }
}