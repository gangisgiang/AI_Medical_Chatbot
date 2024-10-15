using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public class ApiAdapter
    {
        // Method to fetch data from an external API
        public async Task<string> FetchData(string topic)
        {
            try
            {
                // using HttpClient client = new HttpClient();
                // HttpResponseMessage response = await client.GetAsync("https://wsearch.nlm.nih.gov/ws/query?db=healthTopics&term=" + topic);

                // if (response.IsSuccessStatusCode)
                // {
                //     string responseData = await response.Content.ReadAsStringAsync();
                //     return ConvertHtmlToPlainText(responseData);
                // }

                // Build the file path from the topic name (similar to the URL structure)
                string filePath = $"/Users/guest1/Downloads/healthTopics&term={topic}.html";

                if (File.Exists(filePath))
                {
                    string htmlContent = await File.ReadAllTextAsync(filePath);
                    return ConvertHtmlToPlainText(htmlContent);
                }
                else
                {
                    return "Error: Failed to fetch data from the server.";
                }
            }
            catch (Exception ex)
            {
                return "\nRequest failed: " + ex.Message;
            }
        }

        // Method to strip HTML tags and return plain text
        public string ConvertHtmlToPlainText(string html)
        {
            // Remove script and style elements
            html = Regex.Replace(html, "<(script|style)[^>]*?>.*?</\\1>", "", RegexOptions.Singleline);

            // Replace breaks and paragraphs with line breaks
            html = Regex.Replace(html, "<(br|BR|p|P)>", Environment.NewLine);

            // Remove other HTML tags
            html = Regex.Replace(html, "<.*?>", String.Empty);

            // Decode HTML entities like &nbsp; or &amp;
            html = System.Net.WebUtility.HtmlDecode(html);

            // Remove multiple blank lines
            html = Regex.Replace(html, @"^\s*$\n", "", RegexOptions.Multiline);

            return html.Trim();
        }
    }
}
