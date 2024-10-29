using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public class ConvoCmd : Command
    {
        private readonly ChatComponent _chatComponent;
        private readonly ConvoAction _action;
        private readonly Guid _conversationId;
        private readonly string _newName;

        public ConvoCmd(ChatComponent chatComponent, ConvoAction action, Guid conversationId = default, string? newName = null)
        {
            _chatComponent = chatComponent;
            _action = action;
            _conversationId = conversationId;
            _newName = newName;
        }

        public override void Execute()
        {
            switch (_action)
            {
                case ConvoAction.Rename:
                    if (!string.IsNullOrEmpty(_newName))
                    {
                        _chatComponent.RenameConvo(_conversationId, _newName);
                    }
                    else
                    {
                        Console.WriteLine("New name cannot be empty.");
                    }
                    break;

                case ConvoAction.Delete:
                    _chatComponent.DeleteConvo(_conversationId);
                    break;

                case ConvoAction.Continue:
                    _chatComponent.ContinueConversation(_conversationId);
                    break;

                case ConvoAction.Create:
                    _chatComponent.CreateNewConvo();
                    break;

                default:
                    Console.WriteLine("Unknown action.");
                    break;
            }
        }
    }
}