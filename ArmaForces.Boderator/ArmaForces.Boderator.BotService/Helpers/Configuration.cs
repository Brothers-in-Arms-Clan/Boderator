﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ArmaForces.Boderator.BotService.Helpers
{
    public static class Configuration
    {
        public static string DiscordToken => GetParameter("DISCORD_TOKEN");

        private static IDictionary Parameters { get; }

        static Configuration()
        {
            Parameters = Environment.GetEnvironmentVariables();
        }

        public static string GetParameter(string key) => (string)Parameters[key];
    }
}
