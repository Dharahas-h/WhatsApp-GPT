using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace WhatsApp_GPT.Services
{
    public class OpenAIService
    {
        private readonly HttpClient HttpClient;
        private readonly string ApiKey;

        public OpenAIService(IConfiguration config)
        {
            ApiKey = config["OPENAI_API_KEY"] ?? throw new ArgumentNullException("OPENAI_API_KEY not set");
            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
        }

        public async Task<string?> GetCompletionAsync(string prompt)
        {
            var request = new
            {
                model = "text-davinci-003",
                prompt = prompt,
                max_tokens = 150,
                temperature = 0
            };

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync("https://api.openai.com/v1/completions", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"OpenAI API error: {error}");
                return null;
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonDocument.Parse(responseString);

            return responseObject.RootElement
                .GetProperty("choices")[0]
                .GetProperty("text")
                .GetString();
        }
    }
}
