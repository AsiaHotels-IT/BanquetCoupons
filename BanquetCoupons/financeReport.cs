using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using System.IO;
using System.Diagnostics;

namespace BanquetCoupons
{
    public partial class financeReport : UserControl
    {
        public financeReport(string user)
        {
            InitializeComponent();
            this.user = user;   
        }

        private string user;
        private void financeReport_Load(object sender, EventArgs e)
        {
            // เติมปี (สมมติปี 2020-2030)
            for (int y = 2020; y <= 2030; y++)
            {
                cbYear.Items.Add(y.ToString());
            }

            // เติมเดือน 1-12
            for (int m = 1; m <= 12; m++)
            {
                cbMonth.Items.Add(m.ToString());
            }

            // เติมวัน 1-31
            for (int d = 1; d <= 31; d++)
            {
                cbDay.Items.Add(d.ToString());
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

        private void LoadReportData()
        {
            string connStr = connectDB();
            string topic = cbTopic.SelectedItem?.ToString();
            string whereClause = "";

            // ตรวจสอบวันที่
            if (cbYear.SelectedIndex != -1)
            {
                string year = cbYear.SelectedItem.ToString();
                whereClause += $"YEAR({GetDateColumn(topic)}) = {year}";
            }

            if (cbMonth.SelectedIndex != -1)
            {
                string month = (cbMonth.SelectedIndex + 1).ToString();
                if (whereClause != "") whereClause += " AND ";
                whereClause += $"MONTH({GetDateColumn(topic)}) = {month}";
            }

            if (cbDay.SelectedIndex != -1)
            {
                string day = cbDay.SelectedItem.ToString();
                if (whereClause != "") whereClause += " AND ";
                whereClause += $"DAY({GetDateColumn(topic)}) = {day}";
            }

            string sql = null;

            switch (topic)
            {
                case "รายงานการสร้างคูปอง":
                    sql = $@"
                            SELECT BQID, agency, mealDate, mealType, cateringName, serialNum, createAt, oldBQID, status, couponNum
                            FROM Coupons
                            {(string.IsNullOrEmpty(whereClause) ? "" : "WHERE " + whereClause)}
                            ORDER BY createAt DESC
                            ";
                    break;

                case "รายงานการลบคูปอง":
                    sql = $@"
                            SELECT BQID, deleteAt, Username, mealDate, agency, cateringName, quantity, serialNum
                            FROM RemoveCoupons
                            {(string.IsNullOrEmpty(whereClause) ? "" : "WHERE " + whereClause)}
                            ORDER BY deleteAt DESC
                            ";
                    break;

                case "รายงานการแก้ไขคูปอง":
                    sql = $@"
                            SELECT EID, BQID, Username, quantity, editAt
                            FROM UpdateCoupons
                            {(string.IsNullOrEmpty(whereClause) ? "" : "WHERE " + whereClause)}
                            ORDER BY editAt DESC
                            ";
                    break;

                default:
                    sql = null;
                    break;
            }

            if (sql == null) return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private string GetDateColumn(string topic)
        {
            switch (topic)
            {
                case "รายงานการสร้างคูปอง":
                    return "createAt";
                case "รายงานการลบคูปอง":
                    return "deleteAt";
                case "รายงานการแก้ไขคูปอง":
                    return "editAt";
                default:
                    return "createAt";
            }
        }

        private void btnLoadReport_Click(object sender, EventArgs e)
        {
            LoadReportData();
        }

        private void cbTopic_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadReportData();
        }


        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("ไม่มีข้อมูลในตารางให้ส่งออก");
                return;
            }

            try
            {
                string fontPath = Path.Combine(Application.StartupPath, "Fonts", "NotoSansThai-Regular.ttf");
                GlobalFontSettings.FontResolver = new CustomFontResolver(fontPath);

                string filePath = "";

                using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "PDF Files|*.pdf", FileName = "ExportData.pdf" })
                {
                    if (sfd.ShowDialog() != DialogResult.OK) return;
                    filePath = sfd.FileName;
                }

                PdfDocument document = new PdfDocument();
                document.Info.Title = "รายงานข้อมูลจาก DataGridView";

                // กำหนด orientation ตาม topic
                PdfPage page;
                XGraphics gfx;

                string topic = cbTopic.SelectedItem?.ToString() ?? "";

                if (topic == "รายงานการสร้างคูปอง")
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharp.PageOrientation.Landscape;
                    gfx = XGraphics.FromPdfPage(page);
                }
                else
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharp.PageOrientation.Portrait;
                    gfx = XGraphics.FromPdfPage(page);
                }

                XFont titleFont = new XFont("NotoSansThai-Regular", 18);
                XFont font = new XFont("NotoSansThai-Regular", 12);

                double margin = 40;
                double yPoint = margin;

                gfx.DrawString("รายงานข้อมูลจาก DataGridView", titleFont, XBrushes.Black,
                    new XRect(0, yPoint, page.Width, 40), XStringFormats.TopCenter);
                yPoint += 50;

                // ... (โค้ดวาดตารางเหมือนเดิม)

                // กรองคอลัมน์ oldBQID กับ status ออก ถ้าเป็นรายงานสร้างคูปอง
                List<int> colsToShow = new List<int>();
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    string colName = dataGridView1.Columns[i].Name.ToLower();
                    if (topic == "รายงานการสร้างคูปอง")
                    {
                        if (colName != "oldbqid" && colName != "status")
                            colsToShow.Add(i);
                    }
                    else
                    {
                        // รายงานอื่นแสดงทุกคอลัมน์
                        colsToShow.Add(i);
                    }
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

                // วาดข้อมูลแถว
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue;

                    for (int i = 0; i < colCount; i++)
                    {
                        int colIndex = colsToShow[i];
                        string cellText = row.Cells[colIndex].Value?.ToString() ?? "";
                        var rect = new XRect(margin + i * colWidth, yPoint, colWidth, rowHeight);
                        gfx.DrawRectangle(XPens.Black, rect);
                        gfx.DrawString(cellText, font, XBrushes.Black, rect, XStringFormats.CenterLeft);
                    }

                    yPoint += rowHeight;

                    if (yPoint + rowHeight > page.Height.Point - margin)
                    {
                        page = document.AddPage();
                        if (topic == "รายงานการสร้างคูปอง")
                            page.Orientation = PdfSharp.PageOrientation.Landscape;
                        else
                            page.Orientation = PdfSharp.PageOrientation.Portrait;

                        gfx = XGraphics.FromPdfPage(page);
                        yPoint = margin;
                    }
                }

                // footer รายงาน (เหมือนเดิม)
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
