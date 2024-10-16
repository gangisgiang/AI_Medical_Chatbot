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

            Console.WriteLine("Cluster: " + cluster);

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
    }
}