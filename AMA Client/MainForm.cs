using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AMA_Client
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonLogIn_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://discordapp.com/api/oauth2/authorize?client_id=541654043895005184&redirect_uri=https%3A%2F%2Fdiscordapp.com%2Foauth2%2Fauthorized&response_type=code&scope=identify%20connections");
        }

        private void buttonGitHub_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Discord-Plays-Stellaris/Ancapistan-Money-Authority");
        }
    }
}
