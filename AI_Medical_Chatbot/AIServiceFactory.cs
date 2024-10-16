using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Medical_Chatbot
{
    public static class AIServiceFactory
    {
        // Factory method to create an AIService based on the topic
        public static IAIResponse CreateAIService(string topic)
        {
            switch (topic.ToLower())
            {
                case "cardiovascular":
                    return new CardioAIService();
                case "dermatology":
                    return new DermatologyAIService();
                case "endocrine":
                    return new EndocrinologyAIService();
                case "immune":
                    return new ImmunologyAIService();
                case "nephrology":
                    return new NephrologyAIService();
                case "neurology":
                    return new NeuroAIService();
                case "oncology":
                    return new OncologyAIService();
                case "ophthalmology":
                    return new OphthalmologyAIService();
                case "respiratory":
                    return new RespAIService();
                case "rheumatology":
                    return new RheumatologyAIService();
                default:
                    throw new ArgumentException("Unknown topic: " + topic);
            }
        }
    }
}