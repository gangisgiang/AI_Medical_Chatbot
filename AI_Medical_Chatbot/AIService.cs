using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace AI_Medical_Chatbot
{
    public abstract class AIService
    {
        protected ApiAdapter _apiAdapter;
        
        public AIService()
        {
            _apiAdapter = new ApiAdapter();
        }

        // Abstract method to be implemented by each AIService subclass
        protected async Task<string> FetchandConvert(string topic)
        {
            // Use the dynamically passed topic (either cardiovascular or respiratory)
            string rawHtmlData = await _apiAdapter.FetchData(topic);
            return _apiAdapter.ConvertHtmlToPlainText(rawHtmlData);
        }

        public abstract string ClusterTopic(string message);
        
        public string ExtractRelevantInfo(string plainTextData, string topic)
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
            string? firstParagraph = relevantParagraphs.FirstOrDefault(p => p.Split('.').Length > 1)?.Trim();

            if (string.IsNullOrEmpty(firstParagraph))
            {
                return "No relevant information found.";
            }

            return firstParagraph;
        }
    }
}