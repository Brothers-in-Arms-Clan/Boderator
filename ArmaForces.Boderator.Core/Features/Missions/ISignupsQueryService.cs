using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions.Models;
using CSharpFunctionalExtensions;

namespace ArmaForces.Boderator.Core.Missions;

public interface ISignupsQueryService
{
    Task<Result<Signups>> GetSignup(long signupId);
        
    Task<Result<List<Signups>>> GetOpenSignups();
}