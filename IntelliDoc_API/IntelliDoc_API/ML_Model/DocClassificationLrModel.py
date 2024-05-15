#!/usr/bin/env python
# coding: utf-8

# In[12]:


import os
import joblib
import json
import pandas as pd
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.linear_model import LogisticRegression
from sklearn.model_selection import train_test_split
from sklearn.metrics import classification_report, confusion_matrix, accuracy_score


# In[13]:


import re
def clean_text(text):
  text = text.lower()  # Convert to lowercase
  text = re.sub(r'[^\w\s]', '', text)  # Remove punctuation and non-alphanumeric characters
  text = re.sub(r'\s+', ' ', text)  # Remove extra whitespace
  text = re.sub(r'\d+', '', text)  # Remove numbers
  return text 


# In[14]:


from PyPDF2 import PdfReader
from docx import Document
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


# In[15]:


data_dir = r"D:\Documents\USM\USM_NotesExercises\Year 4 Sem 1\CAT405\Dataset" # Replace with your data directory path
documents = []
labels = []
for class_dir in os.listdir(data_dir):
    class_path = os.path.join(data_dir, class_dir)
    
    if os.path.isdir(class_path):
        for filename in os.listdir(class_path):
            file_path = os.path.join(class_path, filename)
            text = read_document(file_path, filename)
            
            # Skip unsupported file formats
            if text == None:
                continue

            documents.append(text)
            labels.append([filename, class_dir])


# In[16]:


# Load the dataset
# Create a DataFrame from the collected data
df = pd.DataFrame(labels, columns=['Doc Name', 'Class'])
df


# In[17]:


# Feature extraction using TF-IDF
vectorizer = TfidfVectorizer(max_features=1000)
X = vectorizer.fit_transform(documents)
y = df['Class']

vectorizer_dict = {
  "Vocabulary": {word: int(value) for word, value in vectorizer.vocabulary_.items()},
  "IDF": vectorizer.idf_.tolist(),
}
with open("TFIDFvectorizer.json", "w") as f:
  json.dump(vectorizer_dict, f)
joblib.dump(vectorizer, 'TFIDFvectorizer.pkl')

# Train-test split
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2)

# Logistic regression model
model = LogisticRegression(multi_class='multinomial', solver='lbfgs', max_iter=800)
trainedModel = model.fit(X_train, y_train)

# dense_features = X_train.toarray()
# model = model.fit(dense_features, y_train)


# In[18]:


# Make predictions on the test set
y_pred = trainedModel.predict(X_test)

# Evaluate the model
print("Accuracy Score:", accuracy_score(y_test, y_pred), "\n")

print("Confusion Matrix:")
print(confusion_matrix(y_test, y_pred), "\n")

print("Classification Report:")
print(classification_report(y_test, y_pred))


# In[19]:


# Use the model for prediction
input_file_directory = r"D:\Documents\USM\USM_NotesExercises\Year 4 Sem 1\CAT405\CAT405_SRD_Report_YeowKokGuan.pdf"
input_file_name = input_file_directory.split("\\")[-1]
new_document = read_document(input_file_directory, input_file_name)
new_features = vectorizer.transform([new_document])
prediction = trainedModel.predict(new_features)
prediction_proba = trainedModel.predict_proba(new_features)

# Function to get predicted classes exceeding the threshold
def get_predicted_classes(probabilities):
    threshold = 0.15
    class_labels = df['Class'].unique()
    top_classes = [class_labels[i] for i, p in enumerate(probabilities[0]) if p > threshold]
    return top_classes

predicted_classes = get_predicted_classes(prediction_proba)

print("=====================================================================")
print("Predicted Class   :", prediction[0])
print("Predicted Classes :", predicted_classes, "\n")
print("~~~~~~~~~~ Predicted Probability ~~~~~~~~~~")
print("Predicted probability:")
print("Academic\t:", prediction_proba[0][0])
print("Administrative\t:", prediction_proba[0][1])
print("Co-curricular\t:", prediction_proba[0][2])
print("Financial\t:", prediction_proba[0][3])
print("Personnel\t:", prediction_proba[0][4])
print("Technical\t:", prediction_proba[0][5])
# print("Politics\t:", prediction_proba[0][6])
# print("Space\t\t:", prediction_proba[0][7])
# print("Sport\t\t:", prediction_proba[0][8])
# print("Technologie\t:", prediction_proba[0][9])
print("=====================================================================\n")


# In[20]:


# import joblib
# joblib.dump(model, 'DocClassificationLrModel.pkl')


# In[21]:


from skl2onnx.common.data_types import FloatTensorType
from skl2onnx import convert_sklearn

# Convert the model to ONNX
# initial_type = [('features', onnx.TensorProto.FLOAT)] # Use onnx.TensorProto.FLOAT32
initial_type = [('features', FloatTensorType([None, None]))]
onx = convert_sklearn(trainedModel, initial_types=initial_type)

# Save the converted model
with open("DocClassificationLrModel.onnx", "wb") as f:
    f.write(onx.SerializeToString())

print("Pipeline conversion complete!")

# pklmodel = joblib.load(r"D:\Documents\USM\USM_NotesExercises\Year 4 Sem 1\CAT405\Logistic Regression Model\DocClassificationLrModel.pkl")
# onnx.save_model(pklmodel, "DocClassificationLrModel.onnx")

