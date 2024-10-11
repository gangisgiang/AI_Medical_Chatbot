using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public class FetchData
    {
        private const string apiURL = "https://wsearch.nlm.nih.gov/ws/query?db=healthTopics&term=";

        public async Task<string> FetchHealthDataTopic(string topic)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Create the request
                    string requestURL = apiURL + Uri.EscapeDataString(topic);
                    HttpResponseMessage response = await client.GetAsync(requestURL);
                    response.EnsureSuccessStatusCode();

                    // Read the response
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nRequest failed: " + e.Message);
                    return null;
                }
            }
        }
    }
}