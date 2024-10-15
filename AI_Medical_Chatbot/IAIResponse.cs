using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public interface IAIResponse
    {
        Task<string> GenerateResponse(string message);
    }
}