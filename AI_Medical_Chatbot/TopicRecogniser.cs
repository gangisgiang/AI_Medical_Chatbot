using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AI_Medical_Chatbot
{
    public class TopicRecogniser
    {
        public readonly Dictionary<string, string> topicMap = new Dictionary<string, string>()
        {
            // Cardiovascular Keywords
            {"cardio", "cardiovascular"}, {"heart", "cardiovascular"}, {"hypertension", "cardiovascular"}, {"blood pressure", "cardiovascular"}, 
            {"stroke", "cardiovascular"}, {"cardiovascular", "cardiovascular"}, {"valve", "cardiovascular"}, 
            {"cardiology", "cardiovascular"}, {"vascular", "cardiovascular"}, {"cardiac", "cardiovascular"}, {"cholesterol", "cardiovascular"},
            {"atherosclerosis", "cardiovascular"}, {"myocardial infarction", "cardiovascular"}, {"arrhythmia", "cardiovascular"}, {"endocarditis", "cardiovascular"}, 
            {"coronary", "cardiovascular"}, {"CAD", "cardiovascular"}, {"plaque", "cardiovascular"}, {"circulation", "cardiovascular"},
            
            // Respiratory Keywords
            {"respiratory", "respiratory"}, {"lung", "respiratory"}, {"asthma", "respiratory"}, {"bronchitis", "respiratory"}, 
            {"chronic obstructive pulmonary", "respiratory"}, {"emphysema", "respiratory"}, {"pneumonia", "respiratory"}, 
            {"tuberculosis", "respiratory"}, {"covid", "respiratory"}, {"coronavirus", "respiratory"}, {"covid-19", "respiratory"}, 
            {"sars-cov-2", "respiratory"}, {"sars", "respiratory"}, {"mers", "respiratory"}, {"influenza", "respiratory"}, {"flu", "respiratory"}, 
            {"cold", "respiratory"}, {"pulmonary", "respiratory"}, {"pulmonology", "respiratory"}, {"alveoli", "respiratory"},
            {"breathe", "respiratory"}, {"breathing", "respiratory"}, {"respiration", "respiratory"}, {"ventilation", "respiratory"}, 
            {"airways", "respiratory"}, {"bronchi", "respiratory"},

            // Neurology Keywords
            {"neurology", "neurology"}, {"brain", "neurology"}, {"nerves", "neurology"}, {"spinal", "neurology"}, {"epilepsy", "neurology"}, 
            {"seizures", "neurology"}, {"alzheimer", "neurology"}, {"parkinson", "neurology"}, {"multiple sclerosis", "neurology"}, 
            {"ms", "neurology"}, {"migraine", "neurology"}, {"headache", "neurology"}, {"neurodegenerative", "neurology"}, {"cord", "neurology" },
            {"neuron", "neurology"}, {"tumor", "neurology"}, {"cognitive", "neurology"}, {"dopamine", "neurology"}, {"motor", "neurology"}, {"nervous", "neurology"},
            
            // Dermatology Keywords
            {"clogged pores", "dermatology"}, {"blackheads", "dermatology"}, {"pimples", "dermatology"}, {"itchy inflamed skin", "dermatology"}, 
            {"dry inflamed skin", "dermatology"}, {"chronic itchy skin", "dermatology"}, {"thick scaly patches", "dermatology"}, 
            {"skin disorder", "dermatology"}, {"malignant melanoma", "dermatology"}, {"skin cancer", "dermatology"}, 
            {"abnormal mole growth", "dermatology"}, {"red visible blood vessels", "dermatology"}, {"persistent facial redness", "dermatology"}, 
            {"pus-filled bumps on face", "dermatology"}, {"hair loss", "dermatology"}, {"alopecia areata", "dermatology"}, 
            {"chronic hair thinning", "dermatology"}, {"severe skin allergies", "dermatology"}, {"contact dermatitis from metal", "dermatology"}, 
            {"red swollen allergic reaction", "dermatology"}, {"allergic reaction to fragrances", "dermatology"}, {"poison ivy rash", "dermatology"}, 
            {"atopic dermatitis", "dermatology"}, {"chronic scaly patches", "dermatology"}, {"melanin production disorder", "dermatology"},

            // Rheumatology Keywords
            {"inflammation", "rheumatology"}, {"joints", "rheumatology"}, {"bones", "rheumatology"}, {"muscles", "rheumatology"},
            {"rheumatoid", "rheumatology"}, {"lupus", "rheumatology"}, {"joint pain", "rheumatology"},
            {"gout", "rheumatology"}, {"ankylosing spondylitis", "rheumatology"}, {"psoriatic", "rheumatology"},
            {"arthritis", "rheumatology"}, {"spine fusion", "rheumatology"},

            // Oncology Keywords
            {"breast cancer cells", "oncology"}, {"lung cancer due to smoking", "oncology"}, 
            {"lung cancer symptoms", "oncology"}, {"prostate gland cancer", "oncology"}, {"prostate cancer male", "oncology"},
            {"blood-forming tissues cancer", "oncology"}, {"leukemia affects bone marrow", "oncology"}, {"blood cell cancer", "oncology"},
            {"melanoma skin pigmentation", "oncology"}, {"skin cancer melanocytes", "oncology"}, {"skin cancer serious", "oncology"},

            // Ophthalmology Keywords
            {"optic nerve", "ophthalmology"}, {"high eye pressure", "ophthalmology"}, {"cloudy lens", "ophthalmology"}, 
            {"lens clouding", "ophthalmology"}, {"retina damage", "ophthalmology"}, {"central vision loss", "ophthalmology"},
            {"retinal blood vessels", "ophthalmology"}, {"eye blood vessel damage", "ophthalmology"}, {"dry, irritated eyes", "ophthalmology"}, 
            {"tear film dysfunction", "ophthalmology"}, {"ocular surface", "ophthalmology"},

            // Immunology Keywords
            {"immune", "immunology"}, {"system", "immunology"}, {"immunity", "immunology"}, 
            {"autoimmune", "immunology"}, {"sclerosis", "immunology"}, {"B-cells", "immunology"},
            {"allergies", "immunology"}, {"pollen", "immunology"}, {"food", "immunology"}, 
            {"immunodeficiency", "immunology"}, {"vaccination", "immunology"}, {"response", "immunology"}, 
            {"hypersensitivity", "immunology"}, {"anaphylaxis", "immunology"}, {"immunotherapy", "immunology"}, 
            {"antibody", "immunology"}, {"immunosuppressive", "immunology"}, {"T-cells", "immunology"},

            // Nephrology Keywords
            {"kidney", "nephrology"}, {"dialysis", "nephrology"}, {"transplant", "nephrology"}, 
            {"polycystic", "nephrology"}, {"cysts", "nephrology"}, {"stones", "nephrology"}, 
            {"nephritis", "nephrology"}, {"glomerulonephritis", "nephrology"}, {"proteinuria", "nephrology"}, 
            {"artery", "nephrology"}, {"stenosis", "nephrology"}, {"nephropathy", "nephrology"}, 
            {"nephron", "nephrology"}, {"glomerulus", "nephrology"}, {"electrolyte", "nephrology"}, 
            {"uremic", "nephrology"}, {"toxins", "nephrology"}, {"diabetic", "nephrology"}, 
            {"hypertensive", "nephrology"}, {"acute", "nephrology"},

            // Endocrinology Keywords
            {"chronic", "endocrinology"}, {"insulin", "endocrinology"}, {"glucose", "endocrinology"}, {"pancreas", "endocrinology"},
            {"adrenal", "endocrinology"}, {"hormonal", "endocrinology"}, {"imbalances", "endocrinology"}, {"reproductive", "endocrinology"},
            {"abnormal", "endocrinology"}, {"hormone", "endocrinology"}, {"endocrine", "endocrinology"}, {"glands", "endocrinology"},
            {"pituitary", "endocrinology"}, {"overproduction", "endocrinology"}, {"sugar level", "endocrinology"},
            {"bone", "endocrinology"}, {"fragility", "endocrinology"}, {"density", "endocrinology"},
            {"thyroid", "endocrinology"}, {"metabolism", "endocrinology"}, {"fractures", "endocrinology"}
        };

        private (string, string) PreProcessInput(string input)
        {
            // Convert input to lowercase and remove special characters
            input = input.ToLower();
            input = Regex.Replace(input, "[^a-zA-Z0-9 ]", "");

            // Tokenize input to better identify keywords
            string[] tokens = input.Split(' ');

            // Look for keywords in the input
            foreach (var entry in topicMap)
            {
                // Check if any token matches the keyword from the dictionary
                if (tokens.Any(token => token.Contains(entry.Key)))
                {
                    Console.WriteLine($"Matched keyword: {entry.Key} -> Topic: {entry.Value}");
                    return (entry.Key, entry.Value);
                }
            }

            return (string.Empty, string.Empty); // Return empty if no match is found
        }

        // Recognise the and return the topic of the input
        public async Task<string> RecogniseAndRespond(string input)
        {
            var (keyword, topic) = PreProcessInput(input);

            if (string.IsNullOrEmpty(topic))
            {
                return "Sorry, I don't have enough information on that. Can you try rephrasing your question?";
            }

            Console.WriteLine("Processing input for topic: " + topic);

            IAIResponse aiService = AIServiceFactory.CreateAIService(topic);

            // Generate the response using the selected AI service
            string response = await aiService.GenerateResponse(input);

            if (!string.IsNullOrEmpty(response))
            {
                return response;
            }

            return "Sorry, I don't have enough information on that. Can you try rephrasing your question?";
        }
    }
}
