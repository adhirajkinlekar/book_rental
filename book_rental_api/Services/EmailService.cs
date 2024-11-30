using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class EmailService
{
    private readonly string _sendGridApiKey;

    public EmailService(IConfiguration configuration)
    {
        _sendGridApiKey = "Add your sendgrip api key here";  // Running out of time! Otherwise in real world application I would have safely stored the api key
    }

    public async Task SendEmailAsync(string toEmail, string subject, string plainTextContent, string htmlContent)
    {
        var client = new SendGridClient(_sendGridApiKey);

        var from = new EmailAddress("Add your email id here", "Adhiraj Kinlekar");

        var to = new EmailAddress(toEmail);

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        try
        {
            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                var body = await response.Body.ReadAsStringAsync();

                throw new Exception($"Error sending email: {response.StatusCode} - {body}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error sending email: {ex.Message}", ex);
        }
    }
}
