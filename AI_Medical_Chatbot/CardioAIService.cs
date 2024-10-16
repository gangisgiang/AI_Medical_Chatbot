using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace AI_Medical_Chatbot
{
    public class CardioAIService : AIService, IAIResponse
    {
        public async Task<string> GenerateResponse(string message)
        {
            // Cluster the input and return cardio-related information
            string cluster = ClusterCardioTopic(message);
            string topic = "cardiovascular";

            // if (cluster.Contains(" "))
            // {
            //     cluster = cluster.Replace(" ", "-");
            // }

            Console.WriteLine("Cluster: " + cluster);

            if (string.IsNullOrEmpty(cluster))
            {
                return await FetchandConvert("cardiovascular");
            }

            string plainTextData = await FetchandConvert(topic);

            string relevantInfo = ExtractRelevantInfo(plainTextData, cluster);

            if (string.IsNullOrEmpty(relevantInfo))
            {
                return "No relevant information found.";
            }

            return relevantInfo;
        }

        private string ClusterCardioTopic(string input)
        {
            // Convert input to lowercase to ensure case-insensitive matching
            input = input.ToLower();

            // Define a manual mapping for keywords to topics based on the file content
            var manualMapping = new Dictionary<string, string>
            {
                { "heart valve disease", "heart valve diseases" },  // More specific
                { "heart surgery", "heart surgery" },
                { "heart disease", "heart diseases" },               // More specific
                { "heart attack", "heart diseases" },               // More specific
                { "myocardial infarction", "heart diseases" },      // More specific
                { "congenital heart disease", "ongenital heart disease" },     // More specific
                { "coronary artery disease", "coronary artery disease" }, // More specific
                { "vascular disease", "vascular diseases" },
                { "valve", "heart valve diseases" },
                { "arrhythmia", "arrhythmia" },
                { "stroke", "stroke" },
                { "prehypertension", "prehypertension" },
                { "hypertension", "hypertension" },
                { "blood pressure", "blood pressure" },
                { "cholesterol", "cholesterol" },
                { "atherosclerosis", "atherosclerosis" },
                { "heart", "heart health" },                        // General term, should match last
                { "cardiovascular system", "cardiovascular system" },
                { "cardio", "cardiovascular system" }
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
            return "cardiovascular";
        }

        private string ExtractRelevantInfo(string plainTextData, string topic)
        {
            // Split the data into paragraphs, ensuring no empty entries
            string[] paragraphs = plainTextData.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // Ensure that we are using the hyphenated version of the topic to match the section ID
            var relevantParagraphs = paragraphs.Where(p => p.Contains($"id=\"{topic}\"", StringComparison.OrdinalIgnoreCase)
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
