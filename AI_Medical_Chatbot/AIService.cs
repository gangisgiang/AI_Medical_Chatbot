using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace AI_Medical_Chatbot
{
    public abstract class AIService
    {
        protected ApiAdapter _apiAdapter;
        
        public AIService()
        {
            _apiAdapter = new ApiAdapter();
        }

        protected async Task<string> FetchandConvert(string topic)
        {
            string rawHtmlData = await _apiAdapter.FetchData(topic);
            return _apiAdapter.ConvertHtmlToPlainText(rawHtmlData);
        }

        public abstract string ClusterTopic(string message);
        public abstract string ExtractRelevantInfo(string plainTextData, string topic);
    }
}