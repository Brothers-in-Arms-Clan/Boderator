using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions.Models;

namespace ArmaForces.Boderator.Core.Missions.Implementation.Persistence.Query;

internal interface ISignupsQueryRepository
{
    public Task<List<Signups>> GetAllSignups();

    public Task<List<Signups>> GetOpenSignups();
        
    public Task<Signups?> GetSignups(long signupId);
}