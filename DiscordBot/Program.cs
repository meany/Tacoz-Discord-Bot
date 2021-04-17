﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using dm.TCZ.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NLog;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace dm.TCZ.DiscordBot
{
    public class Program
    {
        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider services;
        private IConfigurationRoot configuration;
        private Config config;
        private AppDbContext db;
        private static Logger log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
            => new Program().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("Config.DiscordBot.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("Config.DiscordBot.Local.json", optional: true, reloadOnChange: true);

                configuration = builder.Build();

                client = new DiscordSocketClient(new DiscordSocketConfig
                {
                    MessageCacheSize = 100
                });
                client.Log += Log;

                services = new ServiceCollection()
                    .Configure<Config>(configuration)
                    .AddDatabase<AppDbContext>(configuration.GetConnectionString("Database"))
                    .BuildServiceProvider();
                config = services.GetService<IOptions<Config>>().Value;
                db = services.GetService<AppDbContext>();
                db.Database.Migrate();

                if (args.Length > 0)
                {
                    await RunArgs(args).ConfigureAwait(false);
                }
                else
                {
                    await RunBot().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async Task RunArgs(string[] args)
        {
            try
            {
                await Start().ConfigureAwait(false);
                var handle = new Args(client, db);
                switch (args[0])
                {
                    case "arg1":
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async Task RunBot()
        {
            try
            {
                commands = new CommandService();

                await Install().ConfigureAwait(false);
                await Start().ConfigureAwait(false);
                await client.SetGameAsync($"{config.BotPrefix}price").ConfigureAwait(false);

                await Task.Delay(-1).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private Task Log(LogMessage msg)
        {
            log.Info(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task Install()
        {
            try
            {
                var events = new Events(commands, client, services, config, db);
                client.Connected += events.HandleConnected;
                client.MessageReceived += events.HandleCommand;
                client.ReactionAdded += events.HandleReaction;
                client.UserJoined += events.HandleJoin;
                client.UserLeft += events.HandleLeft;
                client.UserBanned += events.HandleBanned;
                await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async Task Start()
        {
            try
            {
                await client.LoginAsync(TokenType.Bot, config.BotToken).ConfigureAwait(false);
                await client.StartAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
