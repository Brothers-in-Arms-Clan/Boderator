using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Signups.Models;

namespace ArmaForces.Boderator.Core.Signups.Query
{
    internal interface ISignupsQueryRepository
    {
        public Task<List<Signup>> GetAllSignups();

        public Task<List<Signup>> GetOpenSignups();
    }
}
