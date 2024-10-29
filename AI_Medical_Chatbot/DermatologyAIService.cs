using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public class DermatologyAIService : AIService, IAIResponse
    {
        public async Task<string> GenerateResponse(string message)
        {
            // Cluster the input and return cardio-related information
            string cluster = ClusterTopic(message);
            string topic = "dermatology";

            // if (cluster.Contains(" "))
            // {
            //     cluster = cluster.Replace(" ", "-");
            // }

            if (string.IsNullOrEmpty(cluster))
            {
                return await FetchandConvert("dermatology");
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
                { "clogged pores", "acne" },
                { "blackheads", "acne" },
                { "pimples", "acne" },
                { "itchy inflamed skin", "eczema" },
                { "dry inflamed", "eczema" },
                { "chronic itchy", "eczema" },
                { "thick scaly patches", "psoriasis" },
                { "autoimmune skin disorder", "psoriasis" },
                { "malignant melanoma", "melanoma" },
                { "skin cancer", "melanoma" },
                { "abnormal mole growth", "melanoma" },
                { "red visible blood vessels", "rosacea" },
                { "persistent facial redness", "rosacea" },
                { "pus-filled bumps on face", "rosacea" },
                { "autoimmune hair loss", "hair-loss" },
                { "alopecia areata", "hair-loss" },
                { "chronic hair thinning", "hair-loss" },
                { "severe skin allergies", "skin-allergies" },
                { "contact dermatitis from metal", "skin-allergies" },
                { "swollen", "skin-allergies" },
                { "fragrances", "skin-allergies" },
                { "rash", "skin-allergies" },
                { "ivy", "skin-allergies"},
                { "atopic dermatitis", "eczema" },
                { "chronic scaly patches", "psoriasis" },
                { "melanin", "melanoma" },
                { "skin disorders", "skin disorders" },
                { "nail disorders", "nail disorders" },
                { "skin disorders", "skin disorders" },
                { "hair disorders", "hair" },
                { "scalp disorders", "hair-loss" },
                { "psoriasis", "psoriasis" },
                { "eczema", "eczema" },
                { "acne", "acne" },
                { "melanoma", "melanoma" },
                { "rosacea", "rosacea" },
                { "hair loss", "hair-loss" },
                { "skin allergies", "skin-allergies" },
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
            return "dermatology";
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