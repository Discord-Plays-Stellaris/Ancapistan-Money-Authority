using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using VIR.Services;

namespace VIR.Modules
{
    class ResourceCommands : ModuleBase
    {
        private readonly CompanyService _companyService;
        private readonly DataBaseHandlingService _dataBaseService;
        private readonly CommandHandlingService _commandService;
        private readonly StockMarketService _marketService;

        public ResourceCommands(CompanyService com, DataBaseHandlingService db, CommandHandlingService comm, StockMarketService markserv)
        {
            _companyService = com;
            _dataBaseService = db;
            _commandService = comm;
            _marketService = markserv;
        }

        [Command("resources")]
        public async Task ViewResourcesAsync()
        {

        }
    }
}
