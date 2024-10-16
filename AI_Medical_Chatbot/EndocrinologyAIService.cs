using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public class EndocrinologyAIService : AIService, IAIResponse
    {
        public async Task<string> GenerateResponse(string message)
        {
            // Cluster the input and return cardio-related information
            string cluster = ClusterTopic(message);
            string topic = "endocrinology";

            Console.WriteLine("Cluster: " + cluster);

            if (string.IsNullOrEmpty(cluster))
            {
                return await FetchandConvert("endocrinology");
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
                {"chronic", "diabetes"}, {"insulin", "diabetes"}, {"glucose", "diabetes"}, {"pancreas", "diabetes"}, 
                {"adrenal", "endocrinology"}, {"hormonal", "hormonal-imbalances"}, {"imbalances", "hormonal-imbalances"}, 
                {"reproductive", "hormonal-imbalances"}, {"abnormal", "hormonal-imbalances"}, {"hormone", "hormonal-imbalances"}, 
                {"endocrine", "hormonal-imbalances"}, {"glands", "hormonal-imbalances"}, {"pituitary", "pituitary-disorders"}, 
                {"gland", "pituitary-disorders"}, {"overproduction", "pituitary-disorders"},
                {"sugar level", "diabetes"}, {"bone", "osteoporosis"}, {"fragility", "osteoporosis"}, 
                {"density", "osteoporosis"}, {"autoimmune", "thyroid-disorders"}, {"thyroid", "thyroid-disorders"},
                {"metabolism", "diabetes"}, {"fractures", "osteoporosis"}, {"endocrine", "endocrinology"},
                {"gland", "endocrinology"}, {"pituitary", "pituitary-disorders"}, {"hormone", "pituitary-disorders"}
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
            return "endocrinology";
        }
    }
}