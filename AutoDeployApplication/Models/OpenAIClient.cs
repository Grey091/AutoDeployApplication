using OpenAI_API;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace AutoDeployApplication.Models
{
    public class OpenAIClient
    {
        private string OPENAI_API_KEY = "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";
        private string OPENAI_API_MODEL = "text-davinci-002-render-sha";
        private int OPENAI_API_MAX_TOKEN = 20;
        private double OPENAI_API_TEMPERATURA = 0.5f;

        private readonly OpenAIAPI openAIAPI;
        private readonly HttpClient _httpClient;

        public OpenAIClient() 
        {
            APIAuthentication auth = new APIAuthentication(OPENAI_API_KEY);
            openAIAPI = new OpenAIAPI(auth);
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", OPENAI_API_KEY);
        }

        public string GetAnswerQuestion(string question) 
        {
            StringBuilder answer = new StringBuilder();
            var completions = openAIAPI.Completions.CreateCompletionAsync(
                prompt: question,
                model: OPENAI_API_MODEL,
                max_tokens: OPENAI_API_MAX_TOKEN,
                temperature: OPENAI_API_TEMPERATURA);

            if (completions.Result == null)
            {
                return "Sorry, I dont have no idea for this";
            }
            foreach (var completion in completions.Result.Completions)
            {
                Console.Write(completion.Text);
                answer.Append(completion.Text + "\n");
            }
            return answer.ToString();
        }

        public async Task<string> SendPrompt(string prompt)
        {
            var requestBody = new
            {
                prompt = prompt,
                model = OPENAI_API_MODEL,
                max_tokens = OPENAI_API_MAX_TOKEN,
                temperature = OPENAI_API_TEMPERATURA
            };

            var response = await _httpClient.PostAsJsonAsync("completions", requestBody);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }
    }
}
