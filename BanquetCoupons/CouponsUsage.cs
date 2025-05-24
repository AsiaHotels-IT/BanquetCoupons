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
                                string bqid = reader["BQID"].ToString();
                                string canteen = reader["cateringName"].ToString();

                                Panel card = new Panel
                                {
                                    Width = 250,
                                    Height = 120,
                                    BorderStyle = BorderStyle.FixedSingle,
                                    Margin = new Padding(10),
                                    BackColor = Color.White,
                                    Cursor = Cursors.Hand // เปลี่ยนเมาส์เป็นรูปมือ
                                };

                                Label lblBQID = new Label
                                {
                                    Text = "🆔 BQID: " + bqid,
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

                                // เพิ่ม event เมื่อคลิกการ์ด
                                card.Click += (s, e) =>
                                {
                                    bqTopic.Text = bqid;
                                    lblCanteen.Text = canteen;

                                    ShowUsageCount();
                                };

                                // เพิ่ม event สำหรับ label ด้านใน เพื่อให้คลิกได้ผลเหมือนกัน
                                foreach (Control ctrl in card.Controls)
                                {
                                    ctrl.Click += (s, e) =>
                                    {
                                        bqTopic.Text = bqid;
                                        lblCanteen.Text = canteen;

                                        ShowUsageCount();
                                    };
                                }

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

        private void btAdd_Click(object sender, EventArgs e)
        {
            saveData();
        }

        void saveData()
        {
            string inputBQID = bqTopic.Text.Trim();
            string inputCouponNum = txtQty.Text.Trim();
            string inputSerialNum = txtSerial.Text.Trim();
            string currentUsername = Environment.UserName;
            string connectionString = connectDB();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // ตรวจสอบว่าคูปองนี้มีอยู่จริงหรือไม่
                    string checkQuery = @"
                SELECT BQID, cateringName
                FROM Coupons
                WHERE BQID = @BQID AND couponNum = @couponNum AND serialNum = @serialNum";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@BQID", inputBQID);
                        checkCmd.Parameters.AddWithValue("@couponNum", inputCouponNum);
                        checkCmd.Parameters.AddWithValue("@serialNum", inputSerialNum);

                        using (SqlDataReader reader = checkCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string bqid = reader["BQID"].ToString();
                                string cateringName = reader["cateringName"].ToString();
                                reader.Close();

                                string useID = userLogin.Text;

                                // 🔒 ตรวจสอบว่ามีการใช้งานคูปองนี้แล้วหรือยัง
                                string duplicateCheckQuery = @"
                            SELECT COUNT(*) 
                            FROM CouponUsage 
                            WHERE BQID = @BQID AND couponNum = @couponNum AND serialNum = @serialNum";

                                using (SqlCommand duplicateCheckCmd = new SqlCommand(duplicateCheckQuery, conn))
                                {
                                    duplicateCheckCmd.Parameters.AddWithValue("@BQID", bqid);
                                    duplicateCheckCmd.Parameters.AddWithValue("@couponNum", inputCouponNum);
                                    duplicateCheckCmd.Parameters.AddWithValue("@serialNum", inputSerialNum);

                                    int count = (int)duplicateCheckCmd.ExecuteScalar();
                                    if (count > 0)
                                    {
                                        MessageBox.Show("มีการใช้งานคูปองนี้แล้ว ไม่สามารถใช้ซ้ำได้");
                                        return;
                                    }
                                }

                                // ➡️ ถ้าไม่ซ้ำ ให้ insert การใช้งานคูปอง
                                string insertQuery = @"
                            INSERT INTO CouponUsage (BQID, Username, useTime, canteenName, useID, couponNum, serialNum)
                            VALUES (@BQID, @Username, @useTime, @canteenName, @useID, @couponNum, @serialNum)";

                                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                                {
                                    insertCmd.Parameters.AddWithValue("@BQID", bqid);
                                    insertCmd.Parameters.AddWithValue("@Username", currentUsername);
                                    insertCmd.Parameters.AddWithValue("@useTime", DateTime.Now);
                                    insertCmd.Parameters.AddWithValue("@canteenName", cateringName);
                                    insertCmd.Parameters.AddWithValue("@useID", useID);
                                    insertCmd.Parameters.AddWithValue("@couponNum", inputCouponNum);
                                    insertCmd.Parameters.AddWithValue("@serialNum", inputSerialNum);

                                    int rowsAffected = insertCmd.ExecuteNonQuery();

                                    if (rowsAffected > 0)
                                    {
                                        // อัปเดตสถานะคูปอง
                                        string updateStatusQuery = @"
                                    UPDATE Coupons
                                    SET status = 'usage'
                                    WHERE BQID = @BQID AND couponNum = @couponNum AND serialNum = @serialNum";

                                        using (SqlCommand updateCmd = new SqlCommand(updateStatusQuery, conn))
                                        {
                                            updateCmd.Parameters.AddWithValue("@BQID", inputBQID);
                                            updateCmd.Parameters.AddWithValue("@couponNum", inputCouponNum);
                                            updateCmd.Parameters.AddWithValue("@serialNum", inputSerialNum);

                                            updateCmd.ExecuteNonQuery();
                                        }


                                        ShowUsageCount();
                                        MessageBox.Show("ใช้คูปองสำเร็จ และบันทึกสถานะแล้ว");
                                    }
                                    else
                                    {
                                        MessageBox.Show("การบันทึกไม่สำเร็จ");
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("ไม่พบคูปองที่ตรงกับเงื่อนไข");
                            }
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
                }
            }
            clearData();
        }

        void ShowUsageCount()
        {
            string connectionString = connectDB();  // ฟังก์ชันเชื่อมต่อ DB ของคุณ
            string inputBQID = bqTopic.Text.Trim(); // สมมติรับ BQID จาก TextBox

            string query = @"
        SELECT COUNT(*) 
        FROM Coupons
        WHERE BQID = @BQID AND status = 'usage'";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@BQID", inputBQID);

                        int totalUsage = (int)cmd.ExecuteScalar(); // ดึงผลรวมมา

                        label3.Text = totalUsage.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
                }
            }
        }

        void clearData()
        {
            txtQty.Text = "";
            txtSerial.Text = "";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clearData();
        }
    }


}

