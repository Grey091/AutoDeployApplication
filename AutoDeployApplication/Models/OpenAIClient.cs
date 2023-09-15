using AutoDeployApplication.Resources;
using OpenAI_API;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace AutoDeployApplication.Models
{
    public class OpenAIClient
    {
        private readonly OpenAIAPI openAIAPI;
        private readonly HttpClient _httpClient;

        public OpenAIClient() 
        {
            APIAuthentication auth = new APIAuthentication(OpenAIClientSettings.OPENAI_API_KEY);
            openAIAPI = new OpenAIAPI(auth);
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(OpenAIClientSettings.OPENAI_API_URI);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(OpenAIClientSettings.BEARER, OpenAIClientSettings.OPENAI_API_KEY);
        }

        public string GetAnswerQuestion(string question) 
        {
            StringBuilder answer = new StringBuilder();
            var completions = openAIAPI.Completions.CreateCompletionAsync(
                prompt: question,
                model: OpenAIClientSettings.OPENAI_API_MODEL,
                max_tokens: OpenAIClientSettings.OPENAI_API_MAX_TOKEN,
                temperature: OpenAIClientSettings.OPENAI_API_TEMPERATURA);

            if (completions.Result == null)
            {
                return MessageConstants.DEFAULT_ANSWER;
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
                model = OpenAIClientSettings.OPENAI_API_MODEL,
                max_tokens = OpenAIClientSettings.OPENAI_API_MAX_TOKEN,
                temperature = OpenAIClientSettings.OPENAI_API_TEMPERATURA
            };

            var response = await _httpClient.PostAsJsonAsync(OpenAIClientSettings.COMPLETIONS, requestBody);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }
    }
}
