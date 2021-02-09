using System.Net;
using CSharpFunctionalExtensions;
using RestSharp;

namespace ArmaforcesMissionBot.Extensions
{
    internal static class RestClientExtensions
    {
        /// <summary>
        /// Executes REST request and converts response content to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Expected response content.</typeparam>
        /// <param name="restClient"><seealso cref="IRestClient"/> used.</param>
        /// <param name="request">Request to execute.</param>
        /// <returns><see cref="Result{T}"/> success or failure depending on response status code.</returns>
        public static Result<T> ExecuteAndReturnData<T>(this IRestClient restClient, IRestRequest request) where T : new()
        {
            var response = restClient.Execute<T>(request);
            return response.StatusCode == HttpStatusCode.OK
                ? Result.Success(response.Data)
                : Result.Failure<T>(response.StatusCode.ToString());
        }
    }
}
