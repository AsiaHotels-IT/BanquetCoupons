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
            loadData();
            fontManager = new FontManager();  // สร้างครั้งเดียวตอนโหลดฟอร์ม

            // ตั้งค่าฟอนต์ให้ Label ใน panel1
            foreach (Control ctl in panel1.Controls)
            {
                if (ctl is Label label)
                {
                    
                    if (label.Name == "lblPreview" )
                        label.Font = fontManager.FontSmall;
                    else if (label.Name == "label9")
                        label.Font = fontManager.FontSmall;
                    else if (label.Name == "lblSerialNumber")
                        label.Font = fontManager.FontSmall;
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

            comboBoxPaperSize.Items.Add("A4");
            comboBoxPaperSize.Items.Add("22.5x35.5");
            comboBoxPaperSize.Items.Add("26.7x36.4");
            comboBoxPaperSize.Items.Add("20.5x48");
            comboBoxPaperSize.SelectedIndex = 0; // ค่าเริ่มต้น

            
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.BackgroundColor = Color.White;
        }

       
        void loadData()
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

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string sql = @"
                SELECT * FROM Coupons
                ORDER BY 
                    CAST(SUBSTRING(BQID, 5, LEN(BQID)) AS INT),
                    serialNum";

                    SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt; // ต้องมี DataGridView ชื่อ dataGridView1 บนฟอร์ม
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
                // อ่าน config จาก ini
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

                    string newBQID = GenerateNewBQID(conn); // <-- สร้าง BQID ใหม่

                    DateTime selectedDate = mealDate.Value;
                    int qty = int.Parse(quantity.Text);

                    // เรียก Stored Procedure แทนการเขียน INSERT ตรงๆ
                    SqlCommand cmd = new SqlCommand("InsertCoupons", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // ส่งค่าพารามิเตอร์เข้า Stored Procedure
                    cmd.Parameters.AddWithValue("@mealDate", selectedDate);
                    cmd.Parameters.AddWithValue("@mealType", mealType.Text);
                    cmd.Parameters.AddWithValue("@agency", agency.Text);
                    cmd.Parameters.AddWithValue("@cateringName", canteenName.Text);
                    cmd.Parameters.AddWithValue("@quantity", qty);
                    cmd.Parameters.AddWithValue("@paperSize", comboBoxPaperSize.Text);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("บันทึกคูปองสำเร็จทั้งหมด", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    loadData();          // โหลดข้อมูลใหม่                   
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

            // เรียก stored procedure เพื่อ insert คูปอง และรับ BQID ที่ใช้
            string bqid = InsertCouponsAndReturnBQID();

            // ดึง serialNum ตาม BQID ที่เพิ่ง insert
            List<string> serialNumbers = GetSerialNumbersFromDB(bqid);

            if (serialNumbers.Count != count)
            {
                MessageBox.Show("จำนวนคูปองไม่ตรงกับที่คาดไว้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            double pageWidth = 21.0, pageHeight = 29.7;
            if (selectedPaper == "22.5x35.5") { pageWidth = 22.5; pageHeight = 35.5; }
            else if (selectedPaper == "26.7x36.4") { pageWidth = 26.7; pageHeight = 36.4; }
            else if (selectedPaper == "20.5x48") { pageWidth = 20.5; pageHeight = 48.0; }

            double couponWidth = 10.0, couponHeight = 5.0, marginX = 0.2, marginY = 0.5, spacingX = 0.2, spacingY = 0.5;
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
                        int index = (p * couponsPerPage) + couponsPrinted;
                        if (index >= count) break;

                        string serial = serialNumbers[index]; // ✅ ใช้ serialNum จริงจาก DB

                        UpdatePanelWithSerialNumber(serial);

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

        private void SaveCurrentCouponAsPDF()
        {
            string selectedPaper = comboBoxPaperSize.SelectedItem?.ToString() ?? "A4";

            double pageWidth = 21.0, pageHeight = 29.7; // Default A4

            if (selectedPaper == "22.5x35.5") { pageWidth = 22.5; pageHeight = 35.5; }
            else if (selectedPaper == "26.7x36.4") { pageWidth = 26.7; pageHeight = 36.4; }
            else if (selectedPaper == "20.5x48") { pageWidth = 20.5; pageHeight = 48.0; }

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
                    PdfDocument document = new PdfDocument();
                    document.Info.Title = "Single Coupon";

                    PdfPage page = document.AddPage();
                    page.Width = XUnit.FromCentimeter(pageWidth);
                    page.Height = XUnit.FromCentimeter(pageHeight);

                    XGraphics gfx = XGraphics.FromPdfPage(page);

                    // 🔼 จัดไว้บนกระดาษ: 0.5 ซม. จากขอบบน และซ้าย
                    double marginTop = 0.5;
                    double marginLeft = 0.5;
                    double couponWidth = 10.0;
                    double couponHeight = 5.0;

                    gfx.DrawImage(
                        ximg,
                        XUnit.FromCentimeter(marginLeft).Point,
                        XUnit.FromCentimeter(marginTop).Point,
                        XUnit.FromCentimeter(couponWidth).Point,
                        XUnit.FromCentimeter(couponHeight).Point
                    );

                    string filePath = Path.Combine(Path.GetTempPath(), "SingleCoupon_Top.pdf");
                    document.Save(filePath);
                    Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                }
            }
        }



        private void UpdatePanelWithSerialNumber(string serial)
        {
            lblSerialNumber.Text = serial; // หรืออะไรก็ตามที่คุณใช้ใน panel1 เพื่อแสดงหมายเลขคูปอง
            lblSerialNumber.Font = fontManager.FontBarcode;
            panel1.Refresh();
        }

        private string InsertCouponsAndReturnBQID()
        {
            // อ่าน config จาก ini
            string iniPath = "config.ini";
            var config = IniReader.ReadIni(iniPath, "Database");

            string server = config.ContainsKey("Server") ? config["Server"] : "";
            string database = config.ContainsKey("Database") ? config["Database"] : "";
            string user = config.ContainsKey("User") ? config["User"] : "";
            string password = config.ContainsKey("Password") ? config["Password"] : "";

            string connectionString = $"Server={server};Database={database};User Id={user};Password={password};";

            string bqid = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("InsertCoupons", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@agency", agency.Text);
                cmd.Parameters.AddWithValue("@mealDate", mealDate.Value);
                cmd.Parameters.AddWithValue("@mealType", mealType.Text);
                cmd.Parameters.AddWithValue("@cateringName", canteenName.Text);
                cmd.Parameters.AddWithValue("@quantity", int.Parse(quantity.Text));
                cmd.Parameters.AddWithValue("@paperSize", comboBoxPaperSize.SelectedItem.ToString());

                conn.Open();
                cmd.ExecuteNonQuery();

                // ดึง BQID ล่าสุดที่ insert
                using (SqlCommand getBqidCmd = new SqlCommand("SELECT TOP 1 BQID FROM Coupons ORDER BY createAt DESC", conn))
                {
                    bqid = (string)getBqidCmd.ExecuteScalar();
                }
            }

            return bqid;
        }

        private List<string> GetSerialNumbersFromDB(string bqid)
        {
            // อ่าน config จาก ini
            string iniPath = "config.ini";
            var config = IniReader.ReadIni(iniPath, "Database");

            string server = config.ContainsKey("Server") ? config["Server"] : "";
            string database = config.ContainsKey("Database") ? config["Database"] : "";
            string user = config.ContainsKey("User") ? config["User"] : "";
            string password = config.ContainsKey("Password") ? config["Password"] : "";

            string connectionString = $"Server={server};Database={database};User Id={user};Password={password};";

            List<string> serials = new List<string>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT serialNum FROM Coupons WHERE BQID = @bqid ORDER BY createAt", conn))
            {
                cmd.Parameters.AddWithValue("@bqid", bqid);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        serials.Add(reader.GetString(0));
                    }
                }
            }

            return serials;
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
            int thickness = 8;

            using (Pen pen = new Pen(Color.Black, thickness))
            {

                // วาดเส้นขวา
                e.Graphics.DrawLine(pen, pnl.Width - 1, 0, pnl.Width - 1, pnl.Height);
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
            btnReprint.Visible= false;
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
            editedCoupons();
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
                string selectedBQID = bqid.Text;  // BQID เดิมที่จะแก้ไข

                // ดึง serialNum จากแถวที่เลือกใน DataGridView
                string serialNum = dataGridView1.CurrentRow.Cells["serialNum"].Value.ToString();

                string connectionString = $"Server={server};Database={database};User Id={user};Password={password};";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    DateTime selectedDate = mealDate.Value;
                    int qty = int.Parse(quantity.Text);

                    SqlCommand cmd = new SqlCommand("InsertOrUpdateCoupon", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@oldBQID", selectedBQID);  // ใส่ BQID เก่า
                    cmd.Parameters.AddWithValue("@mealDate", selectedDate);
                    cmd.Parameters.AddWithValue("@mealType", mealType.Text);
                    cmd.Parameters.AddWithValue("@agency", agency.Text);
                    cmd.Parameters.AddWithValue("@cateringName", canteenName.Text);
                    cmd.Parameters.AddWithValue("@quantity", qty);
                    cmd.Parameters.AddWithValue("@paperSize", comboBoxPaperSize.Text);

                    // เพิ่มพารามิเตอร์ serialNum
                    cmd.Parameters.AddWithValue("@serialNum", serialNum);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("บันทึกคูปองเรียบร้อย", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    loadData();
                    SaveCouponAsPDF();
                    clearData();
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

                    string sql = "INSERT INTO EditedCoupons (BQID , editAt, Username) VALUES (@BQID, GETDATE(), @Username)";
                    SqlCommand cmd = new SqlCommand(sql, conn);

                    // เพิ่ม BQID เดิมเข้าไปด้วย
                    cmd.Parameters.AddWithValue("@BQID", selectedBQID);
                    cmd.Parameters.AddWithValue("@Username", userlogin);

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

            if (string.IsNullOrEmpty(selectedBQID) || string.IsNullOrEmpty(selectedSerialNum))
            {
                MessageBox.Show("กรุณาเลือกคูปองและระบุ Serial Number ที่ต้องการลบ", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                                "FROM Coupons WHERE BQID = @BQID AND serialNum = @serialNum";
                            SqlCommand cmdSelect = new SqlCommand(selectSql, conn, transaction);
                            cmdSelect.Parameters.AddWithValue("@BQID", selectedBQID);
                            cmdSelect.Parameters.AddWithValue("@serialNum", selectedSerialNum);

                            SqlDataReader reader = cmdSelect.ExecuteReader();

                            if (!reader.Read())
                            {
                                reader.Close();
                                MessageBox.Show("ไม่พบคูปองที่ต้องการลบ", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                transaction.Rollback();
                                return;
                            }

                            DateTime mealDate = reader.IsDBNull(0) ? DateTime.MinValue : reader.GetDateTime(0);
                            string mealType = reader.IsDBNull(1) ? null : reader.GetString(1);
                            string agency = reader.IsDBNull(2) ? null : reader.GetString(2);
                            string cateringName = reader.IsDBNull(3) ? null : reader.GetString(3);
                            int quantity = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                            string paperSize = reader.IsDBNull(5) ? null : reader.GetString(5);
                            string serialNum = reader.IsDBNull(6) ? null : reader.GetString(6);

                            reader.Close();

                            // 2. Insert ข้อมูลลง RemoveCoupons
                            string insertSql = @"
                        INSERT INTO RemoveCoupons 
                        (BQID, deleteAt, Username, mealDate, mealType, agency, cateringName, quantity, paperSize, serialNum)
                        VALUES 
                        (@BQID, GETDATE(), @Username, @mealDate, @mealType, @agency, @cateringName, @quantity, @paperSize, @serialNum)";
                            SqlCommand cmdInsert = new SqlCommand(insertSql, conn, transaction);

                            cmdInsert.Parameters.AddWithValue("@BQID", selectedBQID);
                            cmdInsert.Parameters.AddWithValue("@Username", currentUser);
                            cmdInsert.Parameters.Add("@mealDate", SqlDbType.DateTime).Value = (mealDate == DateTime.MinValue) ? (object)DBNull.Value : mealDate;
                            cmdInsert.Parameters.AddWithValue("@mealType", (object)mealType ?? DBNull.Value);
                            cmdInsert.Parameters.AddWithValue("@agency", (object)agency ?? DBNull.Value);
                            cmdInsert.Parameters.AddWithValue("@cateringName", (object)cateringName ?? DBNull.Value);
                            cmdInsert.Parameters.AddWithValue("@quantity", quantity);
                            cmdInsert.Parameters.AddWithValue("@paperSize", (object)paperSize ?? DBNull.Value);
                            cmdInsert.Parameters.AddWithValue("@serialNum", (object)serialNum ?? DBNull.Value);

                            cmdInsert.ExecuteNonQuery();

                            // 3. ลบข้อมูลจากตาราง Coupons จริง โดยใช้ทั้ง BQID และ serialNum
                            string deleteSql = "DELETE FROM Coupons WHERE BQID = @BQID AND serialNum = @serialNum";
                            SqlCommand cmdDelete = new SqlCommand(deleteSql, conn, transaction);
                            cmdDelete.Parameters.AddWithValue("@BQID", selectedBQID);
                            cmdDelete.Parameters.AddWithValue("@serialNum", selectedSerialNum);
                            cmdDelete.ExecuteNonQuery();

                            transaction.Commit();

                            MessageBox.Show("ลบคูปองเรียบร้อยแล้ว", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            loadData();   // โหลดข้อมูลใหม่หลังลบ
                            clearData();  // เคลียร์ฟอร์มหรือค่าใน control ถ้าต้องการ
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
            btnReprint.Visible= false;
            clearData();
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
                btnReprint.Visible = true;
                btnCancel.Visible = true;


                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                mealDate.Value = Convert.ToDateTime(row.Cells["mealDate"].Value);
                mealType.Text = row.Cells["mealType"].Value.ToString();
                agency.Text = row.Cells["agency"].Value.ToString();
                canteenName.Text = row.Cells["cateringName"].Value.ToString();
                quantity.Text = row.Cells["quantity"].Value.ToString();
                comboBoxPaperSize.Text = row.Cells["paperSize"].Value.ToString();
                lblSerialNumber.Text = row.Cells["SerialNum"].Value.ToString();
                bqid.Text = row.Cells["bqid"].Value.ToString();
                lblSerialNumber.Font = fontManager.FontBarcode;

                // หากมี form หรือ panel ซ่อนอยู่ให้แสดง
                // this.panelAddData.Visible = true; หรือ เปิด Form ใหม่ก็ได้
            }
        }

        private void btnReprint_Click(object sender, EventArgs e)
        {
            SaveCurrentCouponAsPDF();
        }
    }
}

