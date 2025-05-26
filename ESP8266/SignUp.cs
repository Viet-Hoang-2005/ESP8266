using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using Npgsql;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;

namespace ESP8266
{
    public partial class SignUp : UserControl
    {
        private MainForm main;
        private Dictionary<string, (string otp, DateTime createdTime)> otpStore;
        private string connString = "Host=aws-0-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.bzjfiynoyelxlpowlhty;Password=laptrinhmang;SSL Mode=Require;Trust Server Certificate=true";

        private string SmtpUsername = "sona.feelthemusic@gmail.com";
        private string SmtpPassword = "sxiz nqtc ddko izzj";
        
        private string srcEmail = "";
        private int OTPResendCooldown = 60;
        private int OTPExpirationMinutes = 5;

        public SignUp(MainForm mainForm)
        {
            InitializeComponent();
            main = mainForm;
            lblCheck.Text = "";
            otpStore = new Dictionary<string, (string otp, DateTime createdTime)>();
        }

        // Hàm tạo mã OTP ngẫu nhiên (6 chữ số)
        private string GenerateOTP()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        // Hàm kiểm tra định dạng email
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
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

        // Hàm gửi email chứa mã OTP
        private async Task<bool> SendOTPEmail(string email, string otp)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(SmtpUsername, SmtpPassword),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(SmtpUsername),
                    Subject = $"Mã OTP xác nhận đăng ký tài khoản SONA",
                    Body = $"Mã OTP của bạn là: <strong>{otp}</strong><br>Vui lòng nhập mã này để hoàn tất đăng ký. Mã có hiệu lực trong {OTPExpirationMinutes} phút.",
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        private async void btnNext_Click(object sender, EventArgs e)
        {
            string email = tbEmail.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                lblCheck.Text = "Vui lòng nhập email.";
                return;
            }

            if (!IsValidEmail(email))
            {
                lblCheck.Text = "Địa chỉ email không hợp lệ!";
                return;
            }

            // Kiểm tra email đã tồn tại trong cơ sở dữ liệu chưa
            if (IsEmailExists(email))
            {
                lblCheck.Text = "Tài khoản Email đã tồn tại, vui lòng đăng nhập.";
                return;
            }

            if (otpStore.ContainsKey(email))
            {
                var otpData = otpStore[email];
                if ((DateTime.Now - otpData.createdTime).TotalSeconds < OTPResendCooldown)
                {
                    lblCheck.Text = $"Vui lòng chờ {Math.Ceiling(OTPResendCooldown - (DateTime.Now - otpData.createdTime).TotalSeconds)} giây trước khi gửi lại!";
                    return;
                }
            }

            string otp = GenerateOTP();
            if (await SendOTPEmail(email, otp))
            {
                otpStore[email] = (otp, DateTime.Now);
                lblCheck.Text = "Mã OTP đã được gửi tới Email của bạn!";
            }
            else
            {
                lblCheck.Text = "Lỗi gửi mã OTP. Vui lòng kiểm tra lại email hoặc thử lại sau!";
            }
        }

        private async void btnRefreshOTP_Click(object sender, EventArgs e)
        {
            string email = tbEmail.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                lblCheck.Text = "Vui lòng nhập email trước khi yêu cầu OTP.";
                return;
            }

            if (!IsValidEmail(email))
            {
                lblCheck.Text = "Địa chỉ email không hợp lệ!";
                return;
            }

            if (otpStore.ContainsKey(email))
            {
                var otpData = otpStore[email];
                double remainingSeconds = OTPResendCooldown - (DateTime.Now - otpData.createdTime).TotalSeconds;
                if (remainingSeconds > 0)
                {
                    lblCheck.Text = $"Vui lòng chờ {Math.Ceiling(remainingSeconds)} giây trước khi gửi lại!";
                    return;
                }
            }

            string newOTP = GenerateOTP();
            if (await SendOTPEmail(email, newOTP))
            {
                otpStore[email] = (newOTP, DateTime.Now);
                lblCheck.Text = "Mã OTP mới đã được gửi tới Email của bạn!";
            }
            else
            {
                lblCheck.Text = "Lỗi gửi mã OTP. Vui lòng kiểm tra lại email hoặc thử lại sau!";
            }
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            string email = tbEmail.Text.Trim();
            string otpInput = tbOTP.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otpInput))
            {
                lblCheck.Text = "Vui lòng nhập đầy đủ email và mã OTP.";
                return;
            }

            if (!otpStore.ContainsKey(email))
            {
                lblCheck.Text = "Mã OTP không tồn tại! Vui lòng yêu cầu mã mới.";
                return;
            }

            var otpData = otpStore[email];
            if ((DateTime.Now - otpData.createdTime).TotalMinutes > OTPExpirationMinutes)
            {
                otpStore.Remove(email);
                lblCheck.Text = "Mã OTP đã hết hạn! Vui lòng yêu cầu mã mới.";
                return;
            }

            if (otpInput != otpData.otp)
            {
                lblCheck.Text = "Mã OTP không chính xác!";
                return;
            }

            srcEmail = email;
            otpStore.Remove(email);

            SignUpInfo signUpInfo = new SignUpInfo(main, srcEmail);
            main.pnLogin.Controls.Clear();
            main.pnLogin.Controls.Add(signUpInfo);
        }

        private async void btnSignUpGoogle_Click(object sender, EventArgs e)
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

                    if (!IsEmailExists(srcEmail))
                    {
                        main.Activate();

                        SignUpInfo signUpInfo = new SignUpInfo(main, srcEmail);
                        main.pnLogin.Controls.Clear();
                        main.pnLogin.Controls.Add(signUpInfo);
                    }
                    else
                    {
                        main.Activate();
                        lblCheck.Text = "Email đã tồn tại, vui lòng đăng nhập!";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đăng nhập: " + ex.Message);
            }
        }

        private void lbDangnhap_Click(object sender, EventArgs e)
        {
            Login login = new Login(main);
            main.pnLogin.Controls.Clear();
            main.pnLogin.Controls.Add(login);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            main.Close();
        }

        private void tbEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnNext_Click(sender, e);
            }
        }

        private void tbOTP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSignUp_Click(sender, e);
            }
        }
    }
}