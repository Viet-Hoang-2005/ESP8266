namespace ESP8266
{
    partial class HomeContent
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
            this.pnTemperature = new Guna.UI2.WinForms.Guna2Panel();
            this.pvTemperature = new OxyPlot.WindowsForms.PlotView();
            this.pvHumidity = new OxyPlot.WindowsForms.PlotView();
            this.pnHumidity = new Guna.UI2.WinForms.Guna2Panel();
            this.pnStatus = new Guna.UI2.WinForms.Guna2Panel();
            this.btnTemperature = new Guna.UI2.WinForms.Guna2Button();
            this.lbRelay = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lbTemperature = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.btnHumidity = new Guna.UI2.WinForms.Guna2Button();
            this.lbHuminity = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.btnRelay = new Guna.UI2.WinForms.Guna2Button();
            this.lblRefreshData = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblStatus = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel6 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pnHome = new Guna.UI2.WinForms.Guna2Panel();
            this.btnChatbot = new Guna.UI2.WinForms.Guna2Button();
            this.pnTemperature.SuspendLayout();
            this.pnHumidity.SuspendLayout();
            this.pnStatus.SuspendLayout();
            this.pnHome.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnTemperature
            // 
            this.pnTemperature.Controls.Add(this.pvTemperature);
            this.pnTemperature.Location = new System.Drawing.Point(33, 468);
            this.pnTemperature.Name = "pnTemperature";
            this.pnTemperature.Size = new System.Drawing.Size(1091, 352);
            this.pnTemperature.TabIndex = 2;
            // 
            // pvTemperature
            // 
            this.pvTemperature.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.pvTemperature.Location = new System.Drawing.Point(18, 3);
            this.pvTemperature.Name = "pvTemperature";
            this.pvTemperature.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.pvTemperature.Size = new System.Drawing.Size(1048, 346);
            this.pvTemperature.TabIndex = 0;
            this.pvTemperature.Text = "plotView1";
            this.pvTemperature.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.pvTemperature.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.pvTemperature.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
            // 
            // pvHumidity
            // 
            this.pvHumidity.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.pvHumidity.Location = new System.Drawing.Point(18, 8);
            this.pvHumidity.Name = "pvHumidity";
            this.pvHumidity.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.pvHumidity.Size = new System.Drawing.Size(1048, 346);
            this.pvHumidity.TabIndex = 0;
            this.pvHumidity.Text = "plotView1";
            this.pvHumidity.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.pvHumidity.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.pvHumidity.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
            // 
            // pnHumidity
            // 
            this.pnHumidity.Controls.Add(this.pvHumidity);
            this.pnHumidity.Location = new System.Drawing.Point(33, 847);
            this.pnHumidity.Name = "pnHumidity";
            this.pnHumidity.Size = new System.Drawing.Size(1091, 354);
            this.pnHumidity.TabIndex = 2;
            // 
            // pnStatus
            // 
            this.pnStatus.BackColor = System.Drawing.Color.White;
            this.pnStatus.Controls.Add(this.btnTemperature);
            this.pnStatus.Controls.Add(this.lbRelay);
            this.pnStatus.Controls.Add(this.lbTemperature);
            this.pnStatus.Controls.Add(this.btnHumidity);
            this.pnStatus.Controls.Add(this.lbHuminity);
            this.pnStatus.Controls.Add(this.btnRelay);
            this.pnStatus.Location = new System.Drawing.Point(33, 188);
            this.pnStatus.Name = "pnStatus";
            this.pnStatus.Size = new System.Drawing.Size(1091, 189);
            this.pnStatus.TabIndex = 3;
            // 
            // btnTemperature
            // 
            this.btnTemperature.BorderRadius = 10;
            this.btnTemperature.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnTemperature.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnTemperature.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnTemperature.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnTemperature.FillColor = System.Drawing.Color.DarkOrange;
            this.btnTemperature.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.btnTemperature.ForeColor = System.Drawing.Color.Black;
            this.btnTemperature.Location = new System.Drawing.Point(503, 48);
            this.btnTemperature.Name = "btnTemperature";
            this.btnTemperature.Size = new System.Drawing.Size(267, 113);
            this.btnTemperature.TabIndex = 1;
            this.btnTemperature.Text = "XX°C";
            // 
            // lbRelay
            // 
            this.lbRelay.BackColor = System.Drawing.Color.Transparent;
            this.lbRelay.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRelay.Location = new System.Drawing.Point(18, 9);
            this.lbRelay.Name = "lbRelay";
            this.lbRelay.Size = new System.Drawing.Size(168, 33);
            this.lbRelay.TabIndex = 0;
            this.lbRelay.Text = "Trạng thái Relay";
            // 
            // lbTemperature
            // 
            this.lbTemperature.BackColor = System.Drawing.Color.Transparent;
            this.lbTemperature.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTemperature.Location = new System.Drawing.Point(503, 9);
            this.lbTemperature.Name = "lbTemperature";
            this.lbTemperature.Size = new System.Drawing.Size(92, 33);
            this.lbTemperature.TabIndex = 0;
            this.lbTemperature.Text = "Nhiệt độ";
            // 
            // btnHumidity
            // 
            this.btnHumidity.BorderRadius = 10;
            this.btnHumidity.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnHumidity.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnHumidity.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnHumidity.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnHumidity.FillColor = System.Drawing.Color.DeepSkyBlue;
            this.btnHumidity.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.btnHumidity.ForeColor = System.Drawing.Color.Black;
            this.btnHumidity.Location = new System.Drawing.Point(799, 48);
            this.btnHumidity.Name = "btnHumidity";
            this.btnHumidity.Size = new System.Drawing.Size(267, 113);
            this.btnHumidity.TabIndex = 1;
            this.btnHumidity.Text = "YY%";
            // 
            // lbHuminity
            // 
            this.lbHuminity.BackColor = System.Drawing.Color.Transparent;
            this.lbHuminity.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbHuminity.Location = new System.Drawing.Point(799, 9);
            this.lbHuminity.Name = "lbHuminity";
            this.lbHuminity.Size = new System.Drawing.Size(70, 33);
            this.lbHuminity.TabIndex = 0;
            this.lbHuminity.Text = "Độ ẩm";
            // 
            // btnRelay
            // 
            this.btnRelay.BorderRadius = 10;
            this.btnRelay.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnRelay.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnRelay.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnRelay.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnRelay.FillColor = System.Drawing.Color.Gray;
            this.btnRelay.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.btnRelay.ForeColor = System.Drawing.Color.Black;
            this.btnRelay.Location = new System.Drawing.Point(18, 48);
            this.btnRelay.Name = "btnRelay";
            this.btnRelay.Size = new System.Drawing.Size(231, 113);
            this.btnRelay.TabIndex = 1;
            this.btnRelay.Text = "None";
            // 
            // lblRefreshData
            // 
            this.lblRefreshData.BackColor = System.Drawing.Color.Transparent;
            this.lblRefreshData.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRefreshData.Location = new System.Drawing.Point(51, 141);
            this.lblRefreshData.Name = "lblRefreshData";
            this.lblRefreshData.Size = new System.Drawing.Size(331, 25);
            this.lblRefreshData.TabIndex = 0;
            this.lblRefreshData.Text = "Dữ liệu được làm mới lúc: không có dữ liệu\r\n\r\n\r\n\r\n";
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(51, 96);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(475, 39);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Trạng thái hiện tại: không có dữ liệu!";
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(201, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(764, 47);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Giám sát Thiết bị đo nhiệt độ và độ ẩm Real-time";
            // 
            // guna2HtmlLabel6
            // 
            this.guna2HtmlLabel6.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel6.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel6.Location = new System.Drawing.Point(54, 414);
            this.guna2HtmlLabel6.Name = "guna2HtmlLabel6";
            this.guna2HtmlLabel6.Size = new System.Drawing.Size(100, 39);
            this.guna2HtmlLabel6.TabIndex = 0;
            this.guna2HtmlLabel6.Text = "Biểu đồ";
            // 
            // pnHome
            // 
            this.pnHome.AutoScroll = true;
            this.pnHome.Controls.Add(this.btnChatbot);
            this.pnHome.Controls.Add(this.lblRefreshData);
            this.pnHome.Controls.Add(this.lblStatus);
            this.pnHome.Controls.Add(this.lblTitle);
            this.pnHome.Controls.Add(this.guna2HtmlLabel6);
            this.pnHome.Controls.Add(this.pnStatus);
            this.pnHome.Controls.Add(this.pnHumidity);
            this.pnHome.Controls.Add(this.pnTemperature);
            this.pnHome.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnHome.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnHome.Location = new System.Drawing.Point(0, 0);
            this.pnHome.Name = "pnHome";
            this.pnHome.Size = new System.Drawing.Size(1174, 868);
            this.pnHome.TabIndex = 2;
            // 
            // btnChatbot
            // 
            this.btnChatbot.BorderRadius = 40;
            this.btnChatbot.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnChatbot.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnChatbot.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnChatbot.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnChatbot.FillColor = System.Drawing.Color.Transparent;
            this.btnChatbot.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnChatbot.ForeColor = System.Drawing.Color.White;
            this.btnChatbot.Image = global::ESP8266.Properties.Resources.chatbot;
            this.btnChatbot.ImageSize = new System.Drawing.Size(100, 100);
            this.btnChatbot.Location = new System.Drawing.Point(1050, 105);
            this.btnChatbot.Name = "btnChatbot";
            this.btnChatbot.Size = new System.Drawing.Size(70, 60);
            this.btnChatbot.TabIndex = 4;
            this.btnChatbot.Click += new System.EventHandler(this.btnChatbot_Click);
            // 
            // HomeContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pnHome);
            this.Name = "HomeContent";
            this.Size = new System.Drawing.Size(1174, 868);
            this.Tag = "";
            this.pnTemperature.ResumeLayout(false);
            this.pnHumidity.ResumeLayout(false);
            this.pnStatus.ResumeLayout(false);
            this.pnStatus.PerformLayout();
            this.pnHome.ResumeLayout(false);
            this.pnHome.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Button btnChatbot;
        private Guna.UI2.WinForms.Guna2Panel pnTemperature;
        private OxyPlot.WindowsForms.PlotView pvTemperature;
        private OxyPlot.WindowsForms.PlotView pvHumidity;
        private Guna.UI2.WinForms.Guna2Panel pnHumidity;
        private Guna.UI2.WinForms.Guna2Panel pnStatus;
        private Guna.UI2.WinForms.Guna2Button btnTemperature;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbRelay;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbTemperature;
        private Guna.UI2.WinForms.Guna2Button btnHumidity;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbHuminity;
        private Guna.UI2.WinForms.Guna2Button btnRelay;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblRefreshData;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblStatus;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel6;
        private Guna.UI2.WinForms.Guna2Panel pnHome;
    }
}
