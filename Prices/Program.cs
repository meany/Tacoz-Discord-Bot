using CoinGecko.Clients;
using dm.TCZ.Data;
using dm.TCZ.Data.Models;
using dm.TCZ.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NLog;
using RestSharp;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace dm.TCZ.Prices
{
    class Program
    {
        private static IServiceProvider services;
        private static IConfigurationRoot configuration;
        private static AppDbContext db;
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private static QuipStorage quipData;
        private static CoinGecko.Entities.Response.Simple.Price data;
        private static BigInteger tacozCirc;
        private static Price prices24hAgo;

        private static readonly string lpContract1 = "KT1GGxCNiJ7yaBAH4hAw5AHXbP3PSmAiy3wK";
        private static readonly string lpContract1dot1 = "KT18oC9954dDwwrUC6uMor1nfgnW1WyVEua4";
        private static readonly string lpContract1dot2 = "KT1VfiExduEkrpM4TbsnAw9QV1VURqaRFGKs";
        private static string lbContractCurrent = lpContract1dot2;

        public static void Main(string[] args)
            => new Program().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("Config.Prices.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("Config.Prices.Local.json", optional: true, reloadOnChange: true);

                configuration = builder.Build();

                services = new ServiceCollection()
                    .AddDatabase<AppDbContext>(configuration.GetConnectionString("Database"))
                    .BuildServiceProvider();
                db = services.GetService<AppDbContext>();

                if (db.Database.GetPendingMigrations().Count() > 0)
                {
                    log.Info("Migrating database");
                    db.Database.Migrate();
                }

                await Start();
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
                log.Info("Getting info");
                //var stat = db.Stats
                //    .AsNoTracking()
                //    .OrderByDescending(x => x.Date)
                //    .FirstOrDefault();

                await GetInfo();

                // market prices
                decimal priceBtcUsd = decimal.Parse(data["bitcoin"]["usd"].ToString(), NumberStyles.Any);
                decimal priceEthUsd = decimal.Parse(data["ethereum"]["usd"].ToString(), NumberStyles.Any);
                decimal priceXtzUsd = decimal.Parse(data["tezos"]["usd"].ToString(), NumberStyles.Any);

                // tacoz prices
                var ratioXtz = BigInteger.Parse(quipData.Tezos);
                var ratioTcz = BigInteger.Parse(quipData.Tacoz);
                var tczForOneXtz = ratioTcz / ratioXtz;
                decimal priceOneXtz = tczForOneXtz.ToXtz();
                decimal priceOneTcz = 1 / priceOneXtz;
                decimal priceTczUsd = priceOneTcz * priceXtzUsd;
                decimal price120kTczUsd = 120_000 * priceTczUsd;
                decimal price120kTczXtz = 120_000 / priceOneXtz;
                decimal priceTczBtc = priceTczUsd / priceBtcUsd;
                decimal price120kTczBtc = 120_000 * priceTczBtc;
                decimal priceTczEth = priceTczUsd / priceEthUsd;
                decimal price120kTczEth = 120_000 * priceTczEth;

                // market cap
                decimal totalTcz = 1_000_000_000_000;
                decimal fullMktCapUsd = priceTczUsd * totalTcz;
                decimal tacozCircXtz = totalTcz - tacozCirc.ToEth();
                decimal circMktCapUsd = priceTczUsd * tacozCircXtz;

                decimal mktCapUsdChgAmt = fullMktCapUsd - prices24hAgo.FullMarketCapUSD;
                Change mktCapUsdChg = (mktCapUsdChgAmt > 0) ? Change.Up : (mktCapUsdChgAmt < 0) ? Change.Down : Change.None;
                decimal mktCapUsdChgPct = (Math.Abs(mktCapUsdChgAmt) / prices24hAgo.FullMarketCapUSD);

                //// volume
                //int volumeUsd = (int)Math.Round(data.MarketData.TotalVolume["usd"].Value);

                // price changes
                decimal changeBtc = price120kTczBtc - prices24hAgo.PriceBTC120k;
                decimal changeEth = price120kTczEth - prices24hAgo.PriceETH120k;
                decimal changeXtz = priceOneXtz - prices24hAgo.PriceTCZForOneXTZ;
                decimal changeUsd = price120kTczUsd - prices24hAgo.PriceUSD120k;
                Change priceBtcChg = (changeBtc > 0) ? Change.Up : (changeBtc < 0) ? Change.Down : Change.None;
                Change priceEthChg = (changeEth > 0) ? Change.Up : (changeEth < 0) ? Change.Down : Change.None;
                Change priceXtzChg = (changeXtz > 0) ? Change.Up : (changeXtz < 0) ? Change.Down : Change.None;
                Change priceUsdChg = (changeUsd > 0) ? Change.Up : (changeUsd < 0) ? Change.Down : Change.None;
                decimal changeBtcPct = (Math.Abs(changeBtc) / prices24hAgo.PriceBTC120k);
                decimal changeEthPct = (Math.Abs(changeEth) / prices24hAgo.PriceETH120k);
                decimal changeXtzPct = (Math.Abs(changeXtz) / prices24hAgo.PriceTCZForOneXTZ);
                decimal changeUsdPct = (Math.Abs(changeUsd) / prices24hAgo.PriceUSD120k);

                var item = new Price
                {
                    Date = DateTime.UtcNow,
                    Group = Guid.NewGuid(),
                    LpRawTacoz = ratioTcz.ToString(),
                    LpRawTezos = ratioXtz.ToString(),
                    PriceTCZForOneXTZ = priceOneXtz,
                    PriceXTZForOneTCZ = priceOneTcz,
                    Source = PriceSource.Quipuswap,

                    FullMarketCapUSD = int.Parse(Math.Round(fullMktCapUsd).ToString()),
                    CircMarketCapUSD = int.Parse(Math.Round(circMktCapUsd).ToString()),

                    PriceBTC = priceTczBtc,
                    PriceBTC120k = price120kTczBtc,
                    PriceETH = priceTczEth,
                    PriceETH120k = price120kTczEth,
                    PriceUSD = priceTczUsd,
                    PriceUSD120k = price120kTczUsd,

                    MarketCapUSDChange = mktCapUsdChg,
                    MarketCapUSDChangePct = mktCapUsdChgPct,
                    PriceBTCChange = priceBtcChg,
                    PriceBTCChangePct = changeBtcPct,
                    PriceETHChange = priceEthChg,
                    PriceETHChangePct = changeEthPct,
                    PriceXTZChange = priceXtzChg,
                    PriceXTZChangePct = changeXtzPct,
                    PriceUSDChange = priceUsdChg,
                    PriceUSDChangePct = changeUsdPct,
                    
                    //VolumeUSD = volumeUsd
                };

                db.Add(item);

                log.Info("Saving prices to database");
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async Task GetInfo()
        {
            GetTczLp();
            GetTczCirc();
            GetSimplePrices();
            GetPrices24hAgo();

            while (prices24hAgo == null || quipData == null || tacozCirc == 0 || data == null)
                await Task.Delay(200);

        }

        private async void GetPrices24hAgo()
        {
            try
            {
                prices24hAgo = await db.Prices
                    .AsNoTracking()
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefaultAsync(x => x.Date <= DateTime.UtcNow.AddHours(-24));
                log.Info($"GetPrices24hAgo: OK");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async void GetSimplePrices()
        {
            try
            {
                var client = CoinGeckoClient.Instance;
                var ids = new string[] { "bitcoin", "ethereum", "tezos" };
                var curs = new string[] { "usd" };
                data = await client.SimpleClient.GetSimplePrice(ids, curs);
                log.Info($"GetSimplePrices: OK");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async void GetTczCirc()
        {
            try
            {
                var client = new RestClient("https://better-call.dev");
                var request = new RestRequest($"v1/account/mainnet/tz1fGotNGshKkoZNYBsyCwMTkaeU6k2T28MF/token_balances?offset=0&size=1", DataFormat.Json);
                var bcdTokenData = await client.GetAsync<BcdToken>(request);
                tacozCirc = BigInteger.Parse(bcdTokenData.Balances[0].Balance);

                log.Info($"GetTczCirc: OK");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async void GetTczLp()
        {
            try
            {
                var client = new RestClient("https://mainnet-tezos.giganode.io");
                var request = new RestRequest($"chains/main/blocks/head/context/contracts/{lbContractCurrent}/storage", DataFormat.Json);
                quipData = await client.GetAsync<QuipStorage>(request);

                log.Info($"GetTczLp: OK");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}