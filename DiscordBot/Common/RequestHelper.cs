﻿using Discord.Commands;
using dm.TCZ.Data;
using dm.TCZ.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dm.TCZ.DiscordBot
{
    public class RequestResult
    {
        public Request Request { get; set; }
        public bool AlreadyWarned { get; set; }
        public double RequestCooldownSeconds { get; set; }
    }

    public static class RequestHelper
    {
        public static async Task<RequestResult> CreateAndCheckRateLimit(AppDbContext db,
            SocketCommandContext context, int seconds)
        {
            // TODO: comman granularity

            var user = context.User;
            string message = context.Message.ToString();
            bool rateLimited = false;
            bool alreadyWarned = false;
            var now = DateTime.Now;
            var span = new TimeSpan();
            var lastReq = await db.Requests
                .AsNoTracking()
                .OrderByDescending(x => x.Date)
                .FirstOrDefaultAsync(x => x.DiscordUserId == user.Id &&
                    //x.Command == message &&
                    x.IsRateLimited == false)
                .ConfigureAwait(false);

            if (lastReq != null)
            {
                span = lastReq.Date.AddSeconds(seconds) - now;
                rateLimited = span.TotalSeconds > 0;

                var warnReq = await db.Requests
                    .AsNoTracking()
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefaultAsync(x => x.DiscordUserId == user.Id &&
                        //x.Command == message &&
                        x.IsRateLimited == true)
                    .ConfigureAwait(false);
                
                if (warnReq != null)
                    alreadyWarned = warnReq.Date > lastReq.Date;
            }

            return new RequestResult
            {
                Request = new Request
                {
                    Command = context.Message.ToString(),
                    Date = now,
                    DiscordUserId = user.Id,
                    DiscordUserName = user.Username,
                    IsRateLimited = rateLimited
                },
                AlreadyWarned = alreadyWarned,
                RequestCooldownSeconds = span.TotalSeconds
            };
        }

        public static async Task Save(AppDbContext db, RequestResult res)
        {
            db.Requests.Add(res.Request);
            await db.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
