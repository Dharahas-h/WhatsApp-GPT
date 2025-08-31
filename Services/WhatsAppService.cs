using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace WhatsApp_GPT.Services
{
    public class WhatsAppService
    {
        private readonly string Token;
        private readonly HttpClient HttpClient;
        private readonly string GraphUrl = "https://graph.facebook.com/v15.0/";

        public WhatsAppService(IConfiguration config)
        {
            Token = config["WhatsAppToken"] ?? throw new ArgumentNullException("WhatsAppToken not set");
            HttpClient = new HttpClient();
        }

        public async Task<bool> SendMessageAsync(string phoneNoId, string to, string body)
        {
            var waMessage = new
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to,
                type = "text",
                text = new { preview_url = false, body }
            };

            var waContent = new StringContent(JsonSerializer.Serialize(waMessage), Encoding.UTF8, "application/json");
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            var response = await HttpClient.PostAsync($"{GraphUrl}{phoneNoId}/messages", waContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"✅ Sent a WhatsApp message to {to}");
                return true;
            }

            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"WhatsApp API error: {error}");
            return false;
        }
    }
}
