using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public class RespAIService : AIService, IAIResponse
    {
        public async Task<string> GenerateResponse(string message)
        {
            // Cluster the input and return cardio-related information
            string cluster = ClusterTopic(message);
            string topic = "respiratory";

            if (string.IsNullOrEmpty(cluster))
            {
                return await FetchandConvert("respiratory");
            }

            string plainTextData = await FetchandConvert(topic);

            string relevantInfo = ExtractRelevantInfo(plainTextData, cluster);

            if (string.IsNullOrEmpty(relevantInfo))
            {
                return "No relevant information found.";
            }

            return relevantInfo;
        }

        public override string ClusterTopic(string input)
        {
            // Convert input to lowercase to ensure case-insensitive matching
            input = input.ToLower();

            // Define a manual mapping for keywords to topics based on the file content
            var manualMapping = new Dictionary<string, string>
            {
                { "tracheostomy", "respiratory failure" },
                { "trachea", "respiratory failure" },
                { "spirometry", "lung diseases" },
                { "sputum", "pneumonia" },
                { "ventilation", "respiratory failure" },
                { "vital capacity", "respiratory failure" },
                { "sleep apnea", "respiratory failure" },
                { "smoking", "lung cancer" },
                { "wheezing", "asthma" },
                { "cyanosis", "respiratory distress syndrome" },
                { "hypoxia", "respiratory distress syndrome" },
                { "hypoxemia", "respiratory distress syndrome" },
                { "dyspnea", "respiratory failure" },
                { "pleurisy", "pneumonia" },
                { "pulmonary embolism", "pulmonary embolism" },
                { "pulmonary fibrosis", "lung diseases" },
                { "pulmonary hypertension", "lung diseases" },
                { "pulmonary", "lung diseases" },
                { "bronchitis", "bronchitis" },
                { "asthma", "asthma" },
                { "emphysema", "chronic obstructive pulmonary disease" },
                { "pneumonia", "pneumonia" },
                { "respiratory syncytial virus", "pneumonia" },
                { "sarcoidosis", "lung diseases" },
                { "influenza", "bronchitis" },
                { "tuberculosis", "tuberculosis" },
                { "cough", "bronchitis" },
                { "chest", "bronchitis" },
                { "distress syndrome", "distress syndrome" },
                { "failure", "respiratory failure" },
                { "lung", "lung diseases" },
                { "breathing", "respiratory failure" },
                { "breath", "respiratory failure" },
                { "breathe", "respiratory failure" },
                { "respiratory", "respiratory failure" }
            };

            manualMapping = manualMapping.ToDictionary(kvp => kvp.Key.ToLower(), kvp => kvp.Value);

            // Check if the input matches any keywords
            foreach (var keyword in manualMapping.Keys.OrderByDescending(k => k.Length))
            {
                if (input.Contains(keyword))
                {
                    return manualMapping[keyword];
                }
            }

            // If no match is found, return a default value
            return "respiratory";
        }

        public override string ExtractRelevantInfo(string plainTextData, string topic)
        {
            string[] paragraphs = plainTextData.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            var relevantParagraphs = paragraphs.Where(p => p.Contains(topic, StringComparison.OrdinalIgnoreCase)
                || p.Contains(topic, StringComparison.OrdinalIgnoreCase)).ToArray();

            if (relevantParagraphs.Length == 0)
            {
                return "No relevant information.";
            }

            string? firstParagraph = relevantParagraphs.FirstOrDefault(p => p.Split('.').Length > 1)?.Trim();

            if (string.IsNullOrEmpty(firstParagraph))
            {
                return "No relevant information found.";
            }

            return firstParagraph;
        }
    }
}