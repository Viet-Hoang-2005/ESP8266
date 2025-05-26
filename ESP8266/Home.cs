using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESP8266
{
    public partial class Home: UserControl
    {
        private MainForm main;
        private string srcemail;
        private const string connString = "Host=aws-0-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.bzjfiynoyelxlpowlhty;Password=laptrinhmang;SSL Mode=Require;Trust Server Certificate=true";

        public Home(MainForm mainForm, string email)
        {
            InitializeComponent();
            main = mainForm;
            srcemail = email;

            HomeContent homeContent = new HomeContent(main, srcemail);
            pnContent.Controls.Clear();
            pnContent.Controls.Add(homeContent);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            main.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            main.WindowState = FormWindowState.Minimized;
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            UserInfo userInfo = new UserInfo(main, srcemail);
            pnContent.Controls.Clear();
            pnContent.Controls.Add(userInfo);
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            HomeContent homeContent = new HomeContent(main, srcemail);
            pnContent.Controls.Clear();
            pnContent.Controls.Add(homeContent);
        }
    }
}
