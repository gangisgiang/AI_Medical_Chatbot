using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public interface IUserService
    {
        void RegisterUser(string username, int userID);
        User GetUser(int userID)
        {
            // Method body goes here
            throw new NotImplementedException();
        }
    }
}