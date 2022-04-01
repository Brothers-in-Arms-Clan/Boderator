using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions.Implementation.Persistence;
using ArmaForces.Boderator.Core.Missions.Implementation.Persistence.Query;
using CSharpFunctionalExtensions;

namespace ArmaForces.Boderator.Core.Missions.Implementation
{
    internal class SignupsQueryService : ISignupsQueryService
    {
        private readonly ISignupsQueryRepository _signupsQueryRepository;

        public SignupsQueryService(ISignupsQueryRepository signupsQueryRepository)
        {
            _signupsQueryRepository = signupsQueryRepository;
        }

        public async Task<Result<Models.Signups>> GetSignup(long signupId)
            => await _signupsQueryRepository.GetSignup(signupId)
               ?? Result.Failure<Models.Signups>($"Signup with ID {signupId} not found");

        public async Task<Result<List<Models.Signups>>> GetOpenSignups()
            => await _signupsQueryRepository.GetOpenSignups();
    }
}
