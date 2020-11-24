using CSharpFunctionalExtensions;
using RestSharp;

namespace ArmaforcesMissionBot.Features.ServerManager
{
    public abstract class ManagerClientBase
    {
        protected string ManagerUrl { get; }

        protected ManagerClientBase(string managerUrl)
        {
            ManagerUrl = managerUrl;
        }

        protected static Result<T> ReturnFailureFromResponse<T>(IRestResponse restResponse)
        {
            return restResponse.ErrorMessage is null && restResponse.ErrorException is null
                ? Result.Failure<T>($"{restResponse.StatusCode}: {restResponse.Content}")
                : Result.Failure<T>($"{restResponse.StatusCode}: {restResponse.ErrorMessage} | Exception: {restResponse.ErrorException}");
        }
    }
}
