using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Discord.OAuth2;
using Discord.Net;
using Discord.Webhook;
using Discord.WebSocket;
using Discord;
using Discord.Commands;
using Discord.Rest;
using AMA_Client.Objects;
using AMA_Client.Properties;
using AMA_Client.Services;
using Newtonsoft.Json.Linq;

namespace AMA_Client
{
    public partial class MainForm : Form
    {
        public string userID { get; private set; }

        public MainForm()
        {
            InitializeComponent();
            db.Connect();
        }

        private void InitializeGUIFields()
        {
            buttonLogIn.Hide();
            labelMoney.Text = $"Balance: ${db.GetField(userID, "money", "users")}";
            labelPP.Text = $"Personal Influence: {db.GetField(userID, "pp", "users")}";
            buttonReload.Show();
            buttonLogOut.Show();
            addSharesToTable();
            addCompaniesToList();
        }

        private void addSharesToTable()
        {
            UserShares shares = new UserShares(db.getJObject(userID, "shares"));
            DataTable table = new DataTable();
            table.Columns.Add("Ticker");
            table.Columns.Add("Owned Shares");
            foreach (string ticker in shares.ownedShares.Keys)
            {
                DataRow row = table.NewRow();
                row["Ticker"] = ticker;
                row["Owned Shares"] = shares.ownedShares[ticker];
                table.Rows.Add(row);
            }
            dataGridViewShares.DataSource = table;
        }

        private void addCompaniesToList()
        {
            listBoxCompanies.Items.Clear();
            JToken jToken = db.GetField(userID, "corps", "users");
            Collection<string> corps = jToken.ToObject<Collection<string>>();
            foreach (string ticker in corps)
            {
                try
                {
                    listBoxCompanies.Items.Add(db.GetField(ticker, "name", "companies"));
                }
                catch (Exception e)
                {  }
            }
        }

        private async void buttonLogIn_Click(object sender, EventArgs e)
        {
            bool isDebug = false;
            #if DEBUG
            isDebug = true;
            #endif

            if (isDebug == false)
            {
                System.Diagnostics.Process.Start("https://discordapp.com/api/oauth2/authorize?client_id=541654043895005184&redirect_uri=https%3A%2F%2Fdiscordapp.com%2Foauth2%2Fauthorized&response_type=code&scope=identify");
                //LoggedInClient client = new LoggedInClient("23048032");
            }
            else if (isDebug == true)
            {
                userID = "300581660330688512"; // Skipper's ID
                //IUser user = await Client.GetUserAsync(Convert.ToUInt64(userID));
                labelUsername.Text = "Skipper#3815"; //user.Username.ToString();
            }
            InitializeGUIFields();
        }

        private void buttonGitHub_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Discord-Plays-Stellaris/Ancapistan-Money-Authority");
        }

        private void buttonReload_Click(object sender, EventArgs e)
        {
            InitializeGUIFields();
        }

        private void buttonLogOut_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
