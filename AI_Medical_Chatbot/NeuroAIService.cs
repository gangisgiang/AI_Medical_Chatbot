using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace AI_Medical_Chatbot
{
    public class NeuroAIService : AIService, IAIResponse
    {
        public async Task<string> GenerateResponse(string message)
        {
            // Cluster the input and return cardio-related information
            string cluster = ClusterNeuroTopic(message);
            string topic = "neurology";

            // if (cluster.Contains(" "))
            // {
            //     cluster = cluster.Replace(" ", "-");
            // }

            Console.WriteLine("Cluster: " + cluster);

            if (string.IsNullOrEmpty(cluster))
            {
                return await FetchandConvert("neurology");
            }

            string plainTextData = await FetchandConvert(topic);

            string relevantInfo = ExtractRelevantInfo(plainTextData, cluster);

            if (string.IsNullOrEmpty(relevantInfo))
            {
                return "No relevant information found.";
            }

            return relevantInfo;
        }

        private string ClusterNeuroTopic(string input)
        {
            // Convert input to lowercase to ensure case-insensitive matching
            input = input.ToLower();

            // Define a manual mapping for keywords to topics with longer and more specific keywords first
            var manualMapping = new Dictionary<string, string>
            {
                { "sensitivity to light", "migraine" },
                { "memory loss", "alzheimers disease" },
                { "movement disorders", "parkinsons disease" },
                { "spinal cord injury", "spinal cord injury" },
                { "autoimmune", "multiple sclerosis" },
                { "dopamine", "parkinsons disease" },
                { "seizures", "epilepsy" },
                { "headaches", "neurology" },
                { "vision problems", "brain tumor" },
                { "brain tumor", "brain tumor" },
                { "Alzheimer", "alzheimers disease" },
                { "Parkinson", "parkinsons disease" },
                { "ALS", "neurodegenerative diseases" },
                { "Huntington", "neurodegenerative diseases" },
                { "Multiple Sclerosis", "multiple sclerosis" },
                { "epilepsy", "epilepsy" },
                { "neurology", "neurology" },
                { "migraines", "migraine" },
                { "migraine", "migraine" },
                { "tumor", "brain tumor" },
                { "cancer", "brain tumor" },
                { "paralysis", "spinal cord injury" },
                { "MS", "multiple sclerosis" },
                { "dementia", "alzheimers disease" },
                { "stroke", "neurology" }
            };


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

        private string ExtractRelevantInfo(string plainTextData, string topic)
        {
            // Split the data into paragraphs, ensuring no empty entries
            string[] paragraphs = plainTextData.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // Ensure that we are using the hyphenated version of the topic to match the section ID
            var relevantParagraphs = paragraphs.Where(p => p.Contains(topic, StringComparison.OrdinalIgnoreCase)
                || p.Contains(topic, StringComparison.OrdinalIgnoreCase)).ToArray();

            if (relevantParagraphs.Length == 0)
            {
                return "No relevant information.";
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
