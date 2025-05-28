using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Reflection;


namespace BanquetCoupons
{
    public partial class Coupons : UserControl
    {

        public Coupons(string user)
        {
            InitializeComponent();
            this.user = user;
            userLogin.Text = user;
        }

        private string user;
        private FontManager fontManager;

        private bool isDatePicked = false;
        private void Coupons_Load(object sender, EventArgs e)
        {
            loadData(currentPage);
            LoadCateringRooms();
            fontManager = new FontManager();  // สร้างครั้งเดียวตอนโหลดฟอร์ม


            // ตั้งค่าฟอนต์ให้ Label ใน panel1
            foreach (Control ctl in panel1.Controls)
            {
                if (ctl is Label label)
                {
                    
                    if (label.Name == "lblPreview" )
                        label.Font = fontManager.FontSmall;
                    else if (label.Name == "label9")
                        label.Font = fontManager.FontTooltip;
                    else if (label.Name == "lblSerialNumber")
                        label.Font = fontManager.FontSmall;
                    else if (label.Name == "seNum")
                        label.Font = fontManager.FontSmallBold;
                    else
                        label.Font = fontManager.FontRegular;
                }
            }

            foreach (Control ctl in panelTopic.Controls)
            {
                if (ctl is Label label)
                {
                    if (label.Name == "label6" || label.Name == "label7" || label.Name == "label8")
                        label.Font = fontManager.FontBold;
                }
            }

            CultureInfo thaiCulture = new CultureInfo("th-TH");
            System.Threading.Thread.CurrentThread.CurrentCulture = thaiCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = thaiCulture;
            mealDate.Format = DateTimePickerFormat.Custom;
            mealDate.CustomFormat = "dd MMMM yyyy"; // เช่น 01 พฤษภาคม 2567
            mealDate.Format = DateTimePickerFormat.Custom;
            mealDate.CustomFormat = " ";  // แสดงค่าว่าง
            isDatePicked = false;

            lblPreview.Paint += lblPreview_Paint;
            panelTopic.Paint += panelTopic_Paint;
            panelCouponName.Paint += panelCouponName_Paint;
            label6.Font = fontManager.FontBold;
            label6.Height = 40;
            label7.Font = fontManager.FontBold;
            label8.Font = fontManager.FontBold;

            comboBoxPaperSize.Items.Add("A4");
            comboBoxPaperSize.Items.Add("22.5x35.5");
            comboBoxPaperSize.Items.Add("26.7x36.4");
            comboBoxPaperSize.Items.Add("20.5x48");
            comboBoxPaperSize.SelectedIndex = 0; // ค่าเริ่มต้น

            dataGridView1.Font = new Font("Segoe UI", 10, FontStyle.Regular);  // ปรับขนาดตามต้องการ
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView1.RowTemplate.Height = 25;  // ปรับให้สูงขึ้นตามฟอนต์

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Height = 25;
            }
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.BackgroundColor = Color.White;


            

            quantity.Minimum = 1;
            quantity.Maximum = 300; // หรือค่าเริ่มต้นทั่วไป เช่น 1000
            quantity.Value = 1;

            //lblPage.Font = fontManager.FontSerial;
        }

        private void LoadCateringRooms()
        {
            string iniPath = Path.Combine(System.Windows.Forms.Application.StartupPath, "config.ini");

            if (!File.Exists(iniPath))
            {
                MessageBox.Show("ไม่พบ config.ini");
                return;
            }

            var lines = File.ReadAllLines(iniPath);
            bool inSection = false;

            canteenName.Items.Clear();

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    inSection = line.Equals("[CateringRoom]", StringComparison.OrdinalIgnoreCase);
                    continue;
                }

                if (inSection && line.Contains("="))
                {
                    var parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        string value = parts[1].Trim();
                        string[] nameParts = value.Split('|');

                        // nameParts[0] = ชื่อภาษาไทย, nameParts[1] = ภาษาอังกฤษ
                        string displayName = nameParts[0];
                        canteenName.Items.Add(displayName);
                    }
                }
            }
        }



        //void loadData()
        //{
        //    string iniPath = "config.ini"; // ที่อยู่ไฟล์ .ini
        //
        //    // อ่าน config จาก section Database
        //    var config = IniReader.ReadIni(iniPath, "Database");
        //
        //    // ดึงค่าตัวแปรออกมา
        //    string server = config.ContainsKey("Server") ? config["Server"] : "";
        //    string database = config.ContainsKey("Database") ? config["Database"] : "";
        //    string user = config.ContainsKey("User") ? config["User"] : "";
        //    string password = config.ContainsKey("Password") ? config["Password"] : "";
        //
        //    // สร้าง connection string (SQL Server Auth)
        //    string connectionString = $"Server={server};Database={database};User Id={user};Password={password};";
        //
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        try
        //        {
        //            conn.Open();
        //
        //            string sql = @"
        //                            SELECT 
        //                                COALESCE(BQID, oldBQID) AS BQID,
        //                                agency,
        //                                mealDate,
        //                                mealType,
        //                                cateringName,
        //                                paperSize,
        //                                COUNT(DISTINCT serialNum) AS quantity
        //                            FROM Coupons
        //                            GROUP BY 
        //                                COALESCE(BQID, oldBQID),
        //                                agency,
        //                                mealDate,
        //                                mealType,
        //                                cateringName,
        //                                paperSize
        //                            ORDER BY 
        //                                CAST(SUBSTRING(COALESCE(BQID, oldBQID), 5, LEN(COALESCE(BQID, oldBQID))) AS INT),
        //                                paperSize;
        //                           ";
        //
        //            SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
        //            DataTable dt = new DataTable();
        //            adapter.Fill(dt);
        //
        //            dataGridView1.DataSource = dt; // ต้องมี DataGridView ชื่อ dataGridView1 บนฟอร์ม
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("เชื่อมต่อฐานข้อมูลไม่สำเร็จ: " + ex.Message);
        //        }
        //    }
        //}

        int currentPage = 1;
        int pageSize = 20;
        int totalRecords = 0;
        int totalPages = 0;

        void loadData(int page)
        {
            string iniPath = "config.ini";
            var config = IniReader.ReadIni(iniPath, "Database");

            string server = config.ContainsKey("Server") ? config["Server"] : "";
            string database = config.ContainsKey("Database") ? config["Database"] : "";
            string user = config.ContainsKey("User") ? config["User"] : "";
            string password = config.ContainsKey("Password") ? config["Password"] : "";

            string connectionString = $"Server={server};Database={database};User Id={user};Password={password};";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // 1. นับจำนวนข้อมูลทั้งหมด
                    string countSql = @"SELECT COUNT(DISTINCT serialNum) FROM Coupons";
                    SqlCommand countCmd = new SqlCommand(countSql, conn);
                    totalRecords = Convert.ToInt32(countCmd.ExecuteScalar());

                    totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                    int offset = (page - 1) * pageSize;

                    // 2. โหลดข้อมูลเฉพาะหน้าที่ต้องการ พร้อม serialNum
                    string sql = $@"
                                    SELECT 
                                        BQID,
                                        agency,
                                        mealDate,
                                        mealType,
                                        cateringName,
                                        paperSize,
                                        COUNT(*) AS quantity,
                                        MIN(serialNum) AS serialNum
                                    FROM Coupons
                                    GROUP BY 
                                        BQID,
                                        agency,
                                        mealDate,
                                        mealType,
                                        cateringName,
                                        paperSize;
                                   ";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Offset", offset);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;

                    //lblPage.Text = $"หน้า {currentPage}/{totalPages}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("เชื่อมต่อฐานข้อมูลไม่สำเร็จ: " + ex.Message);
                }
            }

        }


        private void PreviewMealDate()
        {
            if (!isDatePicked)
                return;

            mealDate.Format = DateTimePickerFormat.Custom;
            mealDate.CustomFormat = "dd MMMM yyyy";

            CultureInfo thaiCulture = new CultureInfo("th-TH");
            string MealDate = mealDate.Value.ToString("dd MMMM yyyy", thaiCulture);

            mealDatePreview.Text = $"{MealDate}";
        }

        private void mealDate_ValueChanged(object sender, EventArgs e)
        {
            isDatePicked = true;
            PreviewMealDate();
        }

        private void PreviewMealType()
        {
            string MealType = mealType.Text;

            lblPreview.Text = $"{MealType}";
        }
      

        private void lblPreview_Paint(object sender, PaintEventArgs e)
        {
            Label lbl = (Label)sender;
            int thickness = 1;

            using (Pen pen = new Pen(Color.Black, thickness))
            {
                // ปิดการเบลอของเส้น (optional)
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

                // วาดเส้นบน
                e.Graphics.DrawLine(pen, 0, 0, lbl.Width, 0);

                // วาดเส้นล่าง
                e.Graphics.DrawLine(pen, 0, lbl.Height - 1, lbl.Width, lbl.Height - 1);

                // วาดเส้นซ้าย
                e.Graphics.DrawLine(pen, 0, 0, 0, lbl.Height);

                // วาดเส้นขวา
                e.Graphics.DrawLine(pen, lbl.Width - 1, 0, lbl.Width - 1, lbl.Height);
            }
        }

        private void mealType_TextChanged(object sender, EventArgs e)
        {
            PreviewMealType();
        }

        private void agency_TextChanged(object sender, EventArgs e)
        {
            PreviewAgency();
        }

        private void PreviewAgency()
        {
            string Agency = agency.Text;
            agencyPreview.Text = $"{Agency}";
        }

        private void PreviewCanteen()
        {
            string Canteen = canteenName.Text;
            canteenPreview.Text = Canteen;
        }

        private void canteenName_TextChanged(object sender, EventArgs e)
        {
            PreviewCanteen();
        }

        private void clearData()
        {
            mealType.Text = "";
            agency.Text = "";
            canteenName.Text = "";
            quantity.Text = "";
            comboBoxPaperSize.Text = "";
            lblSerialNumber.Text = "";
            seNum.Text = "";
            bqid.Text = " ";

            // ล้างวันที่
            mealDate.Format = DateTimePickerFormat.Custom;
            mealDate.CustomFormat = " ";      // แสดงเป็นค่าว่าง
            mealDate.Value = DateTime.Now;    // ค่าภายใน (ไม่แสดง)
            isDatePicked = false;

            // ล้าง preview
            mealDatePreview.Text = " ";

            // ล้างสีพื้นหลัง
            mealType.BackColor = Color.White;
            agency.BackColor = Color.White;
            canteenName.BackColor = Color.White;
            quantity.BackColor = Color.White;
            mealDate.BackColor = Color.White;
            comboBoxPaperSize.BackColor = Color.White;
        }

        private void btnClearForm_Click(object sender, EventArgs e)
        {
            clearData();
        }

        private void saveData()
        {
            try
            {
                // อ่าน config.ini
                string iniPath = "config.ini";
                var config = IniReader.ReadIni(iniPath, "Database");

                string server = config.ContainsKey("Server") ? config["Server"] : "";
                string database = config.ContainsKey("Database") ? config["Database"] : "";
                string user = config.ContainsKey("User") ? config["User"] : "";
                string password = config.ContainsKey("Password") ? config["Password"] : "";

                string connectionString = $"Server={server};Database={database};User Id={user};Password={password};";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string newBQID = GenerateNewBQID(conn);
                    DateTime selectedDate = mealDate.Value;
                    int qty = int.Parse(quantity.Text);

                    // สุ่ม serial เลข 1 ครั้งต่อ BQID
                    Random rnd = new Random();
                    int serial = rnd.Next(10000, 99999);  // เลข 5 หลัก

                    SqlCommand cmd = new SqlCommand("InsertCoupons", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // ส่งพารามิเตอร์ครบถ้วน
                    cmd.Parameters.AddWithValue("@agency", agency.Text);
                    cmd.Parameters.AddWithValue("@mealDate", selectedDate);
                    cmd.Parameters.AddWithValue("@mealType", mealType.Text);
                    cmd.Parameters.AddWithValue("@cateringName", canteenName.Text);
                    cmd.Parameters.AddWithValue("@quantity", qty);  // ใส่จำนวนเต็มเลย ไม่ต้องวนลูป
                    cmd.Parameters.AddWithValue("@paperSize", comboBoxPaperSize.Text);

                    // ส่ง BQID และ SerialNum ด้วย ถ้า Stored Procedure มีรองรับ (ถ้าไม่มี ต้องเพิ่ม)
                    cmd.Parameters.AddWithValue("@couponNum", seNum.Text);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("บันทึกคูปองสำเร็จทั้งหมด", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    loadData(currentPage);          // โหลดข้อมูลใหม่                   
                    SaveCouponAsPDF();   // สร้าง PDF
                    clearData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        string GenerateNewBQID(SqlConnection conn)
        {
          SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(CAST(SUBSTRING(BQID, 5, LEN(BQID)) AS INT)), 0) FROM Coupons", conn);
          int maxId = (int)cmd.ExecuteScalar();
          int newId = maxId + 1;
          return "BQID" + newId.ToString("D3");
        }


        private void SaveCouponAsPDF()
        {
            int count = int.Parse(quantity.Text);
            string selectedPaper = comboBoxPaperSize.SelectedItem.ToString();

            // ✅ serialNumber ดึงจาก DB
            List<string> serialNumbers = GetSerialNumbersFromDatabase(count);

            if (serialNumbers.Count != count)
            {
                MessageBox.Show("จำนวนคูปองในฐานข้อมูลไม่เพียงพอ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            double pageWidth = 21.0, pageHeight = 29.7;
            if (selectedPaper == "22.5x35.5") { pageWidth = 22.5; pageHeight = 35.5; }
            else if (selectedPaper == "26.7x36.4") { pageWidth = 26.7; pageHeight = 36.4; }
            else if (selectedPaper == "20.5x48") { pageWidth = 20.5; pageHeight = 48.0; }

            double couponWidth = 10.0, couponHeight = 5.0, marginX = 0.2, marginY = 0.0, spacingX = 0.0, spacingY = 0.0;
            int couponsPerRow = 2;
            int couponsPerColumn = (int)Math.Floor((pageHeight - (2 * marginY)) / (couponHeight + spacingY));
            int couponsPerPage = couponsPerRow * couponsPerColumn;
            int pageCount = (int)Math.Ceiling(count / (double)couponsPerPage);

            PdfDocument document = new PdfDocument();
            document.Info.Title = "Meal Coupons Preview";

            int couponsPrinted = 0;

            for (int p = 0; p < pageCount; p++)
            {
                PdfPage page = document.AddPage();
                page.Width = XUnit.FromCentimeter(pageWidth);
                page.Height = XUnit.FromCentimeter(pageHeight);
                XGraphics gfx = XGraphics.FromPdfPage(page);

                for (int row = 0; row < couponsPerColumn; row++)
                {
                    for (int col = 0; col < couponsPerRow; col++)
                    {
                        int index = p * couponsPerPage + (row * couponsPerRow + col);
                        if (index >= count) break;

                        string serial = serialNumbers[index]; // ✅ ใช้ serialNum ที่สุ่มไว้

                        UpdatePanelWithSerialNumber("AS" + serial);

                        seNum.Text = ((index + 1).ToString("D3")) + " - " + serial;
                        panel1.CreateControl();
                        panel1.Refresh();
                        System.Windows.Forms.Application.DoEvents();

                        using (Bitmap bmp = new Bitmap(panel1.Width, panel1.Height))
                        {
                            panel1.DrawToBitmap(bmp, new Rectangle(0, 0, panel1.Width, panel1.Height));

                            using (MemoryStream ms = new MemoryStream())
                            {
                                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                ms.Position = 0;
                                XImage ximg = XImage.FromStream(ms);

                                double drawX = marginX + col * (couponWidth + spacingX);
                                double drawY = marginY + row * (couponHeight + spacingY);

                                gfx.DrawImage(
                                    ximg,
                                    XUnit.FromCentimeter(drawX).Point,
                                    XUnit.FromCentimeter(drawY).Point,
                                    XUnit.FromCentimeter(couponWidth).Point,
                                    XUnit.FromCentimeter(couponHeight).Point
                                );
                            }
                        }

                        couponsPrinted++;
                    }
                }
            }

            string tempPath = Path.Combine(Path.GetTempPath(), "MealCouponsPreview.pdf");
            document.Save(tempPath);
            Process.Start(new ProcessStartInfo(tempPath) { UseShellExecute = true });
        }

        private void SaveCouponAsPDF(string bqid)
        {
            string selectedPaper = comboBoxPaperSize.SelectedItem.ToString();

            // ✅ ดึง serialNumbers จาก DB โดยใช้ BQID
            List<string> serialNumbers = GetSerialNumbersFromDatabaseByBQID(bqid);

            if (serialNumbers == null || serialNumbers.Count == 0)
            {
                MessageBox.Show("ไม่พบ serial number สำหรับ BQID นี้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int count = serialNumbers.Count;

            double pageWidth = 21.0, pageHeight = 29.7;
            if (selectedPaper == "22.5x35.5") { pageWidth = 22.5; pageHeight = 35.5; }
            else if (selectedPaper == "26.7x36.4") { pageWidth = 26.7; pageHeight = 36.4; }
            else if (selectedPaper == "20.5x48") { pageWidth = 20.5; pageHeight = 48.0; }

            double couponWidth = 10.0, couponHeight = 5.0, marginX = 0.2, marginY = 0.0, spacingX = 0.0, spacingY = 0.0;
            int couponsPerRow = 2;
            int couponsPerColumn = (int)Math.Floor((pageHeight - (2 * marginY)) / (couponHeight + spacingY));
            int couponsPerPage = couponsPerRow * couponsPerColumn;
            int pageCount = (int)Math.Ceiling(count / (double)couponsPerPage);

            PdfDocument document = new PdfDocument();
            document.Info.Title = "Meal Coupons Preview";

            int couponsPrinted = 0;

            for (int p = 0; p < pageCount; p++)
            {
                PdfPage page = document.AddPage();
                page.Width = XUnit.FromCentimeter(pageWidth);
                page.Height = XUnit.FromCentimeter(pageHeight);
                XGraphics gfx = XGraphics.FromPdfPage(page);

                for (int row = 0; row < couponsPerColumn; row++)
                {
                    for (int col = 0; col < couponsPerRow; col++)
                    {
                        int index = p * couponsPerPage + (row * couponsPerRow + col);
                        if (index >= count) break;

                        string serial = serialNumbers[index];
                        UpdatePanelWithSerialNumber("AS" + serial);
                        seNum.Text = ((index + 1).ToString("D3")) + " - " + serial;

                        panel1.CreateControl();
                        panel1.Refresh();
                        System.Windows.Forms.Application.DoEvents();

                        using (Bitmap bmp = new Bitmap(panel1.Width, panel1.Height))
                        {
                            panel1.DrawToBitmap(bmp, new Rectangle(0, 0, panel1.Width, panel1.Height));

                            using (MemoryStream ms = new MemoryStream())
                            {
                                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                ms.Position = 0;
                                XImage ximg = XImage.FromStream(ms);

                                double drawX = marginX + col * (couponWidth + spacingX);
                                double drawY = marginY + row * (couponHeight + spacingY);

                                gfx.DrawImage(
                                    ximg,
                                    XUnit.FromCentimeter(drawX).Point,
                                    XUnit.FromCentimeter(drawY).Point,
                                    XUnit.FromCentimeter(couponWidth).Point,
                                    XUnit.FromCentimeter(couponHeight).Point
                                );
                            }
                        }

                        couponsPrinted++;
                    }
                }
            }

            string tempPath = Path.Combine(Path.GetTempPath(), "MealCouponsPreview.pdf");
            document.Save(tempPath);
            Process.Start(new ProcessStartInfo(tempPath) { UseShellExecute = true });
        }

        private List<string> GetSerialNumbersFromDatabaseByBQID(string bqid)
        {
            List<string> serialNumbers = new List<string>();

            string iniPath = "config.ini";
            var config = IniReader.ReadIni(iniPath, "Database");
            string connectionString = $"Server={config["Server"]};Database={config["Database"]};User Id={config["User"]};Password={config["Password"]};";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT serialNum FROM Coupons WHERE BQID = @BQID ORDER BY serialNum";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BQID", bqid);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                                serialNumbers.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return serialNumbers;
        }

        private List<string> GetSerialNumbersFromDatabase(int count)
        {
            List<string> serials = new List<string>();

            // อ่าน config จาก ini
            string iniPath = "config.ini";
            var config = IniReader.ReadIni(iniPath, "Database");

            string server = config.ContainsKey("Server") ? config["Server"] : "";
            string database = config.ContainsKey("Database") ? config["Database"] : "";
            string user = config.ContainsKey("User") ? config["User"] : "";
            string password = config.ContainsKey("Password") ? config["Password"] : "";

            string connectionString = $"Server={server};Database={database};User Id={user};Password={password};";
            string query = "SELECT TOP (@count) serialNum FROM Coupons WHERE BQID = (SELECT MAX(BQID) FROM Coupons) ORDER BY couponNum ASC"; // ✅ หรือเปลี่ยนเป็นเงื่อนไขอื่นที่เหมาะสม เช่น WHERE mealDate = @today

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@count", count);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        serials.Add(reader["serialNum"].ToString());
                    }
                }
            }

            return serials;
        }


        private void UpdatePanelWithSerialNumber(string serial)
        {
            lblSerialNumber.Text = serial; // หรืออะไรก็ตามที่คุณใช้ใน panel1 เพื่อแสดงหมายเลขคูปอง
            lblSerialNumber.Font = fontManager.FontBarcode;
            panel1.Refresh();
        }




        private void btnSave_Click(object sender, EventArgs e)
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(mealType.Text))
            {
                mealType.BackColor = Color.MistyRose;
                isValid = false;
            }
            else
            {
                mealType.BackColor = Color.White;
            }
            if (string.IsNullOrWhiteSpace(agency.Text))
            {
                agency.BackColor = Color.MistyRose;
                isValid = false;
            }
            else
            {
                agency.BackColor = Color.White;
            }
            if (string.IsNullOrWhiteSpace(canteenName.Text))
            {
                canteenName.BackColor = Color.MistyRose;
                isValid = false;
            }
            else
            {
                canteenName.BackColor = Color.White;
            }
            if (!int.TryParse(quantity.Text, out int qty) || qty <= 0)
            {
                quantity.BackColor = Color.MistyRose;
                MessageBox.Show("กรุณากรอกจำนวนที่เป็นตัวเลขและต้องมากกว่า 0", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                quantity.BackColor = Color.White;
            }

            if (!isDatePicked)
            {
                mealDate.BackColor = Color.MistyRose;
                MessageBox.Show("กรุณากรอกวันที่", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                mealDate.BackColor = Color.White;
            }

            saveData();
        }

        private void panelTopic_Paint(object sender, PaintEventArgs e)
        {
            Panel pnl = (Panel)sender;
            int thickness = 12;

            using (Pen pen = new Pen(Color.Black, thickness))
            {

                // วาดเส้นขวา
                e.Graphics.DrawLine(pen, pnl.Width - 1, 0, pnl.Width - 1, 80);
            }
        }

        private void panelCouponName_Paint(object sender, PaintEventArgs e)
        {
            Panel pnl = (Panel)sender;
            int thickness = 4;

            using (Pen dashedPen = new Pen(Color.DarkGray, thickness))
            {
                // รูปแบบเส้นประ: [ความยาวเส้น, ความยาวช่องว่าง]
                dashedPen.DashPattern = new float[] { 2, 1 }; // เส้นยาว 4px, เว้นว่าง 2px

                // วาดเส้นประด้านซ้าย
                e.Graphics.DrawLine(dashedPen, 0, 0, 0, pnl.Height);
            }
        }


        private void btnEdit_Click(object sender, EventArgs e)
        {
            btnEditNprint.Visible = true;
            mealDate.Enabled = true;
            mealType.Enabled = true;
            agency.Enabled = true;
            canteenName.Enabled = true;
            comboBoxPaperSize.Enabled = true;
            quantity.Value = 1;
            quantity.Enabled = true;      
            btnCancel.Visible= true;
        }

        private void btnEditNprint_Click(object sender, EventArgs e)
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(mealType.Text))
            {
                mealType.BackColor = Color.MistyRose;
                isValid = false;
            }
            else
            {
                mealType.BackColor = Color.White;
            }
            if (string.IsNullOrWhiteSpace(agency.Text))
            {
                agency.BackColor = Color.MistyRose;
                isValid = false;
            }
            else
            {
                agency.BackColor = Color.White;
            }
            if (string.IsNullOrWhiteSpace(canteenName.Text))
            {
                canteenName.BackColor = Color.MistyRose;
                isValid = false;
            }
            else
            {
                canteenName.BackColor = Color.White;
            }
            if (!int.TryParse(quantity.Text, out int qty) || qty <= 0)
            {
                quantity.BackColor = Color.MistyRose;
                MessageBox.Show("กรุณากรอกจำนวนที่เป็นตัวเลขและต้องมากกว่า 0", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                quantity.BackColor = Color.White;
            }

            if (!isDatePicked)
            {
                mealDate.BackColor = Color.MistyRose;
                MessageBox.Show("กรุณากรอกวันที่", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                mealDate.BackColor = Color.White;
            }

            EditNprint();
            //editedCoupons();
            btnEdit.Visible = false;
            btnEditNprint.Visible = false;
            btnSave.Visible = true;
            btnClearForm.Visible = true;
            btnDel.Visible = false;
            btnCancel.Visible = false;
        }

        private void EditNprint()
        {
            try
            {
                string iniPath = "config.ini";
                var config = IniReader.ReadIni(iniPath, "Database");

                string server = config.ContainsKey("Server") ? config["Server"] : "";
                string database = config.ContainsKey("Database") ? config["Database"] : "";
                string user = config.ContainsKey("User") ? config["User"] : "";
                string password = config.ContainsKey("Password") ? config["Password"] : "";
                string selectedBQID = bqid.Text;
                string currentUser = userLogin.Text;
                string connectionString = $"Server={server};Database={database};User Id={user};Password={password};";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. ดึงข้อมูลคูปองเก่าทั้งหมดตาม BQID
                            string selectSql = "SELECT serialNum FROM Coupons WHERE BQID = @BQID";
                            SqlCommand cmdSelect = new SqlCommand(selectSql, conn, transaction);
                            cmdSelect.Parameters.AddWithValue("@BQID", selectedBQID);

                            List<string> oldSerials = new List<string>();

                            using (SqlDataReader reader = cmdSelect.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    oldSerials.Add(reader["serialNum"].ToString());
                                }
                            }

                            // 2. ลบคูปองเก่า
                            string deleteSql = "DELETE FROM Coupons WHERE BQID = @BQID";
                            SqlCommand cmdDelete = new SqlCommand(deleteSql, conn, transaction);
                            cmdDelete.Parameters.AddWithValue("@BQID", selectedBQID);
                            cmdDelete.ExecuteNonQuery();

                            // 3. เพิ่มคูปองใหม่ โดยใช้ serialNum เดิม
                            DateTime selectedDate = mealDate.Value;
                            string meal = mealType.Text;
                            string ag = agency.Text;
                            string cater = canteenName.Text;
                            string paper = comboBoxPaperSize.Text;
                            int newQuantity = (int)quantity.Value;

                            for (int i = 0; i < newQuantity; i++)
                            {
                                string serial = seNum.Text;

                                SqlCommand cmdInsert = new SqlCommand(
                                    "INSERT INTO Coupons (BQID, mealDate, mealType, agency, cateringName, quantity, paperSize, serialNum, status, couponNum  ) " +
                                    "VALUES (@BQID, @mealDate, @mealType, @agency, @cateringName, @quantity, @paperSize, @serialNum , 'edit', @couponNum)",
                                    conn, transaction);

                                cmdInsert.Parameters.AddWithValue("@BQID", selectedBQID);
                                cmdInsert.Parameters.AddWithValue("@mealDate", selectedDate.Date);
                                cmdInsert.Parameters.AddWithValue("@mealType", meal);
                                cmdInsert.Parameters.AddWithValue("@agency", ag);
                                cmdInsert.Parameters.AddWithValue("@cateringName", cater);
                                cmdInsert.Parameters.AddWithValue("@quantity", 1);
                                cmdInsert.Parameters.AddWithValue("@paperSize", paper);
                                cmdInsert.Parameters.AddWithValue("@serialNum", serial);
                                cmdInsert.Parameters.AddWithValue("@couponNum", (i + 1).ToString("D3"));

                                cmdInsert.ExecuteNonQuery();
                            }



                            transaction.Commit();

                            MessageBox.Show("แก้ไขและสร้างคูปองใหม่เรียบร้อยแล้ว");
                            loadData(currentPage);
                            SaveCouponAsPDF(selectedBQID);
                            clearData();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }



        private void editedCoupons()
        {
            try
            {
                // อ่าน config จาก ini
                string iniPath = "config.ini";
                var config = IniReader.ReadIni(iniPath, "Database");

                string server = config.ContainsKey("Server") ? config["Server"] : "";
                string database = config.ContainsKey("Database") ? config["Database"] : "";
                string user = config.ContainsKey("User") ? config["User"] : "";
                string password = config.ContainsKey("Password") ? config["Password"] : "";
                string selectedBQID = bqid.Text;
                string userlogin = userLogin.Text;
                

                string connectionString = $"Server={server};Database={database};User Id={user};Password={password};";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    int qty = (int)quantity.Value;
                    string sql = "INSERT INTO UpdateCoupons (BQID , editAt, Username, quantity , serialNum) VALUES (@BQID, GETDATE(), @Username, @quantity , @serialNum)";
                    SqlCommand cmd = new SqlCommand(sql, conn);

                    // เพิ่ม BQID เดิมเข้าไปด้วย
                    cmd.Parameters.AddWithValue("@BQID", selectedBQID);
                    cmd.Parameters.AddWithValue("@Username", userlogin);
                    cmd.Parameters.AddWithValue("@quantity", qty);

                    cmd.ExecuteNonQuery();
                    clearData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            string selectedBQID = bqid.Text.Trim();
            string selectedSerialNum = lblSerialNumber.Text.Trim();  // เพิ่มรับ serialNum จาก UI
            string currentUser = userLogin.Text.Trim();

            if (string.IsNullOrEmpty(selectedBQID) )
            {
                MessageBox.Show("กรุณาเลือกคูปองและระบุ Serial Number ที่ต้องการลบ", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // เพิ่ม MessageBox ถามยืนยัน
            DialogResult result = MessageBox.Show("คุณต้องการลบคูปองชุดนี้ใช่หรือไม่?", "ยืนยันการลบ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                // ถ้าไม่กด Yes ให้ยกเลิกการลบ
                return;
            }

            try
            {
                string iniPath = "config.ini";
                var config = IniReader.ReadIni(iniPath, "Database");

                string server = config.ContainsKey("Server") ? config["Server"] : "";
                string database = config.ContainsKey("Database") ? config["Database"] : "";
                string user = config.ContainsKey("User") ? config["User"] : "";
                string password = config.ContainsKey("Password") ? config["Password"] : "";

                string connectionString = $"Server={server};Database={database};User Id={user};Password={password};";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. ดึงข้อมูลคูปองจากตาราง Coupons ด้วย BQID และ serialNum
                            string selectSql = "SELECT mealDate, mealType, agency, cateringName, quantity, paperSize, serialNum " +
                   "FROM Coupons WHERE BQID = @BQID";
                            SqlCommand cmdSelect = new SqlCommand(selectSql, conn, transaction);
                            cmdSelect.Parameters.AddWithValue("@BQID", selectedBQID);

                            SqlDataReader reader = cmdSelect.ExecuteReader();

                            if (!reader.HasRows)
                            {
                                reader.Close();
                                MessageBox.Show("ไม่พบคูปองที่ต้องการลบ", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                transaction.Rollback();
                                return;
                            }

                            // เตรียมลิสต์ไว้เก็บข้อมูลทั้งหมดก่อน insert
                            var couponsToDelete = new List<(DateTime mealDate, string mealType, string agency, string cateringName, int quantity, string paperSize, string serialNum)>();

                            while (reader.Read())
                            {
                                DateTime mealDate = reader.IsDBNull(0) ? DateTime.MinValue : reader.GetDateTime(0);
                                string mealType = reader.IsDBNull(1) ? null : reader.GetString(1);
                                string agency = reader.IsDBNull(2) ? null : reader.GetString(2);
                                string cateringName = reader.IsDBNull(3) ? null : reader.GetString(3);
                                int quantity = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                                string paperSize = reader.IsDBNull(5) ? null : reader.GetString(5);
                                string serialNum = reader.IsDBNull(6) ? null : reader.GetString(6);

                                couponsToDelete.Add((mealDate, mealType, agency, cateringName, quantity, paperSize, serialNum));
                            }

                            reader.Close();

                            // insert ข้อมูลทั้งหมดลง RemoveCoupons
                            foreach (var c in couponsToDelete)
                            {
                                string insertSql = @"
                                            INSERT INTO RemoveCoupons 
                                            (BQID, deleteAt, Username, mealDate, mealType, agency, cateringName, quantity, paperSize, serialNum)
                                            VALUES 
                                            (@BQID, GETDATE(), @Username, @mealDate, @mealType, @agency, @cateringName, @quantity, @paperSize, @serialNum)";

                                SqlCommand cmdInsert = new SqlCommand(insertSql, conn, transaction);
                                cmdInsert.Parameters.AddWithValue("@BQID", selectedBQID);
                                cmdInsert.Parameters.AddWithValue("@Username", currentUser);
                                cmdInsert.Parameters.Add("@mealDate", SqlDbType.DateTime).Value = (c.mealDate == DateTime.MinValue) ? (object)DBNull.Value : c.mealDate;
                                cmdInsert.Parameters.AddWithValue("@mealType", (object)c.mealType ?? DBNull.Value);
                                cmdInsert.Parameters.AddWithValue("@agency", (object)c.agency ?? DBNull.Value);
                                cmdInsert.Parameters.AddWithValue("@cateringName", (object)c.cateringName ?? DBNull.Value);
                                cmdInsert.Parameters.AddWithValue("@quantity", c.quantity);
                                cmdInsert.Parameters.AddWithValue("@paperSize", (object)c.paperSize ?? DBNull.Value);
                                cmdInsert.Parameters.AddWithValue("@serialNum", (object)c.serialNum ?? DBNull.Value);

                                cmdInsert.ExecuteNonQuery();
                            }

                            // 3. ลบข้อมูลทั้งหมดใน Coupons ตาม BQID
                            string deleteSql = "DELETE FROM Coupons WHERE BQID = @BQID";
                            SqlCommand cmdDelete = new SqlCommand(deleteSql, conn, transaction);
                            cmdDelete.Parameters.AddWithValue("@BQID", selectedBQID);
                            cmdDelete.ExecuteNonQuery();

                            transaction.Commit();

                            MessageBox.Show("ลบคูปองทั้งชุดเรียบร้อยแล้ว", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            loadData(currentPage);
                            clearData();

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("เกิดข้อผิดพลาดในการลบคูปอง: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnEdit.Visible= false;
            btnEditNprint.Visible= false;
            btnDel.Visible= false;
            btnCancel.Visible= false;
            btnSave.Visible= true;
            btnClearForm.Visible= true;
            clearData();
            mealDate.Enabled = true;
            mealType.Enabled = true;
            agency.Enabled = true;
            canteenName.Enabled = true;
            comboBoxPaperSize.Enabled = true;
            quantity.Value = 1;
            quantity.Enabled = true;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnSave.Visible = false;
                btnClearForm.Visible = false;
                mealDate.Enabled = false;
                mealType.Enabled = false;
                agency.Enabled = false;
                canteenName.Enabled = false;
                comboBoxPaperSize.Enabled = false;
                quantity.Value = 1;
                quantity.Enabled = false;
                btnEdit.Visible = true;
                bqid.Visible = true;
                btnDel.Visible = true;
                btnCancel.Visible = true;
                btnEditNprint.Visible = true;


                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                mealDate.Value = Convert.ToDateTime(row.Cells["mealDate"].Value);
                mealType.Text = row.Cells["mealType"].Value.ToString();
                agency.Text = row.Cells["agency"].Value.ToString();
                canteenName.Text = row.Cells["cateringName"].Value.ToString();
                quantity.Text = row.Cells["quantity"].Value.ToString();
                comboBoxPaperSize.Text = row.Cells["paperSize"].Value.ToString();
                bqid.Text = row.Cells["bqid"].Value.ToString();
                seNum.Text = row.Cells["serialNum"].Value.ToString();
                // หากมี form หรือ panel ซ่อนอยู่ให้แสดง
                // this.panelAddData.Visible = true; หรือ เปิด Form ใหม่ก็ได้
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                loadData(currentPage);
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                loadData(currentPage);
            }
        }

        private void quantity_ValueChanged(object sender, EventArgs e)
        {
            if (int.TryParse(quantity.Text, out int sumQty))
            {
                if (quantity.Value >= 300)
                {
                    MessageBox.Show($"กรอกตัวเลขได้ไม่เกิน {sumQty}", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    quantity.BackColor = Color.Red;
                    quantity.Value = sumQty;
                }
                else
                {
                    quantity.BackColor = Color.White;
                }
            }
        }
    }
}

