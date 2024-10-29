using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public class CardioAIService : AIService, IAIResponse
    {
        public async Task<string> GenerateResponse(string message)
        {
            string cluster = ClusterTopic(message);
            string topic = "cardiovascular";

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

        public override string ClusterTopic(string input)
        {
            // Convert input to lowercase to ensure case-insensitive matching
            input = input.ToLower();

            // Define a manual mapping for keywords to topics based on the file content
            var manualMapping = new Dictionary<string, string>
            {
                { "heart valve disease", "heart valve diseases" },
                { "heart surgery", "heart surgery" },
                { "heart disease", "heart disease" },             
                { "heart attack", "heart diseases" },            
                { "myocardial infarction", "heart diseases" },     
                { "congenital heart disease", "ongenital heart disease" },  
                { "coronary artery disease", "coronary artery disease" }, 
                { "vascular disease", "vascular diseases" },
                { "valve", "heart valve diseases" },
                { "arrhythmia", "arrhythmia" },
                { "stroke", "stroke" },
                { "prehypertension", "prehypertension" },
                { "hypertension", "hypertension" },
                { "blood pressure", "blood pressure" },
                { "cholesterol", "cholesterol" },
                { "atherosclerosis", "atherosclerosis" },
                { "heart", "heart health" },              
                { "cardiovascular system", "cardiovascular system" },
                { "cardio", "cardiovascular system" }
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

            // If no match is found, return an empty string or a default topic
            return "cardiovascular";
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