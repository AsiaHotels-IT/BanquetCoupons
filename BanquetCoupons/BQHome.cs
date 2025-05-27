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
    public partial class BQHome : UserControl
    {
        public BQHome(string user)
        {
            InitializeComponent();
            this.user = user;

            // สร้าง DateTimePicker
            DateTimePicker dtPicker = new DateTimePicker();
            dtPicker.Location = new Point(30, 30);
            // เพิ่มไปในฟอร์ม
            this.Controls.Add(dtPicker);
        }

        private FontManager fontManager;
        private string user;
        private void BQHome_Load(object sender, EventArgs e)
        {
            fontManager = new FontManager();

            cateringDate.Font = fontManager.FontRegular;
            CultureInfo thaiCulture = new CultureInfo("th-TH");
            System.Threading.Thread.CurrentThread.CurrentCulture = thaiCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = thaiCulture;
            cateringDate.Format = DateTimePickerFormat.Custom;
            cateringDate.CustomFormat = "dd MMMM yyyy" ; // เช่น 01 พฤษภาคม 2567

            // Event เมื่อเลือกวันที่
            cateringDate.ValueChanged += cateringDate_ValueChanged;

            // โหลดข้อมูลเริ่มต้น
            LoadEventsByDate(cateringDate.Value);
            userLogin.Text = user;

            label1.Font= fontManager.FontRegular;
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

        private void cateringDate_ValueChanged(object sender, EventArgs e)
        {
            LoadEventsByDate(cateringDate.Value);
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
                                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                                    AutoSize = true
                                };

                                Label lblCatering = new Label
                                {
                                    Text = "🍽️ " + reader["cateringName"].ToString(),
                                    Location = new Point(10, 35),
                                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                                    AutoSize = true
                                };

                                Label lblAgency = new Label
                                {
                                    Text = "🏢 " + reader["agency"].ToString(),
                                    Location = new Point(10, 55),
                                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                                    AutoSize = true
                                };

                                Label lblQty = new Label
                                {
                                    Text = "จำนวน: " + reader["totalQuantity"].ToString(),
                                    Location = new Point(10, 80),
                                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
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
