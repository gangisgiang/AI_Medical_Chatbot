using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;

namespace AI_Medical_Chatbot
{
    public class RespAIService : AIService, IAIResponse
    {
        private readonly List<string> _respData = new List<string>
        {
            // list keywords that relevant to respiratory medical field
            "respiratory", "breathing", "lung", "cough", "chest", "pneumonia", 
            "asthma", "bronchitis", "influenza", "tuberculosis", "emphysema", 
            "pulmonary", "sputum", "wheezing", "dyspnea", "cyanosis", "hypoxemia", 
            "hypoxia", "pleurisy", "pulmonary embolism", "pulmonary fibrosis", 
            "pulmonary hypertension", "respiratory distress syndrome", "respiratory failure", 
            "respiratory syncytial virus", "sarcoidosis", "sleep apnea", "smoking", "spirometry", 
            "trachea", "tracheostomy", "ventilation", "vital capacity"
        };

        public  async Task<string> GenerateResponse(string message)
        {
            if (_respData.Count == 0)
            {
                string rawHtmlData = await _apiAdapter.FetchData("cardiovascular");
                return _apiAdapter.ConvertHtmlToPlainText(rawHtmlData);
            }

            // Cluster the input and return cardio-related information
            string cluster = ClusterRespTopic(message);
            Console.WriteLine("Cluster: " + cluster);

            if (string.IsNullOrEmpty(cluster))
            {
                string rawHtmlData = await _apiAdapter.FetchData("cardiovascular");
                string processedData = _apiAdapter.ConvertHtmlToPlainText(rawHtmlData);
                return ExtractRelevantInfo(processedData, message);
            }

            string htmlData = await _apiAdapter.FetchData(cluster);
            string plainTextData = _apiAdapter.ConvertHtmlToPlainText(htmlData);

            string relevantInfo = ExtractRelevantInfo(plainTextData, cluster);

            if (string.IsNullOrEmpty(relevantInfo))
            {
                return "No relevant information found.";
            }

            return relevantInfo;
        }

        private string ClusterRespTopic(string input)
        {
            MLContext mlContext = new MLContext();
            List<TopicData> data = _respData.Select(x => new TopicData { Topic = x }).ToList();
            IDataView dataView = mlContext.Data.LoadFromEnumerable(data);

            // Define the pipeline
            var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(TopicData.Topic)).
                Append(mlContext.Clustering.Trainers.KMeans("Features", numberOfClusters: 8));

            // Train the model
            var model = pipeline.Fit(dataView);
            var predictionEngine = mlContext.Model.CreatePredictionEngine<TopicData, ClusterPrediction>(model);

            // Cluster the input and predict the cluster
            TopicData inputTopic = new TopicData { Topic = input };
            ClusterPrediction prediction = predictionEngine.Predict(inputTopic);

            switch (prediction.PredictedLabel)
            {
                case 1:
                    return "lung";
                case 2:
                    return "asthma";
                case 3:
                    return "pneumonia";
                case 4:
                    return "bronchitis";
                case 5:
                    return "chronic obstructive pulmonary disease";
                case 6:
                    return "emphysema";
                case 7:
                    return "respiratory distress syndrome";
                case 8:
                    return "respiratory failure";
                default:
                    return string.Empty;
            }
        }

        private string ExtractRelevantInfo(string data, string topic)
        {
            // Split the data into paragraphs
            string[] paragraphs = data.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // Filter the paragraphs based on the topic
            var relevantParagraphs = paragraphs.Where(p => p.Contains(topic, StringComparison.OrdinalIgnoreCase) 
            || _respData.Any(keyword => p.Contains(keyword, StringComparison.OrdinalIgnoreCase))).ToArray();

            Console.WriteLine("Relevant information found:");
            foreach (string paragraph in relevantParagraphs)
            {
                Console.WriteLine(paragraph);
            }

            if (relevantParagraphs.Length == 0)
            {
                return "No relevant information found.";
            }

            // Return the relevant information
            string firstParagraph = relevantParagraphs[0];
            string firstSentence = firstParagraph.Split('.').FirstOrDefault()?.Trim() + ".";
            return firstSentence;
        }

        // Define the data classes
        public class TopicData
        {
            public string? Topic { get; set; }
        }

        public class ClusterPrediction
        {
            [ColumnName("PredictedLabel")]
            public uint PredictedLabel { get; set; }
        }
    }
}