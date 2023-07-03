using System.Net.Mail;

namespace Gifts_Store.Services
{
    public static class EmailService
    {
        public static void SendEmailFromGiftShop(string body, string subject, string toEmail, string pdfBody, string attachmentFileName)
        {
            string fromEmail = "giftshopauto@outlook.com";
            string fromPassword = "Q2W3e4r5@";
            SendEmailWithPDF(body, subject, toEmail, fromEmail, fromPassword, pdfBody, attachmentFileName);
        }

        public static void SendEmailFromGiftShop(string body, string subject, string toEmail)
        {
            string fromEmail = "giftshopauto@outlook.com";
            string fromPassword = "Q2W3e4r5@";
            SendEmail(body, subject, toEmail, fromEmail, fromPassword);
        }
        public static void SendEmail(string body, string subject, string toEmail, string fromEmail, string fromPassword)
        {
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(fromEmail);
            mailMessage.To.Add(new MailAddress(toEmail));
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = body;

            SmtpClient client = new SmtpClient("smtp-mail.outlook.com");
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(fromEmail, fromPassword);
            client.Port = 587;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            client.Send(mailMessage);
        }

        public static void SendEmailWithPDF(string body, string subject, string toEmail, string fromEmail, string fromPassword, string pdfBody, string attachmentFileName)
        {
            // Create an instance of the PDFGenerator class
            PDFGeneratorService pdfGenerator = new PDFGeneratorService();

            // Generate the PDF byte array
            byte[] pdfBytes = pdfGenerator.GeneratePDF(pdfBody);

            // Create a memory stream for the PDF byte array
            MemoryStream pdfStream = new MemoryStream(pdfBytes);

            // Create the PDF attachment
            Attachment pdfAttachment = new Attachment(pdfStream, attachmentFileName, "application/pdf");

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(fromEmail);
            mailMessage.To.Add(new MailAddress(toEmail));
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = body;
            mailMessage.Attachments.Add(pdfAttachment);

            SmtpClient client = new SmtpClient("smtp-mail.outlook.com");
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(fromEmail, fromPassword);
            client.Port = 587;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            client.Send(mailMessage);
        }
    }
}
