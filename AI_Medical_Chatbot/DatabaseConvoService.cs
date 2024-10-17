using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AI_Medical_Chatbot
{
    public class DatabaseConvoService
    {
        private const string ConvoPath = "conversations.json";

        public void SaveConversations(List<Conversation> conversations)
        {
            string json = JsonSerializer.Serialize(conversations);
            File.WriteAllText(ConvoPath, json);
        }

        public List<Conversation> LoadConversations()
        {
            if (File.Exists(ConvoPath))
            {
                string json = File.ReadAllText(ConvoPath);
                return JsonSerializer.Deserialize<List<Conversation>>(json) ?? new List<Conversation>();
            }
            return new List<Conversation>();
        }
    }
}
