using System.Collections.Generic;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Signups.Models;

namespace ArmaForces.Boderator.Core.Signups
{
    public interface ISignupsQueryService
    {
        Task<Signup> GetSignup(int signupId);
        
        Task<List<Signup>> GetOpenSignups();
    }
}
