using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public class OphthalmologyAIService : AIService, IAIResponse
    {
        public async Task<string> GenerateResponse(string message)
        {
            // Cluster the input and return cardio-related information
            string cluster = ClusterTopic(message);
            string topic = "ophthalmology";

            if (string.IsNullOrEmpty(cluster))
            {
                return await FetchandConvert("ophthalmology");
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
                // Ophthalmology-specific keywords
                { "retinal blood vessels", "diabetic retinopathy" }, { "eye blood vessel damage", "diabetic retinopathy" }, 
                { "central vision loss", "macular degeneration" }, { "retina damage", "macular degeneration" }, 
                { "lens clouding", "cataracts" }, { "cloudy lens", "cataracts" },
                { "high eye pressure", "glaucoma" }, { "optic nerve damage", "glaucoma" }, 
                { "dry, irritated eyes", "dry eye syndrome" }, { "tear film dysfunction", "dry eye syndrome" }, 
                { "ocular surface", "dry eye syndrome" }
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
            return "ophthalmology";
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