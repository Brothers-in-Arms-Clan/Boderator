using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Signups.Models;

namespace ArmaForces.Boderator.Core.Signups.Query
{
    internal class SignupsQueryService : ISignupsQueryService
    {
        private readonly ISignupsQueryRepository _signupsQueryRepository;

        public SignupsQueryService(ISignupsQueryRepository signupsQueryRepository)
        {
            _signupsQueryRepository = signupsQueryRepository;
        }

        public async Task<List<Signup>> GetOpenSignups()
            => await _signupsQueryRepository.GetOpenSignups();
    }
}
