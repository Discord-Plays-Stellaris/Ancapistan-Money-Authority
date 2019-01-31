using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIR.Objects
{
    public class ResourceTransferResult
    {
        public string Message { get; set; }
        public bool Error { get; set; }
        public string IdSender { get; set; }
        public string Type { get; set; }
        public ulong Amount { get; set; }
    }

    public class ResourceListingResult
    {
        public string Message { get; set; }
        public bool Error { get; set; }
        public string IdSeller { get; set; }
        public string Type { get; set; }
        public ulong Amount { get; set; }
    }
}
