using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public class RheumatologyAIService : AIService, IAIResponse
    {
        public async Task<string> GenerateResponse(string message)
        {
            // Cluster the input and return cardio-related information
            string cluster = ClusterTopic(message);
            string topic = "rheumatology";

            if (string.IsNullOrEmpty(cluster))
            {
                return await FetchandConvert("rheumatology");
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
                {"rheumatology", "rheumatology"}, {"inflammation", "rheumatology"}, {"joints", "rheumatology"}, {"muscles", "rheumatology"}, 
                {"rheumatoid arthritis", "rheumatoid-arthritis"}, {"autoimmune", "rheumatoid-arthritis"}, {"pain", "rheumatoid-arthritis"}, 
                {"lupus", "lupus"}, {"systemic", "lupus"}, {"gout", "gout"}, {"arthritis", "gout"}, {"ankylosing spondylitis", "ankylosing-spondylitis"},
                {"spine", "ankylosing-spondylitis"}, {"psoriatic arthritis", "psoriatic-arthritis"}, {"psoriasis", "psoriatic-arthritis"}, 
                {"arthritis", "rheumatology"}, {"immune system", "lupus"}, {"red patches", "psoriatic-arthritis"}, {"inflammation of the spine", "ankylosing-spondylitis"}
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
            return "rheumatology";
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