namespace ESP8266
{
    partial class ChatBot
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbChat = new Guna.UI2.WinForms.Guna2TextBox();
            this.pnChatBot = new Guna.UI2.WinForms.Guna2Panel();
            this.rtbMessages = new System.Windows.Forms.RichTextBox();
            this.btnRefresh = new Guna.UI2.WinForms.Guna2Button();
            this.btnSend = new Guna.UI2.WinForms.Guna2Button();
            this.pnChatBot.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbChat
            // 
            this.tbChat.BorderRadius = 20;
            this.tbChat.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbChat.DefaultText = "";
            this.tbChat.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbChat.ForeColor = System.Drawing.Color.Black;
            this.tbChat.Location = new System.Drawing.Point(3, 521);
            this.tbChat.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbChat.Name = "tbChat";
            this.tbChat.PlaceholderText = "Nhập tin nhắn";
            this.tbChat.SelectedText = "";
            this.tbChat.Size = new System.Drawing.Size(929, 48);
            this.tbChat.TabIndex = 1;
            this.tbChat.TextOffset = new System.Drawing.Point(10, 0);
            this.tbChat.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbChat_KeyDown);
            // 
            // pnChatBot
            // 
            this.pnChatBot.Controls.Add(this.rtbMessages);
            this.pnChatBot.Controls.Add(this.tbChat);
            this.pnChatBot.Controls.Add(this.btnRefresh);
            this.pnChatBot.Controls.Add(this.btnSend);
            this.pnChatBot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnChatBot.Location = new System.Drawing.Point(0, 0);
            this.pnChatBot.Name = "pnChatBot";
            this.pnChatBot.Size = new System.Drawing.Size(1069, 590);
            this.pnChatBot.TabIndex = 3;
            // 
            // rtbMessages
            // 
            this.rtbMessages.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.rtbMessages.Location = new System.Drawing.Point(3, 0);
            this.rtbMessages.Name = "rtbMessages";
            this.rtbMessages.Size = new System.Drawing.Size(1063, 497);
            this.rtbMessages.TabIndex = 3;
            this.rtbMessages.Text = "";
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.Transparent;
            this.btnRefresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRefresh.BorderRadius = 20;
            this.btnRefresh.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnRefresh.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnRefresh.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnRefresh.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnRefresh.FillColor = System.Drawing.Color.Transparent;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Image = global::ESP8266.Properties.Resources.icons8_refresh_96;
            this.btnRefresh.ImageSize = new System.Drawing.Size(50, 50);
            this.btnRefresh.Location = new System.Drawing.Point(1018, 521);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(48, 48);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnSend
            // 
            this.btnSend.BackColor = System.Drawing.Color.Transparent;
            this.btnSend.BackgroundImage = global::ESP8266.Properties.Resources.icons8_email_send_96;
            this.btnSend.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSend.BorderRadius = 20;
            this.btnSend.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnSend.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnSend.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnSend.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnSend.FillColor = System.Drawing.Color.Transparent;
            this.btnSend.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSend.ForeColor = System.Drawing.Color.White;
            this.btnSend.Location = new System.Drawing.Point(954, 521);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(48, 48);
            this.btnSend.TabIndex = 2;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // ChatBot
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pnChatBot);
            this.Name = "ChatBot";
            this.Size = new System.Drawing.Size(1069, 590);
            this.pnChatBot.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private Guna.UI2.WinForms.Guna2TextBox tbChat;
        private Guna.UI2.WinForms.Guna2Button btnSend;
        private Guna.UI2.WinForms.Guna2Panel pnChatBot;
        private System.Windows.Forms.RichTextBox rtbMessages;
        private Guna.UI2.WinForms.Guna2Button btnRefresh;
    }
}
