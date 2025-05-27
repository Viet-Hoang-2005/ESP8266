using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Supabase.Gotrue;
using System.Text.RegularExpressions;

namespace ESP8266
{
    public partial class UserInfo : UserControl
    {
        private string idUser;
        private string srcEmail;
        private MainForm main;
        private const string connString = "Host=aws-0-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.bzjfiynoyelxlpowlhty;Password=laptrinhmang;SSL Mode=Require;Trust Server Certificate=true";

        public UserInfo(MainForm mainForm, string email)
        {
            InitializeComponent();
            main = mainForm;
            srcEmail = email;

            InitializeUser();
            InitializeDevice();
        }

        private void InitializeUser()
        {
            lblCheckName.Text = lblCheckSdt.Text = lblCheckEmail.Text = lblCheckConfirm.Text = lblStatus.Text = "";
            lblcheckPass.ForeColor = Color.FromArgb(102, 102, 102);

            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT * FROM users WHERE email = @email";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", srcEmail);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idUser = reader["id_user"].ToString();
                                tbNameUser.Text = lblNameUser.Text = reader["name_user"].ToString();
                                tbSdt.Text = lblSdt.Text = reader["phone_number"].ToString();
                                tbEmail.Text = lblEmail.Text = reader["email"].ToString();
                                tbEmail.Enabled = false;
                                tbPass.Text = reader["password"].ToString();
                                tbConfirm.Text = reader["password"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải thông tin người dùng: " + ex.Message);
            }
        }

        private void InitializeDevice()
        {
            lblCheckNameDevice.Text = lblCheckIDChannel.Text = lblCheckAPIKey.Text = "";

            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT * FROM devices WHERE id_user = @id_user";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_user", int.Parse(idUser));
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                tbNameDevice.Text = reader["device_name"].ToString();
                                tbAPIKey.Text = reader["api_key"].ToString();
                                tbIDChannel.Text = reader["channel_id"].ToString();
                                tbDescription.Text = reader["description"].ToString();
                            }
                            else
                            {
                                // Nếu không có thiết bị, để trống các trường
                                tbNameDevice.Text = "";
                                tbAPIKey.Text = "";
                                tbIDChannel.Text = "";
                                tbDescription.Text = "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải thông tin thiết bị: " + ex.Message);
            }
        }

        // Hàm kiểm tra thiết bị đã tồn tại trong cơ sở dữ liệu chưa
        private bool DeviceExists()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM devices WHERE id_user = @id_user";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_user", int.Parse(idUser));
                        long count = (long)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Lỗi kiểm tra thiết bị: {ex.Message}");
                return false;
            }
        }

        private bool checkUserInfo()
        {
            lblCheckName.Text = lblCheckSdt.Text = lblCheckEmail.Text = lblCheckConfirm.Text = lblStatus.Text = "";
            lblcheckPass.ForeColor = Color.FromArgb(102, 102, 102);
            lblStatus.ForeColor = Color.Green;

            if (string.IsNullOrWhiteSpace(tbNameUser.Text))
            {
                lblCheckName.Text = "Tên người dùng không được để trống!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(tbSdt.Text))
            {
                lblCheckSdt.Text = "Số điện thoại không được để trống!";
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

            if (string.IsNullOrWhiteSpace(tbPass.Text))
            {
                lblcheckPass.ForeColor = Color.Red;
                return false;
            }

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
            if (string.IsNullOrWhiteSpace(tbConfirm.Text))
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

        private bool checkDevice()
        {
            lblCheckNameDevice.Text = lblCheckIDChannel.Text = lblCheckAPIKey.Text = "";

            if (string.IsNullOrWhiteSpace(tbNameDevice.Text))
            {
                lblCheckNameDevice.Text = "Vui lòng nhập tên thiết bị!";
                return false;
            }
            if (string.IsNullOrEmpty(tbIDChannel.Text))
            {
                lblCheckIDChannel.Text = "Vui lòng nhập ID channel";
                return false;
            }
            if (string.IsNullOrEmpty(tbAPIKey.Text))
            {
                lblCheckAPIKey.Text = "Vui lòng nhập API key";
                return false;
            }
            return true;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            InitializeUser();
            InitializeDevice();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Kiểm tra thông tin người dùng và thiết bị
            if (!checkUserInfo() || !checkDevice())
            {
                return;
            }

            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    // Cập nhật thông tin người dùng trong bảng users
                    string userQuery = "UPDATE users SET name_user = @name_user, phone_number = @phone_number, password = @password WHERE id_user = @id_user";
                    using (var userCmd = new NpgsqlCommand(userQuery, conn))
                    {
                        userCmd.Parameters.AddWithValue("@id_user", int.Parse(idUser));
                        userCmd.Parameters.AddWithValue("@name_user", tbNameUser.Text.Trim());
                        userCmd.Parameters.AddWithValue("@phone_number", tbSdt.Text.Trim());
                        userCmd.Parameters.AddWithValue("@password", tbPass.Text.Trim());

                        int userRowsAffected = userCmd.ExecuteNonQuery();
                        if (userRowsAffected == 0)
                        {
                            lblStatus.Text = "Không thể cập nhật thông tin người dùng!";
                            lblStatus.ForeColor = Color.Red;
                            return;
                        }
                    }

                    // Kiểm tra thiết bị đã tồn tại hay chưa
                    if (DeviceExists())
                    {
                        // Nếu thiết bị đã tồn tại, thực hiện UPDATE
                        string deviceQuery = "UPDATE devices SET device_name = @device_name, channel_id = @channel_id, api_key = @api_key, description = @description WHERE id_user = @id_user";
                        using (var deviceCmd = new NpgsqlCommand(deviceQuery, conn))
                        {
                            deviceCmd.Parameters.AddWithValue("@id_user", int.Parse(idUser));
                            deviceCmd.Parameters.AddWithValue("@device_name", tbNameDevice.Text.Trim());
                            deviceCmd.Parameters.AddWithValue("@channel_id", tbIDChannel.Text.Trim());
                            deviceCmd.Parameters.AddWithValue("@api_key", tbAPIKey.Text.Trim());
                            deviceCmd.Parameters.AddWithValue("@description", tbDescription.Text.Trim());

                            int deviceRowsAffected = deviceCmd.ExecuteNonQuery();
                            if (deviceRowsAffected == 0)
                            {
                                lblStatus.Text = "Không thể cập nhật thông tin thiết bị!";
                                lblStatus.ForeColor = Color.Red;
                                return;
                            }
                        }
                    }
                    else
                    {
                        // Nếu thiết bị chưa tồn tại, thực hiện INSERT
                        string insertDeviceQuery = "INSERT INTO devices (id_user, device_name, channel_id, api_key, description) VALUES (@id_user, @device_name, @channel_id, @api_key, @description)";
                        using (var deviceCmd = new NpgsqlCommand(insertDeviceQuery, conn))
                        {
                            deviceCmd.Parameters.AddWithValue("@id_user", int.Parse(idUser));
                            deviceCmd.Parameters.AddWithValue("@device_name", tbNameDevice.Text.Trim());
                            deviceCmd.Parameters.AddWithValue("@channel_id", tbIDChannel.Text.Trim());
                            deviceCmd.Parameters.AddWithValue("@api_key", tbAPIKey.Text.Trim());
                            deviceCmd.Parameters.AddWithValue("@description", tbDescription.Text.Trim());

                            deviceCmd.ExecuteNonQuery();
                        }
                    }    

                    InitializeUser();
                    InitializeDevice();

                    lblStatus.Text = "Cập nhật thông tin thành công!";
                }
            }
            catch (NpgsqlException ex)
            {
                if (ex.Message.Contains("duplicate key value violates unique constraint"))
                {
                    if (ex.Message.Contains("phone_number"))
                        lblCheckSdt.Text = "Số điện thoại đã tồn tại!";
                }
                else
                {
                    lblCheckSdt.Text = "Lỗi khi cập nhật dữ liệu: " + ex.Message;
                    lblStatus.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Lỗi không xác định: " + ex.Message;
                lblStatus.ForeColor = Color.Red;
            }
        }

        private void btnOut_Click(object sender, EventArgs e)
        {
            main.ShowLogin();
            //main.Close();
        }
    }
}