using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace AI_Medical_Chatbot
{
	public abstract class AIService
	{
		// This method is used to generate a response to the user's message
		public abstract Task<string> GenerateResponse(string message);

		// Fetch data from the API
		protected async Task<string> FetchData(string topic)
		{
			string apiURL = $"https://wsearch.nlm.nih.gov/ws/query?db=healthTopics&term={topic}";

			using (HttpClient client = new HttpClient())
			{
				try
				{
					HttpResponseMessage responseMessage = await client.GetAsync(apiURL);
					responseMessage.EnsureSuccessStatusCode();
					return await responseMessage.Content.ReadAsStringAsync();
				}
				catch (HttpRequestException e)
				{
					return "\nRequest failed: " + e.Message;
				}
			}
		}
	}
}

