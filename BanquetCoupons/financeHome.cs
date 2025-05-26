using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;


namespace BanquetCoupons
{
    public partial class financeHome : UserControl
    {
        public financeHome(string user)
        {
            InitializeComponent();
            this.user = user;
        }

        private string user;
        private FontManager fontManager;
        private DataTable fullDataTable;

        private void financeHome_Load(object sender, EventArgs e)
        {
            userLogin.Text = user;
            fontManager = new FontManager();
            // เดือน
            cbMonth.Items.AddRange(new string[]
            {
                "มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน",
                "พฤษภาคม", "มิถุนายน", "กรกฎาคม", "สิงหาคม",
                "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม"
            });

            // ปี (สมมุติให้ย้อนหลัง 5 ปี)
            for (int year = DateTime.Now.Year; year >= DateTime.Now.Year - 2; year--)
            {
                cbYear.Items.Add(year.ToString());
            }

            cbMonth.SelectedIndex = DateTime.Now.Month - 1;
            cbYear.SelectedItem = DateTime.Now.Year.ToString();

            LoadChart(); // โหลดกราฟเริ่มต้น
            int selectedMonth = cbMonth.SelectedIndex + 1; // เดือนเริ่มที่ 0 ต้อง +1
            int selectedYear = int.Parse(cbYear.SelectedItem.ToString());

            LoadEventsByMonthAndYear(selectedMonth, selectedYear);
        }

        private void LoadChart()
        {
            if (cbMonth.SelectedIndex == -1 || cbYear.SelectedIndex == -1) return;

            int selectedMonth = cbMonth.SelectedIndex + 1;
            string selectedYear = cbYear.SelectedItem.ToString();

            DateTime fromDate = new DateTime(int.Parse(selectedYear), selectedMonth, 1);
            DateTime toDate = fromDate.AddMonths(1).AddDays(-1);

            int usedCoupons = 0;
            int unusedCoupons = 0;

            string connStr = connectDB();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
            SELECT 
                SUM(CASE WHEN status = 'usage' THEN 1 ELSE 0 END) AS UsedCount,
                SUM(CASE WHEN status IS NULL THEN 1 ELSE 0 END) AS UnusedCount
            FROM Coupons
            WHERE mealDate BETWEEN @fromDate AND @toDate;
        ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@fromDate", fromDate);
                    cmd.Parameters.AddWithValue("@toDate", toDate);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usedCoupons = reader["UsedCount"] != DBNull.Value ? Convert.ToInt32(reader["UsedCount"]) : 0;
                            unusedCoupons = reader["UnusedCount"] != DBNull.Value ? Convert.ToInt32(reader["UnusedCount"]) : 0;
                        }
                    }
                }
            }

            // เคลียร์ข้อมูลเดิมใน chart
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.Titles.Clear();

            ChartArea area = new ChartArea("MainArea");
            chart1.ChartAreas.Add(area);

            Series series = new Series("คูปอง");
            // Tooltips
            series.ToolTip = "#VAL คูปอง";
            series.ChartType = SeriesChartType.Column;
            series.Points.AddXY("ใช้แล้ว", usedCoupons);
            series.Points.AddXY("ยังไม่ใช้", unusedCoupons);
            chart1.Series.Add(series);

            chart1.Titles.Add($"สรุปคูปอง เดือน {cbMonth.Text} {selectedYear}");
        }


        string connectDB()
        {
            string iniPath = "config.ini"; // ที่อยู่ไฟล์ .ini

            // อ่าน config จาก section Database
            var config = IniReader.ReadIni(iniPath, "Database");

            // ดึงค่าตัวแปรออกมา
            string server = config.ContainsKey("Server") ? config["Server"] : "";
            string database = config.ContainsKey("Database") ? config["Database"] : "";
            string user = config.ContainsKey("User") ? config["User"] : "";
            string password = config.ContainsKey("Password") ? config["Password"] : "";

            // สร้าง connection string (SQL Server Auth)
            string connectionString = $"Server={server};Database={database};User Id={user};Password={password};";
            return connectionString;
        }

        private void cbMonth_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            LoadChart();
        }

        private void cbYear_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            LoadChart();
        }

        private void LoadEventsByMonthAndYear(int selectedMonth, int selectedYear)
        {
            flowLayoutPanel1.Controls.Clear();

            string connectionString = connectDB();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    BQID,
                    cateringName,
                    agency,
                    mealDate,
                    SUM(quantity) AS totalQuantity,
                    SUM(CASE WHEN status = 'usage' THEN quantity ELSE 0 END) AS usedQuantity
                FROM Coupons
                WHERE MONTH(createAt) = @Month AND YEAR(createAt) = @Year
                GROUP BY BQID, cateringName, agency, mealDate";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Month", selectedMonth);
                        cmd.Parameters.AddWithValue("@Year", selectedYear);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int total = Convert.ToInt32(reader["totalQuantity"]);
                                int used = Convert.ToInt32(reader["usedQuantity"]);

                                // แปลงวันที่ mealDate
                                DateTime mealDate = Convert.ToDateTime(reader["mealDate"]);
                                string formattedDate = mealDate.ToString("dd/MM/yyyy"); // แสดงแบบวัน/เดือน/ปี

                                int remaining = total - used;
                                double percentUsed = total > 0 ? ((double)used / total) * 100 : 0;

                                Panel card = new Panel
                                {
                                    Width = 260,
                                    Height = 200,
                                    BorderStyle = BorderStyle.FixedSingle,
                                    Margin = new Padding(10),
                                    BackColor = Color.White
                                };

                                int y = 10;
                                int spacing = 20;

                                Label lblBQID = new Label
                                {
                                    Text = "🆔 BQID: " + reader["BQID"].ToString(),
                                    Location = new Point(10, y),
                                    Font = new Font("Segoe UI", 9, FontStyle.Regular),
                                    AutoSize = true
                                }; y += spacing;

                                Label lblMealDate = new Label
                                {
                                    Text = "📅 วันที่จัด: " + formattedDate,
                                    Location = new Point(10, y),
                                    Font = new Font("Segoe UI", 9, FontStyle.Regular),
                                    AutoSize = true
                                }; y += spacing;

                                Label lblCatering = new Label
                                {
                                    Text = "🍽️ " + reader["cateringName"].ToString(),
                                    Location = new Point(10, y),
                                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                                    AutoSize = true
                                }; y += spacing;

                                Label lblAgency = new Label
                                {
                                    Text = "🏢 " + reader["agency"].ToString(),
                                    Location = new Point(10, y),
                                    AutoSize = true
                                }; y += spacing;

                                Label lblQty = new Label
                                {
                                    Text = "🎟️ รวม: " + total,
                                    Location = new Point(10, y),
                                    AutoSize = true
                                }; y += spacing;

                                Label lblUsed = new Label
                                {
                                    Text = "✅ ใช้แล้ว: " + used,
                                    Location = new Point(10, y),
                                    AutoSize = true
                                }; y += spacing;

                                Label lblRemaining = new Label
                                {
                                    Text = "📦 คงเหลือ: " + remaining,
                                    Location = new Point(10, y),
                                    AutoSize = true
                                }; y += spacing;

                                Label lblPercent = new Label
                                {
                                    Text = $"📊 ใช้แล้ว: {percentUsed:F1}%",
                                    Location = new Point(10, y),
                                    AutoSize = true
                                };

                                card.Tag = reader["BQID"].ToString();
                                card.Cursor = Cursors.Hand;
                                card.Click += Card_Click;

                                // เพิ่ม Label ลง Panel
                                card.Controls.Add(lblBQID);
                                card.Controls.Add(lblMealDate);
                                card.Controls.Add(lblCatering);
                                card.Controls.Add(lblAgency);
                                card.Controls.Add(lblQty);
                                card.Controls.Add(lblUsed);
                                card.Controls.Add(lblRemaining);
                                card.Controls.Add(lblPercent);

                                flowLayoutPanel1.Controls.Add(card);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
                }
            }
        }

        private void Card_Click(object sender, EventArgs e)
        {
            Panel card = sender as Panel;
            if (card == null) return;

            string bqid = card.Tag as string;
            if (string.IsNullOrEmpty(bqid)) return;

            LoadChartForBQID(bqid);
        }

        private void LoadChartForBQID(string bqid)
        {
            if (cbMonth.SelectedIndex == -1 || cbYear.SelectedIndex == -1) return;

            int selectedMonth = cbMonth.SelectedIndex + 1;
            int selectedYear = int.Parse(cbYear.SelectedItem.ToString());

            DateTime fromDate = new DateTime(selectedYear, selectedMonth, 1);
            DateTime toDate = fromDate.AddMonths(1).AddDays(-1);

            int usedCoupons = 0;
            int unusedCoupons = 0;

            string connStr = connectDB();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
            SELECT 
                SUM(CASE WHEN status = 'usage' THEN quantity ELSE 0 END) AS UsedCount,
                SUM(CASE WHEN status IS NULL THEN quantity ELSE 0 END) AS UnusedCount
            FROM Coupons
            WHERE BQID = @BQID AND mealDate BETWEEN @fromDate AND @toDate;
        ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@BQID", bqid);
                    cmd.Parameters.AddWithValue("@fromDate", fromDate);
                    cmd.Parameters.AddWithValue("@toDate", toDate);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usedCoupons = reader["UsedCount"] != DBNull.Value ? Convert.ToInt32(reader["UsedCount"]) : 0;
                            unusedCoupons = reader["UnusedCount"] != DBNull.Value ? Convert.ToInt32(reader["UnusedCount"]) : 0;
                        }
                    }
                }
            }

            // เคลียร์และสร้างกราฟใหม่
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.Titles.Clear();

            ChartArea area = new ChartArea("MainArea");
            chart1.ChartAreas.Add(area);


            Series series = new Series("คูปอง");
            // Tooltips
            series.ToolTip = "#VAL คูปอง";
            series.ChartType = SeriesChartType.Column;
            series.Points.AddXY("ใช้แล้ว", usedCoupons);
            series.Points.AddXY("ยังไม่ใช้", unusedCoupons);
            chart1.Series.Add(series);

            chart1.Titles.Add($"สรุปคูปอง {bqid}");
        }

        private void cbMonth_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cbYear.SelectedIndex != -1)
            {
                int selectedMonth = cbMonth.SelectedIndex + 1;
                int selectedYear = int.Parse(cbYear.SelectedItem.ToString());
                LoadEventsByMonthAndYear(selectedMonth, selectedYear);
            }
        }

        private void cbYear_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cbMonth.SelectedIndex != -1)
            {
                int selectedMonth = cbMonth.SelectedIndex + 1;
                int selectedYear = int.Parse(cbYear.SelectedItem.ToString());
                LoadEventsByMonthAndYear(selectedMonth, selectedYear);
            }
        }

        class CouponReportItem
{
    public string BQID { get; set; }
    public DateTime MealDate { get; set; }
    public string CateringName { get; set; }
    public string Agency { get; set; }
    public int TotalQuantity { get; set; }
    public int UsedQuantity { get; set; }
}

    }
}
