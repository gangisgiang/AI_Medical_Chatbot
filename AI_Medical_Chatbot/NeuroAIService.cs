using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public class NeuroAIService : AIService, IAIResponse
    {
        public async Task<string> GenerateResponse(string message)
        {
            // Cluster the input and return cardio-related information
            string cluster = ClusterTopic(message);
            string topic = "neurology";

            if (string.IsNullOrEmpty(cluster))
            {
                return await FetchandConvert("neurology");
            }


            string cluste = ClusterTopic(message);
            Console.WriteLine("Cluster: " + cluste);

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

            // Define a manual mapping for keywords to topics with longer and more specific keywords first
            var manualMapping = new Dictionary<string, string>
            {
                { "sensitivity to light", "migraine" },
                { "memory loss", "alzheimers disease" },
                { "movement disorders", "parkinsons" },
                { "spinal cord injury", "spinal cord injury" },
                { "autoimmune", "multiple sclerosis" },
                { "dopamine", "parkinsons disease" },
                { "seizures", "epilepsy" },
                { "headaches", "neurology" },
                { "vision problems", "brain tumor" },
                { "brain tumor", "brain tumor" },
                { "alzheimer", "alzheimer" },
                { "parkinson", "parkinson" },
                { "ALS", "neurodegenerative diseases" },
                { "Huntington", "neurodegenerative diseases" },
                { "Multiple", "multiple sclerosis" },
                { "Sclerosis", "multiple sclerosis"},
                { "epilepsy", "epilepsy" },
                { "neurology", "neurology" },
                { "migraine", "migraine" },
                { "tumor", "brain tumor" },
                { "cancer", "brain tumor" },
                { "paralysis", "spinal cord injury" },
                { "MS", "multiple sclerosis" },
                { "dementia", "dementia" },
                { "stroke", "stroke" }
            };
            // make the word in the dictonary to be in lower case
            manualMapping = manualMapping.ToDictionary(kvp => kvp.Key.ToLower(), kvp => kvp.Value);

            // Check if the input matches any keywords
            foreach (var keyword in manualMapping.Keys.OrderByDescending(k => k.Length))
            {
                if (input.Contains(keyword))
                {
                    return manualMapping[keyword];
                }
            }

            // If no match is found, return an empty string or a default topic
            return "neurology";
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