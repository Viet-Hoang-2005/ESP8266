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
using System.Security.Cryptography;

namespace ESP8266
{
    public partial class ForgetPassword : UserControl
    {
        private MainForm main;
        private Dictionary<string, (string otp, DateTime createdTime)> otpStore;
        private const string connString = "Host=aws-0-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.bzjfiynoyelxlpowlhty;Password=laptrinhmang;SSL Mode=Require;Trust Server Certificate=true";

        private const string SmtpUsername = "sona.feelthemusic@gmail.com";
        private const string SmtpPassword = "sxiz nqtc ddko izzj";

        private int OTPResendCooldown = 60;
        private int OTPExpirationMinutes = 5;

        public ForgetPassword(MainForm mainForm)
        {
            InitializeComponent();
            main = mainForm;

            lblCheckEmail.Text = lblCheckOTP.Text = lblCheckConfirmPass.Text = "";
            lblCheckPass.ForeColor = Color.FromArgb(102, 102, 102);

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
                lblCheckEmail.Text = $"Lỗi kết nối: {ex.Message}";
                return false;
            }
        }

        // Hàm mã hóa mật khẩu bằng SHA256 (để đồng bộ với form Login nếu cần)
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
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
                    Subject = $"Mã OTP xác nhận quên mật khẩu tài khoản SONA",
                    Body = $"Mã OTP của bạn là: <strong>{otp}</strong><br>Vui lòng nhập mã này để hoàn tất việc đổi mật khẩu. Mã có hiệu lực trong {OTPExpirationMinutes} phút.",
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

        private bool checkPassFormat()
        {
            // Kiểm tra mật khẩu có phù hợp với yêu cầu không
            bool checkNum = false;
            bool checkLetter = false;
            bool checkSpecial = false;

            for (int i = 0; i < tbPass.Text.Length; i++)
            {
                if (char.IsDigit(tbPass.Text[i]))
                    checkNum = true;
                else if (char.IsLetter(tbPass.Text[i]))
                    checkLetter = true;
                else if (tbPass.Text[i] == '@' || tbPass.Text[i] == '#' || tbPass.Text[i] == '!' || tbPass.Text[i] == '?')
                    checkSpecial = true;
            }

            if (!checkNum || !checkLetter || !checkSpecial)
            {
                lblCheckPass.ForeColor = Color.Red;
                return false;
            }

            if (string.IsNullOrEmpty(tbConfirmPass.Text))
            {
                lblCheckConfirmPass.Text = "Vui lòng xác nhận mật khẩu!";
                return false;
            }

            if (tbPass.Text != tbConfirmPass.Text)
            {
                lblCheckConfirmPass.Text = "Mật khẩu nhập lại chưa chính xác!";
                return false;
            }
            return true;
        }

        private void lblLogin_Click(object sender, EventArgs e)
        {
            Login login = new Login(main);
            main.pnLogin.Controls.Clear();
            main.pnLogin.Controls.Add(login);
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            lblCheckEmail.Text = lblCheckOTP.Text = lblCheckConfirmPass.Text = "";
            lblCheckPass.ForeColor = Color.FromArgb(102, 102, 102);

            string email = tbEmail.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                lblCheckEmail.Text = "Vui lòng nhập email.";
                return;
            }

            if (!IsValidEmail(email))
            {
                lblCheckEmail.Text = "Địa chỉ email không hợp lệ!";
                return;
            }

            if (!IsEmailExists(email))
            {
                lblCheckEmail.Text = "Tài khoản Email chưa tồn tại, vui lòng đăng ký.";
                return;
            }

            if (otpStore.ContainsKey(email))
            {
                var otpData = otpStore[email];
                if ((DateTime.Now - otpData.createdTime).TotalSeconds < OTPResendCooldown)
                {
                    lblCheckOTP.Text = $"Vui lòng chờ {Math.Ceiling(OTPResendCooldown - (DateTime.Now - otpData.createdTime).TotalSeconds)} giây trước khi gửi lại!";
                    return;
                }
            }

            string otp = GenerateOTP();
            if (await SendOTPEmail(email, otp))
            {
                otpStore[email] = (otp, DateTime.Now);
                lblCheckOTP.Text = "Mã OTP đã được gửi tới Email của bạn!";
            }
            else
            {
                lblCheckOTP.Text = "Lỗi gửi mã OTP. Vui lòng kiểm tra lại email hoặc thử lại sau!";
            }
        }

        private async void btnRefreshOTP_Click(object sender, EventArgs e)
        {
            lblCheckEmail.Text = lblCheckOTP.Text = lblCheckConfirmPass.Text = "";
            lblCheckPass.ForeColor = Color.FromArgb(102, 102, 102);

            string email = tbEmail.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                lblCheckOTP.Text = "Vui lòng nhập email trước khi yêu cầu OTP.";
                return;
            }

            if (!IsValidEmail(email))
            {
                lblCheckEmail.Text = "Địa chỉ email không hợp lệ!";
                return;
            }

            if (otpStore.ContainsKey(email))
            {
                var otpData = otpStore[email];
                double remainingSeconds = OTPResendCooldown - (DateTime.Now - otpData.createdTime).TotalSeconds;
                if (remainingSeconds > 0)
                {
                    lblCheckOTP.Text = $"Vui lòng chờ {Math.Ceiling(remainingSeconds)} giây trước khi gửi lại!";
                    return;
                }
            }

            string newOTP = GenerateOTP();
            if (await SendOTPEmail(email, newOTP))
            {
                otpStore[email] = (newOTP, DateTime.Now);
                lblCheckOTP.Text = "Mã OTP mới đã được gửi tới Email của bạn!";
            }
            else
            {
                lblCheckOTP.Text = "Lỗi gửi mã OTP. Vui lòng kiểm tra lại email hoặc thử lại sau!";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            main.Close();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            lblCheckEmail.Text = lblCheckOTP.Text = lblCheckConfirmPass.Text = "";
            lblCheckPass.ForeColor = Color.FromArgb(102, 102, 102);

            string email = tbEmail.Text.Trim();
            string otpInput = tbOTP.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                lblCheckEmail.Text = "Vui lòng nhập địa chỉ Email!";
                return;
            }

            if (string.IsNullOrEmpty(otpInput))
            {
                lblCheckOTP.Text = "Vui lòng nhập mã OTP!";
                return; // Sửa: Thêm return để dừng nếu OTP trống
            }

            if (!otpStore.ContainsKey(email))
            {
                lblCheckOTP.Text = "Mã OTP không tồn tại! Vui lòng yêu cầu mã mới.";
                return;
            }

            var otpData = otpStore[email];
            if ((DateTime.Now - otpData.createdTime).TotalMinutes > OTPExpirationMinutes)
            {
                otpStore.Remove(email);
                lblCheckOTP.Text = "Mã OTP đã hết hạn! Vui lòng yêu cầu mã mới.";
                return;
            }

            if (otpInput != otpData.otp)
            {
                lblCheckOTP.Text = "Mã OTP không chính xác!";
                return;
            }

            if (!checkPassFormat()) return;

            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string query = "UPDATE users SET password = @password WHERE email = @email";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@password", tbPass.Text);
                        cmd.Parameters.AddWithValue("@email", email);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            otpStore.Remove(email); // Xóa OTP sau khi cập nhật thành công

                            Login login = new Login(main);
                            main.pnLogin.Controls.Clear();
                            main.pnLogin.Controls.Add(login); // Chuyển về form Login
                        }
                        else
                        {
                            lblCheckEmail.Text = "Không tìm thấy tài khoản để đổi mật khẩu.";
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                lblCheckEmail.Text = $"Lỗi đổi mật khẩu: {ex.Message}";
            }
            catch (Exception ex)
            {
                lblCheckEmail.Text = $"Lỗi không xác định: {ex.Message}";
            }
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
                btnConfirm_Click(sender, e);
            }
        }

        private void tbPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnConfirm_Click(sender, e);
            }
        }

        private void tbConfirmPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnConfirm_Click(sender, e);
            }
        }
    }
}