using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESP8266
{
    public partial class MainForm: Form
    {
        private Home home;

        public MainForm()
        {
            InitializeComponent();
            SignUp signUp = new SignUp(this);
            pnLogin.Controls.Add(signUp);

        }

        public void ShowHome(string s)
        {
            if (home == null)
            {
                home = new Home(this, s);
            }

            pnLogin.Controls.Clear();
            pnLogin.Visible = false;

            pnMain.Controls.Clear();
            pnMain.Controls.Add(home);
            pnMain.Visible = true;
            home.Visible = true;
            this.Activate();
        }

        public void ShowLogin()
        {
            //pnMain.Controls.Clear();
            //pnMain.Visible = false;

            //pnLogin.Controls.Clear();
            //Login login = new Login(this);

            //pnLogin.Controls.Add(login);
            //pnLogin.Visible = true;
            //pnLogin.BringToFront();
            //this.Activate();
        }
    }
}
