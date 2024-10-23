using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public class ImmunologyAIService : AIService, IAIResponse
    {
        public async Task<string> GenerateResponse(string message)
        {
            string cluster = ClusterTopic(message);
            string topic = "immunology";

            if (string.IsNullOrEmpty(cluster))
            {
                return await FetchandConvert("immunology");
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
            input = input.ToLower();

            var manualMapping = new Dictionary<string, string>
            {
                { "immune system", "immunology" }, 
                { "immunity", "immunology" }, 
                { "autoimmune diseases", "immunology" }, 
                { "rheumatoid arthritis", "immunology" }, 
                { "lupus", "immunology" }, 
                { "multiple sclerosis", "immunology" }, 
                { "allergies", "immunology" }, 
                { "pollen allergies", "immunology" }, 
                { "food allergies", "immunology" }, 
                { "immunodeficiency", "immunology" }, 
                { "vaccination", "immunology" }, 
                { "immune response", "immunology" }, 
                { "hypersensitivity", "immunology" }, 
                { "anaphylaxis", "immunology" },
                { "immunotherapy", "immunology" }, 
                { "antibody", "immunology" },
                { "immunosuppressive", "immunology" }, 
                { "T-cells", "immunology" }, 
                { "B-cells", "immunology" }
            };

            foreach (var keyword in manualMapping.Keys.OrderByDescending(k => k.Length))
            {
                if (input.Contains(keyword))
                {
                    return manualMapping[keyword];
                }
            }

            return "immunology";
        }
    }
}