using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfSharp.Drawing;
using System.IO;

namespace BanquetCoupons
{
    public partial class BQReport : UserControl
    {
        public BQReport(string user)
        {
            InitializeComponent();
            this.user = user;
        }

        private string user;
        private FontManager fontManager;

        private void BQReport_Load(object sender, EventArgs e)
        {
            userLogin.Text = user;
            fontManager = new FontManager();  // สร้างครั้งเดียวตอนโหลดฟอร์ม

            cateringDate.Font = fontManager.FontRegular;
            CultureInfo thaiCulture = new CultureInfo("th-TH");
            System.Threading.Thread.CurrentThread.CurrentCulture = thaiCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = thaiCulture;
            cateringDate.Format = DateTimePickerFormat.Custom;
            cateringDate.CustomFormat = "dd MMMM yyyy"; // เช่น 01 พฤษภาคม 2567

            
            cateringDate.ValueChanged += cateringDate_ValueChanged; // Event เมื่อเลือกวันที่
            LoadEventsByDate(cateringDate.Value); 
            dataGridReport.Font = new Font("Segoe UI", 10, FontStyle.Regular);  // ปรับขนาดตามต้องการ
            dataGridReport.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridReport.RowTemplate.Height = 25;  // ปรับให้สูงขึ้นตามฟอนต์

            foreach (DataGridViewRow row in dataGridReport.Rows)
            {
                row.Height = 25;
            }
            dataGridReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridReport.BackgroundColor = Color.White;
            dataGridReport.ClearSelection();
            dataGridReport.CurrentCell = null;
            dataGridReport.ReadOnly = true; // ปิดการแก้ไขข้อมูล 
            dataGridReport.ClearSelection();// ไม่ให้ผู้ใช้เลือกแถวหรือเซลล์
            dataGridReport.Enabled = false; // หรือหากต้องการปิดอินเทอร์แอคทั้งหมด

            SetFontForAllControls(this, fontManager.FontRegular, fontManager.FontBold);

        }

        void SetFontForAllControls(Control parent, Font regularFont, Font boldFont)
        {
            foreach (Control ctl in parent.Controls)
            {
                if (ctl is Label label)
                {
                    if (label.Name == "bqTopic" || label.Name == "label4" || label.Name == "qty")
                        label.Font = boldFont;
                    else if (label.Name == "label2")
                        label.Font = boldFont;
                    else
                        label.Font = regularFont;
                }

                // เรียกซ้ำหากเป็นคอนเทนเนอร์
                if (ctl.HasChildren)
                {
                    SetFontForAllControls(ctl, regularFont, boldFont);
                }
            }
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
                WHERE CAST(createAt AS DATE) = @SelectedDate
                GROUP BY BQID, cateringName, agency
                ORDER BY BQID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@SelectedDate", SqlDbType.Date).Value = selectedDate.Date;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string bqid = reader["BQID"] != DBNull.Value ? reader["BQID"].ToString() : "-";
                                string canteen = reader["cateringName"] != DBNull.Value ? reader["cateringName"].ToString() : "-";
                                string agency = reader["agency"] != DBNull.Value ? reader["agency"].ToString() : "-";
                                string totalQty = reader["totalQuantity"] != DBNull.Value ? reader["totalQuantity"].ToString() : "0";

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
                                    Text = "🆔 BQID: " + bqid,
                                    Location = new Point(10, 10),
                                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                                    AutoSize = true
                                };

                                Label lblCatering = new Label
                                {
                                    Text = "🍽️ " + canteen,
                                    Location = new Point(10, 35),
                                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                                    AutoSize = true
                                };

                                Label lblAgency = new Label
                                {
                                    Text = "🏢 " + agency,
                                    Location = new Point(10, 55),
                                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                                    AutoSize = true
                                };

                                Label lblQty = new Label
                                {
                                    Text = "จำนวน: " + totalQty,
                                    Location = new Point(10, 80),
                                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                                    AutoSize = true
                                };

                                card.Controls.Add(lblBQID);
                                card.Controls.Add(lblCatering);
                                card.Controls.Add(lblAgency);
                                card.Controls.Add(lblQty);

                                // Click event สำหรับ Panel และ Label ภายใน
                                EventHandler clickEvent = (s, e) =>
                                {
                                    bqTopic.Text = bqid;
                                    lblCanteen.Text = canteen;
                                    lblCus.Text = agency;
                                    ShowUsageCount();
                                };

                                card.Click += clickEvent;
                                foreach (Control ctrl in card.Controls)
                                {
                                    ctrl.Click += clickEvent;
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
        void ShowUsageCount()
        {
            string connectionString = connectDB();  // ฟังก์ชันเชื่อมต่อ DB ของคุณ
            string inputBQID = bqTopic.Text.Trim(); // สมมติรับ BQID จาก TextBox

            string query = @"
                            SELECT COUNT(*) 
                            FROM Coupons
                            WHERE BQID = @BQID ";

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

        int currentPage = 1;
        int pageSize = 10;
        int totalRecords = 0;
        int totalPages = 0;
        DataTable fullDataTable = new DataTable(); // เก็บข้อมูลทั้งหมดไว้ตรงนี้

        void showReport()
        {
            string connection = connectDB();
            string selectBQID = bqTopic.Text.Trim();
            DateTime selectedDate = cateringDate.Value.Date;

            string query = @"
                    SELECT 
                        BQID, 
                        createAt, 
                        couponNum, 
                        serialNum 
                    FROM Coupons
                    WHERE BQID = @BQID 
                      AND CONVERT(DATE, createAt) = @SelectedDate  
                    ORDER BY createAt";

            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@BQID", selectBQID);
                        cmd.Parameters.AddWithValue("@SelectedDate", selectedDate);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        fullDataTable = new DataTable(); // โหลดข้อมูลทั้งหมดไว้ก่อน
                        adapter.Fill(fullDataTable);

                        // รวม couponNum กับ serialNum
                        fullDataTable.Columns.Add("หมายเลขคูปอง", typeof(string));
                        foreach (DataRow row in fullDataTable.Rows)
                        {
                            string coupon = row["couponNum"]?.ToString() ?? "";
                            string serial = row["serialNum"]?.ToString() ?? "";
                            row["หมายเลขคูปอง"] = $"{coupon} - {serial}";
                        }

                        // ลบคอลัมน์ที่ไม่ต้องแสดง
                        fullDataTable.Columns.Remove("couponNum");
                        fullDataTable.Columns.Remove("serialNum");

                        // คำนวณจำนวนหน้า
                        totalRecords = fullDataTable.Rows.Count;
                        totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                        currentPage = 1;

                        showCurrentPage(); // แสดงหน้าปัจจุบัน
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
                }
            }
        }

        void showCurrentPage()
        {
            // คำนวณ row เริ่มต้นและสิ้นสุด
            int startRow = (currentPage - 1) * pageSize;
            int endRow = Math.Min(startRow + pageSize, totalRecords);

            DataTable currentPageTable = fullDataTable.Clone(); // โครงสร้างเหมือนเดิม

            for (int i = startRow; i < endRow; i++)
            {
                currentPageTable.ImportRow(fullDataTable.Rows[i]);
            }

            dataGridReport.DataSource = currentPageTable;

            // ปรับหัวตาราง
            if (dataGridReport.Columns.Contains("BQID"))
                dataGridReport.Columns["BQID"].HeaderText = "BQID";

            if (dataGridReport.Columns.Contains("createAt"))
                dataGridReport.Columns["createAt"].HeaderText = "วันที่สร้าง";

            if (dataGridReport.Columns.Contains("หมายเลขคูปอง"))
                dataGridReport.Columns["หมายเลขคูปอง"].HeaderText = "หมายเลขคูปอง";

            // อัปเดตหน้า
            lblPage.Text = $"หน้า {currentPage} / {totalPages}";
            btnPrev.Enabled = currentPage > 1;
            btnNext.Enabled = currentPage < totalPages;
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

        private void ExportDataTableToPdf(DataTable dataTable, string filePath)
        {
            string fontPath = Path.Combine(Application.StartupPath, "Fonts", "NotoSansThai-Regular.ttf");
            string CateringRoom = lblCanteen.Text;
            string Customer = lblCus.Text;
            GlobalFontSettings.FontResolver = new CustomFontResolver(fontPath);

            PdfDocument document = new PdfDocument();
            document.Info.Title = "รายงานคูปอง";

            XFont font = new XFont("NotoSansThai-Regular", 12);
            XFont headerFont = new XFont("NotoSansThai-Regular", 16);

            double rowHeight = 25;
            int rowsPerPage = 23;
            int totalRows = dataTable.Rows.Count;
            int totalPages = (int)Math.Ceiling((double)totalRows / rowsPerPage);

            double[] columnWidths;
            int columnCount = dataTable.Columns.Count + 1; // +1 for ลำดับ

            // เตรียมความกว้างคอลัมน์ (คำนวณตามหน้าแรก)
            columnWidths = new double[columnCount];
            columnWidths[0] = 50; // ลำดับ
            double remainingWidth = 595 - 80 - columnWidths[0]; // A4 width (595pt) ลบ margin ซ้าย-ขวา 40+40 และลำดับ
            double widthPerColumn = remainingWidth / (columnCount - 1);
            for (int i = 1; i < columnCount; i++)
                columnWidths[i] = widthPerColumn;

            int index = 1;

            for (int pageIndex = 0; pageIndex < totalPages; pageIndex++)
            {
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                double yPoint = 40;

                // Header รายงาน (ทุกหน้า)
                gfx.DrawString("รายงานสร้างคูปอง", headerFont, XBrushes.Black, new XRect(20, yPoint, page.Width, page.Height), XStringFormats.TopCenter);
                yPoint += 35;
                gfx.DrawString("ผู้ใช้บริการ: " + Customer, font, XBrushes.Black, new XRect(40, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
                yPoint += 20;
                gfx.DrawString("ห้องจัดเลี้ยง: " + CateringRoom, font, XBrushes.Black, new XRect(40, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
                yPoint += 25;

                // วาด header ตาราง
                double xStart = 40;
                gfx.DrawRectangle(XPens.Black, xStart, yPoint, columnWidths[0], rowHeight);
                gfx.DrawString("ลำดับ", font, XBrushes.Black, new XRect(xStart + 5, yPoint + 5, columnWidths[0], rowHeight), XStringFormats.TopLeft);
                xStart += columnWidths[0];

                foreach (DataColumn col in dataTable.Columns)
                {
                    int colIndex = Array.IndexOf(dataTable.Columns.Cast<DataColumn>().ToArray(), col) + 1;
                    gfx.DrawRectangle(XPens.Black, xStart, yPoint, columnWidths[colIndex], rowHeight);
                    gfx.DrawString(col.ColumnName, font, XBrushes.Black, new XRect(xStart + 5, yPoint + 5, columnWidths[colIndex], rowHeight), XStringFormats.TopLeft);
                    xStart += columnWidths[colIndex];
                }
                yPoint += rowHeight;

                // วาดข้อมูลแถว (23 แถวต่อหน้า)
                int startRow = pageIndex * rowsPerPage;
                int endRow = Math.Min(startRow + rowsPerPage, totalRows);

                for (int rowIndex = startRow; rowIndex < endRow; rowIndex++)
                {
                    DataRow row = dataTable.Rows[rowIndex];
                    xStart = 40;

                    // ลำดับ
                    gfx.DrawRectangle(XPens.Black, xStart, yPoint, columnWidths[0], rowHeight);
                    gfx.DrawString(index.ToString(), font, XBrushes.Black, new XRect(xStart + 5, yPoint + 5, columnWidths[0], rowHeight), XStringFormats.TopLeft);
                    xStart += columnWidths[0];

                    // คอลัมน์อื่น ๆ
                    for (int col = 0; col < dataTable.Columns.Count; col++)
                    {
                        string cellText = row[col]?.ToString() ?? "";
                        gfx.DrawRectangle(XPens.Black, xStart, yPoint, columnWidths[col + 1], rowHeight);
                        gfx.DrawString(cellText, font, XBrushes.Black, new XRect(xStart + 5, yPoint + 5, columnWidths[col + 1], rowHeight), XStringFormats.TopLeft);
                        xStart += columnWidths[col + 1];
                    }

                    yPoint += rowHeight;
                    index++;
                }

                // Footer (แสดงเฉพาะหน้าสุดท้าย)
                if (pageIndex == totalPages - 1)
                {
                    double footerMargin = 60;
                    double footerLineHeight = 20;

                    gfx.DrawString($"ผู้จัดทำรายงาน: {user}", font, XBrushes.Black,
                        new XRect(40, page.Height - footerMargin, page.Width, 20), XStringFormats.TopLeft);

                    gfx.DrawString("วันที่พิมพ์: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), font, XBrushes.Black,
                        new XRect(40, page.Height - footerMargin + footerLineHeight, page.Width, 20), XStringFormats.TopLeft);

                    gfx.DrawString("หมายเหตุ: รายงานนี้สร้างจากระบบอัตโนมัติ", font, XBrushes.Gray,
                        new XRect(40, page.Height - footerMargin + (footerLineHeight * 2), page.Width, 20), XStringFormats.TopLeft);
                }
            }

            document.Save(filePath);
            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }




        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportDataTableToPdf(fullDataTable, "Report.pdf");
        }

        private void btnReport_Click_1(object sender, EventArgs e)
        {

            btnExport.Visible = true;
            if (bqTopic.Text == "BQID" || lblCanteen.Text == "ห้องจัดเลี้ยง" || label3.Text == "-")
            {
                btnExport.Visible = false;
                MessageBox.Show("กรุณาเลือกงานจัดเลี้ยง");

            }

            showReport();
        }

        private void btnExport_Click_1(object sender, EventArgs e)
        {
            ExportDataTableToPdf(fullDataTable, "Report.pdf");
        }

        private void btnNext_Click(object sender, EventArgs e)
        {

            if (currentPage < totalPages)
            {
                currentPage++;
                showCurrentPage();
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                showCurrentPage();
            }
        }

        private void lblPage_Click(object sender, EventArgs e)
        {

        }
    }
}
