using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dm.TCZ.Response
{
    public partial class BcdToken
    {
        public List<BcdTokenBalance> Balances { get; set; }
        public long Total { get; set; }
    }

    public partial class BcdTokenBalance
    {
        public string Contract { get; set; }
        public string Network { get; set; }
        public long Level { get; set; }
        [JsonProperty("token_id")]
        public long TokenId { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public long Decimals { get; set; }
        [JsonProperty("thumbnail_uri")]
        public Uri ThumbnailUri { get; set; }
        public string Balance { get; set; }
    }
}
