using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public class RenameConvoCmd : Command
    {
        private readonly ChatComponent _chatComponent;
        private readonly Guid _conversationId;
        private readonly string _newName;

        public RenameConvoCmd(ChatComponent chatComponent, Guid conversationId, string newName)
        {
            _chatComponent = chatComponent;
            _conversationId = conversationId;
            _newName = newName;
        }

        public override void Execute()
        {
            _chatComponent.RenameConvo(_conversationId, _newName);
        }
    }
}