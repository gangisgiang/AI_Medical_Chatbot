using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public interface UserService_I
    {
        void RegisterUser(string username, int userID);
        User GetUser(int userID)
        {
            throw new NotImplementedException();
        }
    }
}