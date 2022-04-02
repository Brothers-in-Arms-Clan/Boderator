using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArmaForces.Boderator.Core.Missions.Models;
using Microsoft.EntityFrameworkCore;

namespace ArmaForces.Boderator.Core.Missions.Implementation.Persistence.Query;

internal class SignupsQueryRepository : ISignupsQueryRepository
{
    private readonly MissionContext _context;

    public SignupsQueryRepository(MissionContext context)
    {
        _context = context;
    }

    public async Task<List<Signups>> GetAllSignups()
    {
        return await _context.Signups.ToListAsync();
    }

    public async Task<List<Signups>> GetOpenSignups()
        => await _context.Signups
            .Where(x => x.CloseDate > DateTime.Now)
            .ToListAsync();

    public async Task<Signups?> GetSignups(long signupId)
        => await _context.Signups
            .FindAsync(signupId)
            .AsTask();
}