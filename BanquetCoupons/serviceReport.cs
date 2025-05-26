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
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Fonts;
using System.IO;
using System.Diagnostics;
using PdfSharp.UniversalAccessibility.Drawing;

namespace BanquetCoupons
{
    public partial class serviceReport : UserControl
    {
        public serviceReport(string user)
        {
            InitializeComponent();
            this.user = user;


        }

        private string user;
        private FontManager fontManager;
        private void serviceReport_Load(object sender, EventArgs e)
        {
            userLogin.Text = user;
            fontManager = new FontManager();  // สร้างครั้งเดียวตอนโหลดฟอร์ม

            cateringDate.Font = fontManager.FontRegular;
            CultureInfo thaiCulture = new CultureInfo("th-TH");
            System.Threading.Thread.CurrentThread.CurrentCulture = thaiCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = thaiCulture;
            cateringDate.Format = DateTimePickerFormat.Custom;
            cateringDate.CustomFormat = "dd MMMM yyyy"; // เช่น 01 พฤษภาคม 2567

            // Event เมื่อเลือกวันที่
            cateringDate.ValueChanged += cateringDate_ValueChanged;
            // โหลดข้อมูลเริ่มต้น
            LoadEventsByDate(cateringDate.Value);

            dataGridReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridReport.BackgroundColor = Color.White;
            dataGridReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridReport.ClearSelection();
            dataGridReport.CurrentCell = null;
            dataGridReport.ReadOnly = true; // ปิดการแก้ไขข้อมูล 
            dataGridReport.ClearSelection();// ไม่ให้ผู้ใช้เลือกแถวหรือเซลล์
            dataGridReport.Enabled = false; // หรือหากต้องการปิดอินเทอร์แอคทั้งหมด
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
                                string bqid = reader["BQID"].ToString();
                                string canteen = reader["cateringName"].ToString();
                                string agency = reader["agency"].ToString();

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
                                    lblCus.Text = agency;

                                    ShowUsageCount();
                                };

                                // เพิ่ม event สำหรับ label ด้านใน เพื่อให้คลิกได้ผลเหมือนกัน
                                foreach (Control ctrl in card.Controls)
                                {
                                    ctrl.Click += (s, e) =>
                                    {
                                        bqTopic.Text = bqid;
                                        lblCanteen.Text = canteen;
                                        lblCus.Text = agency;

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

        void showReport()
        {
            string connection = connectDB();
            string selectBQID = bqTopic.Text.Trim();
            DateTime selectedDate = cateringDate.Value.Date;

            string query = "SELECT BQID, useTime, couponNum, serialNum FROM CouponUsage " +
                "WHERE BQID = @BQID AND CONVERT(DATE, useTime) = @SelectedDate  ORDER BY useTime";

            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    using(SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@BQID", selectBQID);
                        cmd.Parameters.AddWithValue("@SelectedDate", selectedDate);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        // ✅ เพิ่มคอลัมน์ใหม่
                        dt.Columns.Add("หมายเลขคูปอง", typeof(string));

                        foreach (DataRow row in dt.Rows)
                        {
                            string coupon = row["couponNum"]?.ToString() ?? "";
                            string serial = row["serialNum"]?.ToString() ?? "";
                            row["หมายเลขคูปอง"] = $"{coupon} - {serial}";
                        }

                        // ✅ ไม่ต้องแสดง 2 คอลัมน์เดิม
                        dt.Columns.Remove("couponNum");
                        dt.Columns.Remove("serialNum");

                        dataGridReport.DataSource = dt;
                        // ปรับหัวตารางให้อ่านง่าย
                        dataGridReport.Columns["BQID"].HeaderText = "BQID";
                        dataGridReport.Columns["useTime"].HeaderText = "เวลาใช้คูปอง";
                        dataGridReport.Columns["หมายเลขคูปอง"].HeaderText = "หมายเลขคูปอง";
                    }
                }catch(Exception ex)
                {
                    MessageBox.Show("Error : " + ex.Message);
                }
            }
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            btnExport.Visible = true;
            if (bqTopic.Text == "BQID" || lblCanteen.Text == "ห้องจัดเลี้ยง" || label3.Text == "-")
                {
                btnExport.Visible = false;
                MessageBox.Show("กรุณาเลือกงานจัดเลี้ยง");
                
            }
            
            showReport();
        }

        void cleartData()
        {
            bqTopic.Text = "BQID";
            lblCanteen.Text = "ห้องจัดเลี้ยง";
            label3.Text = "-";
        }

        private void ExportDataGridViewToPdf(DataGridView dgv, string filePath)
        {
            string fontPath = Path.Combine(Application.StartupPath, "Fonts", "NotoSansThai-Regular.ttf");
            string CateringRoom = lblCanteen.Text;
            string Customer = lblCus.Text;
            GlobalFontSettings.FontResolver = new CustomFontResolver(fontPath);

            PdfDocument document = new PdfDocument();
            document.Info.Title = "รายงานคูปอง";
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont font = new XFont("NotoSansThai-Regular", 12);
            XFont headerFont = new XFont("NotoSansThai-Regular", 16);

            double yPoint = 40;

            gfx.DrawString("รายงานคูปองการใช้บริการ", headerFont, XBrushes.Black, new XRect(20, yPoint, page.Width, page.Height), XStringFormats.TopCenter);
            yPoint += 35;

            gfx.DrawString("ผู้ใช้บริการ: " + Customer, font, XBrushes.Black, new XRect(40, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
            yPoint += 20;

            gfx.DrawString("ห้องจัดเลี้ยง: " + CateringRoom, font, XBrushes.Black, new XRect(40, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
            yPoint += 20;

            double rowHeight = 25;
            double xStart = 40;
            yPoint += 25;

            // ✅ เพิ่มคอลัมน์ลำดับ
            int originalColumnCount = dgv.Columns.Count;
            int columnCount = originalColumnCount + 1; // บวกคอลัมน์ลำดับ

            double[] columnWidths = new double[columnCount];
            columnWidths[0] = 50; // ลำดับ
            columnWidths[1] = 100; // BQID
            columnWidths[2] = 150; // createAt
            columnWidths[3] = 200; // หมายเลขคูปอง

            // ✅ Header
            xStart = 40;
            gfx.DrawRectangle(XPens.Black, xStart, yPoint, columnWidths[0], rowHeight);
            gfx.DrawString("ลำดับ", font, XBrushes.Black, new XRect(xStart + 5, yPoint + 5, columnWidths[0], rowHeight), XStringFormats.TopLeft);
            xStart += columnWidths[0];

            for (int i = 0; i < originalColumnCount; i++)
            {
                string headerText = dgv.Columns[i].HeaderText;
                gfx.DrawRectangle(XPens.Black, xStart, yPoint, columnWidths[i + 1], rowHeight);
                gfx.DrawString(headerText, font, XBrushes.Black, new XRect(xStart + 5, yPoint + 5, columnWidths[i + 1], rowHeight), XStringFormats.TopLeft);
                xStart += columnWidths[i + 1];
            }
            yPoint += rowHeight;

            // ✅ Rows
            int index = 1;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (!row.IsNewRow)
                {
                    xStart = 40;

                    // ลำดับ
                    gfx.DrawRectangle(XPens.Black, xStart, yPoint, columnWidths[0], rowHeight);
                    gfx.DrawString(index.ToString(), font, XBrushes.Black, new XRect(xStart + 5, yPoint + 5, columnWidths[0], rowHeight), XStringFormats.TopLeft);
                    xStart += columnWidths[0];

                    for (int i = 0; i < originalColumnCount; i++)
                    {
                        string cellText = row.Cells[i].Value?.ToString() ?? "";
                        gfx.DrawRectangle(XPens.Black, xStart, yPoint, columnWidths[i + 1], rowHeight);
                        gfx.DrawString(cellText, font, XBrushes.Black, new XRect(xStart + 5, yPoint + 5, columnWidths[i + 1], rowHeight), XStringFormats.TopLeft);
                        xStart += columnWidths[i + 1];
                    }

                    yPoint += rowHeight;
                    index++;
                }
            }

            // ✅ Footer
            double footerMargin = 60;
            double footerLineHeight = 20;

            gfx.DrawString($"ผู้จัดทำรายงาน: {user}", font, XBrushes.Black,
                new XRect(40, page.Height - footerMargin, page.Width, 20), XStringFormats.TopLeft);

            gfx.DrawString("วันที่พิมพ์: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), font, XBrushes.Black,
                new XRect(40, page.Height - footerMargin + footerLineHeight, page.Width, 20), XStringFormats.TopLeft);

            gfx.DrawString("หมายเหตุ: รายงานนี้สร้างจากระบบอัตโนมัติ", font, XBrushes.Gray,
                new XRect(40, page.Height - footerMargin + (footerLineHeight * 2), page.Width, 20), XStringFormats.TopLeft);

            // ✅ Save และเปิด PDF
            document.Save(filePath);
            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }


        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportDataGridViewToPdf(dataGridReport, "Report.pdf");
        }


       
    }
}
