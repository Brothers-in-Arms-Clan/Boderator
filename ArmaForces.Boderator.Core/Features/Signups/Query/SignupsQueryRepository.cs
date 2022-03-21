using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Signups.Models;
using Microsoft.EntityFrameworkCore;

namespace ArmaForces.Boderator.Core.Signups.Query
{
    internal class SignupsQueryRepository : ISignupsQueryRepository
    {
        private readonly SignupsContext _context;

        public SignupsQueryRepository(SignupsContext context)
        {
            _context = context;
        }

        public async Task<List<Signup>> GetAllSignups()
        {
            return await _context.Signups.ToListAsync();
        }

        public async Task<List<Signup>> GetOpenSignups()
            => await _context.Signups
                .Where(x => x.CloseDate > DateTime.Now)
                .ToListAsync();

        public async Task<Signup?> GetSignup(int signupId)
            => await _context.Signups
                .FindAsync(signupId)
                .AsTask();
    }
}
