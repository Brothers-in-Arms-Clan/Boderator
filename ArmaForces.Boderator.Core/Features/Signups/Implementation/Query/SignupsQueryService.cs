using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Signups.Models;
using CSharpFunctionalExtensions;

namespace ArmaForces.Boderator.Core.Signups.Implementation.Query
{
    internal class SignupsQueryService : ISignupsQueryService
    {
        private readonly ISignupsQueryRepository _signupsQueryRepository;

        public SignupsQueryService(ISignupsQueryRepository signupsQueryRepository)
        {
            _signupsQueryRepository = signupsQueryRepository;
        }

        public async Task<Result<Signup>> GetSignup(int signupId)
            => await _signupsQueryRepository.GetSignup(signupId)
               ?? Result.Failure<Signup>($"Signup with ID {signupId} not found");

        public async Task<Result<List<Signup>>> GetOpenSignups()
            => await _signupsQueryRepository.GetOpenSignups();
    }
}
