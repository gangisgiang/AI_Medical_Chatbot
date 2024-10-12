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
            // If there's no element in _cardioData, fetch "cardiovascular" data
            if (_cardioData.Count == 0)
            {
                return await FetchData("cardiovascular");
            }

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
            MLContext mlContext = new MLContext();
            List<TopicData> data = _cardioData.Select(x => new TopicData { Topic = x }).ToList();
            IDataView dataView = mlContext.Data.LoadFromEnumerable(data);

            EstimatorChain<ClusteringPredictionTransformer<Microsoft.ML.Trainers.KMeansModelParameters>> pipeline 
            = mlContext.Transforms.Text.FeaturizeText("Features", nameof(TopicData.Topic)).
              Append(mlContext.Clustering.Trainers.KMeans("Features", numberOfClusters: 5));

            TransformerChain<ClusteringPredictionTransformer<Microsoft.ML.Trainers.KMeansModelParameters>> model = pipeline.Fit(dataView);
            PredictionEngine<TopicData, ClusterPrediction> predictionEngine = mlContext.Model.CreatePredictionEngine<TopicData, ClusterPrediction>(model);

            // Cluster the input and predict the cluster
            TopicData inputTopic = new TopicData { Topic = input };
            ClusterPrediction prediction = predictionEngine.Predict(inputTopic);

            switch (prediction.PredictedLabel)
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
            public string? Topic { get; set; }
        }

        // Cluster prediction class
        public class ClusterPrediction
        {
            [ColumnName("PredictedLabel")]
            public uint PredictedLabel;
            public float[] Score;
        }
    }
}