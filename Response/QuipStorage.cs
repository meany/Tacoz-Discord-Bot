using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace dm.TCZ.Response
{
    public partial class QuipStorage
    {
        public string Prim { get; set; }
        public List<QuipStorageArg> Args { get; set; }

        public string Tezos {
            get {
                return Args[1].Args[0].Args[1].Args[2].Int;
            }
        }

        public string Tacoz {
            get {
                return Args[1].Args[0].Args[3].Int;
            }
        }
    }

    public partial class QuipStorageArg
    {
        public string Prim { get; set; }
        public List<PurpleArg> Args { get; set; }
        public string Int { get; set; }
    }

    public partial class PurpleArg
    {
        public string Int { get; set; }
        public string Prim { get; set; }
        public List<FluffyArg> Args { get; set; }
    }

    public partial class FluffyArg
    {
        public string Prim { get; set; }
        public List<TentacledArg> Args { get; set; }
        public string Int { get; set; }
    }

    public partial class TentacledArg
    {
        public string Prim { get; set; }
        public List<StickyArg> Args { get; set; }
        public string String { get; set; }
        public string Int { get; set; }
    }

    public partial class StickyArg
    {
        public string Prim { get; set; }
        public List<IndigoArg> Args { get; set; }
        public string Int { get; set; }
        public DateTimeOffset? String { get; set; }
    }

    public partial class IndigoArg
    {
        public string Prim { get; set; }
        public List<IndecentArg> Args { get; set; }
    }

    public partial class IndecentArg
    {
        public string String { get; set; }
    }
}
