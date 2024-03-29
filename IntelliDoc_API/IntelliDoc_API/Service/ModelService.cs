using DocumentFormat.OpenXml.Packaging;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Data;
using Accord.Math;
using Accord.MachineLearning;

namespace IntelliDoc_API.Service
{
    public class ModelService
    {
        protected readonly InferenceSession session;

        public ModelService() // Inject model path in constructor
        {
            string modelPath = "D:\\Documents\\USM\\USM_NotesExercises\\Year 4 Sem 1\\CAT405\\Logistic Regression Model\\DocClassificationLrModel.onnx";
            session = new InferenceSession(modelPath);
        }

/*        public double[] PredictProbability(float[][] features)
        {
            // Similar logic as Predict, but access probabilities from output
            var inputData = new float[features.Length][];
            for (int i = 0; i < features.Length; i++)
                inputData[i] = features[i].Cast<float>().ToArray();

            var inputNames = session.InputNames;
            var inputTensors = new List<NamedOnnxValue>();
            foreach (var name in inputNames)
            {
                inputTensors.Add(NamedOnnxValue.CreateFromTensor(name, new Tensor<float>(inputData)));
            }

            IEnumerable<NamedOnnxValue> outputs = session.Run(inputTensors.ToArray());
            var output = outputs.FirstOrDefault();
            var outputData = output.Value.AsTensor<float>();

            // Assuming probabilities are the first output
            return outputData.Data;
        }*/

        // Helper method to get class label based on prediction index (optional)
        private string GetLabel(int predictionIndex)
        {
            // Implement logic to retrieve the class label based on the predicted index
            // This might involve using a lookup table or the model's metadata
            throw new NotImplementedException("GetLabel not implemented");
        }

        // Extract the document text.
        public string DocumentTextExtraction(string docPath, string docName)
        {
            if (System.IO.File.Exists(docPath))
            {
                string text;
                if (docName.EndsWith(".pdf"))
                {
                    var pageText = new StringBuilder();
                    var document = new PdfDocument(new PdfReader(docPath)); // Read PDF using new PdfDocument & new PdfReader
                    var pageNum = document.GetNumberOfPages();
                    for (int page = 1; page <= pageNum; page++)
                    {
                        // New LocationTextExtractionStrategy creates a new text extraction renderer
                        var strategy = new LocationTextExtractionStrategy();
                        var parser = new PdfCanvasProcessor(strategy);
                        parser.ProcessPageContent(document.GetFirstPage());
                        pageText.Append(strategy.GetResultantText());
                    }
                    text = pageText.ToString();
                }
                else if (docName.EndsWith(".docx"))
                {
                    // Use Open XML SDK for Word document text extraction
                    var document = WordprocessingDocument.Open(docPath, false);
                    DocumentFormat.OpenXml.Wordprocessing.Body body = document.MainDocumentPart.Document.Body;
                    text = body.InnerText;
                }
                else if (docName.EndsWith(".txt"))
                {
                    // Read text directly for TXT files
                    var reader = new StreamReader(docPath, Encoding.UTF8);
                    text = reader.ReadToEnd();
                }
                else
                    throw new Exception($"Unsupported file format: {docName}");
                return text;
            }
            else
                throw new Exception($"File not found: {docPath}");
        }

        // Preprocess the document text.
        public string PreprocessText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text; // Handle empty or null text (optional)
            text = text.ToLowerInvariant(); // Convert to lowercase
            text = Regex.Replace(text, "[^\\w\\s]", ""); // Remove punctuation and non-alphanumeric characters
            text = Regex.Replace(text, @"\s+", " "); // Remove extra whitespace
            return text;
        }

        // Convert the preprocessed text to features.
        public float[][] ConvertTextToFeatures(string text)
        {
/*
            var tfidfVectorizer = new TFIDF();
            var tfidfFeatures = tfidfVectorizer.Transform(new[] { text }.Tokenize());
            Console.WriteLine("=====================================================================");
            Console.WriteLine("tfidfFeatures");
            Console.WriteLine(tfidfFeatures.Rows());
            Console.WriteLine(tfidfFeatures.Columns());
            Console.WriteLine(tfidfFeatures[0][0]);
            Console.WriteLine("=====================================================================");
*/
            var mlContext = new MLContext();

            // Create an IDataView from a single string
            // var dataView = mlContext.Data.LoadFromEnumerable(new[] { extractedText }, SchemaDefinition.Create(typeof(string)));
            var dataView = mlContext.Data.LoadFromEnumerable(new[] { new { ExtractedText = text } });
/*
            Console.WriteLine("=====================================================================");
            Console.WriteLine(dataView.Preview());
            Console.WriteLine(dataView.Preview().Schema);
            Console.WriteLine(dataView.Preview().RowView.ToList().Count);
            Console.WriteLine(dataView.Preview().ColumnView.ToList().Count);
            Console.WriteLine("=====================================================================");
            foreach (var col in dataView.Preview().ColumnView)
            {
                Console.WriteLine(col.Column + "\t");
            }
            Console.WriteLine("=====================================================================");
            foreach (var row in dataView.Preview().RowView)
            {
                foreach (var col in row.Values)
                {
                    Console.WriteLine($"({col.Key}, {col.Value})\t");
                }
            }
            Console.WriteLine("=====================================================================");
*/
            // Text featurization pipeline
            var textFeaturizer = mlContext.Transforms.Text.FeaturizeText(
                outputColumnName: "Features",
                inputColumnName: "ExtractedText"
            );

            // Transform the data and extract features
            var transformedData = textFeaturizer.Fit(dataView).Transform(dataView);
            Console.WriteLine("=====================================================================");
            Console.WriteLine(transformedData.Preview());
            Console.WriteLine(transformedData.Preview().Schema);
            Console.WriteLine(transformedData.Preview().RowView.ToList().Count);
            Console.WriteLine(transformedData.Preview().ColumnView.ToList().Count);
            Console.WriteLine("=====================================================================");
            foreach (var col in transformedData.Preview().ColumnView)
            {
                Console.WriteLine(col.Column + "\t");
            }
            Console.WriteLine("=====================================================================");
            foreach (var row in transformedData.Preview().RowView)
            {
                foreach (var col in row.Values)
                {
                    Console.WriteLine($"({col.Key}, {col.Value})\t");
                }
            }
            Console.WriteLine("=====================================================================");
            var features = transformedData.GetColumn<float[]>("Features").Select(x => x).ToArray();
            // var features = mlContext.Data.CreateEnumerable<float[]>(transformedData, true).ToList().Select(row => row).ToArray();
            Console.WriteLine(features.Rows());
            Console.WriteLine(features.Columns());
/*            for (int i = 0; i < features.Columns(); i++)
            {
                Console.WriteLine($"({i}, {features.GetColumn(i)})" + "\t");
            }*/
            Console.WriteLine("=====================================================================");
            return features;
        }

        public void Predict(float[][] features)
        {
            // Convert features data to ONNX tensors (assuming single data point)
            var inputData = new float[1][];
            // inputData[0] = features[0].Cast<float>().ToArray();
            inputData[0] = features[0];
            Console.WriteLine("=====================================================================");
            Console.WriteLine(inputData.Columns());
            Console.WriteLine("=====================================================================");
            var inputNames = session.InputNames;
            var inputTensors = new List<NamedOnnxValue>();
            Console.WriteLine("=====================================================================");
            Console.WriteLine("Input Names: ");
            Console.WriteLine(inputNames);
            Console.WriteLine("=====================================================================");
            foreach (var name in inputNames)
            {
                var tensor = new DenseTensor<float>(inputData.SelectMany(x => x).ToArray(), new int[] { inputData.Length, inputData[0].Length });
                inputTensors.Add(NamedOnnxValue.CreateFromTensor(name, tensor));
            }

            Console.WriteLine("=====================================================================");
            Console.WriteLine("Input Tensors: ");
            Console.WriteLine(inputTensors.Capacity);
            Console.WriteLine("=====================================================================");
            // Run inference
            IEnumerable<NamedOnnxValue> outputs = session.Run(inputTensors.ToArray());

            // Get the predicted class index (assuming single output)
            var output = outputs.FirstOrDefault();
            var outputData = output.AsTensor<float>();
            Console.WriteLine("=====================================================================");
            Console.WriteLine("Testing!!!!");
            Console.WriteLine(outputs.Count());
            Console.WriteLine(outputs.ToList());
            Console.WriteLine("=====================================================================");
            // int predictedClassIndex = Array.IndexOf(outputData.Cast<float>().ToArray(), outputData.Max());

            // return new[] { predictedClassIndex };  // Return array with single prediction
        }
    }
}