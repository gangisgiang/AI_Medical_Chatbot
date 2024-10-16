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
            string cluster = ClusterTopic(message);
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

        public override string ClusterTopic(string input)
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
    }
}