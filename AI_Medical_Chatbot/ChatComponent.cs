using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Linq;

namespace AI_Medical_Chatbot
{
    public class ChatComponent
    {
        private static ChatComponent? _instance;
        private readonly TopicRecogniser topicRecogniser = new TopicRecogniser();
        public Dictionary<string, List<Conversation>> ConversationList;
        private User currentUser;
        private readonly DatabaseConvoService dbService;
        private Conversation currentConvo;        
        private bool isGuestUser = false;

        private ChatComponent(DatabaseConvoService dbService)
        {
            this.dbService = dbService;
            ConversationList = dbService.LoadConversations(); // Load all conversations for all users
        }

        public static ChatComponent GetInstance(DatabaseConvoService dbService)
        {
            if (_instance == null)
            {
                _instance = new ChatComponent(dbService);
            }
            return _instance;
        }

        public void ShowMainMenu()
        {
            Console.WriteLine("Welcome to the AI Medical Chatbot System");
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Start a new conversation");
                Console.WriteLine("2. View conversation history");
                Console.WriteLine("3. Continue a previous conversation");
                Console.WriteLine("4. Rename a conversation");
                Console.WriteLine("5. Delete a conversation");
                Console.WriteLine("6. Exit");
                Console.Write("Please select an option: ");
                
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        CreateNewConvo();
                        break;
                    case "2":
                        DisplayConvoList();
                        break;
                    case "3":
                        ContinueConversationInterface();
                        break;
                    case "4":
                        RenameConvoInterface();
                        break;
                    case "5":
                        DeleteConvoInterface();
                        break;
                    case "6":
                        exit = true;
                        Console.WriteLine("Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        public void SetCurrentUser(User user)
        {
            currentUser = user;
            LoadConversationsForUser();
        }

        public void AskChatbot(string text)
        {
            if (isGuestUser)
            {
                Task.Run(async () => await ProcessResponse(text));
            }
            else
            {
                // Standard user flow with saving conversation
                if (currentUser == null || currentConvo == null)
                {
                    return;
                }

                Message message = new Message(text, currentUser.UserID, 0);
                currentUser.MessagesHistory.Add(message);
                currentConvo.AddMessage(message); // Add message to the active conversation
                SaveConversationsForUser(); // Save after adding the message

                // Process chatbot response asynchronously
                Task.Run(async () => await ProcessResponse(text));
            }
        }

        private async Task ProcessResponse(string text)
        {
            if (isGuestUser)
            {
                // Generate a response without saving to conversation history
                string guestResponse = await topicRecogniser.GenerateResponse(text);
                Console.WriteLine($"Chatbot: {guestResponse}");
                Console.Write("Ask your question (or type 'exit' to end): ");
                return;
            }

            if (currentUser == null || currentConvo == null)
            {
                return;
            }

            string response = await topicRecogniser.GenerateResponse(text);
            ChatbotAnswer(response);
        }

        public void DisplayCurrentConvo()
        {
            if (currentConvo != null)
            {
                Console.WriteLine($"Conversation: {currentConvo.ConvoName}");
                currentConvo.DisplayMessages();
            }
        }

        private void ChatbotAnswer(string text)
        {
            if (currentUser == null || currentConvo == null)
            {
                return;
            }

            Message message = new Message(text, 0, currentUser.UserID);
            currentUser.MessagesHistory.Add(message);
            currentConvo.AddMessage(message);
            SaveConversationsForUser();

            // Display the chatbot's response message only
            Console.WriteLine($"Chatbot: {text}");
            Console.WriteLine("Ask your question (or type 'exit' to end): ");
        }

        public void CreateNewConvo()
        {
            if (currentUser == null)
            {
                return;
            }

            string convoName = $"Conversation ({Guid.NewGuid()})"; // Unique ID
            Conversation convo = new Conversation(Guid.NewGuid(), convoName);

            if (!ConversationList.ContainsKey(currentUser.Username))
            {
                ConversationList[currentUser.Username] = new List<Conversation>();
            }

            ConversationList[currentUser.Username].Add(convo);
            currentConvo = convo;
            SaveConversationsForUser(); // Save after creating the new conversation
            Console.WriteLine($"{convoName} created.");
        }

        public void ContinueConversationInterface()
        {
            if (currentUser == null || !ConversationList.TryGetValue(currentUser.Username, out List<Conversation>? value) || value.Count == 0)
            {
                Console.WriteLine("No conversations to continue.");
                return;
            }

            Console.WriteLine("Available Conversations:");
            foreach (var convo in value)
            {
                Console.WriteLine($"- {convo.ConvoName} (ID: {convo.ConvoID})");
            }

            Console.Write("Enter the ID of the conversation you want to continue: ");
            string convoIdInput = Console.ReadLine();
            if (Guid.TryParse(convoIdInput, out Guid convoId))
            {
                ContinueConversation(convoId);
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        public bool ContinueConversation(Guid convoId)
        {
            if (currentUser == null || !ConversationList.ContainsKey(currentUser.Username))
            {
                Console.WriteLine("No conversation selected.");
                return false;
            }

            var userConversations = ConversationList[currentUser.Username];

            var selectedConvo = userConversations.FirstOrDefault(c => c.ConvoID.Equals(convoId));

            if (selectedConvo == null)
            {
                Console.WriteLine($"No conversation selected. Debug: Could not find conversation with ID: {convoId}");
                return false;
            }

            currentConvo = selectedConvo;
            Console.WriteLine($"Continuing conversation: {selectedConvo.ConvoName}");

            // Display only the messages from the selected conversation
            currentConvo.DisplayMessages(); 
            return true;
        }

        public void DisplayConvoList()
        {
            if (currentUser == null || !ConversationList.TryGetValue(currentUser.Username, out List<Conversation>? value) || value.Count == 0)
            {
                Console.WriteLine("No conversation history found.");
                return;
            }

            Console.WriteLine("Conversations available:");
            foreach (var convo in value)
            {
                Console.WriteLine($"--- {convo.ConvoName} (ID: {convo.ConvoID}) ---");
            }
        }

        public void DisplayConvoMessages(Guid convoId)
        {
            if (currentUser == null || !ConversationList.TryGetValue(currentUser.Username, out List<Conversation>? value) || value.Count == 0)
            {
                Console.WriteLine("No conversation history found.");
                return;
            }

            var userConversations = ConversationList[currentUser.Username];
            var selectedConvo = userConversations.FirstOrDefault(c => c.ConvoID == convoId);

            if (selectedConvo == null)
            {
                Console.WriteLine("Conversation not found.");
                return;
            }

            Console.WriteLine($"Messages for conversation: {selectedConvo.ConvoName}");
            selectedConvo.DisplayMessages();
        }

        private void LoadConversationsForUser()
        {
            if (currentUser == null) return;

            if (!ConversationList.TryGetValue(currentUser.Username, out var userConversations))
            {
                // Initialize an empty list for the user if there are no existing conversations
                ConversationList[currentUser.Username] = new List<Conversation>();
            }
        }

        public void SetGuestMode(bool isGuest)
        {
            isGuestUser = isGuest;
        }

        private void SaveConversationsForUser()
        {
            if (!isGuestUser && currentUser != null)
            {
                dbService.SaveConversations(ConversationList);
            }
        }

        public void RenameConvoInterface()
        {
            if (currentUser == null || !ConversationList.TryGetValue(currentUser.Username, out List<Conversation>? value) || value.Count == 0)
            {
                Console.WriteLine("No conversations to rename.");
                return;
            }

            Console.WriteLine("Available Conversations:");
            foreach (var convo in value)
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
            if (currentUser == null) return;

            var userConversations = ConversationList[currentUser.Username];
            var conversation = userConversations.FirstOrDefault(c => c.ConvoID == convoId);

            if (conversation == null)
            {
                Console.WriteLine($"Conversation with ID '{convoId}' not found.");
                return;
            }

            conversation.ConvoName = newName;
            SaveConversationsForUser(); // Save after renaming
            Console.WriteLine($"Conversation renamed to: {newName}");
        }

        public void DeleteConvoInterface()
        {
            if (currentUser == null || !ConversationList.TryGetValue(currentUser.Username, out List<Conversation>? value) || value.Count == 0)
            {
                Console.WriteLine("No conversations to delete.");
                return;
            }

            Console.WriteLine("Available Conversations:");
            foreach (var convo in value)
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
            if (currentUser == null) return;

            var userConversations = ConversationList[currentUser.Username];
            var conversation = userConversations.FirstOrDefault(c => c.ConvoID == convoId);

            if (conversation == null)
            {
                Console.WriteLine($"Conversation with ID '{convoId}' not found.");
                return;
            }

            userConversations.Remove(conversation);
            SaveConversationsForUser(); // Save after deletion
            Console.WriteLine($"{conversation.ConvoName} deleted.");
        }
    }
}