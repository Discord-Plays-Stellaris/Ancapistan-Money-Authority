using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VIR.Objects.Company;

namespace VIR.Services
{
    public sealed class ResourceHandlingService
    {
        public static ResourceHandlingService _instance {get;set;}
        private DataBaseHandlingService __Database;

        public ResourceHandlingService(IServiceProvider services, DataBaseHandlingService database)
        {
            _instance = new ResourceHandlingService
            {
                __Database = database
            };
        }

        public ResourceHandlingService()
        {
            
        }

        public async void InsertIntoResources(JObject resource)
        {
            await __Database.SetJObjectAsync(resource, "resources", false);
        }

        public JObject GetResource(string id)
        {
            return __Database.getJObjectAsync(id, "resources").Result;
        }
    }
}
