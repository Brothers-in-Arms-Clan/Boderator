namespace ArmaForces.Boderator.Core.Missions.Models;

public enum SignupsStatus : ushort
{
    /// <summary>
    /// Signups are created and waiting for opening.
    /// </summary>
    Created,
    
    /// <summary>
    /// Signups are open for some selected players.
    /// </summary>
    Preconcrete,
    
    /// <summary>
    /// Signups are open for all players.
    /// </summary>
    Open,
    
    /// <summary>
    /// Signups are closed.
    /// </summary>
    Closed
}
