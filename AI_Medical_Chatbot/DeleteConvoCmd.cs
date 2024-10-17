using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public class DeleteConvoCmd : Command
    {
        private readonly ChatComponent _chatComponent;
        private readonly Guid _conversationId;

        public DeleteConvoCmd(ChatComponent chatComponent, Guid conversationId)
        {
            _chatComponent = chatComponent;
            _conversationId = conversationId;
        }

        public override void Execute()
        {
            _chatComponent.DeleteConvo(_conversationId);
        }
    }
}