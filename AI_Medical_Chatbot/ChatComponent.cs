﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

namespace AI_Medical_Chatbot
{
    public class ChatComponent
    {
        private static ChatComponent? _instance;
        private readonly TopicRecogniser topicRecogniser = new TopicRecogniser();
        public List<Conversation> ConversationList { get; set; } = new List<Conversation>();
        private readonly DatabaseConvoService dbService;

        private ChatComponent(DatabaseConvoService dbService)
        {
            this.dbService = dbService;
            LoadConversations();
        }

        public static ChatComponent GetInstance(DatabaseConvoService dbService)
        {
            if (_instance == null)
            {
                _instance = new ChatComponent(dbService);
            }
            return _instance;
        }

        public void ShowMainMenu(User user)
        {
            Console.WriteLine("Welcome to the AI Medical Chatbot System");
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Start a new conversation");
                Console.WriteLine("2. Show conversation history");
                Console.WriteLine("3. Rename a conversation");
                Console.WriteLine("4. Delete a conversation");
                Console.WriteLine("5. Exit");
                Console.Write("Please select an option: ");
                
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        CreateNewConvo();
                        break;
                    case "2":
                        DisplayConvo();
                        break;
                    case "3":
                        RenameConvoInterface();
                        break;
                    case "4":
                        DeleteConvoInterface();
                        break;
                    case "5":
                        exit = true;
                        Console.WriteLine("Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        public void AskChatbot(User user, string text)
        {
            Message message = new Message(text, user.UserID, 0);
            user.MessagesHistory.Add(message);

            // Add message to the latest conversation
            if (ConversationList.Count > 0)
            {
                ConversationList[ConversationList.Count - 1].AddMessage(message);
            }

            // Process chatbot response asynchronously
            Task.Run(async () => await ProcessResponse(user, text));
        }

        private async Task ProcessResponse(User user, string text)
        {
            string response = await topicRecogniser.GenerateResponse(text);
            ChatbotAnswer(user, response);
        }

        private void ChatbotAnswer(User user, string text)
        {
            Message message = new Message(text, 0, user.UserID);
            user.MessagesHistory.Add(message);

            if (ConversationList.Count > 0)
            {
                ConversationList[ConversationList.Count - 1].AddMessage(message);
            }
        }

        public void CreateNewConvo()
        {
            string convoName = $"Conversation ({Guid.NewGuid()})"; // Unique ID
            Conversation convo = new Conversation(Guid.NewGuid(), convoName);
            ConversationList.Add(convo);
            Console.WriteLine($"{convoName} created.");
        }

        public void RenameConvoInterface()
        {
            if (ConversationList.Count == 0)
            {
                Console.WriteLine("No conversations to rename.");
                return;
            }

            Console.WriteLine("Available Conversations:");
            foreach (var convo in ConversationList)
            {
                Console.WriteLine($"- {convo.ConvoName} (ID: {convo.ConvoID})");
            }

            Console.Write("Enter the ID of the conversation you want to rename: ");
            string convoIdInput = Console.ReadLine();
            if (Guid.TryParse(convoIdInput, out Guid convoId))
            {
                Console.Write("Enter the new name for the conversation: ");
                string newName = Console.ReadLine();
                RenameConvo(convoId, newName);
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        public void RenameConvo(Guid convoId, string newName)
        {
            // Find the conversation by ConvoID
            var conversation = ConversationList.FirstOrDefault(c => c.ConvoID == convoId);

            if (conversation == null)
            {
                Console.WriteLine($"Conversation with ID '{convoId}' not found.");
                return;
            }

            // Rename the conversation
            conversation.ConvoName = newName;
            Console.WriteLine($"Conversation renamed to: {newName}");
        }

        public void DeleteConvoInterface()
        {
            if (ConversationList.Count == 0)
            {
                Console.WriteLine("No conversations to delete.");
                return;
            }

            Console.WriteLine("Available Conversations:");
            foreach (var convo in ConversationList)
            {
                Console.WriteLine($"- {convo.ConvoName} (ID: {convo.ConvoID})");
            }

            Console.Write("Enter the ID of the conversation you want to delete: ");
            string convoIdInput = Console.ReadLine();
            if (Guid.TryParse(convoIdInput, out Guid convoId))
            {
                DeleteConvo(convoId);
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        public void DeleteConvo(Guid convoId)
        {
            // Find the conversation by ConvoID
            var conversation = ConversationList.FirstOrDefault(c => c.ConvoID == convoId);

            if (conversation == null)
            {
                Console.WriteLine($"Conversation with ID '{convoId}' not found.");
                return;
            }

            // Delete the conversation
            Console.WriteLine($"{conversation.ConvoName} deleted.");
            ConversationList.Remove(conversation);
        }

        public void DisplayConvo()
        {
            if (ConversationList.Count == 0)
            {
                Console.WriteLine("No conversations available.");
                return;
            }

            foreach (var convo in ConversationList)
            {
                Console.WriteLine($"--- {convo.ConvoName} (ID: {convo.ConvoID}) ---");
                convo.DisplayMessages();
            }
        }

        public void SaveConversations()
        {
            dbService.SaveConversations(ConversationList);
        }

        private void LoadConversations()
        {
            ConversationList = dbService.LoadConversations();
        }
    }
}
