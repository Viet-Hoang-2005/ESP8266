using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace ESP8266
{
    public partial class SignUpInfo: UserControl
    {
        private MainForm main;
        private string srcEmail;
        private string connString = "Host=aws-0-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.bzjfiynoyelxlpowlhty;Password=laptrinhmang;SSL Mode=Require;Trust Server Certificate=true";

        public SignUpInfo(MainForm mainForm, string email)
        {
            InitializeComponent();
            main = mainForm;
            srcEmail = email;

            lblCheckName.Text = lblCheckSdt.Text = lblCheckConfirm.Text = "";
            lblcheckPass.ForeColor = Color.FromArgb(102, 102, 102);
        }

        private bool IsPhoneNumberExists(string phoneNumber)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE phone_number = @phone_number";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@phone_number", phoneNumber);
                        long count = (long)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                lblCheckSdt.Text = $"Lỗi kiểm tra số điện thoại: {ex.Message}";
                return false;
            }
        }

        // Hàm kiểm tra thông tin đăng nhập và báo lỗi nếu không hợp lệ
        private bool checkSignUpInfor()
        {
            // Đặt các giá trị mặc định ban đầu cho các label thông báo
            lblCheckName.Text = lblCheckSdt.Text = lblCheckConfirm.Text = "";
            lblcheckPass.ForeColor = Color.FromArgb(102, 102, 102);

            // Kiểm tra các trường thông tin có trống hay không
            if (string.IsNullOrEmpty(tbUser.Text))
            {
                lblCheckName.Text = "Tên người dùng không được để trống!";
                return false;
            }

            if (string.IsNullOrEmpty(tbSdt.Text))
            {
                lblCheckSdt.Text = "Vui lòng nhập số điện thoại!";
                return false;
            }

            for (int i = 0; i < tbSdt.Text.Length; i++)
            {
                if (!char.IsDigit(tbSdt.Text[i]))
                {
                    lblCheckSdt.Text = "Số điện thoại không hợp lệ!";
                    return false;
                }
            }

            if (IsPhoneNumberExists(tbSdt.Text))
            {
                lblCheckSdt.Text = "Số điện thoại đã tồn tại!";
                return false;
            }    

            if (string.IsNullOrEmpty(tbPass.Text))
            {
                lblcheckPass.ForeColor = Color.Red;
                return false;
            }

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
                lblcheckPass.ForeColor = Color.Red;
                return false;
            }

            if (string.IsNullOrEmpty(tbConfirm.Text))
            {
                lblCheckConfirm.Text = "Vui lòng xác nhận mật khẩu!";
                return false;
            }

            if (tbPass.Text != tbConfirm.Text)
            {
                lblCheckConfirm.Text = "Mật khẩu nhập lại chưa chính xác!";
                return false;
            }

            return true;
        }


        private void btnSignUp_Click(object sender, EventArgs e)
        {
            if (checkSignUpInfor())
            {
                try
                {
                    using (var conn = new NpgsqlConnection(connString))
                    {
                        conn.Open();
                        string query = "INSERT INTO users (name_user, email, phone_number, password) VALUES (@name_user, @email, @phone_number, @password)";
                        using (var cmd = new NpgsqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@name_user", tbUser.Text.Trim());
                            cmd.Parameters.AddWithValue("@email", srcEmail);
                            cmd.Parameters.AddWithValue("@phone_number", tbSdt.Text.Trim());
                            cmd.Parameters.AddWithValue("@password", tbPass.Text.Trim());

                            cmd.ExecuteNonQuery();
                            main.ShowHome(srcEmail);
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblCheckName.Text = "Lỗi thêm người dùng vào dữ liệu: " + ex.Message;
                }
            }
            
        }

        private void lblLogin_Click(object sender, EventArgs e)
        {
            Login login = new Login(main);
            main.pnLogin.Controls.Clear();
            main.pnLogin.Controls.Add(login);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            main.Close();
        }

        private void tbUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSignUp_Click(sender, e);
            }
        }

        private void tbSdt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSignUp_Click(sender, e);
            }
        }

        private void tbPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSignUp_Click(sender, e);
            }
        }

        private void tbConfirm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSignUp_Click(sender, e);
            }
        }
    }
}
