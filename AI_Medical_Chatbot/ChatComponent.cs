using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
	public class ChatComponent
	{
		private int convoCount = 0;
		public List<Conversation> ConversationList { get; set; } = new List<Conversation>();
		private readonly DatabaseService databaseService = new DatabaseService();
		private const string convoCountFile = "convoCount.txt";

		public ChatComponent(DatabaseService dbService)
		{
			databaseService = dbService;
			// Load conversations from the database
			LoadConversations();
			LoadConvoCount();
		}

		public void AskChatbot(User user, string text)
		{
			// Send message to the chatbot
			Message message = new Message(text, user.UserID, 0);
			user.MessagesHistory.Add(message);

			// Add message to the latest conversation
			ConversationList[convoCount - 1].AddMessage(message);
		}

		public void ChatbotAnswer(User user, string text)
		{
			// Receive message from the chatbot
			Message message = new Message(text, 0, user.UserID);
			user.MessagesHistory.Add(message);
			
			ConversationList[convoCount - 1].AddMessage(message);
		}

		public void CreateNewConvo()
		{
			if (convoCount == 0)
			{
				// Create the initial conversation
				Conversation initialConvo = new Conversation(convoCount, "Conversation");
				convoCount++;
				ConversationList.Add(initialConvo);
				SaveConvoCount();
			}
			else
			{
				// Create a new conversation
				Conversation convo = new Conversation(convoCount, "Conversation " + "(" + convoCount + ")");
				convoCount++;
				ConversationList.Add(convo);
				Console.WriteLine(convo.ConvoName + " created.");
				SaveConvoCount();
			}
		}

		public void SaveConversations()
		{
			string json = JsonSerializer.Serialize(ConversationList);
			File.WriteAllText("conversations.json", json);
		}

		public void LoadConversations()
		{
			if (File.Exists("conversations.json"))
			{
				string json = File.ReadAllText("conversations.json");
				ConversationList = JsonSerializer.Deserialize<List<Conversation>>(json) ?? new List<Conversation>();
				convoCount = ConversationList.Count;
			}
		}

		public void SaveConvoCount()
		{
			File.WriteAllText(convoCountFile, convoCount.ToString());
		}

		public void LoadConvoCount()
		{
			if (File.Exists(convoCountFile))
			{
				string count = File.ReadAllText(convoCountFile);
				convoCount = int.Parse(count);
			}
		}

		public async Task FetchData(string apiURL, string json)
		{
			using (HttpClient client = new HttpClient())
			{
				try
				{
					StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
					HttpResponseMessage response = await client.PostAsync(apiURL, content);
					response.EnsureSuccessStatusCode();
					string responseBody = await response.Content.ReadAsStringAsync();
					Console.WriteLine(responseBody);
				}
				catch (HttpRequestException e)
				{
					Console.WriteLine("\nException Caught!");
					Console.WriteLine("Message :{0} ", e.Message);
				}
			}
		}

		public void RenameConvo(int convoIndex, string newName)
		{
			if (convoCount == 0)
			{
				Console.WriteLine("No conversation to rename.");
				return;
			}

			if (convoIndex < 0 || convoIndex >= convoCount)
			{
				Console.WriteLine("Invalid conversation index.");
				return;
			}

			// Rename the conversation
			ConversationList[convoCount - 1].ConvoName = newName;
			Console.WriteLine("Conversation renamed to: " + newName);
		}

		public void DisplayConvo()
		{
			// Display the messages in the chosen conversation
			foreach (Conversation convo in ConversationList)
			{
				Console.WriteLine("--- " + convo.ConvoName + " ---");
				convo.DisplayMessages();
			}	
		}

		public void DeleteConvo(int convoIndex)
		{
			if (convoCount == 0)
			{
				Console.WriteLine("No conversation to delete.");
				return;
			}

			if (convoIndex < 0 || convoIndex >= convoCount)
			{
				Console.WriteLine("Invalid conversation index.");
				return;
			}

			// Delete the conversation
			Console.WriteLine(ConversationList[convoIndex].ConvoName + " deleted.");
			ConversationList.RemoveAt(convoIndex);
		}
	}
}