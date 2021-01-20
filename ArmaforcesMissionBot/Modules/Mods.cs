using System;
using System.Threading.Tasks;
using ArmaForces.ArmaServerManager.Discord.Features.Mods;
using ArmaforcesMissionBot.Attributes;
using Discord.Commands;

namespace ArmaforcesMissionBot.Modules
{
    [Name("ArmaServerManager - Mods")]
    public class Mods : ModsModule
    {
        public Mods(IModsManagerClient modsManagerClient) : base(modsManagerClient)
        {
        }

        [Summary("Pozwala zaplanować aktualizację wszystkich modyfikacji. Np. AF!updateMods 2020-07-17T19:00.")]
        [ContextDMOrChannel]
        public override Task UpdateMods(DateTime? scheduleAt = null) => base.UpdateMods(scheduleAt);

        [Summary("Pozwala zaplanować aktualizację wybranego modsetu. Np. AF!updateMods default 2020-07-17T19:00.")]
        [ContextDMOrChannel]
        public override Task UpdateMods(string modsetName = null, DateTime? scheduleAt = null) => base.UpdateMods(modsetName, scheduleAt);
    }
}
