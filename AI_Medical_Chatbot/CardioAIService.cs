using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.ML;
using Microsoft.ML.Data;


namespace AI_Medical_Chatbot
{
    public class CardioAIService : AIService
    {
        private readonly List<string> _cardioData = new List<string>
        { 
            "heart", "hypertension", "blood pressure", "stroke", "cardiovascular", 
            "cardio", "cholesterol", "cardiac", "cardiology", "cardiologist", 
            "cardiomyopathy", "cardiopulmonary", "cardiorespiratory" 
        };

        public override async Task<string> GenerateResponse(string message)
        {
            // Cluster the input and return cardio-related information
            string cluster = ClusterCardioTopic(message);

            // If clustering doesn't work, return general information
            if (string.IsNullOrEmpty(cluster))
            {
                await FetchData("cardiovascular");
            }

            // Fetch data based on the cluster
            string data = await FetchData(cluster);
            return data;
        }

        private string ClusterCardioTopic(string input)
        {
            var mlContext = new MLContext();
            var data = _cardioData.Select(x => new TopicData { Topic = x }).ToList();
            var dataView = mlContext.Data.LoadFromEnumerable(data);

            var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(TopicData.Topic)).
            Append(mlContext.Clustering.Trainers.KMeans("Features", numberOfClusters: 5));

            var model = pipeline.Fit(dataView);
            var predictionEngine = mlContext.Model.CreatePredictionEngine<TopicData, TopicPrediction>(model);
            
            // Cluster the input and predict the cluster
            var inputTopic = new TopicData { Topic = input };
            var prediction = predictionEngine.Predict(inputTopic);

            switch (prediction.PredictedClusterLabel)
            {
                case 1:
                    return "heart disease";
                case 2:
                    return "hypertension";
                case 3:
                    return "stroke";
                case 4:
                    return "cardiovascular health";
                case 5:
                    return "cholesterol";
                default:
                    return "";
            }
        }

        // Define the data classes
        public class TopicData
        {
            public string Topic { get; set; }
        }

        // Cluster prediction class
        public class ClusterPrediction
        {
            [ColumnName("PredictedLabel")]
            public uint PredictedClusterId;
            public float[] Score;
        }
    }
}