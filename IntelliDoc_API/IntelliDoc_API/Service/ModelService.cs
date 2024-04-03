using DocumentFormat.OpenXml.Packaging;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace IntelliDoc_API.Service
{
    public class ModelService
    {
        protected TfidfVectorizer? vectorizer;
        protected InferenceSession session;


        public ModelService()
        {
            string currentDir = Directory.GetCurrentDirectory();

            string tfidfVectorizerPath = currentDir + @"\ML_Model\TFIDFvectorizer.json";
            string jsonString = File.ReadAllText(tfidfVectorizerPath, Encoding.UTF8);
            vectorizer = JsonSerializer.Deserialize<TfidfVectorizer>(jsonString);

            string modelPath = currentDir + @"\ML_Model\DocClassificationLrModel.onnx";
            session = new InferenceSession(modelPath);
        }

        public class TfidfVectorizer
        {
            public Dictionary<string, int>? Vocabulary { get; set; }
            public float[]? IDF { get; set; }
        }

        public string[] GetCategoryList()
        {
            string[] categories = new string[] { "Business", "Entertainment", "Food", "Graphics", "Historical", "Medical", "Politics", "Space", "Sport", "Technologie", "Others" };
            return categories;
        }

        // Extract the document text.
        public string DocumentTextExtraction(byte[] docBytes, string docName)
        {
            if (docBytes == null || docBytes.Length == 0)
                throw new Exception($"{docName}, Document byte array cannot be null or empty.");

            string text;
            var memoryStream = new MemoryStream(docBytes);
            if (docName.EndsWith(".pdf"))
            {
                var pageText = new StringBuilder();
                var document = new PdfDocument(new PdfReader(memoryStream)); // Read PDF using new PdfDocument & new PdfReader
                var pageNum = document.GetNumberOfPages();
                for (int page = 1; page <= pageNum; page++)
                {
                    // New LocationTextExtractionStrategy creates a new text extraction renderer
                    var strategy = new LocationTextExtractionStrategy();
                    var parser = new PdfCanvasProcessor(strategy);
                    parser.ProcessPageContent(document.GetPage(page));
                    pageText.Append(strategy.GetResultantText());
                }
                text = pageText.ToString();
            }
            else if (docName.EndsWith(".docx"))
            {
                // Use Open XML SDK for Word document text extraction
                var document = WordprocessingDocument.Open(memoryStream, false);
                DocumentFormat.OpenXml.Wordprocessing.Body body = document.MainDocumentPart.Document.Body;
                text = body.InnerText;
            }
            else if (docName.EndsWith(".txt"))
            {
                // Read text directly for TXT files
                text = Encoding.UTF8.GetString(docBytes);
            }
            else
                throw new Exception($"Unsupported file format: {docName}");
            return text;
        }

        // Preprocess the document text.
        public string PreprocessText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text; // Handle empty or null text (optional)
            text = text.ToLowerInvariant(); // Convert to lowercase
            text = Regex.Replace(text, "[^\\w\\s]", ""); // Remove punctuation and non-alphanumeric characters
            text = Regex.Replace(text, @"\s+", " "); // Remove extra whitespace
            text = Regex.Replace(text, @"\d+", ""); // Remove numbers
            return text;
        }

        // Convert the preprocessed text to features.
        public float[] Transform(string text)
        {
            var tokens = Regex.Matches(text.ToLower(), @"\b\w\w+\b").Cast<Match>().Select(m => m.Value).ToArray(); // Tokenization
            var tf = new Dictionary<int, int>(); // Initialize sparse vector for TF (term frequencies)

            // Calculate term frequencies
            foreach (var token in tokens)
            {
                if (vectorizer.Vocabulary.ContainsKey(token))
                {
                    int index = vectorizer.Vocabulary[token];
                    tf[index] = tf.GetValueOrDefault(index, 0) + 1;
                }
            }
            
            var tfidfFeatures = new float[vectorizer.Vocabulary.Count]; // Initialize dense vector for TF-IDF features

            // Calculate TF-IDF for each term in vocabulary
            foreach (var term in vectorizer.Vocabulary)
            {
                int index = term.Value;
                int termFreq = tf.GetValueOrDefault(index, 0); // Handles terms not in the text
                tfidfFeatures[index] = termFreq * vectorizer.IDF[index];
            }

            var sumOfSquares = tfidfFeatures.Sum(x => x * x);
            var l2Norm = (float)Math.Sqrt(sumOfSquares); // Normalization
            for (int i = 0; i < tfidfFeatures.Length; i++)
                tfidfFeatures[i] /= l2Norm;

            return tfidfFeatures;
        }

        // Predict the text based on the features.
        public List<string> Predict(float[] features)
        {
            // Convert features data to ONNX tensors
            var inputData = new float[1][];
            inputData[0] = features;

            var inputNames = session.InputNames;
            var inputTensors = new List<NamedOnnxValue>();
            var tensor = new DenseTensor<float>(inputData.SelectMany(x => x).ToArray(), new int[] { inputData.Length, inputData[0].Length });

            inputTensors.Add(NamedOnnxValue.CreateFromTensor(inputNames[0], tensor));

            IEnumerable<DisposableNamedOnnxValue> outputs = session.Run(inputTensors);

            // Set the threshold to assign multiple classes.
            float threshold = 0.1F;

            var predictedProbabilities = outputs.Last().AsEnumerable<NamedOnnxValue>().First().AsDictionary<string, float>(); // The probabilities belongs to each class.
            var predictedClass = outputs.First().AsTensor<string>()[0]; // The class with the highest probability.
            var predictedClasses = predictedProbabilities.Where(item => item.Value > threshold).Select(item => item.Key).ToList(); // The classes with the probability greater than threshold value.

            return predictedClasses;
        }

        public List<string> Classify(byte[] docBytes, string docName)
        {
            // Extract the document text from document file.
            string documentText = DocumentTextExtraction(docBytes, docName);

            // Preprocess the document text.
            documentText = PreprocessText(documentText);

            // Convert the preprocessed document text to features.
            var features = Transform(documentText);

            // Make prediction
            var prediction = Predict(features);

            return prediction;
        }
    }
}