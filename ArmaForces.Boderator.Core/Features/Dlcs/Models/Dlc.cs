namespace ArmaForces.Boderator.Core.Dlcs.Models;

public class Dlc
{
    /// <summary>
    /// Id of a DLC.
    /// </summary>
    public int DlcId { get; init; }

    /// <summary>
    /// Name of a DLC.
    /// </summary>
    public string Name { get; init; } = string.Empty;
}
