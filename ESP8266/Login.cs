using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Util.Store;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Google.Apis.Oauth2.v2.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Net.Sockets;
using Npgsql;

namespace ESP8266
{
    public partial class Login : UserControl
    {
        private const string connString = "Host=aws-0-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.bzjfiynoyelxlpowlhty;Password=laptrinhmang;SSL Mode=Require;Trust Server Certificate=true";

        private bool isPasswordVisible = false;
        private string srcEmail = "";
        private MainForm main;

        public Login(MainForm mainForm)
        {
            InitializeComponent();
            lblCheck.Text = "";
            main = mainForm;
        }

        // Hàm kiểm tra email đã tồn tại trong cơ sở dữ liệu chưa
        private bool IsEmailExists(string email)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE email = @email";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        long count = (long)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                lblCheck.Text = $"Lỗi kết nối: {ex.Message}";
                return false;
            }
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string email = tbEmail.Text.Trim();
            string password = tbPass.Text.Trim();

            // Kiểm tra xem người dùng đã nhập đầy đủ thông tin chưa
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                lblCheck.Text = "Vui lòng nhập đầy đủ email và mật khẩu.";
                return;
            }

            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT id_user, email FROM users WHERE email = @email AND password = @password";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@password", password); // Lưu ý: Nên mã hóa mật khẩu trong thực tế
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Đăng nhập thành công
                                srcEmail = reader["email"].ToString();
                                main.ShowHome(srcEmail); // Chuyển đến form Home với email của người dùng
                            }
                            else
                            {
                                // Đăng nhập thất bại
                                lblCheck.Text = "Email hoặc mật khẩu không đúng.";
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                lblCheck.Text = $"Lỗi kết nối: {ex.Message}";
            }
            catch (Exception ex)
            {
                lblCheck.Text = $"Lỗi: {ex.Message}";
            }
        }

        private async void btnLoginGoogle_Click(object sender, EventArgs e)
        {
            try
            {
                string credPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(credPath);

                var clientSecrets = new ClientSecrets
                {
                    ClientId = "266768311409-sa0qg8353t75tscss8c71v44usk0cimq.apps.googleusercontent.com",
                    ClientSecret = "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"
                };

                var scopes = new[] { "profile", "email" };

                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    clientSecrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)
                );

                if (credential != null && credential.Token != null)
                {
                    var oauthService = new Oauth2Service(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential
                    });

                    Userinfo userInfo = await oauthService.Userinfo.Get().ExecuteAsync();
                    srcEmail = userInfo.Email;

                    if (IsEmailExists(srcEmail))
                    {
                        main.Activate();
                        main.ShowHome(srcEmail);
                    }
                    else
                    {
                        main.Activate();
                        lblCheck.Text = "Email chưa tồn tại, vui lòng đăng kí tài khoản!";     
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đăng nhập: " + ex.Message);
            }
        }

        private void lbQuenmatkhau_Click(object sender, EventArgs e)
        {
            ForgetPassword forgetPassword = new ForgetPassword(main);
            main.pnLogin.Controls.Clear();
            main.pnLogin.Controls.Add(forgetPassword);
        }

        private void lbDangky_Click(object sender, EventArgs e)
        {
            SignUp signUp = new SignUp(main);
            main.pnLogin.Controls.Clear();
            main.pnLogin.Controls.Add(signUp);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            main.Close();
        }

        private void lbViewPassword_Click(object sender, EventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;
            tbPass.UseSystemPasswordChar = !isPasswordVisible;
            lbViewPassword.Text = !isPasswordVisible ? "Ẩn" : "Hiện";
            pnViewPass.BackgroundImage = !isPasswordVisible ? Properties.Resources.icons8_hide_30 : Properties.Resources.Show;
        }

        private void tbPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnDangNhap_Click(sender, e);
            }    
        }

        private void tbEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnDangNhap_Click(sender, e);
            }
        }

        private void pnViewPass_Click(object sender, EventArgs e)
        {
            lbViewPassword_Click(sender, e);
        }
    }
}