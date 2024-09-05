using MHealth.Repositories.Abstract;
using System.Net;
using System.Net.Mail;

/**
 * This Repository stores all email services
 *
 * @author Jehezkiel Hardwin Tandijaya
 */
namespace MHealth.Repositories.Implementation
{
    public class EmailRepository : IEmailRepository
    {
        private readonly IConfiguration _configuration;

        /**
        * Default constructor which creates the object of the Email Repository.
        *
        * @param    _configuration          A configuration
        */
        public EmailRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /**
        * SendEmail service is to send email to the user.
        *
        * @param    fromEmail        A String
        * @param    toEmail          A String
        * @param    subject          A String
        * @param    message          A String
        * @param    attachmentPath   A String
        */
        public async Task SendEmail(string fromEmail, string toEmail, string subject, string message, string attachmentPath = null)
        {

            var mail = _configuration["EmailSMTPService"];
            var pw = _configuration["EmailPasswordSMTPService"];


            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.Credentials = new NetworkCredential(mail, pw);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true, // You can set this to false if you're sending plain text emails
                    To = { new MailAddress(toEmail) }
                };

                if (!string.IsNullOrEmpty(attachmentPath))
                {
                    // Attach the file if attachmentPath is provided
                    var attachment = new Attachment(attachmentPath);
                    mailMessage.Attachments.Add(attachment);
                }


                try
                {
                    // Send the email asynchronously
                    await client.SendMailAsync(mailMessage);
                    Console.WriteLine("Email sent successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
                finally
                {
                    if (!string.IsNullOrEmpty(attachmentPath))
                    {
                        mailMessage.Attachments[0].Dispose();
                    }
                    mailMessage.Dispose();
                }
            }
        }
    }
}
