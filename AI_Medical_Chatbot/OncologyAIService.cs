using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public class OncologyAIService : AIService, IAIResponse
    {
        public async Task<string> GenerateResponse(string message)
        {
            // Cluster the input and return cardio-related information
            string cluster = ClusterRespTopic(message);
            string topic = "oncology";

            Console.WriteLine("Cluster: " + cluster);

            if (string.IsNullOrEmpty(cluster))
            {
                return await FetchandConvert("oncology");
            }

            string plainTextData = await FetchandConvert(topic);

            string relevantInfo = ExtractRelevantInfo(plainTextData, cluster);

            if (string.IsNullOrEmpty(relevantInfo))
            {
                return "No relevant information found.";
            }

            return relevantInfo;
        }

        private string ClusterRespTopic(string input)
        {
            // Convert input to lowercase to ensure case-insensitive matching
            input = input.ToLower();

            // Define a manual mapping for keywords to topics based on the file content
            var manualMapping = new Dictionary<string, string>
            {
                { "early breast cancer detection", "breast-cancer" },
                { "breast cancer cells", "breast-cancer" },
                { "lung cancer due to smoking", "lung-cancer" },
                { "lung cancer symptoms", "lung-cancer" },
                { "prostate gland cancer", "prostate-cancer" },
                { "prostate cancer male", "prostate-cancer" },
                { "blood-forming tissues cancer", "leukemia" },
                { "leukemia affects bone marrow", "leukemia" },
                { "blood cell cancer", "leukemia" },
                { "melanoma skin pigmentation", "melanoma" },
                { "skin cancer melanocytes", "melanoma" },
                { "skin cancer serious", "melanoma" }
            };

            // Check if the input matches any keywords
            foreach (var keyword in manualMapping.Keys.OrderByDescending(k => k.Length))
            {
                if (input.Contains(keyword))
                {
                    return manualMapping[keyword];
                }
            }

            // If no match is found, return a default value
            return "oncology";
        }

        private string ExtractRelevantInfo(string plainTextData, string topic)
        {
            // Split the data into paragraphs, ensuring no empty entries
            string[] paragraphs = plainTextData.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // Check for paragraphs that include the specific ID or topic
            var relevantParagraphs = paragraphs.Where(p => p.Contains($"id=\"{topic}\"", StringComparison.OrdinalIgnoreCase)
                || p.Contains(topic, StringComparison.OrdinalIgnoreCase)).ToArray();

            if (relevantParagraphs.Length == 0)
            {
                return "No relevant information found.";
            }

            // Ensure that the first relevant paragraph is meaningful and not just a header
            string firstParagraph = relevantParagraphs.FirstOrDefault(p => p.Split('.').Length > 1)?.Trim();

            if (string.IsNullOrEmpty(firstParagraph))
            {
                return "No relevant information found.";
            }

            return firstParagraph;
        }
    }
}