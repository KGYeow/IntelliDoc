import re
import joblib
import onnxruntime as ort
from PyPDF2 import PdfReader
from docx import Document

def clean_text(text):
  text = text.lower()  # Convert to lowercase
  text = re.sub(r'[^\w\s]', '', text)  # Remove punctuation and non-alphanumeric characters
  text = re.sub(r'\s+', ' ', text)  # Remove extra whitespace
  text = re.sub(r'\d+', '', text)  # Remove numbers
  return text

def read_document(file_path, filename):
    # Text extraction based on file extension
    if filename.endswith(".pdf"):
        with open(file_path, 'rb') as pdf_file:
            # Use PyPDF2 for PDF text extraction
            pdf_reader = PdfReader(pdf_file)
            text = ""
            for page in pdf_reader.pages:
                text += page.extract_text()

    elif filename.endswith(".docx"):
        # Use python-docx for Word document text extraction
        doc = Document(file_path)
        text = ""
        for paragraph in doc.paragraphs:
            text += paragraph.text

    elif filename.endswith(".txt"):
        # Read text directly for TXT files
        with open(file_path, 'r', encoding='utf-8') as file:
            text = file.read()
            
    else:
        # Skip unsupported file formats
        return None
    
    # Preprocessing steps (clean text, lowercase, etc.)
    text = clean_text(text) # Implement your cleaning function here
    return text

# Function to get predicted classes exceeding the threshold
def get_predicted_classes(probabilities):
    threshold = 0.1
    # class_labels = ['Business', 'Entertainment', 'Food', 'Graphics', 'Historical', 'Medical', 'Politics', 'Space', 'Sport', 'Technologie']
    # top_classes = [class_labels[i] for i, p in enumerate(probabilities[0]) if p > threshold]
    filtered_keys = [key for key, value in probabilities.items() if value > threshold]
    return filtered_keys

# Get the input document path
input_file_directory = r"D:\Documents\USM\USM_NotesExercises\Year 4 Sem 1\CAT405\dataset\Food\food_1.txt"
input_file_name = input_file_directory.split("\\")[-1]
document_text = read_document(input_file_directory, input_file_name)

# Load the fitted vectorizer from the pickle file
vectorizer_path = r"D:\Documents\USM\USM_NotesExercises\Year 4 Sem 1\CAT405\Logistic Regression Model\TFIDFvectorizer.pkl"
with open(vectorizer_path, "rb") as f:
    vectorizer = joblib.load(f)
    
# Transform the document text using the loaded vectorizer
features = vectorizer.transform([document_text])

# # Load the ONNX model
model_path = r"D:\Documents\USM\USM_NotesExercises\Year 4 Sem 1\CAT405\Logistic Regression Model\DocClassificationLrModel.onnx"
ort_session = ort.InferenceSession(model_path)

# Get the input names from the model
input_name = ort_session.get_inputs()[0].name

# Perform prediction using the model and features
prediction = ort_session.run(None, {input_name: features.toarray().astype('float32')})
predicted_class = prediction[0][0]
predicted_classes = get_predicted_classes(prediction[1][0])
predicted_probabilities = prediction[1][0]

print("=====================================================================")
print("Predicted Class   :", predicted_class)
print("Predicted Classes :", predicted_classes, "\n")
print("~~~~~~~~~~ Predicted Probability ~~~~~~~~~~")
print("Business\t:", predicted_probabilities["Business"])
print("Entertainment\t:", predicted_probabilities["Entertainment"])
print("Food\t\t:", predicted_probabilities["Food"])
print("Graphics\t:", predicted_probabilities["Graphics"])
print("Historical\t:", predicted_probabilities["Historical"])
print("Medical\t\t:", predicted_probabilities["Medical"])
print("Politics\t:", predicted_probabilities["Politics"])
print("Space\t\t:", predicted_probabilities["Space"])
print("Sport\t\t:", predicted_probabilities["Sport"])
print("Technologie\t:", predicted_probabilities["Technologie"])
print("=====================================================================\n")