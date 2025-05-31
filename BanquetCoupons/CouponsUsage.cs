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

                    // 1. ดึงข้อมูล CouponUsage quantity รวมทุก BQID ที่มีในวันนั้นเก็บใน Dictionary
                    var usageQuantities = new Dictionary<string, int>();
                    string usageQuery = @"
                SELECT BQID, ISNULL(SUM(quantity),0) AS totalUsage
                FROM CouponUsage
                WHERE CAST(useTime AS DATE) = @SelectedDate
                GROUP BY BQID";

                    using (SqlCommand cmdUsage = new SqlCommand(usageQuery, conn))
                    {
                        cmdUsage.Parameters.Add("@SelectedDate", SqlDbType.Date).Value = selectedDate.Date;
                        using (SqlDataReader readerUsage = cmdUsage.ExecuteReader())
                        {
                            while (readerUsage.Read())
                            {
                                string bqid = readerUsage["BQID"].ToString();
                                int qty = Convert.ToInt32(readerUsage["totalUsage"]);
                                usageQuantities[bqid] = qty;
                            }
                        }
                    }

                    // 2. ดึงข้อมูล Coupons ตามวัน
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
                                string agency = reader["agency"].ToString();
                                string totalQty = reader["totalQuantity"].ToString();

                                int usageQty = usageQuantities.ContainsKey(bqid) ? usageQuantities[bqid] : 0;

                                // สร้างการ์ด UI
                                Panel card = new Panel
                                {
                                    Width = 250,
                                    Height = 140,
                                    BorderStyle = BorderStyle.FixedSingle,
                                    Margin = new Padding(10),
                                    BackColor = Color.White,
                                    Cursor = Cursors.Hand
                                };

                                Label lblBQID = new Label
                                {
                                    Text = "🆔 BQID: " + bqid,
                                    Location = new Point(10, 10),
                                    Font = new Font("Segoe UI", 9),
                                    AutoSize = true
                                };

                                Label lblCatering = new Label
                                {
                                    Text = "🍽️ " + canteen,
                                    Location = new Point(10, 30),
                                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                                    AutoSize = true
                                };

                                Label lblAgency = new Label
                                {
                                    Text = "🏢 " + agency,
                                    Location = new Point(10, 55),
                                    AutoSize = true
                                };

                                Label lblQty = new Label
                                {
                                    Text = "จำนวนที่จอง: " + totalQty,
                                    Location = new Point(10, 80),
                                    AutoSize = true
                                };

                                Label lblUsageQty = new Label
                                {
                                    Text = "จำนวนที่ใช้: " + usageQty,
                                    Location = new Point(10, 105),
                                    AutoSize = true
                                };

                                card.Controls.Add(lblBQID);
                                card.Controls.Add(lblCatering);
                                card.Controls.Add(lblAgency);
                                card.Controls.Add(lblQty);
                                card.Controls.Add(lblUsageQty);

                                // Event click
                                EventHandler clickHandler = (s, e) =>
                                {
                                    bqTopic.Text = bqid;
                                    lblCanteen.Text = canteen;
                                    lblSumqty.Text = totalQty;
                                    ShowUsageCount();
                                };

                                card.Click += clickHandler;
                                foreach (Control ctrl in card.Controls)
                                {
                                    ctrl.Click += clickHandler;
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
            string inputQty = txtQty.Text.Trim(); // จำนวนคูปองที่ต้องการใช้
            string sumFromQty = lblSumqty.Text; // จำนวนคูปองทั้งหมด
            string currentUsername = userLogin.Text;
            string connectionString = connectDB();

            if (!int.TryParse(inputQty, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("กรุณากรอกจำนวนคูปองที่ใช้งานเป็นตัวเลขที่มากกว่า 0");
                return;
            }

            if (!int.TryParse(sumFromQty, out int fromQty))
            {
                MessageBox.Show("ข้อมูลจำนวนคูปองทั้งหมดไม่ถูกต้อง");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // 1. ตรวจสอบยอดรวมคูปองที่ใช้ไปแล้ว
                    string usedQtyQuery = @"SELECT ISNULL(SUM(quantity),0) FROM CouponUsage WHERE BQID = @BQID";
                    int usedQty = 0;
                    using (SqlCommand usedCmd = new SqlCommand(usedQtyQuery, conn))
                    {
                        usedCmd.Parameters.AddWithValue("@BQID", inputBQID);
                        object result = usedCmd.ExecuteScalar();
                        if (result != null)
                            usedQty = Convert.ToInt32(result);
                    }

                    int totalAfterAdd = usedQty + quantity;

                    if (totalAfterAdd > fromQty)
                    {
                        MessageBox.Show("จำนวนครบแล้ว ไม่สามารถใช้งานเกินจำนวนคูปองที่มี");
                        clearData();
                        return; // หยุดทำงาน ไม่บันทึก
                        
                    }

                    // 2. ตรวจสอบว่ามี BQID อยู่ใน Coupons หรือไม่
                    string checkQuery = @"
                SELECT BQID, cateringName 
                FROM Coupons 
                WHERE BQID = @BQID";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@BQID", inputBQID);

                        using (SqlDataReader reader = checkCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string bqid = reader["BQID"].ToString();
                                string cateringName = reader["cateringName"].ToString();
                                reader.Close();

                                string insertQuery = @"
                            INSERT INTO CouponUsage (BQID, Username, useTime, canteenName, quantity, fromQty)
                            VALUES (@BQID, @Username, @useTime, @canteenName, @quantity, @fromQty)";

                                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                                {
                                    insertCmd.Parameters.AddWithValue("@BQID", bqid);
                                    insertCmd.Parameters.AddWithValue("@Username", currentUsername);
                                    insertCmd.Parameters.AddWithValue("@useTime", DateTime.Now);
                                    insertCmd.Parameters.AddWithValue("@canteenName", cateringName);
                                    insertCmd.Parameters.AddWithValue("@quantity", quantity);
                                    insertCmd.Parameters.AddWithValue("@fromQty", fromQty);

                                    int rowsAffected = insertCmd.ExecuteNonQuery();

                                    if (rowsAffected > 0)
                                    {
                                        ShowUsageCount();
                                        MessageBox.Show("บันทึกจำนวนคูปองที่ใช้งานสำเร็จ");
                                        LoadEventsByDate(DateTime.Now);
                                    }
                                    else
                                    {
                                        MessageBox.Show("การบันทึกไม่สำเร็จ");
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("ไม่พบข้อมูล BQID นี้ในระบบ");
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

                        //label3.Text = totalUsage.ToString();
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
            bqTopic.Text = "BQID";
            lblCanteen.Text = "ห้องจัดเลี้ยง";
            lblSumqty.Text = "0";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clearData();
        }

        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtQty_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(lblSumqty.Text, out int sumQty) && int.TryParse(txtQty.Text, out int value))
            {
                if (value > sumQty)
                {
                    MessageBox.Show($"กรอกตัวเลขได้ไม่เกิน {sumQty}", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQty.BackColor = Color.Red;
                    txtQty.Text = 0.ToString();
                    txtQty.SelectionStart = txtQty.Text.Length; // เลื่อน cursor ไปท้าย
                }
                else
                {
                    txtQty.BackColor = Color.White;
                }
            }
        }
    }


}

