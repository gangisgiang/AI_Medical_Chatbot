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
            if (_cardioData.Count == 0)
            {
                var rawHtmlData = await FetchData("cardiovascular");
                return ConvertHtmlToPlainText(rawHtmlData);
            }

            // Cluster the input and return cardio-related information
            string cluster = ClusterCardioTopic(message);
            Console.WriteLine("Cluster: " + cluster);

            // If clustering doesn't work, return general information
            if (string.IsNullOrEmpty(cluster))
            {
                var rawHtmlData = await FetchData("cardiovascular");
                string processedData = ConvertHtmlToPlainText(rawHtmlData);
                return ExtractRelevantInfo(processedData, message);
            }

            // Fetch data based on the cluster
            var htmlData = await FetchData(cluster);
            string plainTextData = ConvertHtmlToPlainText(htmlData);

            // Console.WriteLine("Fetched data: " + plainTextData);

            string relevantInfo = ExtractRelevantInfo(plainTextData, cluster);

            if (string.IsNullOrEmpty(relevantInfo))
            {
                return "No relevant information found.";
            }

            // Extract relevant information from the fetched data
            return relevantInfo;
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
                    return "cardiovascular";
                case 5:
                    return "cholesterol";
                default:
                    return string.Empty;
            }
        }

        // Extract relevant information from the fetched data
        private string ExtractRelevantInfo(string plainTextData, string topic)
        {
            // Split the data into paragraphs
            string[] paragraphs = plainTextData.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // Filter paragraphs containing the topic or related information
            var relevantParagraphs = paragraphs.
            Where(p => p.Contains(topic, StringComparison.OrdinalIgnoreCase) || 
                       _cardioData.Any(keyword => p.Contains(keyword, StringComparison.OrdinalIgnoreCase))).
                       ToArray();

            Console.WriteLine("Relevant information found:");
            foreach (var paragraph in paragraphs)
            {
                Console.WriteLine(paragraph);
            }

            if (relevantParagraphs.Length == 0)
            {
                return "No relevant information found.";
            }

            // Return the first sentence of the first relevant paragraph
            string firstParagraph = relevantParagraphs[0];
            string firstSentence = firstParagraph.Split('.').FirstOrDefault()?.Trim() + ".";
            return firstSentence;
            // Return the relevant information as a short summary or the first paragraph
            // return relevantParagraphs.Length > 0 ? relevantParagraphs[0] : "No relevant information found.";
            
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
        }
    }
}