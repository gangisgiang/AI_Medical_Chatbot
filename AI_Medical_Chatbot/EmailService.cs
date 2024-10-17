using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace AI_Medical_Chatbot
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _senderEmail = "medicalaaichatbot217@gmail.com";
        private readonly string _senderPassword = "yijmfiowyssqqivq";

        public EmailService()
        {
        }

        // public void SendEmail(string recipientEmail, string subject, string body)
        // {
        //     try
        //     {
        //         var smtpClient = new SmtpClient(_smtpServer)
        //         {
        //             Port = _smtpPort,
        //             Credentials = new NetworkCredential(_senderEmail, _senderPassword),
        //             EnableSsl = true,
        //         };

        //         var mailMessage = new MailMessage
        //         {
        //             From = new MailAddress(_senderEmail),
        //             Subject = subject,
        //             Body = body,
        //             IsBodyHtml = false, // Set to true if sending HTML emails
        //         };

        //         mailMessage.To.Add(recipientEmail);
        //         smtpClient.Send(mailMessage);

        //         Console.WriteLine($"Email sent to {recipientEmail}");
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Failed to send email. Error: {ex.Message}");
        //     }
        // }
        public void SendEmail(string recipientEmail, string subject, string body)
{
    SmtpClient smtpClient = null;
    MailMessage mailMessage = null;

    try
    {
        Console.WriteLine("Initializing SMTP client...");
        
        // Set up the SMTP client with Gmail configuration
        smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587, // TLS port for Gmail
            Credentials = new NetworkCredential("medicalaaichatbot217@gmail.com", "yijmfiowyssqqivq"),
            EnableSsl = true,
        };

        Console.WriteLine("SMTP client initialized.");

        // Create a new email message
        mailMessage = new MailMessage
        {
            From = new MailAddress("medicalaaichatbot217@gmail.com"),
            Subject = subject,
            Body = body,
            IsBodyHtml = false, // Set to true if sending HTML emails
        };

        Console.WriteLine("Preparing email message...");

        // Add recipient email
        mailMessage.To.Add(recipientEmail);

        Console.WriteLine($"Attempting to send email to {recipientEmail}...");

        // Send the email
        smtpClient.Send(mailMessage);

        // Inform about the success
        Console.WriteLine($"Email successfully sent to {recipientEmail}");
    }
    catch (SmtpFailedRecipientsException ex)
    {
        foreach (SmtpFailedRecipientException t in ex.InnerExceptions)
        {
            var status = t.StatusCode;
            if (status == SmtpStatusCode.MailboxBusy || status == SmtpStatusCode.MailboxUnavailable)
            {
                Console.WriteLine("Delivery failed - retrying in 5 seconds.");
                System.Threading.Thread.Sleep(5000);
                smtpClient.Send(mailMessage); // Retry sending
            }
            else
            {
                Console.WriteLine($"Failed to deliver message to {t.FailedRecipient}: {t.Message}");
            }
        }
    }
    catch (SmtpException smtpEx)
    {
        Console.WriteLine($"SMTP Error: {smtpEx.Message}");
        Console.WriteLine($"Status Code: {smtpEx.StatusCode}");
        Console.WriteLine($"Error Details: {smtpEx.InnerException}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"General Error: {ex.Message}");
    }
    finally
    {
        // Dispose resources
        smtpClient?.Dispose();
        mailMessage?.Dispose();
    }
}

    }
}