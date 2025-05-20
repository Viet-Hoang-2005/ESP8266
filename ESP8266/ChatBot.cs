using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESP8266
{

    public partial class ChatBot : Form
    {
        private readonly HttpClient httpClient = new HttpClient();
        private readonly string geminiApiKey = "AIzaSyDYgaiUtUoh4HYayXW6UqYIVj34Lsqdshk"; // API key cho Gemini API

        // Biến lưu trữ dữ liệu môi trường từ form MenuForm
        private string currentTemperature = "Không có dữ liệu";
        private string currentHumidity = "Không có dữ liệu";

        // Biến lưu trữ giá trị trước đó của nhiệt độ và độ ẩm để kiểm tra sự thay đổi
        private string prevTemperature = null;
        private string prevHumidity = null;

        // Biến lưu trữ dữ liệu lịch sử
        private List<(DateTime Time, float? Temperature, float? Humidity)> historicalData = new List<(DateTime, float?, float?)>();

        private bool isRoleDefined = false; // Cờ để kiểm tra xem vai trò đã được định nghĩa chưa

        public ChatBot()
        {
            InitializeComponent();
            StartNewConversation(); // Hiển thị thông báo chào hỏi khi khởi tạo
        }

        // Cập nhật dữ liệu môi trường từ form MenuForm
        public void UpdateTemperatureAndHumidity(string temperature, string humidity)
        {
            currentTemperature = temperature;
            currentHumidity = humidity;

            // Chỉ cập nhật dữ liệu nếu dữ liệu thay đổi
            if (currentTemperature != prevTemperature || currentHumidity != prevHumidity)
            {
                prevTemperature = currentTemperature;
                prevHumidity = currentHumidity;
            }
        }

        // Cập nhật dữ liệu lịch sử từ MenuForm
        public void UpdateHistoricalData(List<(DateTime Time, float? Temperature, float? Humidity)> feeds)
        {
            historicalData = feeds; // Cập nhật danh sách lịch sử
        }

        // Phương thức gửi tin nhắn đến ChatBot
        private async void btnSend_Click(object sender, EventArgs e)
        {
            string userInput = tbChat.Text.Trim();
            if (string.IsNullOrEmpty(userInput))
                return;

            AppendMessage($"Bạn: {userInput}\n"); // Hiển thị câu hỏi của người dùng trên RichTextBox
            string prompt = !isRoleDefined ? DefineRole() + GetCurrentEnvironmentData() + GetHistoricalDataSummary() + $"Câu hỏi: {userInput}" // Định nghĩa vai trò và gửi câu hỏi
                                        : GetCurrentEnvironmentData() + GetHistoricalDataSummary() + $"Câu hỏi: {userInput}";
            tbChat.Text = "";
            isRoleDefined = true;

            string response = await GetGeminiResponse(prompt); // Nhận phản hồi từ Gemini
            AppendMessage($"ChatBot: {response}\n\n"); // Hiển thị phản hồi từ Gemini trên RichTextBox
        }

        // Phương thức tạo cuộc trò chuyện mới
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            rtbMessages.Clear();
            isRoleDefined = false;

            StartNewConversation(); // Bắt đầu cuộc trò chuyện mới
            historicalData.Clear(); // Xóa dữ liệu lịch sử khi khởi tạo lại
        }

        private void tbChat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSend_Click(sender, e);
                e.SuppressKeyPress = true;
            }
        }

        // Lời giới thiệu khi bắt đầu cuộc trò chuyện mới
        private void StartNewConversation()
        {
            AppendMessage($"ChatBot: Xin chào! Tôi là ChatBot, sẵn sàng giúp bạn đánh giá trạng thái môi trường hiện tại và lịch sử.\n");
        }

        // Định nghĩa vai trò cho Gemini
        private string DefineRole()
        {
            return "Bạn là một trợ lý AI đánh giá trạng thái môi trường dựa trên dữ liệu nhiệt độ và độ ẩm hiện tại và lịch sử. Cung cấp phân tích ngắn gọn (tối đa 150 từ) và khuyến nghị nếu cần. ";
        }

        // Lấy dữ liệu môi trường hiện tại
        private string GetCurrentEnvironmentData()
        {
            return $"Dữ liệu môi trường hiện tại: Nhiệt độ = {prevTemperature}°C, Độ ẩm = {prevHumidity}%. ";
        }

        // Lấy tóm tắt dữ liệu lịch sử
        private string GetHistoricalDataSummary()
        {
            if (historicalData == null || !historicalData.Any())
                return "Không có dữ liệu lịch sử. ";

            var summary = new StringBuilder();
            summary.Append("Dữ liệu lịch sử (một phần): ");
            
            // Lấy 5 bản ghi gần nhất làm ví dụ
            var recentRecords = historicalData.OrderByDescending(f => f.Time).Take(100).ToList();
            foreach (var record in recentRecords)
            {
                summary.Append($"[{record.Time:yyyy-MM-dd hh:mm:ss tt}] Nhiệt độ: {record.Temperature ?? 0}°C, Độ ẩm: {record.Humidity ?? 0}%; ");
            }
            return summary.ToString();
        }

        // Gửi yêu cầu đến Gemini API và nhận phản hồi
        private async Task<string> GetGeminiResponse(string prompt)
        {
            try
            {
                string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={geminiApiKey}"; // URL của Gemini API với tham số là endpoint generateContent của mô hình gemini-1.5-flash và API key

                // Tạo đối tượng với cấu trúc JSON theo định dạng yêu cầu của Gemini API
                var requestBody = new
                {
                    contents = new[]
                    {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
                };

                var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody), System.Text.Encoding.UTF8, "application/json"); // Chuyển đổi đối tượng thành JSON để gửi yêu cầu đến API
                var response = await httpClient.PostAsync(url, content); // Gửi yêu cầu POST bất đồng bộ đến URL đã chỉ định, với nội dung là content
                response.EnsureSuccessStatusCode(); // Kiểm tra xem phản hồi có mã trạng thái HTTP thành công không

                string responseBody = await response.Content.ReadAsStringAsync(); // Đọc nội dung phản hồi từ API dưới dạng chuỗi JSON
                JObject jsonResponse = JObject.Parse(responseBody); // Chuyển đổi chuỗi JSON thành đối tượng JObject để dễ dàng truy cập các thuộc tính

                // Trích xuất nội dung phản hồi từ Gemini
                string result = jsonResponse["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
                return result ?? "Không nhận được phản hồi từ Gemini.";
            }
            catch (Exception ex)
            {
                return $"Lỗi: {ex.Message}";
            }
        }

        // Phương thức thêm tin nhắn vào RichTextBox
        private void AppendMessage(string message)
        {
            rtbMessages.SelectionStart = rtbMessages.TextLength;
            rtbMessages.SelectionLength = 0;

            rtbMessages.AppendText(message);
            rtbMessages.ScrollToCaret();
        }
    }
}