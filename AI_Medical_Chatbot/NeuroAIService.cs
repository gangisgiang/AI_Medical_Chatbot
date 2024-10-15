using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;


namespace AI_Medical_Chatbot
{
    public class NeuroAIService : AIService, IAIResponse
    {
        private readonly List<string> _neuroData = new List<string>
        { 
            "neuron", "synapse", "axon", "dendrite", "cerebellum", "cerebral cortex", "spinal cord",
            "nervous system", "myelin", "glial cells", "cerebrospinal fluid", "electroencephalogram",
            "computed tomography", "multiple sclerosis", "epilepsy", "seizures", "dementia", "tremor", 
            "cognitive impairment", "motor neuron disease", "neuroinflammation", "neuropathy", 
            "cranial nerves", "neurotoxin", "neurovascular disorder", "brainstem"
        };

        public async Task<string> GenerateResponse(string message)
        {
            if (_neuroData.Count == 0)
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
            List<TopicData> data = _neuroData.Select(x => new TopicData { Topic = x }).ToList();
            IDataView dataView = mlContext.Data.LoadFromEnumerable(data);

            var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(TopicData.Topic)).
              Append(mlContext.Clustering.Trainers.KMeans("Features", numberOfClusters: 10));

            var model = pipeline.Fit(dataView);
            var predictionEngine = mlContext.Model.CreatePredictionEngine<TopicData, ClusterPrediction>(model);

            // Cluster the input and predict the cluster
            TopicData inputTopic = new TopicData { Topic = input };
            ClusterPrediction prediction = predictionEngine.Predict(inputTopic);

            switch (prediction.PredictedLabel)
            {
                case 1:
                    return "neuron";
                case 2:
                    return "brain structure";
                case 3:
                    return "nervous system";
                case 4:
                    return "neuroimaging";
                case 5:
                    return "neurological diseases";
                case 6:
                    return "seizure disorders";
                case 7:
                    return "cognitive and motor issues";
                case 8:
                    return "nervous system disorders";
                case 9:
                    return "inflammation and toxins";
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
            || _neuroData.Any(keyword => p.Contains(keyword, StringComparison.OrdinalIgnoreCase))).ToArray();

            Console.WriteLine("Relevant information found:");
            foreach (var paragraph in paragraphs)
            {
                Console.WriteLine(paragraph);
            }

            if (relevantParagraphs.Length == 0)
            {
                return "No relevant information found.";
            }

            // Return only the first sentence of the first relevant paragraph
            string firstParagraph = relevantParagraphs[0];
            string firstSentence = firstParagraph.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim() + ".";
            return firstSentence;
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