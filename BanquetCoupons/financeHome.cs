using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        private void financeHome_Load(object sender, EventArgs e)
        {
            userLogin.Text = user;

            // เดือน
            cbMonth.Items.AddRange(new string[]
            {
                "01 - มกราคม", "02 - กุมภาพันธ์", "03 - มีนาคม", "04 - เมษายน",
                "05 - พฤษภาคม", "06 - มิถุนายน", "07 - กรกฎาคม", "08 - สิงหาคม",
                "09 - กันยายน", "10 - ตุลาคม", "11 - พฤศจิกายน", "12 - ธันวาคม"
            });

            // ปี (สมมุติให้ย้อนหลัง 5 ปี)
            for (int year = DateTime.Now.Year; year >= DateTime.Now.Year - 5; year--)
            {
                cbYear.Items.Add(year.ToString());
            }

            cbMonth.SelectedIndex = DateTime.Now.Month - 1;
            cbYear.SelectedItem = DateTime.Now.Year.ToString();

            LoadChart(); // โหลดกราฟเริ่มต้น
        }

        private void LoadChart()
        {
            if (cbMonth.SelectedIndex == -1 || cbYear.SelectedIndex == -1) return;

            // ดึงค่าจาก ComboBox
            string selectedMonth = cbMonth.SelectedItem.ToString().Substring(0, 2);
            string selectedYear = cbYear.SelectedItem.ToString();

            // สร้างช่วงวันที่
            DateTime fromDate = new DateTime(int.Parse(selectedYear), int.Parse(selectedMonth), 1);
            DateTime toDate = fromDate.AddMonths(1).AddDays(-1);

            // TODO: ดึงข้อมูลจากแหล่งข้อมูลจริง เช่น DataTable หรือ Database
            int usedCoupons = 25;    // ตัวอย่างจำนวนที่ถูกใช้
            int unusedCoupons = 75;  // ตัวอย่างจำนวนที่ยังไม่ใช้

            // โหลดเข้า Pie Chart
            chart1.Series.Clear();
            Series series = new Series("คูปอง");
            series.ChartType = SeriesChartType.Pie;
            series.Points.AddXY("ใช้แล้ว", usedCoupons);
            series.Points.AddXY("ยังไม่ใช้", unusedCoupons);
            chart1.Series.Add(series);

            chart1.Titles.Clear();
            chart1.Titles.Add($"สรุปคูปอง เดือน {cbMonth.Text} {selectedYear}");
        }


        private void cbMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadChart();
        }

        private void cbYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadChart();
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

    }
}
