using System;
using System.Net.Http;
using System.Threading.Tasks;
using ArmaForces.Boderator.BotService.Tests.TestCollections;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace ArmaForces.Boderator.BotService.Tests
{
    [Collection(Collections.ApiTest)]
    public abstract class ApiTestBase
    {
        private readonly HttpClient _httpClient;

        protected IServiceProvider Provider { get; }

        protected ApiTestBase(TestApiServiceFixture testApi)
        {
            _httpClient = testApi.HttpClient;

            Provider = new ServiceCollection()
                .BuildServiceProvider();
        }

        protected async Task<Result<T>> HttpGetAsync<T>(string path)
        {
            var httpResponseMessage = await _httpClient.GetAsync(path);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }

            var responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            var error = string.IsNullOrWhiteSpace(responseBody)
                ? httpResponseMessage.ReasonPhrase
                : responseBody;

            return Result.Failure<T>(error);
        }

        protected async Task<Result> HttpPostAsync<T>(string path, T body)
        {
            var httpResponseMessage = await _httpClient.PostAsync(path, new StringContent(JsonConvert.SerializeObject(body)));
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return Result.Success();
            }

            var responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            var error = string.IsNullOrWhiteSpace(responseBody)
                ? httpResponseMessage.ReasonPhrase
                : responseBody;

            return Result.Failure<T>(error);
        }
    }
}
