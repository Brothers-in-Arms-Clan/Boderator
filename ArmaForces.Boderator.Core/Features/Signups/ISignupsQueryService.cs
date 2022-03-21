using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Signups.Models;
using CSharpFunctionalExtensions;

namespace ArmaForces.Boderator.Core.Signups
{
    public interface ISignupsQueryService
    {
        Task<Result<Signup>> GetSignup(int signupId);
        
        Task<Result<List<Signup>>> GetOpenSignups();
    }
}
