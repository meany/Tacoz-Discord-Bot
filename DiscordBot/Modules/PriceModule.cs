using Discord;
using Discord.Commands;
using Discord.WebSocket;
using dm.TCZ.DiscordBot;
using dm.TCZ.Data;
using dm.TCZ.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace dm.TCZ.DiscordBot.Modules
{
    [Name("1. Prices & Stats")]
    public class PriceModule : ModuleBase<SocketCommandContext>
    {
        private readonly Config config;
        private readonly AppDbContext db;
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public PriceModule(IOptions<Config> config, AppDbContext db)
        {
            this.config = config.Value;
            this.db = db;
        }

        [Command("price", RunMode = RunMode.Async)]
        [Summary("Displays the prices and statistics of Tacoz (TCZ).")]
        [Remarks("")]
        [Alias("p", "prices", "stat", "stats")]
        public async Task Price()
        {
            try
            {
                if (Context.Channel is IDMChannel)
                {
                    await Discord.ReplyAsync(Context,
                        message: "Please make this request in one of the official channels.")
                        .ConfigureAwait(false);
                    return;
                }

                if (!config.ChannelIds.Contains(Context.Channel.Id))
                    return;

                if (await RequestHelper.IsRateLimited(db, Context, config))
                {
                    log.Info("Request rate limited");
                    return;
                }

                using var a = Context.Channel.EnterTypingState();
                log.Info("Requesting prices and stats");

                var emotes = new Emotes(Context);
                var taco = await emotes.Get(config.EmoteTacoz).ConfigureAwait(false);
                //var taco = new Emoji("🌮");

                var item = await Common.GetAllInfo(db).ConfigureAwait(false);

                string title = $"Current Price and Statistics";
                string footerText = $"{item.Price.Date.ToDate()}. Powered by Quipuswap.";
                if (item.IsOutOfSync())
                    footerText += "\nStats might be out of sync. The admin has been contacted.";

                string label1 = $"1 XTZ/TCZ: {item.Price.PriceTCZForOneXTZ.FormatTcz(true)}";
                string value1 = $"{item.Price.PriceXTZChange.Indicator()}{item.Price.PriceXTZChangePct.FormatPct(0)}%";
                string label2 = $"Price/USD: ${item.Price.PriceUSD120k.FormatUsd(2)}";
                string value2 = $"{item.Price.PriceUSDChange.Indicator()}{item.Price.PriceUSDChangePct.FormatPct()}%";

                var output = new EmbedBuilder();
                output.WithColor(Color.THEME_RED)
                .WithAuthor(author =>
                {
                    author.WithName(title);
                })
                .WithDescription($"**{item.Price.PriceUSD.FormatUsd(6)} USD** for a single {taco}!")
                .AddField($"— Prices", "```ml\n" +
                    $"{label1,-20} {value1,10:C}\n" +
                    $"1 TCZ/XTZ: {item.Price.PriceXTZForOneTCZ.FormatTcz()}" +
                    "```")
                .AddField($"— Market (120k TCZ)", "```ml\n" +
                    $"{label2,-20} {value2,10:C}\n" +
                    $"Price/BTC: ₿{item.Price.PriceBTC120k.FormatBtc()}\n" +
                    $"Price/ETH: Ξ{item.Price.PriceETH120k.FormatEth()}\n" +
                    $"Full MCap: ${item.Price.FullMarketCapUSD.FormatLarge()}\n" +
                    $"Circ MCap: ${item.Price.CircMarketCapUSD.FormatLarge()}\n" +
                    "```")
                .WithFooter(footer =>
                {
                    footer.WithText(footerText);
                });

                await Discord.ReplyAsync(Context, output, reply: true).ConfigureAwait(false);
                log.Info("Prices and stats successfully sent");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}