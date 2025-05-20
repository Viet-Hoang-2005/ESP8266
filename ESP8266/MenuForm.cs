using Newtonsoft.Json.Linq;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
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
    public partial class MenuForm : Form
    {
        // Các biến lưu trữ thông tin bao gồm Id và API từ ThingSpeak
        private string channelId = "2943515";
        private string readApiKey = "LRSNX2AOSRS49MZQ";

        private int refreshInterval = 30; // Thời gian làm mới dữ liệu là 30 giây
        private int espOfflineThreshold = 5 * 60; // Thời gian tối đa để coi ESP8266 là offline 5 phút
        private HttpClient httpClient = new HttpClient();

        private bool isFirstUpdate = false; // Cờ kiểm soát lần cập nhật đầu tiên
        private ChatBot chatBot = new ChatBot();

        public MenuForm()
        {
            InitializeComponent();
            StartDataRefresh();

            // Thêm sự kiện cho các biểu đồ để không bị cuộn khi dùng chuột
            pvTemperature.MouseWheel += PlotView_MouseWheel;
            pvHumidity.MouseWheel += PlotView_MouseWheel;

            StartPosition = FormStartPosition.CenterScreen;
        }

        // Hàm căn giữa cho panel để giữ bố cục khi form được phóng to
        private void MenuForm_Resize(object sender, EventArgs e)
        {
            pnHome.Location = new Point(
                (this.ClientSize.Width - pnHome.Width) / 2,
                pnHome.Location.Y
            );
        }

        // Hàm lấy dữ liệu từ ThingSpeak
        private async Task<JObject> FetchData(string url)
        {
            try
            {
                var response = await httpClient.GetStringAsync(url); // Gửi yêu cầu GET để lấy dữ liệu từ URL đã chỉ định
                return JObject.Parse(response); // Chuyển đổi chuỗi JSON thành đối tượng JObject để dễ dàng truy cập các thuộc tính
            }
            catch
            {
                return null;
            }
        }

        // Hàm cập nhật thời gian làm mới dữ liệu
        private void UpdateRefreshTime(string createdAt, DateTime vietnamTime)
        {
            if (createdAt != null)
            {
                try
                {
                    // Phân tích thời gian từ chuỗi JSON và đảm bảo nó là UTC
                    var createdAtUtc = DateTime.Parse(createdAt, null, System.Globalization.DateTimeStyles.RoundtripKind);
                    if (createdAtUtc.Kind != DateTimeKind.Utc)
                    {
                        createdAtUtc = DateTime.SpecifyKind(createdAtUtc, DateTimeKind.Utc); // Đảm bảo là UTC
                    }

                    // Chuyển đổi sang giờ Việt Nam (UTC+7)
                    var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                    var createdAtVietnam = TimeZoneInfo.ConvertTimeFromUtc(createdAtUtc, vietnamTimeZone);
                    
                    var timeDiff = (DateTime.UtcNow - createdAtUtc).TotalSeconds; // Tính thời gian chênh lệch để kiểm tra trạng thái ESP8266

                    lblStatus.Text = timeDiff <= espOfflineThreshold // Cập nhật trạng thái ESP8266 và thời gian làm mới dữ liệu
                        ? $"Trạng thái ESP8266: Online"
                        : $"Trạng thái ESP8266: Offline";
                    lblRefreshData.Text = $"Dữ liệu được làm mới lúc: {createdAtVietnam:yyyy-MM-dd hh:mm:ss tt}";

                }
                catch (Exception ex)
                {
                    lblStatus.Text = "Lỗi khi chuyển đổi thời gian.";
                    lblRefreshData.Text = $"Dữ liệu được làm mới lúc: {vietnamTime:yyyy-MM-dd hh:mm:ss tt} (Lỗi: {ex.Message})";
                }
            }
            else
            {
                lblStatus.Text = "Trạng thái ESP8266: Không có thông tin thời gian.";
                lblRefreshData.Text = $"Dữ liệu được làm mới lúc: {vietnamTime:yyyy-MM-dd hh:mm:ss tt} (Không có dữ liệu)";
            }
        }

        // Hàm cập nhật dữ liệu từ ThingSpeak
        private async Task UpdateData()
        {
            try
            {
                // Lấy dữ liệu mới nhất (1 bản ghi) từ ThingSpeak và chuyển đổi giờ UTC sang giờ Việt Nam
                var latestData = await FetchData($"https://api.thingspeak.com/channels/{channelId}/feeds.json?api_key={readApiKey}&results=1");
                var currentTimeUtc = DateTime.UtcNow;
                var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(currentTimeUtc, vietnamTimeZone);

                // Trích xuất các thông tin từ dữ liệu từ file JSON nhận được
                if (latestData != null)
                {
                    var feed = latestData["feeds"][0];
                    string temperature = feed["field1"]?.ToString() ?? "Không có dữ liệu";
                    string humidity = feed["field2"]?.ToString() ?? "Không có dữ liệu";
                    string relayState = feed["field3"]?.ToString();
                    string createdAt = feed["created_at"]?.ToString();

                    UpdateRefreshTime(createdAt, vietnamTime); // Cập nhật thời gian làm mới dữ liệu
                    UpdateRelayStatus(relayState); // Cập nhật trạng thái Relay
                    UpdateTemperatureAndHumidity(temperature, humidity); // Cập nhật nhiệt độ và độ ẩm

                    // Lấy dữ liệu lịch sử để vẽ biểu đồ và truyền cho ChatBot (200 bản ghi)
                    var recentData = await FetchData($"https://api.thingspeak.com/channels/{channelId}/feeds.json?api_key={readApiKey}&results=200");
                    if (recentData != null)
                    {
                        var feeds = recentData["feeds"]
                            .Select(f =>
                            {
                                float tempValue, humValue;
                                float? temp = float.TryParse(f["field1"]?.ToString(), out tempValue) ? (float?)Math.Round(tempValue, 2) : null;
                                float? hum = float.TryParse(f["field2"]?.ToString(), out humValue) ? (float?)Math.Round(humValue, 2) : null;

                                string createdAtStr = f["created_at"]?.ToString();
                                DateTime time;
                                if (!string.IsNullOrEmpty(createdAtStr))
                                {
                                    time = DateTime.Parse(createdAtStr, null, System.Globalization.DateTimeStyles.RoundtripKind);
                                    if (time.Kind != DateTimeKind.Utc)
                                    {
                                        time = DateTime.SpecifyKind(time, DateTimeKind.Utc);
                                    }
                                    time = TimeZoneInfo.ConvertTimeFromUtc(time, vietnamTimeZone); // Chuyển đổi sang giờ Việt Nam
                                }
                                else
                                {
                                    time = vietnamTime; // Sử dụng thời gian hiện tại đã chuyển đổi
                                }

                                return (
                                    Time: time,
                                    Temperature: temp,
                                    Humidity: hum
                                );
                            })
                            .Where(f => f.Temperature.HasValue || f.Humidity.HasValue)
                            .ToList();

                        // Vẽ biểu đồ nhiệt độ và độ ẩm
                        PlotTemperature(feeds);
                        PlotHumidity(feeds);

                        // Truyền dữ liệu lịch sử cho ChatBot
                        chatBot.UpdateHistoricalData(feeds);
                    }
                    else
                    {
                        pvTemperature.Model = new PlotModel { Title = "Không có dữ liệu nhiệt độ" };
                        pvHumidity.Model = new PlotModel { Title = "Không có dữ liệu độ ẩm" };
                        chatBot.UpdateHistoricalData(new List<(DateTime Time, float? Temperature, float? Humidity)>());
                    }
                }
                else
                {
                    lblStatus.Text = "Không có dữ liệu mới nhất từ ThingSpeak.";
                    pvTemperature.Model = new PlotModel { Title = "Không có dữ liệu nhiệt độ" };
                    pvHumidity.Model = new PlotModel { Title = "Không có dữ liệu độ ẩm" };
                    chatBot.UpdateHistoricalData(new List<(DateTime Time, float? Temperature, float? Humidity)>());
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Lỗi: {ex.Message}";
                pvTemperature.Model = new PlotModel { Title = $"Lỗi: {ex.Message}" };
                pvHumidity.Model = new PlotModel { Title = $"Lỗi: {ex.Message}" };
                chatBot.UpdateHistoricalData(new List<(DateTime Time, float? Temperature, float? Humidity)>());
            }
        }

        // Hàm cập nhật trạng thái Relay
        private void UpdateRelayStatus(string relayState)
        {
            if (relayState != null)
            {
                if (int.TryParse(relayState, out int relayValue))
                {
                    btnRelay.Text = relayValue == 1 ? "Bật" : "Tắt";
                    btnRelay.FillColor = relayValue == 1 ? Color.PaleGreen : Color.LightCoral;
                }
                else
                {
                    btnRelay.Text = "Dữ liệu không hợp lệ";
                    btnRelay.FillColor = Color.Gray;
                }
            }
            else
            {
                btnRelay.Text = "Không có dữ liệu";
                btnRelay.FillColor = Color.Gray;
            }
        }

        // Hàm cập nhật nhiệt độ và độ ẩm
        private void UpdateTemperatureAndHumidity(string temperature, string humidity)
        {
            if (temperature != "Không có dữ liệu" && float.TryParse(temperature, out float tempValue))
                btnTemperature.Text = $"{Math.Round(tempValue, 2)}°C";
            else
                btnTemperature.Text = "Không có dữ liệu";

            if (humidity != "Không có dữ liệu" && float.TryParse(humidity, out float humValue))
                btnHumidity.Text = $"{Math.Round(humValue, 2)}%";
            else
                btnHumidity.Text = "Không có dữ liệu";

            chatBot.UpdateTemperatureAndHumidity(temperature, humidity); // Cập nhật dữ liệu cho ChatBot
        }

        // Hàm làm mới dữ liệu từ ThingSpeak
        private async void StartDataRefresh()
        {
            while (!IsDisposed)
            {
                await UpdateData();

                // Nếu lần cập nhật đầu tiên đã hoàn thành, đợi 10 phút trước khi cập nhật tiếp theo
                if (isFirstUpdate)
                {
                    await Task.Delay(refreshInterval * 1000);
                }
                else
                {
                    isFirstUpdate = true; // Đánh dấu lần đầu tiên hoàn thành
                }
            }
        }

        // Hàm xử lý sự kiện cuộn chuột để không bị cuộn khi dùng chuột
        private void PlotView_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        // Hàm xử lý sự kiện thay đổi trục thời gian
        private void DateAxis_AxisChanged(object sender, AxisChangedEventArgs e)
        {
            var axis = sender as DateTimeAxis;
            if (axis == null) return;

            double range = axis.ActualMaximum - axis.ActualMinimum; // Tính khoảng cách giữa giá trị tối đa và tối thiểu

            if (range > 365) // > 1 năm
                axis.StringFormat = "yyyy";
            else if (range > 31) // > 1 tháng
                axis.StringFormat = "MM/yyyy";
            else if (range > 1) // > 1 ngày
                axis.StringFormat = "dd/MM";
            else if (range > 1.0 / 24) // > 1 giờ
                axis.StringFormat = "HH:mm";
            else // < 1 giờ
                axis.StringFormat = "HH:mm:ss";
        }

        // Hàm vẽ biểu đồ nhiệt độ dựa trên giá trị thời gian, nhiệt độ và độ ẩm
        private void PlotTemperature(List<(DateTime Time, float? Temperature, float? Humidity)> feeds)
        {
            var model = new PlotModel { Title = "Biểu đồ Nhiệt độ (Giờ Việt Nam)" };

            // Tạo trục thời gian
            var dateAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom, // Đặt trục thời gian ở dưới cùng
                Title = "Thời gian",
                IntervalType = DateTimeIntervalType.Auto, // Tự động điều chỉnh khoảng cách giữa các nhãn
                MajorGridlineStyle = LineStyle.Solid, // Đường lưới chính
                MinorGridlineStyle = LineStyle.Dot // Đường lưới phụ
            };

            // Thêm sự kiện điều chỉnh định dạng nhãn theo khoảng thời gian
            dateAxis.AxisChanged += DateAxis_AxisChanged;
            model.Axes.Add(dateAxis);

            // Tạo trục nhiệt độ
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left, // Đặt trục nhiệt độ ở bên trái
                Title = "Nhiệt độ (°C)",
                Minimum = 0, // Giá trị tối thiểu của trục là 0 độ
                Maximum = 50 //  Giá trị tối đa của trục là 50 độ
            });

            var tempSeries = new LineSeries
            {
                Title = "Nhiệt độ",
                Color = OxyColors.Orange
            };

            // Thêm các điểm dữ liệu vào biểu đồ
            foreach (var feed in feeds)
            {
                if (feed.Temperature.HasValue) // Kiểm tra xem nhiệt độ có giá trị hay không
                {
                    double xValue = DateTimeAxis.ToDouble(feed.Time); // Chuyển đổi thời gian thành giá trị số
                    tempSeries.Points.Add(new DataPoint(xValue, feed.Temperature.Value)); // Thêm điểm dữ liệu vào biểu đồ
                }
            }

            model.Series.Add(tempSeries);
            pvTemperature.Model = model;
        }

        // Hàm vẽ biểu đồ độ ẩm
        private void PlotHumidity(List<(DateTime Time, float? Temperature, float? Humidity)> feeds)
        {
            var model = new PlotModel { Title = "Biểu đồ Độ ẩm (Giờ Việt Nam)" };
            var dateAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Thời gian",
                IntervalType = DateTimeIntervalType.Auto,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            };
            dateAxis.AxisChanged += DateAxis_AxisChanged;
            model.Axes.Add(dateAxis);

            var humidityAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Độ ẩm (%)",
                Minimum = 0,
                Maximum = 100,
            };
            model.Axes.Add(humidityAxis);

            var humSeries = new LineSeries
            {
                Title = "Độ ẩm",
                Color = OxyColors.DeepSkyBlue
            };

            foreach (var feed in feeds)
            {
                if (feed.Humidity.HasValue)
                {
                    double xValue = DateTimeAxis.ToDouble(feed.Time);
                    humSeries.Points.Add(new DataPoint(xValue, feed.Humidity.Value));
                }
            }

            model.Series.Add(humSeries);
            pvHumidity.Model = model;
        }

        private void btnChatbot_Click(object sender, EventArgs e)
        {
            if (chatBot == null || chatBot.IsDisposed)
            {
                chatBot = new ChatBot();
            }
            chatBot.Show();
            chatBot.BringToFront();
        }
    }
}