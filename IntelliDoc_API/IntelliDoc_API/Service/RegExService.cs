using DocumentFormat.OpenXml.Packaging;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using System.Text;
using System.Text.RegularExpressions;

namespace IntelliDoc_API.Service
{
    public class RegExService
    {
        public RegExService()
        {
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
            else if (docName.EndsWith(".docx") || docName.EndsWith(".doc"))
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

        public List<string> FindPattern(byte[] docBytes, string docName)
        {
            string docText = DocumentTextExtraction(docBytes, docName);

            List<string> matchedPatterns = new List<string>();
            matchedPatterns = Regex.Matches(docText, @"(LAMPIRAN [A-Z]{1}[0-9]{2})\b").Cast<Match>().Select(m => m.Value).ToList();

            return matchedPatterns;
        }
    }
}