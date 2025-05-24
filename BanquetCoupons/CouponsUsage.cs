using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BanquetCoupons
{
    public partial class CouponsUsage : UserControl
    {
        public CouponsUsage(string user)
        {
            InitializeComponent();
            this.user = user;
        }

        private string user;
        private FontManager fontManager;
        private void CouponsUsage_Load(object sender, EventArgs e)
        {
            userLogin.Text = user;

            fontManager = new FontManager();  // สร้างครั้งเดียวตอนโหลดฟอร์ม
            label1.Font = fontManager.FontRegular;
            

            foreach (Control ctl in panel1.Controls)
            {
                if (ctl is Label label)
                {

                    if (label.Name == "lblPreview")
                        label.Font = fontManager.FontSmall;
                    else if (label.Name == "label9")
                        label.Font = fontManager.FontSmall;
                    else if (label.Name == "lblSerialNumber")
                        label.Font = fontManager.FontSmall;
                    else if (label.Name == "seNum")
                        label.Font = fontManager.FontSmallBold;
                    else
                        label.Font = fontManager.FontRegular;
                }
            }

            panel2.Paint += panel2_Paint;
            CultureInfo thaiCulture = new CultureInfo("th-TH");
            System.Threading.Thread.CurrentThread.CurrentCulture = thaiCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = thaiCulture;
            dateToday.Text = DateTime.Now.ToString("dddd dd MMMM yyyy");
            dateToday.Font = fontManager.FontShowDate;

            LoadEventsToday();
        }

        private void LoadEventsToday()
        {
            LoadEventsByDate(DateTime.Today);
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            Panel pnl = (Panel)sender;
            int thickness = 8;

            using (Pen pen = new Pen(Color.FromArgb(255, 204, 51), thickness))
            {
                // ปิดการเบลอของเส้น (optional)
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

                // วาดเส้นบน
                e.Graphics.DrawLine(pen, 0, 0, pnl.Width, 0);

                // วาดเส้นล่าง
                e.Graphics.DrawLine(pen, 0, pnl.Height - 1, pnl.Width, pnl.Height - 1);

                // วาดเส้นซ้าย
                e.Graphics.DrawLine(pen, 0, 0, 0, pnl.Height);

                // วาดเส้นขวา
                e.Graphics.DrawLine(pen, pnl.Width - 1, 0, pnl.Width - 1, pnl.Height);
            }
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
        private void LoadEventsByDate(DateTime selectedDate)
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
                            SUM(quantity) AS totalQuantity
                        FROM Coupons
                        WHERE CAST(mealDate AS DATE) = @SelectedDate
                        GROUP BY BQID, cateringName, agency";


                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@SelectedDate", SqlDbType.Date).Value = selectedDate.Date;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Panel card = new Panel
                                {
                                    Width = 250,
                                    Height = 120,
                                    BorderStyle = BorderStyle.FixedSingle,
                                    Margin = new Padding(10),
                                    BackColor = Color.White
                                };

                                Label lblBQID = new Label
                                {
                                    Text = "🆔 BQID: " + reader["BQID"].ToString(),
                                    Location = new Point(10, 10),
                                    Font = new Font("Segoe UI", 9, FontStyle.Regular),
                                    AutoSize = true
                                };

                                Label lblCatering = new Label
                                {
                                    Text = "🍽️ " + reader["cateringName"].ToString(),
                                    Location = new Point(10, 30),
                                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                                    AutoSize = true
                                };

                                Label lblAgency = new Label
                                {
                                    Text = "🏢 " + reader["agency"].ToString(),
                                    Location = new Point(10, 55),
                                    AutoSize = true
                                };

                                Label lblQty = new Label
                                {
                                    Text = "จำนวน: " + reader["totalQuantity"].ToString(),
                                    Location = new Point(10, 80),
                                    AutoSize = true
                                };

                                card.Controls.Add(lblBQID);
                                card.Controls.Add(lblCatering);
                                card.Controls.Add(lblAgency);
                                card.Controls.Add(lblQty);

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
    }
}
