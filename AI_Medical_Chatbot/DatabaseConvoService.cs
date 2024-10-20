using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AI_Medical_Chatbot
{
    public class DatabaseConvoService
    {
        private const string ConvoPath = "conversations.json";

        public void SaveConversations(Dictionary<string, List<Conversation>> conversations)
        {
            try
            {
                string json = JsonSerializer.Serialize(conversations, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConvoPath, json);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error saving conversations: " + e.Message);
            }
        }

        public Dictionary<string, List<Conversation>> LoadConversations()
        {
            try
            {
                if (File.Exists(ConvoPath))
                {
                    string json = File.ReadAllText(ConvoPath);
                    var conversations = JsonSerializer.Deserialize<Dictionary<string, List<Conversation>>>(json) ?? new Dictionary<string, List<Conversation>>();
                    return conversations;
                }
                else
                {
                    Console.WriteLine("No conversation history found.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading conversations: " + e.Message);
            }

            return new Dictionary<string, List<Conversation>>();
        }
    } 
}
