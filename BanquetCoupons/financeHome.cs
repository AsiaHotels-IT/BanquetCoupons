using PdfSharp.Drawing.Layout;
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
using System.Drawing.Printing;
using System.IO;
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
        private FontManager fontManager;

        private void financeHome_Load(object sender, EventArgs e)
        {
            userLogin.Text = user;

            fontManager = new FontManager();
            // เดือน
            cbMonth.Items.AddRange(new string[]
            {
                "มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน",
                "พฤษภาคม", "มิถุนายน", "กรกฎาคม", "สิงหาคม",
                "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม"
            });

            cbYear.Items.Add(DateTime.Now.Year.ToString());
            

            cbMonth.SelectedIndex = DateTime.Now.Month - 1;
            cbYear.SelectedItem = DateTime.Now.Year.ToString();

            int selectedMonth = cbMonth.SelectedIndex + 1; // เดือนเริ่มที่ 0 ต้อง +1
            int selectedYear = int.Parse(cbYear.SelectedItem.ToString());

            LoadEventsByMonthAndYear(selectedMonth, selectedYear);
            dataGridView1.Font = new Font("Segoe UI", 10, FontStyle.Regular);  // ปรับขนาดตามต้องการ
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView1.RowTemplate.Height = 25;  // ปรับให้สูงขึ้นตามฟอนต์

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Height = 25;
            }
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.BackgroundColor = Color.White;

            this.UseWaitCursor = false;
            this.Cursor = Cursors.Default;
            dataGridView1.UseWaitCursor = false;
            dataGridView1.Cursor = Cursors.Default;
            cbMonth.Cursor= Cursors.Default;
            cbYear.Cursor= Cursors.Default;
            btnPrev.Cursor= Cursors.Default;
            btnNext.Cursor= Cursors.Default;
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



        private void LoadEventsByMonthAndYear(int selectedMonth, int selectedYear)
        {
            dataGridView1.DataSource = null;  // ✅ แก้ไขตรงนี้ก่อน
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            string connectionString = connectDB();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    var usageQuantities = new Dictionary<string, int>();
                    string usageQuery = @"
                SELECT BQID, ISNULL(SUM(quantity), 0) AS totalUsage
                FROM CouponUsage
                WHERE MONTH(useTime) = @Month AND YEAR(useTime) = @Year
                GROUP BY BQID";

                    using (SqlCommand cmdUsage = new SqlCommand(usageQuery, conn))
                    {
                        cmdUsage.Parameters.AddWithValue("@Month", selectedMonth);
                        cmdUsage.Parameters.AddWithValue("@Year", selectedYear);
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

                    string query = @"
                SELECT 
                    BQID,
                    cateringName,
                    agency,
                    mealDate,
                    SUM(quantity) AS totalQuantity
                FROM Coupons
                WHERE MONTH(mealDate) = @Month AND YEAR(mealDate) = @Year
                GROUP BY BQID, cateringName, agency, mealDate";

                    DataTable table = new DataTable();
                    table.Columns.Add("BQID");
                    table.Columns.Add("วันที่จัด");
                    table.Columns.Add("Catering");
                    table.Columns.Add("Agency");
                    table.Columns.Add("รวม");
                    table.Columns.Add("ใช้แล้ว");
                    table.Columns.Add("คงเหลือ");
                    table.Columns.Add("เปอร์เซ็นต์ใช้ (%)");

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Month", selectedMonth);
                        cmd.Parameters.AddWithValue("@Year", selectedYear);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int total = Convert.ToInt32(reader["totalQuantity"]);
                                string bqid = reader["BQID"].ToString();
                                int used = usageQuantities.ContainsKey(bqid) ? usageQuantities[bqid] : 0;
                                int remaining = total - used;
                                double percentUsed = total > 0 ? ((double)used / total) * 100 : 0;

                                string mealDate = Convert.ToDateTime(reader["mealDate"]).ToString("dd/MM/yyyy");

                                table.Rows.Add(
                                    bqid,
                                    mealDate,
                                    reader["cateringName"].ToString(),
                                    reader["agency"].ToString(),
                                    total,
                                    used,
                                    remaining,
                                    percentUsed.ToString("F1")
                                );
                            }
                        }
                    }

                    dataGridView1.DataSource = table;

                    dataGridView1.Columns[0].HeaderText = "BQID";
                    dataGridView1.Columns[1].HeaderText = "วันที่จัด";
                    dataGridView1.Columns[2].HeaderText = "ผู้จัดเลี้ยง";
                    dataGridView1.Columns[3].HeaderText = "หน่วยงาน";
                    dataGridView1.Columns[4].HeaderText = "รวม";
                    dataGridView1.Columns[5].HeaderText = "ใช้แล้ว";
                    dataGridView1.Columns[6].HeaderText = "คงเหลือ";
                    dataGridView1.Columns[7].HeaderText = "เปอร์เซ็นต์ใช้ (%)";

                    fullTable = table;
                    totalPages = (int)Math.Ceiling((double)fullTable.Rows.Count / pageSize);
                    currentPage = 1;

                    DisplayPage(currentPage); // แสดงหน้าแรก

                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
                }
            }
        }




        private void cbMonth_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cbYear.SelectedIndex != -1)
            {
                int selectedMonth = cbMonth.SelectedIndex + 1;
                int selectedYear = int.Parse(cbYear.SelectedItem.ToString());
                LoadEventsByMonthAndYear(selectedMonth, selectedYear);
            }
        }

        private void cbYear_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cbMonth.SelectedIndex != -1)
            {
                int selectedMonth = cbMonth.SelectedIndex + 1;
                int selectedYear = int.Parse(cbYear.SelectedItem.ToString());
                LoadEventsByMonthAndYear(selectedMonth, selectedYear);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
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
            XFont font = new XFont(fontName, fontSize, XFontStyleEx.Regular);

            while (gfx.MeasureString(text, font).Width > maxWidth && fontSize > 6)
            {
                fontSize -= 0.5;
                font = new XFont(fontName, fontSize, XFontStyleEx.Regular);
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

                XFont titleFont = new XFont("NotoSansThai-Regular", 16, XFontStyleEx.Bold);
                XFont defaultFont = new XFont("NotoSansThai-Regular", 10, XFontStyleEx.Regular);
                XTextFormatter tf = new XTextFormatter(gfx);

                double margin = 40;
                double yPoint = margin;
                double rowHeight = 25;

                gfx.DrawString("รายงานข้อมูลคูปอง", titleFont, XBrushes.Black,
                    new XRect(0, yPoint, page.Width, 40), XStringFormats.TopCenter);
                yPoint += 50;

                List<int> colsToShow = new List<int>();
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    colsToShow.Add(i);
                }

                int colCount = colsToShow.Count;
                double pageWidth = page.Width.Point - margin * 2;

                // กำหนดน้ำหนักความกว้างของแต่ละคอลัมน์
                HashSet<string> narrowColumns = new HashSet<string> { "รวม", "ใช้แล้ว", "คงเหลือ", "เปอร์เซ็น" };
                HashSet<string> wideColumns = new HashSet<string> { "หน่วยงาน", "ผู้จัดเลี้ยง" };

                Dictionary<int, double> columnWeights = new Dictionary<int, double>();
                double totalWeight = 0;

                foreach (int colIndex in colsToShow)
                {
                    string header = dataGridView1.Columns[colIndex].HeaderText;
                    double weight = narrowColumns.Contains(header) ? 0.5 :
                                    wideColumns.Contains(header) ? 2.0 : 1.0;

                    columnWeights[colIndex] = weight;
                    totalWeight += weight;
                }

                Dictionary<int, double> actualWidths = new Dictionary<int, double>();
                foreach (var kv in columnWeights)
                {
                    actualWidths[kv.Key] = (kv.Value / totalWeight) * pageWidth;
                }

                void DrawHeader()
                {
                    double x = margin;
                    foreach (int colIndex in colsToShow)
                    {
                        double width = actualWidths[colIndex];
                        string headerText = dataGridView1.Columns[colIndex].HeaderText;

                        var rect = new XRect(x, yPoint, width, rowHeight);
                        gfx.DrawRectangle(XPens.Black, rect);
                        gfx.DrawString(headerText, defaultFont, XBrushes.Black, rect, XStringFormats.Center);
                        x += width;
                    }
                    yPoint += rowHeight;
                }

                DrawHeader();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue;

                    if (yPoint + rowHeight > page.Height.Point - margin - 60)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharp.PageOrientation.Landscape;
                        gfx = XGraphics.FromPdfPage(page);
                        tf = new XTextFormatter(gfx);
                        yPoint = margin;
                        DrawHeader();
                    }

                    double x = margin;

                    foreach (int colIndex in colsToShow)
                    {
                        string cellText = row.Cells[colIndex].Value?.ToString() ?? "";
                        double width = actualWidths[colIndex];
                        var rect = new XRect(x, yPoint, width, rowHeight);
                        gfx.DrawRectangle(XPens.Black, rect);

                        // ปรับขนาดฟอนต์ให้พอดีกับช่อง
                        XFont fittedFont = FitTextToWidth(gfx, cellText, width - 6, "NotoSansThai-Regular", 12);

                        var paddedRect = new XRect(rect.X + 3, rect.Y, rect.Width - 6, rect.Height);
                        gfx.DrawString(cellText, fittedFont, XBrushes.Black, paddedRect, XStringFormats.CenterLeft);

                        x += width;
                    }

                    yPoint += rowHeight;
                }

                // Footer
                string footerUser = "ชื่อผู้จัดทำรายงาน";
                gfx.DrawString($"ผู้จัดทำรายงาน: {footerUser}", defaultFont, XBrushes.Black,
                    new XRect(margin, page.Height - 80, page.Width, 20), XStringFormats.TopLeft);

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
        private int pageSize = 15;
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
