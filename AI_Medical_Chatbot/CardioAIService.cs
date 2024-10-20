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
            // Cluster the input and return cardio-related information
            string cluster = ClusterTopic(message);
            string topic = "cardiovascular";

            // if (cluster.Contains(" "))
            // {
            //     cluster = cluster.Replace(" ", "-");
            // }

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
                { "heart valve disease", "heart valve diseases" },  // More specific
                { "heart surgery", "heart surgery" },
                { "heart disease", "heart disease" },               // More specific
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
    }
}