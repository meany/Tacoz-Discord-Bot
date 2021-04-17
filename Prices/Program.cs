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
        private IServiceProvider services;
        private IConfigurationRoot configuration;
        private AppDbContext db;
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private QuipStorage quipData;
        private CoinGecko.Entities.Response.Simple.Price data;

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
                decimal fullMktCapUsd = priceTczUsd * 1_000_000_000_000;
                decimal circMktCapUsd = priceTczUsd * 612_550_568;

                //decimal mktCapUsd = decimal.Parse(data.MarketData.MarketCap["usd"].Value.ToString());
                //decimal mktCapUsdChgAmt = (data.MarketData.MarketCapChange24HInCurrency.Count == 0) ? 0 : decimal.Parse(data.MarketData.MarketCapChange24HInCurrency["usd"].ToString(), NumberStyles.Any);
                //Change mktCapUsdChg = (mktCapUsdChgAmt > 0) ? Change.Up : (mktCapUsdChgAmt < 0) ? Change.Down : Change.None;
                //decimal mktCapUsdChgPct = (data.MarketData.MarketCapChangePercentage24HInCurrency.Count == 0) ? 0 : decimal.Parse(data.MarketData.MarketCapChangePercentage24HInCurrency["usd"].ToString(), NumberStyles.Any);

                //// volume
                //int volumeUsd = (int)Math.Round(data.MarketData.TotalVolume["usd"].Value);

                //// prices
                //decimal priceBtc = decimal.Parse(data.MarketData.CurrentPrice["btc"].Value.ToString(), NumberStyles.Any);

                //string changeBtc = "0";
                //string changeEth = "0";
                //string changeUsd = "0";
                //string changeBtcPct = "0";
                //string changeEthPct = "0";
                //string changeUsdPct = "0";
                //if (data.MarketData.PriceChange24HInCurrency.Count > 0 &&
                //    data.MarketData.PriceChangePercentage24HInCurrency.Count > 0)
                //{
                //    changeBtc = data.MarketData.PriceChange24HInCurrency["btc"].ToString();
                //    changeBtcPct = data.MarketData.PriceChangePercentage24HInCurrency["btc"].ToString();
                //    changeEth = data.MarketData.PriceChange24HInCurrency["eth"].ToString();
                //    changeEthPct = data.MarketData.PriceChangePercentage24HInCurrency["eth"].ToString();
                //    changeUsd = data.MarketData.PriceChange24HInCurrency["usd"].ToString();
                //    changeUsdPct = data.MarketData.PriceChangePercentage24HInCurrency["usd"].ToString();
                //}

                //decimal priceBtcChgAmt = decimal.Parse(changeBtc, NumberStyles.Any);
                //Change priceBtcChg = (priceBtcChgAmt > 0) ? Change.Up : (priceBtcChgAmt < 0) ? Change.Down : Change.None;
                //decimal priceBtcChgPct = decimal.Parse(changeBtcPct, NumberStyles.Any);

                //decimal priceEth = decimal.Parse(data.MarketData.CurrentPrice["eth"].Value.ToString(), NumberStyles.Any);
                //decimal priceEthChgAmt = decimal.Parse(changeEth, NumberStyles.Any);
                //Change priceEthChg = (priceEthChgAmt > 0) ? Change.Up : (priceEthChgAmt < 0) ? Change.Down : Change.None;
                //decimal priceEthChgPct = decimal.Parse(changeEthPct, NumberStyles.Any);

                //decimal priceUsd = decimal.Parse(data.MarketData.CurrentPrice["usd"].Value.ToString(), NumberStyles.Any);
                //decimal priceUsdChgAmt = decimal.Parse(changeUsd, NumberStyles.Any);
                //Change priceUsdChg = (priceUsdChgAmt > 0) ? Change.Up : (priceUsdChgAmt < 0) ? Change.Down : Change.None;
                //decimal priceUsdChgPct = decimal.Parse(changeUsdPct, NumberStyles.Any);

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

                    //MarketCapUSDChange = mktCapUsdChg,
                    //MarketCapUSDChangePct = mktCapUsdChgPct,
                    //PriceBTCChange = priceBtcChg,
                    //PriceBTCChangePct = priceBtcChgPct,
                    //PriceETHChange = priceEthChg,
                    //PriceETHChangePct = priceEthChgPct,
                    //PriceUSDChange = priceUsdChg,
                    //PriceUSDChangePct = priceUsdChgPct,
                    //PriceXTZChange = priceXtzChg,
                    //PriceXTZChangePct = priceXtzChgPct,
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
            GetSimplePrices();

            while (quipData == null || data == null)
                await Task.Delay(200);

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

        private async void GetTczLp()
        {
            try
            {
                var client = new RestClient("https://mainnet.smartpy.io");
                var request = new RestRequest("chains/main/blocks/head/context/contracts/KT1NNwvwvJVrw5Fuq4Nqu4upqqsktsUapzFK/storage", DataFormat.Json);
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