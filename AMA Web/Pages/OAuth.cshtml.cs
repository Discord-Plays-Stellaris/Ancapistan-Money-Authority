using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AMA_Web.Properties;

namespace AMA_Web.Pages
{
    public class OAuthModel : PageModel
    {
        public string responseString { get; set; }

        public void OnGet()
        {
            responseString = GetInfo(Request.Query["code"]);
        }

        public string GetInfo(string code)
        {
            HttpClient client = new HttpClient();

            var values = new Dictionary<string, string>
            {
                ["client_id"] = Resources.client_id,
                ["client_secret"] = Resources.client_secret,
                ["grant_type"] = Resources.grant_type,
                ["code"] = code,
                ["redirect_uri"] = Resources.redirect_uri,
                ["scope"] = Resources.scope
            };

            var content = new FormUrlEncodedContent(values);

            var response = client.PostAsync("https://discordapp.com/api/oauth2/token", content);

            return response.ToString();
        }
    }
}