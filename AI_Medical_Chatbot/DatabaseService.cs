using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AI_Medical_Chatbot
{
	public class DatabaseService
	{
		private readonly string ConvoPath = "conversations.json";

		// Save the conversations to the database
		public void SaveConversations(List<Conversation> conversations)
		{
			string json = JsonSerializer.Serialize(conversations);
			File.WriteAllText(ConvoPath, json);
		}

		// Load the conversations from the database
		public List<Conversation> LoadConversations()
		{
			if (File.Exists(ConvoPath))
			{
				string json = File.ReadAllText(ConvoPath);
				return JsonSerializer.Deserialize<List<Conversation>>(json);
			}
			return new List<Conversation>();
		}
	}
}

