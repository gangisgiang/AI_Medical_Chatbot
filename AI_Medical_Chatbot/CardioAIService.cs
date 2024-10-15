using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace AI_Medical_Chatbot
{
    public class CardioAIService : AIService, IAIResponse
    {
        private readonly List<string> _cardioData = new List<string>
        {
            "heart", "hypertension", "blood pressure", "stroke", "cardiovascular system",
            "cardio", "cholesterol", "cardiac", "cardiology", "cardiologist",
            "cardiomyopathy", "cardiopulmonary", "cardiorespiratory", "atherosclerosis",
            "myocardial infarction", "arrhythmia", "heart attack", "congestive heart failure",
            "vascular disease", "endocarditis", "heart valve disease", "coronary artery disease"
        };

        public async Task<string> GenerateResponse(string message)
        {
            if (_cardioData.Count == 0)
            {
                return await FetchandConvert("cardiovascular");
            }

            // Cluster the input and return cardio-related information
            string cluster = ClusterCardioTopic(message);
            Console.WriteLine("Cluster: " + cluster);

            if (string.IsNullOrEmpty(cluster))
            {
                return await FetchandConvert("cardiovascular");
            }

            string plainTextData = await FetchandConvert(cluster);

            string relevantInfo = ExtractRelevantInfo(plainTextData, cluster);

            if (string.IsNullOrEmpty(relevantInfo))
            {
                return "No relevant information found.";
            }

            return relevantInfo;
        }

        private string ClusterCardioTopic(string input)
        {
            MLContext mlContext = new MLContext();
            List<TopicData> data = _cardioData.Select(x => new TopicData { Topic = x }).ToList();
            IDataView dataView = mlContext.Data.LoadFromEnumerable(data);

            var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(TopicData.Topic)).
              Append(mlContext.Clustering.Trainers.KMeans("Features", numberOfClusters: 1));

            var model = pipeline.Fit(dataView);
            var predictionEngine = mlContext.Model.CreatePredictionEngine<TopicData, ClusterPrediction>(model);

            TopicData inputTopic = new TopicData { Topic = input };
            ClusterPrediction prediction = predictionEngine.Predict(inputTopic);

            switch (prediction.PredictedLabel)
            {
                case 1:
                //     return "heart disease";
                // case 2:
                //     return "cardiovascular system";
                // case 3:
                //     return "blood pressure";
                // case 4:
                //     return "stroke";
                // case 5:
                //     return "cholesterol";
                // case 6:
                //     return "arrhythmia";
                // case 7:
                //     return "congestive heart failure";
                // case 8:
                //     return "heart valve disease";
                // case 9:
                //     return "vascular disease";
                // case 10:
                    return "cardiovascular";
                default:
                    return string.Empty;
            }
        }

        private string ExtractRelevantInfo(string plainTextData, string topic)
        {
            // Split the data into paragraphs
            string[] paragraphs = plainTextData.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // Filter paragraphs containing the topic or related information
            var relevantParagraphs = paragraphs.Where(p => p.Contains(topic, StringComparison.OrdinalIgnoreCase) 
            || _cardioData.Any(keyword => p.Contains(keyword, StringComparison.OrdinalIgnoreCase))).ToArray();

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

            // var paragraphs = plainTextData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            // var relevantParagraphs = paragraphs.Where(p => _respData.Any(keyword => p.Contains(keyword, StringComparison.OrdinalIgnoreCase))).ToArray();

            // return relevantParagraphs.Length == 0 ? "No relevant information found." : relevantParagraphs.First().Split('.').FirstOrDefault()?.Trim() + ".";
        }

        public class TopicData
        {
            public string? Topic { get; set; }
        }

        public class ClusterPrediction
        {
            [ColumnName("PredictedLabel")]
            public uint PredictedLabel;
        }
    }
}
