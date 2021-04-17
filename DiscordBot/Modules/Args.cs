﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using dm.TCZ.Data;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace dm.TCZ.DiscordBot
{
    public class Args
    {
        private readonly DiscordSocketClient discordClient;
        private readonly AppDbContext db;
        private static Logger log = LogManager.GetCurrentClassLogger();

        public Args(DiscordSocketClient discordClient,AppDbContext db)
        {
            this.discordClient = discordClient;
            this.db = db;
        }
    }
}