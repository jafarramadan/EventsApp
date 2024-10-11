using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;

namespace EventsApp.Repository.Services
{
    public class MailjetService
    {
        private readonly IConfiguration _configuration;
        private readonly MailjetClient _client;

        public MailjetService(IConfiguration configuration, MailjetClient client)
        {
            _configuration = configuration;
            _client = new MailjetClient(
                _configuration["Mailjet:ApiKey"],
                _configuration["Mailjet:SecretKey"]
                );

        }
        public async Task<bool> SendEmail(string ParticipantName, string Email, string htmlpart)
        {
            var request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
            .Property(Send.FromEmail, "jafar.n3n3@gmail.com")
            .Property(Send.FromName, "Jafar")
            .Property(Send.Subject, "Event Registiration")
            .Property(Send.HtmlPart, htmlpart)
            .Property(Send.Recipients, new JArray
            {
                new JObject
                {
                    {"Email", Email},
                   {"ParticipantName", ParticipantName}


                }
            });

            MailjetResponse respons = await _client.PostAsync(request);
            return respons.IsSuccessStatusCode;

        }
    }
}
