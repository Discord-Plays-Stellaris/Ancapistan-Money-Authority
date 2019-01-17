using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIR.Objects
{
    /// <summary>
    /// An object to contain information about the Stock Market.
    /// </summary>
    public class StockMarketObject
    {
        public string acronym { get; private set; }
        public string marketName { get; private set; }
        public string id { get; private set; }

        /// <summary>
        /// The StockMarketObject constructer
        /// </summary>
        /// <param name="acro">The acronym for the stock market</param>
        /// <param name="name">The name of the stock market.</param>
        public StockMarketObject(string acro, string name)
        {
            acronym = acro;
            marketName = name;
            id = "MarketInfo";
        }
    }
}
