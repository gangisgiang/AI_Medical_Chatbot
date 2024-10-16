using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public class NephrologyAIService : AIService, IAIResponse
    {
        public async Task<string> GenerateResponse(string message)
        {
            // Cluster the input and return cardio-related information
            string cluster = ClusterTopic(message);
            string topic = "nephrology";

            Console.WriteLine("Cluster: " + cluster);

            if (string.IsNullOrEmpty(cluster))
            {
                return await FetchandConvert("nephrology");
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
                {"chronic kidney disease", "Chronic Kidney Disease"},
                {"stones", "Kidney Stones"},
                {"nephritis", "Nephritis"},
                {"dialysis", "Dialysis"},
                {"polycystic kidney disease", "Polycystic Kidney Disease"},
                {"renal replacement", "Dialysis"},
                {"end-stage", "Chronic Kidney Disease"},
                {"loss of kidney function", "Chronic Kidney Disease"},
                {"transplant", "Dialysis"},
                {"inherited disorder", "Polycystic Kidney Disease"},
                {"cysts", "Polycystic Kidney Disease"},
                {"enlarged kidneys", "Polycystic Kidney Disease"},
                {"inflammation", "Nephritis"},
                {"filter blood", "Nephritis"},
                {"waste", "Nephritis"},
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
            return "nephrology";
        }
    }
}