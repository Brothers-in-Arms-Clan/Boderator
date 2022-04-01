using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArmaForces.Boderator.Core.Missions.Implementation.Persistence.Query
{
    internal interface ISignupsQueryRepository
    {
        public Task<List<Models.Signups>> GetAllSignups();

        public Task<List<Models.Signups>> GetOpenSignups();
        
        public Task<Models.Signups?> GetSignup(long signupId);
    }
}
