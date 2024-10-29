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

            manualMapping = manualMapping.ToDictionary(kvp => kvp.Key.ToLower(), kvp => kvp.Value);

            foreach (var keyword in manualMapping.Keys.OrderByDescending(k => k.Length))
            {
                if (input.Contains(keyword))
                {
                    return manualMapping[keyword];
                }
            }

            return "immunology";
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