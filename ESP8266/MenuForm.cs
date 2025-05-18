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
        private readonly string channelId = "2943515";
        private readonly string readApiKey = "LRSNX2AOSRS49MZQ";

        private readonly int refreshInterval = 10 * 60; // Thời gian làm mới dữ liệu là 10 phút
        private readonly int espOfflineThreshold = 5 * 60; // Thời gian tối đa để coi ESP8266 là offline 5 phút
        private readonly HttpClient httpClient = new HttpClient();

        private bool isFirstUpdateCompleted = false; // Cờ kiểm soát lần cập nhật đầu tiên
        private ChatBot chatBot; // Thêm tham chiếu đến ChatBot

        public MenuForm()
        {
            InitializeComponent();
            StartDataRefresh();

            // Thêm sự kiện cho các biểu đồ để không bị cuộn khi dùng chuột
            pvTemperature.MouseWheel += PlotView_MouseWheel;
            pvHumidity.MouseWheel += PlotView_MouseWheel;

            // Gọi form ChatBot
            chatBot = new ChatBot();
            pnChatBot.Controls.Clear();
            pnChatBot.Controls.Add(chatBot);
        }

        // Hàm căn giữa cho panel để giữ bố cục khi form được phóng to
        private void MenuForm_Resize(object sender, EventArgs e)
        {
            pnHome.Location = new Point(
                (this.ClientSize.Width - pnHome.Width) / 2,
                pnHome.Location.Y
            );
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

        // Hàm làm mới dữ liệu từ ThingSpeak
        private async void StartDataRefresh()
        {
            while (!IsDisposed)
            {
                await UpdateData();

                // Nếu lần cập nhật đầu tiên đã hoàn thành, đợi 10 phút trước khi cập nhật tiếp theo
                if (isFirstUpdateCompleted)
                {
                    await Task.Delay(refreshInterval * 1000);
                }
                else
                {
                    isFirstUpdateCompleted = true; // Đánh dấu lần đầu tiên hoàn thành
                }
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
                var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(currentTimeUtc, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

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

                    // Lấy dữ liệu lịch sử để vẽ biểu đồ (200 bản ghi)
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
                                time = DateTime.Parse(createdAtStr).ToUniversalTime();
                            }
                            else
                            {
                                time = DateTime.UtcNow;
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
                    }
                    else
                    {
                        pvTemperature.Model = new PlotModel { Title = "Không có dữ liệu nhiệt độ" };
                        pvHumidity.Model = new PlotModel { Title = "Không có dữ liệu độ ẩm" };
                    }
                }
                else
                {
                    lblStatus.Text = "Không có dữ liệu mới nhất từ ThingSpeak.";
                    pvTemperature.Model = new PlotModel { Title = "Không có dữ liệu nhiệt độ" };
                    pvHumidity.Model = new PlotModel { Title = "Không có dữ liệu độ ẩm" };
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Lỗi: {ex.Message}";
                pvTemperature.Model = new PlotModel { Title = $"Lỗi: {ex.Message}" };
                pvHumidity.Model = new PlotModel { Title = $"Lỗi: {ex.Message}" };
            }
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
                var createdAtUtc = DateTime.Parse(createdAt).ToUniversalTime(); // Chuyển đổi thời gian từ chuỗi JSON sang UTC
                var createdAtVietnam = TimeZoneInfo.ConvertTimeFromUtc(createdAtUtc, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")); // Chuyển đổi sang giờ Việt Nam
                var timeDiff = (DateTime.UtcNow - createdAtUtc).TotalSeconds; // Tính toán thời gian chênh lệch giữa thời gian hiện tại và thời gian tạo dữ liệu

                // Cập nhật trạng thái ESP8266 và thời gian làm mới dữ liệu
                lblStatus.Text = timeDiff <= espOfflineThreshold
                ? $"Trạng thái ESP8266: Online"
                : $"Trạng thái ESP8266: Offline";
                lblRefreshData.Text = $"Dữ liệu được làm mới lúc: {createdAtVietnam:yyyy-MM-dd HH:mm:ss}";
            }
            else
            {
                lblStatus.Text = "Trạng thái ESP8266: Không có thông tin thời gian.";
                lblRefreshData.Text = $"Dữ liệu được làm mới lúc: {vietnamTime:yyyy-MM-dd HH:mm:ss} (Không có dữ liệu)";
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

        // Hàm vẽ biểu đồ nhiệt độ
        private void PlotTemperature(List<(DateTime Time, float? Temperature, float? Humidity)> feeds)
        {
            var model = new PlotModel { Title = "Biểu đồ Nhiệt độ (Giờ Việt Nam)" };

            // Tạo trục thời gian
            var dateAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom, // Vị trí trục nằm ở dưới
                Title = "Thời gian",
                IntervalType = DateTimeIntervalType.Auto, // Tự động điều chỉnh khoảng cách giữa các nhãn
                MajorGridlineStyle = LineStyle.Solid, // Đường lưới chính
                MinorGridlineStyle = LineStyle.Dot // Đường lưới phụ
            };

            dateAxis.AxisChanged += DateAxis_AxisChanged; // Thêm sự kiện thay đổi trục thời gian
            model.Axes.Add(dateAxis); // Thêm trục thời gian vào mô hình

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left, // Vị trí trục nằm ở bên trái
                Title = "Nhiệt độ (°C)",
                Minimum = 0, // Giá trị tối thiểu của trục
                Maximum = 50 // Giá trị tối đa của trục
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

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Độ ẩm (%)",
                Minimum = 0,
                Maximum = 50
            });

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
    }
}