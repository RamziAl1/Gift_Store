using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Net.Mail;
using static iTextSharp.text.pdf.PdfWriter;

namespace Gifts_Store.Services
{
    public class PDFGeneratorService
    {
        public byte[] GeneratePDF(string data)
        {
            byte[] pdfBytes;

            using (MemoryStream ms = new MemoryStream())
            {
                // Create a new PDF document
                Document document = new Document();

                // Create a PDF writer to write the document to the memory stream
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                // Open the document
                document.Open();

                // Add content to the document
                document.Add(new Paragraph(data));

                // Close the document
                document.Close();

                // Get the PDF byte array from the memory stream
                pdfBytes = ms.ToArray();
            }

            return pdfBytes;
        }
    }
}
