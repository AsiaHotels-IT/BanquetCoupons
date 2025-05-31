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
using PdfSharp.Drawing.Layout;
using System.Windows.Documents;


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
           
            int currentYear = DateTime.Now.Year;
            int futureYear = currentYear + 10; // เพิ่มล่วงหน้าอีก 10 ปี

            for (int y = 2025; y <= futureYear; y++)
            {
                cbYear.Items.Add(y.ToString());
            }

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

            userLogin.Text = user;

            dataGridView1.Font = new Font("Segoe UI", 10, FontStyle.Regular);  // ปรับขนาดตามต้องการ
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView1.RowTemplate.Height = 25;  // ปรับให้สูงขึ้นตามฟอนต์

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Height = 25;
            }
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.BackgroundColor = Color.White;

            dataGridView1.ReadOnly = true; // ป้องกันไม่ให้แก้ไข
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // เลือกทั้งแถวเมื่อคลิก
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
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
                            SELECT 
                                BQID,
                                agency,
                                CONVERT(VARCHAR(10), mealDate, 103) AS mealDate,
                                mealType,
                                cateringName,
                                serialNum,
                                SUM(quantity) AS quantity,  -- รวม quantity
                                CONVERT(VARCHAR(10), MIN(createAt), 103) AS createAt
                            FROM Coupons
                            {(string.IsNullOrEmpty(whereClause) ? "" : "WHERE " + whereClause)}
                            GROUP BY BQID, agency, mealDate, mealType, cateringName, serialNum
                            ORDER BY BQID DESC
                           ";


                    break;

                case "รายงานการลบคูปอง":
                    sql = $@"
                            SELECT BQID, deleteAt, Username, mealDate, agency, cateringName, quantity, serialNum
                            FROM RemoveCoupons
                            {(string.IsNullOrEmpty(whereClause) ? "" : "WHERE " + whereClause)}
                            ORDER BY BQID DESC
                            ";
                    break;

                case "รายงานการแก้ไขคูปอง":
                    sql = $@"
                            SELECT EID, BQID, Username, quantity, serialNum, editAt
                            FROM UpdateCoupons
                            {(string.IsNullOrEmpty(whereClause) ? "" : "WHERE " + whereClause)}
                            ORDER BY BQID DESC
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

                fullTable = dt; // ⭐ เก็บข้อมูลทั้งหมด
                totalPages = (int)Math.Ceiling((double)fullTable.Rows.Count / pageSize); // ⭐ คำนวณจำนวนหน้า
                currentPage = 1; // ⭐ รีเซ็ตกลับหน้าแรก

                DisplayPage(currentPage); // ⭐ แสดงหน้าแรก
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
            exportFilePDF();


        }

        private XFont FitTextToWidth(XGraphics gfx, string text, double maxWidth, string fontName, double initialSize)
        {
            double fontSize = initialSize;
            XFont font = new XFont(fontName, fontSize);

            // ลดขนาดฟอนต์ลงจนพอดีกับคอลัมน์ หรือถึงขนาดต่ำสุด
            while (gfx.MeasureString(text, font).Width > maxWidth && fontSize > 6)
            {
                fontSize -= 0.5;
                font = new XFont(fontName, fontSize);
            }

            return font;
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
                page.Orientation = PdfSharp.PageOrientation.Landscape;
                XGraphics gfx = XGraphics.FromPdfPage(page);
        
                XFont titleFont = new XFont("NotoSansThai-Regular", 16);
                XFont defaultFont = new XFont("NotoSansThai-Regular", 10);
        
                string topic = cbTopic.SelectedItem?.ToString() ?? "";
                double margin = 40;
                double yPoint = margin;
        
                gfx.DrawString($"{topic}", titleFont, XBrushes.Black,
                    new XRect(0, yPoint, page.Width, 40), XStringFormats.TopCenter);
                yPoint += 50;
        
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
                        colsToShow.Add(i);
                    }
                }
        
                int colCount = colsToShow.Count;
                double pageWidth = page.Width.Point - margin * 2;
                double colWidth = pageWidth / colCount;
                double rowHeight = 25;
        
                // Header
                for (int i = 0; i < colCount; i++)
                {
                    int colIndex = colsToShow[i];
                    string headerText = dataGridView1.Columns[colIndex].HeaderText;
                    var rect = new XRect(margin + i * colWidth, yPoint, colWidth, rowHeight);
                    gfx.DrawRectangle(XPens.Black, rect);
                    gfx.DrawString(headerText, defaultFont, XBrushes.Black, rect, XStringFormats.Center);
                }
                yPoint += rowHeight;
        
                // Rows
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
        
                        XFont adjustedFont = FitTextToWidth(gfx, cellText, paddedRect.Width, "NotoSansThai-Regular", 10);
        
                        XTextFormatter tf = new XTextFormatter(gfx);
                        tf.Alignment = XParagraphAlignment.Left;

                        XSize textSize = gfx.MeasureString(cellText, adjustedFont);
                        double offsetY = rect.Y + (rowHeight - textSize.Height) / 2;

                        var adjustedRect = new XRect(rect.X + 5, offsetY, rect.Width - 10, textSize.Height);
                        tf.DrawString(cellText, adjustedFont, XBrushes.Black, adjustedRect, XStringFormats.TopLeft);

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
        
                // Footer
        
                gfx.DrawString($"วันที่พิมพ์: {DateTime.Now:dd/MM/yyyy HH:mm}", defaultFont, XBrushes.Black,
                    new XRect(margin, page.Height - 60, page.Width, 20), XStringFormats.TopLeft);
        
                gfx.DrawString("หมายเหตุ: รายงานนี้สร้างจากระบบอัตโนมัติ", defaultFont, XBrushes.Gray,
                    new XRect(margin, page.Height - 40, page.Width, 20), XStringFormats.TopLeft);
        
                document.Save(filePath);
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
            }
        }

        private DataTable fullTable; // เก็บข้อมูลทั้งหมด
        private int pageSize = 20;
        private int currentPage = 1;
        private int totalPages = 1;



        private void DisplayPage(int page)
        {
            if (fullTable == null) return;

            DataTable currentPageTable = fullTable.Clone(); // โครงสร้างเหมือนกัน

            int startIndex = (page - 1) * pageSize;
            int endIndex = Math.Min(startIndex + pageSize, fullTable.Rows.Count);

            for (int i = startIndex; i < endIndex; i++)
            {
                currentPageTable.ImportRow(fullTable.Rows[i]);
            }

            dataGridView1.DataSource = currentPageTable;
            //labelPageInfo.Text = $"หน้า {currentPage} จาก {totalPages}";
            btnNext.Enabled = currentPage < totalPages;
            btnPrev.Enabled = currentPage > 1;

        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                DisplayPage(currentPage);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                DisplayPage(currentPage);
            }
        }
    }
}
