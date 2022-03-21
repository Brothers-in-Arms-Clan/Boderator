using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Signups.Models;

namespace ArmaForces.Boderator.Core.Signups.Query
{
    internal interface ISignupsQueryService
    {
        Task<List<Signup>> GetOpenSignups();
    }
}
