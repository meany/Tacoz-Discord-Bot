using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace dm.TCZ.Data.Models
{
    public enum PriceSource
    {
        Quipuswap = 0
    }

    public class Price
    {
        [JsonIgnore]
        public int PriceId { get; set; }
        public DateTime Date { get; set; }
        [JsonIgnore]
        public PriceSource Source { get; set; }
        [JsonIgnore]
        public Guid Group { get; set; }

        public string LpRawTezos { get; set; }
        public string LpRawTacoz { get; set; }

        [Column(TypeName = "decimal(16, 8)")]
        public decimal PriceTCZForOneXTZ { get; set; }
        [Column(TypeName = "decimal(16, 8)")]
        public decimal PriceXTZForOneTCZ { get; set; }

        [Column(TypeName = "decimal(11, 6)")]
        public decimal PriceUSD { get; set; }
        [Column(TypeName = "decimal(11, 6)")]
        public decimal PriceUSD120k { get; set; }
        public Change PriceUSDChange { get; set; }
        [Column(TypeName = "decimal(12, 8)")]
        public decimal PriceUSDChangePct { get; set; }
        [Column(TypeName = "decimal(16, 8)")]
        public decimal PriceETH { get; set; }
        [Column(TypeName = "decimal(16, 8)")]
        public decimal PriceETH120k { get; set; }
        public Change PriceETHChange { get; set; }
        [Column(TypeName = "decimal(12, 8)")]
        public decimal PriceETHChangePct { get; set; }
        [Column(TypeName = "decimal(16, 8)")]
        public decimal PriceBTC { get; set; }
        [Column(TypeName = "decimal(16, 8)")]
        public decimal PriceBTC120k { get; set; }
        public Change PriceBTCChange { get; set; }
        [Column(TypeName = "decimal(12, 8)")]
        public decimal PriceBTCChangePct { get; set; }
        public Change PriceXTZChange { get; set; }
        [Column(TypeName = "decimal(12, 8)")]
        public decimal PriceXTZChangePct { get; set; }

        public int FullMarketCapUSD { get; set; }
        public int CircMarketCapUSD { get; set; }
        public Change MarketCapUSDChange { get; set; }
        [Column(TypeName = "decimal(12, 8)")]
        public decimal MarketCapUSDChangePct { get; set; }

        public int VolumeUSD { get; set; }
        public Change VolumeUSDChange { get; set; }
        [Column(TypeName = "decimal(12, 8)")]
        public decimal VolumeUSDChangePct { get; set; }
    }
}
