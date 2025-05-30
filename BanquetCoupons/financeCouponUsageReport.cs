using PdfSharp.Drawing.Layout;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace BanquetCoupons
{
    public partial class financeCouponUsageReport : UserControl
    {
        public financeCouponUsageReport(string user)
        {
            InitializeComponent();
            this.user = user;
        }

        private string user;
        private FontManager fontManager;

        private void financeCouponUsageReport_Load_1(object sender, EventArgs e)
        {
            fontManager = new FontManager();

            cbYear.Items.Add(DateTime.Now.Year.ToString());

            cbMonth.Items.AddRange(new string[]
            {
                "มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน",
                "พฤษภาคม", "มิถุนายน", "กรกฎาคม", "สิงหาคม",
                "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม"
            });

            // เติมวัน 1-31
            for (int d = 1; d <= 31; d++)
            {
                cbDay.Items.Add(d.ToString());
            }

            dataGridView1.Font = new Font("Segoe UI", 10, FontStyle.Regular);  // ปรับขนาดตามต้องการ
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView1.RowTemplate.Height = 25;  // ปรับให้สูงขึ้นตามฟอนต์

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Height = 25;
            }
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.BackgroundColor = Color.White;
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

        private void LoadReportData()
        {
            string connStr = connectDB();
            string whereClause = "";

            // ตรวจสอบวันที่จาก cbYear, cbMonth, cbDay
            if (cbYear.SelectedIndex != -1)
            {
                string year = cbYear.SelectedItem.ToString();
                whereClause += $"YEAR(useTime) = {year}";
            }

            if (cbMonth.SelectedIndex != -1)
            {
                string month = (cbMonth.SelectedIndex + 1).ToString();
                if (whereClause != "") whereClause += " AND ";
                whereClause += $"MONTH(useTime) = {month}";
            }

            if (cbDay.SelectedIndex != -1)
            {
                string day = cbDay.SelectedItem.ToString();
                if (whereClause != "") whereClause += " AND ";
                whereClause += $"DAY(useTime) = {day}";
            }

            // คำสั่ง SQL ดึงข้อมูลจาก CouponUsage
            string sql = $@"
                            SELECT *
                            FROM CouponUsage
                            {(string.IsNullOrEmpty(whereClause) ? "" : "WHERE " + whereClause)}
                            ORDER BY useTime DESC
                           ";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void btnLoadReport_Click(object sender, EventArgs e)
        {
            LoadReportData();
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            if(dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("ไม่มีข้อมูลในตารางให้ส่งออก");
                return;
            }
            exportFilePDF();
        }

        private void exportFilePDF()
        {
            try
            {
                string fontPath = Path.Combine(Application.StartupPath, "Fonts", "NotoSansThai-Regular.ttf");
                GlobalFontSettings.FontResolver = new CustomFontResolver(fontPath);

                string filePath = Path.Combine(Path.GetTempPath(), "ExportData.pdf");

                PdfDocument document = new PdfDocument();
                document.Info.Title = "รายงานข้อมูลจาก DataGridView";

                PdfPage page = document.AddPage();
                page.Orientation = PdfSharp.PageOrientation.Landscape; // ใช้แนวนอนเพื่อรองรับข้อมูลกว้าง
                XGraphics gfx = XGraphics.FromPdfPage(page);

                XFont titleFont = new XFont("NotoSansThai-Regular", 16);
                XFont font = new XFont("NotoSansThai-Regular", 10);

                double margin = 40;
                double yPoint = margin;

                gfx.DrawString("รายงานข้อมูลการใช้คูปอง", titleFont, XBrushes.Black,
                    new XRect(0, yPoint, page.Width, 40), XStringFormats.TopCenter);
                yPoint += 50;

                // แสดงทุกคอลัมน์
                List<int> colsToShow = new List<int>();
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    colsToShow.Add(i);
                }

                int colCount = colsToShow.Count;
                double pageWidth = page.Width.Point - margin * 2;
                double colWidth = pageWidth / colCount;
                double rowHeight = 25;

                // วาด header ตาราง
                for (int i = 0; i < colCount; i++)
                {
                    int colIndex = colsToShow[i];
                    string headerText = dataGridView1.Columns[colIndex].HeaderText;
                    var rect = new XRect(margin + i * colWidth, yPoint, colWidth, rowHeight);
                    gfx.DrawRectangle(XPens.Black, rect);
                    gfx.DrawString(headerText, font, XBrushes.Black, rect, XStringFormats.Center);
                }
                yPoint += rowHeight;

                // วาดข้อมูลแต่ละแถว
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue;

                    for (int i = 0; i < colCount; i++)
                    {
                        int colIndex = colsToShow[i];
                        string cellText = row.Cells[colIndex].Value?.ToString() ?? "";
                        var rect = new XRect(margin + i * colWidth, yPoint, colWidth, rowHeight);

                        gfx.DrawRectangle(XPens.Black, rect);

                        var paddedRect = new XRect(rect.X + 5, rect.Y, rect.Width - 10, rect.Height);
                        XTextFormatter tf = new XTextFormatter(gfx);
                        tf.Alignment = XParagraphAlignment.Left;
                        tf.DrawString(cellText, font, XBrushes.Black, paddedRect, XStringFormats.TopLeft);
                    }

                    yPoint += rowHeight;

                    if (yPoint + rowHeight > page.Height.Point - margin)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharp.PageOrientation.Landscape;
                        gfx = XGraphics.FromPdfPage(page);
                        yPoint = margin;
                    }
                }

                // footer
                string footerUser = "ชื่อผู้จัดทำรายงาน";
                gfx.DrawString($"ผู้จัดทำรายงาน: {footerUser}", font, XBrushes.Black,
                    new XRect(margin, page.Height - 80, page.Width, 20), XStringFormats.TopLeft);

                gfx.DrawString($"วันที่พิมพ์: {DateTime.Now:dd/MM/yyyy HH:mm}", font, XBrushes.Black,
                    new XRect(margin, page.Height - 60, page.Width, 20), XStringFormats.TopLeft);

                gfx.DrawString("หมายเหตุ: รายงานนี้สร้างจากระบบอัตโนมัติ", font, XBrushes.Gray,
                    new XRect(margin, page.Height - 40, page.Width, 20), XStringFormats.TopLeft);

                document.Save(filePath);
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
            }
        }

    }
}
