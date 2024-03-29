using IntelliDoc_API.Dto.Classification;
using IntelliDoc_API.Models;
using IntelliDoc_API.Service;
using Microsoft.AspNetCore.Mvc;

namespace IntelliDoc_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentClassificationController : BaseController
    {
        protected readonly ModelService modelService;

        public DocumentClassificationController(IConfiguration configuration, UserService userService, IntelliDocDBContext context, ModelService modelService) : base(configuration, userService, context)
        {
            this.modelService = modelService;
        }

        // Classify the document.
        [HttpGet]
        [Route("")]
        public IActionResult PredictDocumentClasses()
        {
            string DocPath = "D:\\Documents\\USM\\USM_NotesExercises\\Year 4 Sem 1\\CAT405\\dataset\\Food\\food_1.txt";
            string DocName = "food_1.txt";
            // D:\Documents\USM\USM_NotesExercises\Year 4 Sem 1\CAT405\dataset\Food\food_1.txt
            try
            {
                string documentText = modelService.DocumentTextExtraction(DocPath, DocName);
                documentText = modelService.PreprocessText(documentText);

                // Convert text to a suitable format for the model
                var features = modelService.ConvertTextToFeatures(documentText);

                // Extract predicted class and probabilities (modify based on your IronPython function output)
                /*                var predictedClass = prediction["class"];
                                var probabilities = prediction["probabilities"];*/

                // return Ok(new { predictedClass, probabilities });
                // return Ok(features);

                // Make prediction
                modelService.Predict(features);
                // var prediction = modelService.Predict(features);
                // var predictionProba = modelService.PredictProbability(new double[][] { features });

                return Ok(features);
                // return Ok(new { predictedClass = prediction, predictedProbability = predictionProba });

                // Convert input data to the format expected by the model (double[][])
                // var doubleInputs = inputs.Select(x => x.ToDoubleArray()).ToArray();
                // var predictions = modelService.Predict(doubleInputs);
                // return Ok(predictions);
            }
            catch (Exception ex)    
            {
                throw new Exception(ex.Message);
            }
        }
    }
}