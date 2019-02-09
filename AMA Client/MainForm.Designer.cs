namespace AMA_Client
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabUser = new System.Windows.Forms.TabPage();
            this.buttonLogOut = new System.Windows.Forms.Button();
            this.buttonReload = new System.Windows.Forms.Button();
            this.labelShares = new System.Windows.Forms.Label();
            this.dataGridViewShares = new System.Windows.Forms.DataGridView();
            this.labelCompaniesList = new System.Windows.Forms.Label();
            this.listBoxCompanies = new System.Windows.Forms.ListBox();
            this.labelMoney = new System.Windows.Forms.Label();
            this.labelPP = new System.Windows.Forms.Label();
            this.labelUsername = new System.Windows.Forms.Label();
            this.buttonLogIn = new System.Windows.Forms.Button();
            this.tabEconomy = new System.Windows.Forms.TabPage();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.buttonGitHub = new System.Windows.Forms.Button();
            this.textBoxCredits = new System.Windows.Forms.TextBox();
            this.buttonQuit = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabUser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewShares)).BeginInit();
            this.tabAbout.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabUser);
            this.tabControl1.Controls.Add(this.tabEconomy);
            this.tabControl1.Controls.Add(this.tabAbout);
            this.tabControl1.Location = new System.Drawing.Point(-1, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(802, 450);
            this.tabControl1.TabIndex = 0;
            // 
            // tabUser
            // 
            this.tabUser.Controls.Add(this.buttonQuit);
            this.tabUser.Controls.Add(this.buttonLogOut);
            this.tabUser.Controls.Add(this.buttonReload);
            this.tabUser.Controls.Add(this.labelShares);
            this.tabUser.Controls.Add(this.dataGridViewShares);
            this.tabUser.Controls.Add(this.labelCompaniesList);
            this.tabUser.Controls.Add(this.listBoxCompanies);
            this.tabUser.Controls.Add(this.labelMoney);
            this.tabUser.Controls.Add(this.labelPP);
            this.tabUser.Controls.Add(this.labelUsername);
            this.tabUser.Controls.Add(this.buttonLogIn);
            this.tabUser.Location = new System.Drawing.Point(4, 25);
            this.tabUser.Name = "tabUser";
            this.tabUser.Padding = new System.Windows.Forms.Padding(3);
            this.tabUser.Size = new System.Drawing.Size(794, 421);
            this.tabUser.TabIndex = 0;
            this.tabUser.Text = "User";
            this.tabUser.UseVisualStyleBackColor = true;
            // 
            // buttonLogOut
            // 
            this.buttonLogOut.Location = new System.Drawing.Point(710, 50);
            this.buttonLogOut.Name = "buttonLogOut";
            this.buttonLogOut.Size = new System.Drawing.Size(75, 34);
            this.buttonLogOut.TabIndex = 9;
            this.buttonLogOut.Text = "Log Out";
            this.buttonLogOut.UseVisualStyleBackColor = true;
            this.buttonLogOut.Visible = false;
            this.buttonLogOut.Click += new System.EventHandler(this.buttonLogOut_Click);
            // 
            // buttonReload
            // 
            this.buttonReload.Location = new System.Drawing.Point(710, 7);
            this.buttonReload.Name = "buttonReload";
            this.buttonReload.Size = new System.Drawing.Size(75, 34);
            this.buttonReload.TabIndex = 8;
            this.buttonReload.Text = "Reload";
            this.buttonReload.UseVisualStyleBackColor = true;
            this.buttonReload.Visible = false;
            this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
            // 
            // labelShares
            // 
            this.labelShares.AutoSize = true;
            this.labelShares.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelShares.Location = new System.Drawing.Point(219, 106);
            this.labelShares.Name = "labelShares";
            this.labelShares.Size = new System.Drawing.Size(64, 17);
            this.labelShares.TabIndex = 7;
            this.labelShares.Text = "Shares:";
            // 
            // dataGridViewShares
            // 
            this.dataGridViewShares.AllowUserToAddRows = false;
            this.dataGridViewShares.AllowUserToDeleteRows = false;
            this.dataGridViewShares.AllowUserToResizeColumns = false;
            this.dataGridViewShares.AllowUserToResizeRows = false;
            this.dataGridViewShares.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dataGridViewShares.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewShares.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewShares.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridViewShares.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewShares.Location = new System.Drawing.Point(219, 130);
            this.dataGridViewShares.Name = "dataGridViewShares";
            this.dataGridViewShares.RowHeadersVisible = false;
            this.dataGridViewShares.RowTemplate.Height = 24;
            this.dataGridViewShares.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridViewShares.Size = new System.Drawing.Size(240, 276);
            this.dataGridViewShares.TabIndex = 6;
            // 
            // labelCompaniesList
            // 
            this.labelCompaniesList.AutoSize = true;
            this.labelCompaniesList.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCompaniesList.Location = new System.Drawing.Point(13, 107);
            this.labelCompaniesList.Name = "labelCompaniesList";
            this.labelCompaniesList.Size = new System.Drawing.Size(92, 17);
            this.labelCompaniesList.TabIndex = 5;
            this.labelCompaniesList.Text = "Companies:";
            // 
            // listBoxCompanies
            // 
            this.listBoxCompanies.FormattingEnabled = true;
            this.listBoxCompanies.ItemHeight = 16;
            this.listBoxCompanies.Location = new System.Drawing.Point(13, 130);
            this.listBoxCompanies.Name = "listBoxCompanies";
            this.listBoxCompanies.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBoxCompanies.Size = new System.Drawing.Size(199, 276);
            this.listBoxCompanies.TabIndex = 4;
            // 
            // labelMoney
            // 
            this.labelMoney.AutoSize = true;
            this.labelMoney.Location = new System.Drawing.Point(10, 50);
            this.labelMoney.Name = "labelMoney";
            this.labelMoney.Size = new System.Drawing.Size(111, 17);
            this.labelMoney.TabIndex = 3;
            this.labelMoney.Text = "Balance: $00.00";
            // 
            // labelPP
            // 
            this.labelPP.AutoSize = true;
            this.labelPP.Location = new System.Drawing.Point(10, 33);
            this.labelPP.Name = "labelPP";
            this.labelPP.Size = new System.Drawing.Size(141, 17);
            this.labelPP.TabIndex = 2;
            this.labelPP.Text = "Personal Influence: 0";
            // 
            // labelUsername
            // 
            this.labelUsername.AutoSize = true;
            this.labelUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUsername.Location = new System.Drawing.Point(7, 4);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Size = new System.Drawing.Size(202, 29);
            this.labelUsername.TabIndex = 1;
            this.labelUsername.Text = "Username#0000";
            // 
            // buttonLogIn
            // 
            this.buttonLogIn.Location = new System.Drawing.Point(710, 7);
            this.buttonLogIn.Name = "buttonLogIn";
            this.buttonLogIn.Size = new System.Drawing.Size(75, 34);
            this.buttonLogIn.TabIndex = 0;
            this.buttonLogIn.Text = "Log In";
            this.buttonLogIn.UseVisualStyleBackColor = true;
            this.buttonLogIn.Click += new System.EventHandler(this.buttonLogIn_Click);
            // 
            // tabEconomy
            // 
            this.tabEconomy.Location = new System.Drawing.Point(4, 25);
            this.tabEconomy.Name = "tabEconomy";
            this.tabEconomy.Padding = new System.Windows.Forms.Padding(3);
            this.tabEconomy.Size = new System.Drawing.Size(794, 421);
            this.tabEconomy.TabIndex = 1;
            this.tabEconomy.Text = "Economy";
            this.tabEconomy.UseVisualStyleBackColor = true;
            // 
            // tabAbout
            // 
            this.tabAbout.Controls.Add(this.buttonGitHub);
            this.tabAbout.Controls.Add(this.textBoxCredits);
            this.tabAbout.Location = new System.Drawing.Point(4, 25);
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.Size = new System.Drawing.Size(794, 421);
            this.tabAbout.TabIndex = 2;
            this.tabAbout.Text = "About";
            this.tabAbout.UseVisualStyleBackColor = true;
            // 
            // buttonGitHub
            // 
            this.buttonGitHub.Location = new System.Drawing.Point(621, 14);
            this.buttonGitHub.Name = "buttonGitHub";
            this.buttonGitHub.Size = new System.Drawing.Size(153, 32);
            this.buttonGitHub.TabIndex = 1;
            this.buttonGitHub.Text = "Github Repo for AMA";
            this.buttonGitHub.UseVisualStyleBackColor = true;
            this.buttonGitHub.Click += new System.EventHandler(this.buttonGitHub_Click);
            // 
            // textBoxCredits
            // 
            this.textBoxCredits.BackColor = System.Drawing.Color.White;
            this.textBoxCredits.Location = new System.Drawing.Point(10, 4);
            this.textBoxCredits.Multiline = true;
            this.textBoxCredits.Name = "textBoxCredits";
            this.textBoxCredits.ReadOnly = true;
            this.textBoxCredits.Size = new System.Drawing.Size(775, 409);
            this.textBoxCredits.TabIndex = 0;
            this.textBoxCredits.Text = resources.GetString("textBoxCredits.Text");
            // 
            // buttonQuit
            // 
            this.buttonQuit.Location = new System.Drawing.Point(710, 379);
            this.buttonQuit.Name = "buttonQuit";
            this.buttonQuit.Size = new System.Drawing.Size(75, 34);
            this.buttonQuit.TabIndex = 10;
            this.buttonQuit.Text = "Quit";
            this.buttonQuit.UseVisualStyleBackColor = true;
            this.buttonQuit.Click += new System.EventHandler(this.buttonQuit_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Ancapistan Money Authority";
            this.tabControl1.ResumeLayout(false);
            this.tabUser.ResumeLayout(false);
            this.tabUser.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewShares)).EndInit();
            this.tabAbout.ResumeLayout(false);
            this.tabAbout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabUser;
        private System.Windows.Forms.TabPage tabEconomy;
        private System.Windows.Forms.TabPage tabAbout;
        private System.Windows.Forms.TextBox textBoxCredits;
        private System.Windows.Forms.Button buttonLogIn;
        private System.Windows.Forms.Button buttonGitHub;
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.Label labelMoney;
        private System.Windows.Forms.Label labelPP;
        private System.Windows.Forms.ListBox listBoxCompanies;
        private System.Windows.Forms.Label labelCompaniesList;
        private System.Windows.Forms.DataGridView dataGridViewShares;
        private System.Windows.Forms.Label labelShares;
        private System.Windows.Forms.Button buttonReload;
        private System.Windows.Forms.Button buttonLogOut;
        private System.Windows.Forms.Button buttonQuit;
    }
}