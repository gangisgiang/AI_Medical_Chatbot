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
		// This method is used to generate a response to the user's message
		public abstract Task<string> GenerateResponse(string message);

		// Fetch data from the API
		// protected async Task<string> FetchData(string topic)
		// {
		// 	string url = "https://wsearch.nlm.nih.gov/ws/query?db=healthTopics&term=" + topic;

		// 	using (HttpClient client = new HttpClient())
		// 	{
		// 		try
		// 		{
		// 			HttpResponseMessage response = await client.GetAsync(url);
		// 			response.EnsureSuccessStatusCode();
		// 			return await response.Content.ReadAsStringAsync();
		// 		}
		// 		catch (HttpRequestException e)
		// 		{
		// 			return "\nRequest failed: " + e.Message;
		// 		}
		// 	}
		// }

		public async Task<string> FetchData(string topic)
		{
			try
			{
				using HttpClient client = new HttpClient();
				HttpResponseMessage response = await client.GetAsync("https://wsearch.nlm.nih.gov/ws/query?db=healthTopics&term=" + topic);

				if (response.IsSuccessStatusCode)
				{
					string contentType = response.Content.Headers.ContentType.MediaType;
					string responseData = await response.Content.ReadAsStringAsync();

					// Convert HTML to plain text
					return ConvertHtmlToPlainText(responseData);
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
			// Remove all script and style elements
			html = Regex.Replace(html, "<(script|style)[^>]*?>.*?</\\1>", "", RegexOptions.Singleline);
			
			// Replace breaks and paragraphs with line breaks
			html = Regex.Replace(html, "<(br|BR|p|P)>", Environment.NewLine);

			// Remove all other HTML tags
			html = Regex.Replace(html, "<.*?>", String.Empty);

			// Decode HTML entities like &nbsp; or &amp;
			html = System.Net.WebUtility.HtmlDecode(html);

			// Remove multiple blank lines
			html = Regex.Replace(html, @"^\s*$\n", "", RegexOptions.Multiline);

			return html.Trim();
		}
	}
}

