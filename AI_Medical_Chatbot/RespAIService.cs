// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;

// namespace AI_Medical_Chatbot
// {
//     public class RespAIService
//     {
//         private readonly List<string> _respData = new List<string>
//         {
//             // list keywords that relevant to respiratory medical field
//             "respiratory", "breathing", "lung", "cough", "chest", "pneumonia", 
//             "asthma", "bronchitis", "influenza", "tuberculosis", "emphysema", 
//             "pulmonary", "sputum", "wheezing", "dyspnea", "cyanosis", "hypoxemia", 
//             "hypoxia", "pleurisy", "pulmonary embolism", "pulmonary fibrosis", 
//             "pulmonary hypertension", "respiratory distress syndrome", "respiratory failure", 
//             "respiratory syncytial virus", "sarcoidosis", "sleep apnea", "smoking", "spirometry", 
//             "trachea", "tracheostomy", "ventilation", "vital capacity"
//         };

//         public override async Task<string> GenerateResponse(string message)
//         {
//             if (_respData.Count == 0)
//             {
//                 var rawHtmlData = await FetchData("respiratory");
//                 return ConvertHtmlToPlainText(rawHtmlData);
//             }

//             // Cluster the input and return respiratory-related information
//             string cluster = ClusterRespTopic(message);
//             Console.WriteLine("Cluster: " + cluster);

//             // If clustering doesn't work, return general information
//             if (string.IsNullOrEmpty(cluster))
//             {
//                 var rawHtmlData = await FetchData("respiratory");
//                 string processedData = ConvertHtmlToPlainText(rawHtmlData);
//                 return ExtractRelevantInfo(processedData, message);
//             }

//             // Fetch data based on the cluster
//             var htmlData = await FetchData(cluster);
//             string plainTextData = ConvertHtmlToPlainText(htmlData);

//             // Console.WriteLine("Fetched data: " + plainTextData);

//             string relevantInfo = ExtractRelevantInfo(plainTextData, cluster);

//             if (string.IsNullOrEmpty(relevantInfo))
//             {
//                 return "No relevant information found.";
//             }

//             // Extract relevant information from the fetched data
//             return relevantInfo;
//         }

//         private string ClusterRespTopic(string input)
//         {
//             MLContext mlContext = new MLContext();
//             List<TopicData> data = _respData.Select(x => new TopicData { Topic = x }).ToList();
//             IDataView trainingData = mlContext.Data.LoadFromEnumerable(data);

//             // Define the pipeline
//             var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(TopicData.Topic)).
//                 Append(mlContext.Clustering.Trainers.KMeans("Features", numberOfClusters: 3));

//             // Train the model
//             var model = pipeline.Fit(dataView);
//             var predictionEngine = mlContext.Model.CreatePredictionEngine<TopicData, ClusterPrediction>(model);

//             // Cluster the input and predict the cluster
//             TopicData inputTopic = new TopicData { Topic = input };
//             ClusterPrediction prediction = predictionEngine.Predict(inputTopic);

//             switch (prediction.PredictedLabel)
//             {
//                 case 1:
//                     return "lung";
//                 case 2:
//                     return "asthma";
//                 case 3:
//                     return "pneumonia";
//                 default:
//                     return string.Empty;
//             }
//         }
//     }
// }