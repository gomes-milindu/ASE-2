using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WebApplication1.Service.Interface;

namespace WebApplication1.Service.Impl
{
    public class SmsSender : ISmsService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public SmsSender(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        public async Task SendSmsAsync(string toMobile, string smsMessage)
        {
            string gatewayUrl = _configuration["SmsSettings:GatewayUrl"];
            string apiKey = _configuration["SmsSettings:ApiKey"];
            string sender_Id = _configuration["SmsSettings:SenderId"];
            string email = _configuration["SmsSettings:Email"];

            Console.WriteLine("Sender Id " + sender_Id);

            var smsPayload = new
            {
                senderID = sender_Id,
                to = toMobile,
                msg = smsMessage
            };

     

            try
            {
                string jsonPayload = JsonSerializer.Serialize(smsPayload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{email}:{apiKey}"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

                HttpResponseMessage response = await _httpClient.PostAsync(gatewayUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"SMS Gateway responded with error code: {response.StatusCode}");
                }

                
               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🚨 Failed to send SMS via Gateway: {ex.Message}");
                throw;
            }
        }
    }
}
