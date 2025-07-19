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
using Org.BouncyCastle.Asn1.Pkcs;


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
            label6.Height = 40;

            comboBoxPaperSize.Items.Add("A4");
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

            dataGridView1.ReadOnly = true; // ป้องกันไม่ให้แก้ไข
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // เลือกทั้งแถวเมื่อคลิก
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;


            canteenName.DropDownStyle = ComboBoxStyle.DropDownList;

            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            lblSerialNumber.Font = fontManager.FontBarcode;
            listBox1.SelectedItem = "TH";


        }

        PrivateFontCollection pfc = new PrivateFontCollection();


        private void LoadCateringRooms()
        {
            string iniPath = Path.Combine(System.Windows.Forms.Application.StartupPath, "config.ini");

            if (!File.Exists(iniPath))
            {
                MessageBox.Show("ไม่พบ config.ini");
                return;
            }

            canteenName.MaxDropDownItems = 5;
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
                                        paperSize
                                    ORDER BY BQID desc
                                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
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

            DateTime selectedDate = mealDate.Value;

            // วันที่แบบไทย
            CultureInfo thaiCulture = new CultureInfo("th-TH");
            string mealDateThai = selectedDate.ToString("dd MMMM yyyy", thaiCulture);
            mealDatePreview.Text = mealDateThai;

            // วันที่แบบอังกฤษ
            CultureInfo engCulture = new CultureInfo("en-US");
            string mealDateEnglish = selectedDate.ToString("dd MMMM yyyy", engCulture);
            label18.Text = mealDateEnglish;
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
            label17.Text = MealType;
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
            label19.Text = $"{Agency}";

            label19.Font = new Font(label19.Font.FontFamily, 30, label19.Font.Style); // กลับไปขนาดปกติ
            agencyPreview.Font = new Font(agencyPreview.Font.FontFamily, 30, agencyPreview.Font.Style); // กลับไปขนาดปกติ

        }

        private void PreviewCanteen()
        {
            string Canteen = canteenName.Text;
            canteenPreview.Text = Canteen;
            label20.Text = Canteen;

            label20.Font = new Font(label20.Font.FontFamily, 30, label20.Font.Style); // กลับไปขนาดปกติ
            canteenPreview.Font = new Font(canteenPreview.Font.FontFamily, 30, canteenPreview.Font.Style); // กลับไปขนาดปกติ

            int selectedIndex = canteenName.SelectedIndex;
            label20.AutoSize = false;
            label20.Location = new Point(339, 331);

            if (selectedIndex == 18)
            {
                // เพิ่ม \n เพื่อขึ้นบรรทัดใหม่เอง หรือปล่อยให้ตัดอัตโนมัติ
                label20.Text = "   RATCHATHEWI \nGRAND BALLROOM";
                label20.AutoSize = true;
                label20.Location = new Point(339, 311);
            }
        }

        private void canteenName_TextChanged(object sender, EventArgs e)
        {
            PreviewCanteen();
        }

        private void clearData()
        {
            mealType.Text = "";
            agency.Text = "";
            canteenName.SelectedIndex = 0;
            quantity.Text = "";
            comboBoxPaperSize.Text = "";
            lblSerialNumber.Text = "";
            seNum.Text = "";
            bqid.Text = " ";

            // ล้างวันที่
            mealDate.Format = DateTimePickerFormat.Custom;
            mealDate.CustomFormat = " ";      // แสดงเป็นค่าว่าง
            mealDate.Value = DateTime.Now;
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

            double pageWidth = 21.3, pageHeight = 30.0;

            double couponWidth = 10.5, couponHeight = 5.5, marginX = 0.0, marginY = 0.0, spacingX = 0.0, spacingY = 0.0;
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
                        // ✅ เพิ่มขนาดฟอนต์ตรงนี้ให้ใหญ่ขึ้น (ลำดับที่ 2)
                        seNum.Font = new Font(seNum.Font.FontFamily, 20, FontStyle.Bold); // หรือเปลี่ยนเป็น FontBold ที่คุณปรับไว้แล้วก็ได้
                        seNum.Text = ((index + 1).ToString("D3")) + " - " + serial;
                        label9.Font = new Font(label9.Font.FontFamily, 25, FontStyle.Regular);
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

            // ✅ ดึง serialNumbers จาก DB โดยใช้ BQID
            List<string> serialNumbers = GetSerialNumbersFromDatabaseByBQID(bqid);

            if (serialNumbers == null || serialNumbers.Count == 0)
            {
                MessageBox.Show("ไม่พบ serial number สำหรับ BQID นี้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int count = serialNumbers.Count;

            double pageWidth = 21.3, pageHeight = 30.0;

            double couponWidth = 10.5, couponHeight = 5.5, marginX = 0.0, marginY = 0.0, spacingX = 0.0, spacingY = 0.0;
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

                        // ✅ เพิ่มขนาดฟอนต์ตรงนี้ให้ใหญ่ขึ้น (ลำดับที่ 2)
                        seNum.Font = new Font(seNum.Font.FontFamily, 20, FontStyle.Bold); // หรือเปลี่ยนเป็น FontBold ที่คุณปรับไว้แล้วก็ได้
                        label9.Font = new Font(label9.Font.FontFamily, 25, FontStyle.Regular);
                        lblPreview.Font = new Font(lblPreview.Font.FontFamily, 28, FontStyle.Regular);
                        seNum.Text = ((index + 1).ToString("D3")) + " - " + serial;

                        panel1.CreateControl();
                        panel1.Refresh();
                        System.Windows.Forms.Application.DoEvents();

                        // ✅ ลำดับที่ 1: สร้าง Bitmap และกำหนด DPI เป็น 300
                        using (Bitmap bmp = new Bitmap(panel1.Width, panel1.Height))
                        {
                            bmp.SetResolution(300, 300); // 🟢 เพิ่มบรรทัดนี้ให้รองรับการพิมพ์ขนาดคมชัด

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
            lblSerialNumber.Text = serial; 
            panel1.Refresh();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool isValid = true;
            if (string.IsNullOrWhiteSpace(mealType.Text))
            {
                mealType.BackColor = Color.MistyRose;
                MessageBox.Show("กรุณากรอกประเภทมื้ออาหาร", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
                isValid = false;
            }
            else
            {
                mealType.BackColor = Color.White;
            }
            if (string.IsNullOrWhiteSpace(agency.Text))
            {
                agency.BackColor = Color.MistyRose;
                MessageBox.Show("กรุณากรอกชื่อหน่วยงาน", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
                isValid = false;
            }
            else
            {
                agency.BackColor = Color.White;
            }
            if (string.IsNullOrWhiteSpace(canteenName.Text))
            {
                canteenName.BackColor = Color.MistyRose;
                MessageBox.Show("กรุณากรอกชื่อห้องจัดเลี้ยง", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
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
            int thickness = 40;

            using (Pen pen = new Pen(Color.Black, thickness))
            {

                // วาดเส้นขวา
                e.Graphics.DrawLine(pen, pnl.Width - 1, 0, pnl.Width - 1, 250);
            }
        }

        private void panelCouponName_Paint(object sender, PaintEventArgs e)
        {
            Panel pnl = (Panel)sender;
            int thickness = 15;

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
            btnCancel.Visible = true;
        }

        private void btnEditNprint_Click(object sender, EventArgs e)
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(mealType.Text))
            {
                mealType.BackColor = Color.MistyRose;
                MessageBox.Show("กรุณากรอกประเภทมื้ออาหาร", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
                isValid = false;
            }
            else
            {
                mealType.BackColor = Color.White;
            }
            if (string.IsNullOrWhiteSpace(agency.Text))
            {
                agency.BackColor = Color.MistyRose;
                MessageBox.Show("กรุณากรอกชื่อหน่วยงาน", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
                isValid = false;
            }
            else
            {
                agency.BackColor = Color.White;
            }
            if (string.IsNullOrWhiteSpace(canteenName.Text))
            {
                canteenName.BackColor = Color.MistyRose;
                MessageBox.Show("กรุณากรอกชื่อห้องจัดเลี้ยง", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
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
                            // ✅ บันทึกจำนวนรวมใน UpdateCoupons หลังจากเพิ่มคูปองเสร็จ
                            SqlCommand cmdUsage = new SqlCommand(
                                "INSERT INTO UpdateCoupons (BQID, serialNum, quantity, editAt, Username) " +
                                "VALUES (@BQID, @serialNum, @quantity, GETDATE(), @Username)",
                                conn, transaction);

                            cmdUsage.Parameters.AddWithValue("@BQID", selectedBQID);
                            cmdUsage.Parameters.AddWithValue("@serialNum", seNum.Text); // serial เดิมที่ใช้
                            cmdUsage.Parameters.AddWithValue("@quantity", newQuantity); // จำนวนรวมที่สร้าง
                            cmdUsage.Parameters.AddWithValue("@Username", userLogin.Text);
                            cmdUsage.ExecuteNonQuery();


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

        private void editedCoupons(string serialNums)
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
                    cmd.Parameters.AddWithValue("@serialNum", serialNums);

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

            if (string.IsNullOrEmpty(selectedBQID))
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
                            // กลุ่มคูปองตาม serialNum ที่เหมือนกัน
                            var groupedCoupons = couponsToDelete
                                .GroupBy(c => c.serialNum)
                                .Select(g => new
                                {
                                    serialNum = g.Key,
                                    quantity = g.Sum(c => c.quantity),
                                    mealDate = g.First().mealDate,
                                    mealType = g.First().mealType,
                                    agency = g.First().agency,
                                    cateringName = g.First().cateringName,
                                    paperSize = g.First().paperSize
                                });

                            foreach (var group in groupedCoupons)
                            {
                                string insertSql = @"
                                                    INSERT INTO RemoveCoupons 
                                                    (BQID, deleteAt, Username, mealDate, mealType, agency, cateringName, quantity, paperSize, serialNum)
                                                    VALUES 
                                                    (@BQID, GETDATE(), @Username, @mealDate, @mealType, @agency, @cateringName, @quantity, @paperSize, @serialNum)
                                                    ";

                                SqlCommand cmdInsert = new SqlCommand(insertSql, conn, transaction);
                                cmdInsert.Parameters.AddWithValue("@BQID", selectedBQID);
                                cmdInsert.Parameters.AddWithValue("@Username", currentUser);
                                cmdInsert.Parameters.Add("@mealDate", SqlDbType.DateTime).Value = (group.mealDate == DateTime.MinValue) ? (object)DBNull.Value : group.mealDate;
                                cmdInsert.Parameters.AddWithValue("@mealType", (object)group.mealType ?? DBNull.Value);
                                cmdInsert.Parameters.AddWithValue("@agency", (object)group.agency ?? DBNull.Value);
                                cmdInsert.Parameters.AddWithValue("@cateringName", (object)group.cateringName ?? DBNull.Value);
                                cmdInsert.Parameters.AddWithValue("@quantity", group.quantity);
                                cmdInsert.Parameters.AddWithValue("@paperSize", (object)group.paperSize ?? DBNull.Value);
                                cmdInsert.Parameters.AddWithValue("@serialNum", (object)group.serialNum ?? DBNull.Value);

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
            btnEdit.Visible = false;
            btnEditNprint.Visible = false;
            btnDel.Visible = false;
            btnCancel.Visible = false;
            btnSave.Visible = true;
            btnClearForm.Visible = true;
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
                btnAddPrint.Visible = true;


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

        int currentPage = 1;
        int pageSize = 10;
        int totalRecords = 0;
        int totalPages = 0;

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
            quantity.Minimum = 1;
            quantity.Maximum = 300;

            if (int.TryParse(quantity.Text, out int sumQty))
            {
                if (sumQty > quantity.Maximum)
                {
                    MessageBox.Show($"กรอกตัวเลขได้ไม่เกิน {quantity.Maximum}", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    quantity.BackColor = Color.Red;
                    quantity.Value = quantity.Maximum; // กำหนดแค่สูงสุดที่อนุญาต
                }
                else
                {
                    quantity.BackColor = Color.White;
                    quantity.Value = sumQty;
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedLanguage = listBox1.SelectedItem.ToString();
            if (selectedLanguage == "TH")
            {
                label6.Visible = true;
                label7.Visible = true;
                label8.Visible = true;
                label9.Visible = true;
                lblPreview.Visible = true;
                agencyPreview.Visible = true;
                canteenPreview.Visible = true;
                mealDatePreview.Visible = true;

                label10.Visible = false;
                label11.Visible = false;
                label12.Visible = false;
                label16.Visible = false;
                label17.Visible = false;
                label18.Visible = false;
                label19.Visible = false;
                label20.Visible = false;


            }
            else if (selectedLanguage == "EN")
            {
                label6.Visible = false;
                label7.Visible = false;
                label8.Visible = false;
                label9.Visible = false;
                lblPreview.Visible = false;
                agencyPreview.Visible = false;
                canteenPreview.Visible = false;
                mealDatePreview.Visible = false;

                label10.Visible = true;
                label11.Visible = true;
                label12.Visible = true;
                label16.Visible = true;
                label17.Visible = true;
                label18.Visible = true;
                label19.Visible = true;
                label20.Visible = true;
            }
        }

        private void agencyPreview_Click(object sender, EventArgs e)
        {

        }

        private void label17_Paint(object sender, PaintEventArgs e)
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

        private void btnAddPrint_Click(object sender, EventArgs e)
        {
            {
                bool isValid = true;

                if (string.IsNullOrWhiteSpace(mealType.Text))
                {
                    mealType.BackColor = Color.MistyRose;
                    MessageBox.Show("กรุณากรอกประเภทมื้ออาหาร", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                    isValid = false;
                }
                else
                {
                    mealType.BackColor = Color.White;
                }
                if (string.IsNullOrWhiteSpace(agency.Text))
                {
                    agency.BackColor = Color.MistyRose;
                    MessageBox.Show("กรุณากรอกชื่อหน่วยงาน", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                    isValid = false;
                }
                else
                {
                    agency.BackColor = Color.White;
                }
                if (string.IsNullOrWhiteSpace(canteenName.Text))
                {
                    canteenName.BackColor = Color.MistyRose;
                    MessageBox.Show("กรุณากรอกชื่อห้องจัดเลี้ยง", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
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

                EditAddprint();

                btnEdit.Visible = false;
                btnEditNprint.Visible = false;
                btnSave.Visible = true;
                btnClearForm.Visible = true;
                btnDel.Visible = false;
                btnCancel.Visible = false;

            }
        }
        private void EditAddprint()
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

                // --- ขอเหตุผลจากผู้ใช้ก่อน ---
                AddCouponInput userInput = Prompt.ShowAddCouponDialog("ระบุจำนวนและเหตุผลในการเพิ่มคูปอง");
                int addQty = userInput.Quantity;
                string reason = userInput.Reason;

                if (addQty <= 0)
                {
                    MessageBox.Show("กรุณากรอกจำนวนที่เป็นตัวเลขและมากกว่า 0", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(reason))
                {
                    MessageBox.Show("กรุณากรอกเหตุผล!", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<string> newCouponNums = new List<string>();
                List<string> usedSerialNums = new List<string>();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. ดึง serialNum เดิมใน BQID นี้
                            List<string> oldSerials = new List<string>();
                            string selectSerialSql = "SELECT DISTINCT serialNum FROM Coupons WHERE BQID = @BQID";
                            using (SqlCommand cmd = new SqlCommand(selectSerialSql, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@BQID", selectedBQID);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        oldSerials.Add(reader["serialNum"].ToString());
                                    }
                                }
                            }
                            if (oldSerials.Count == 0)
                            {
                                MessageBox.Show("ไม่พบ serialNum เดิมใน BQID นี้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            // 2. หาค่า couponNum เดิมสูงสุด
                            int lastCouponNum = 0;
                            using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(CAST(couponNum AS int)),0) FROM Coupons WHERE BQID = @BQID", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@BQID", selectedBQID);
                                object result = cmd.ExecuteScalar();
                                lastCouponNum = Convert.ToInt32(result);
                            }

                            // 3. ดึงข้อมูลหลักของ BQID เดิม (เพื่อเติมข้อมูลอื่น ๆ)
                            string sqlDetail = "SELECT TOP 1 mealDate, mealType, agency, cateringName, paperSize FROM Coupons WHERE BQID = @BQID";
                            DateTime mealDate = DateTime.Now;
                            string mealType = "", agency = "", cateringName = "", paperSize = "";
                            using (SqlCommand cmd = new SqlCommand(sqlDetail, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@BQID", selectedBQID);
                                using (var r = cmd.ExecuteReader())
                                {
                                    if (r.Read())
                                    {
                                        mealDate = r.GetDateTime(0);
                                        mealType = r.GetString(1);
                                        agency = r.GetString(2);
                                        cateringName = r.GetString(3);
                                        paperSize = r.GetString(4);
                                    }
                                    else
                                    {
                                        MessageBox.Show("ไม่พบ BQID นี้");
                                        return;
                                    }
                                }
                            }

                            // 4. เพิ่มคูปองใหม่ ใช้ serialNum เดิม กระจายตามจำนวนที่เพิ่ม
                            int serialCount = oldSerials.Count;
                            List<string> addedSerials = new List<string>();
                            List<string> addedCouponNums = new List<string>();

                            for (int i = 1; i <= addQty; i++)
                            {
                                int newCouponNum = lastCouponNum + i;
                                string couponNumStr = newCouponNum.ToString("D3");
                                // วน serialNum เดิม เช่น ถ้ามี 3 serialNum แล้วเพิ่ม 6 ใบ จะวน serialNum เดิม
                                string useSerialNum = oldSerials[(newCouponNum - 1) % serialCount];
                                addedSerials.Add(useSerialNum);
                                addedCouponNums.Add(couponNumStr);

                                string sqlInsert = @"INSERT INTO Coupons 
                            (BQID, mealDate, mealType, agency, cateringName, quantity, paperSize, serialNum, status, couponNum)
                            VALUES (@BQID, @mealDate, @mealType, @agency, @cateringName, 1, @paperSize, @serialNum, 'added', @couponNum)";
                                using (SqlCommand cmd = new SqlCommand(sqlInsert, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@BQID", selectedBQID);
                                    cmd.Parameters.AddWithValue("@mealDate", mealDate);
                                    cmd.Parameters.AddWithValue("@mealType", mealType);
                                    cmd.Parameters.AddWithValue("@agency", agency);
                                    cmd.Parameters.AddWithValue("@cateringName", cateringName);
                                    cmd.Parameters.AddWithValue("@paperSize", paperSize);
                                    cmd.Parameters.AddWithValue("@serialNum", useSerialNum);
                                    cmd.Parameters.AddWithValue("@couponNum", couponNumStr);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // 5. Log ลง UpdateCoupons
                            string serialNum = oldSerials[0];
                            string sqlLog = @"INSERT INTO UpdateCoupons (BQID, serialNum, quantity, editAt, Username, Reason)
                                            VALUES (@BQID, @serialNum, @quantity, GETDATE(), @Username, @Reason)";
                            using (SqlCommand cmd = new SqlCommand(sqlLog, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@BQID", selectedBQID);
                                cmd.Parameters.AddWithValue("@serialNum", serialNum);
                                cmd.Parameters.AddWithValue("@quantity", addQty);
                                cmd.Parameters.AddWithValue("@Username", currentUser);
                                cmd.Parameters.AddWithValue("@Reason", reason);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            MessageBox.Show("เพิ่มคูปองใหม่และพร้อมปริ๊นแล้ว");
                            loadData(currentPage);

                            // 6. ปริ๊นเฉพาะที่เพิ่มใหม่ (ส่ง serialNum และ couponNum ที่เพิ่มใหม่)
                            PrintCoupons(selectedBQID, addedSerials, addedCouponNums);

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
        public static class Prompt
        {
            public static AddCouponInput ShowAddCouponDialog(string caption)
            {
                AddCouponInput result = new AddCouponInput();

                Form prompt = new Form()
                {
                    Width = 400,
                    Height = 220,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = caption,
                    StartPosition = FormStartPosition.CenterScreen
                };

                Label lblQty = new Label() { Left = 20, Top = 20, Text = "จำนวนที่ต้องการเพิ่ม:", Width = 340 };
                TextBox qtyBox = new TextBox() { Left = 20, Top = 45, Width = 340 };

                Label lblReason = new Label() { Left = 20, Top = 80, Text = "เหตุผลในการเพิ่มคูปอง:", Width = 340 };
                TextBox reasonBox = new TextBox() { Left = 20, Top = 105, Width = 340 };

                Button confirmation = new Button() { Text = "OK", Left = 200, Width = 80, Top = 150, DialogResult = DialogResult.OK };
                Button cancel = new Button() { Text = "Cancel", Left = 290, Width = 80, Top = 150, DialogResult = DialogResult.Cancel };
                confirmation.Click += (sender, e) => { prompt.Close(); };
                cancel.Click += (sender, e) => { prompt.Close(); };

                prompt.Controls.Add(lblQty);
                prompt.Controls.Add(qtyBox);
                prompt.Controls.Add(lblReason);
                prompt.Controls.Add(reasonBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(cancel);
                prompt.AcceptButton = confirmation;
                prompt.CancelButton = cancel;

                if (prompt.ShowDialog() == DialogResult.OK)
                {
                    if (!int.TryParse(qtyBox.Text, out result.Quantity) || result.Quantity <= 0)
                        result.Quantity = 0;
                    result.Reason = reasonBox.Text.Trim();
                }
                else
                {
                    result.Quantity = 0;
                    result.Reason = "";
                }
                return result;
            }
        }
        private void PrintCoupons(string bqid, List<string> serialNums, List<string> couponNums)
        {
            if (serialNums == null || couponNums == null || serialNums.Count == 0 || couponNums.Count == 0)
            {
                MessageBox.Show("ไม่มีข้อมูลคูปองสำหรับปริ๊น", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string iniPath = "config.ini";
            var config = IniReader.ReadIni(iniPath, "Database");
            string connectionString = $"Server={config["Server"]};Database={config["Database"]};User Id={config["User"]};Password={config["Password"]};";

            // เตรียมข้อมูลคูปองที่จะปริ๊น
            List<CouponData> coupons = new List<CouponData>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // สร้าง WHERE ... serialNum = @s0 AND couponNum = @c0 OR serialNum = @s1 AND couponNum = @c1 ...
                List<string> whereParts = new List<string>();
                for (int i = 0; i < serialNums.Count; i++)
                    whereParts.Add($"(serialNum = @s{i} AND couponNum = @c{i})");
                string whereClause = string.Join(" OR ", whereParts);

                string query = $@"
                    SELECT serialNum, couponNum
                    FROM Coupons
                    WHERE BQID = @BQID AND ({whereClause})
                    ORDER BY CAST(couponNum AS int) ASC
                ";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BQID", bqid);
                    for (int i = 0; i < serialNums.Count; i++)
                    {
                        cmd.Parameters.AddWithValue($"@s{i}", serialNums[i]);
                        cmd.Parameters.AddWithValue($"@c{i}", couponNums[i]);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            coupons.Add(new CouponData
                            {
                                SerialNum = reader["serialNum"].ToString(),
                                CouponNum = reader["couponNum"].ToString()
                            });
                        }
                    }
                }
            }

            if (coupons.Count == 0)
            {
                MessageBox.Show("ไม่พบคูปองที่ต้องการปริ๊น", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // --------------- สร้าง PDF ----------------
            double pageWidth = 21.3, pageHeight = 30.0; // A4
            double couponWidth = 10.5, couponHeight = 5.5, marginX = 0.0, marginY = 0.0, spacingX = 0.0, spacingY = 0.0;
            int couponsPerRow = 2;
            int couponsPerColumn = (int)Math.Floor((pageHeight - (2 * marginY)) / (couponHeight + spacingY));
            int couponsPerPage = couponsPerRow * couponsPerColumn;
            int pageCount = (int)Math.Ceiling(coupons.Count / (double)couponsPerPage);

            PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
            document.Info.Title = "Banquet Coupons Print";

            for (int p = 0; p < pageCount; p++)
            {
                var page = document.AddPage();
                page.Width = PdfSharp.Drawing.XUnit.FromCentimeter(pageWidth);
                page.Height = PdfSharp.Drawing.XUnit.FromCentimeter(pageHeight);
                var gfx = PdfSharp.Drawing.XGraphics.FromPdfPage(page);

                for (int row = 0; row < couponsPerColumn; row++)
                {
                    for (int col = 0; col < couponsPerRow; col++)
                    {
                        int index = p * couponsPerPage + (row * couponsPerRow + col);
                        if (index >= coupons.Count) break;

                        var coupon = coupons[index];
                        UpdatePanelWithSerialNumber("AS" + coupon.SerialNum);
                        seNum.Font = new Font(seNum.Font.FontFamily, 20, FontStyle.Bold);
                        seNum.Text = $"{coupon.CouponNum} - {coupon.SerialNum}";
                        label9.Font = new Font(label9.Font.FontFamily, 25, FontStyle.Regular);
                        lblPreview.Font = new Font(lblPreview.Font.FontFamily, 28, FontStyle.Regular);

                        panel1.CreateControl();
                        panel1.Refresh();
                        System.Windows.Forms.Application.DoEvents();

                        using (Bitmap bmp = new Bitmap(panel1.Width, panel1.Height))
                        {
                            bmp.SetResolution(300, 300);
                            panel1.DrawToBitmap(bmp, new Rectangle(0, 0, panel1.Width, panel1.Height));
                            using (MemoryStream ms = new MemoryStream())
                            {
                                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                ms.Position = 0;
                                var ximg = PdfSharp.Drawing.XImage.FromStream(ms);

                                double drawX = marginX + col * (couponWidth + spacingX);
                                double drawY = marginY + row * (couponHeight + spacingY);

                                gfx.DrawImage(
                                    ximg,
                                    PdfSharp.Drawing.XUnit.FromCentimeter(drawX).Point,
                                    PdfSharp.Drawing.XUnit.FromCentimeter(drawY).Point,
                                    PdfSharp.Drawing.XUnit.FromCentimeter(couponWidth).Point,
                                    PdfSharp.Drawing.XUnit.FromCentimeter(couponHeight).Point
                                );
                            }
                        }
                    }
                }
            }

            string tempPath = Path.Combine(Path.GetTempPath(), $"BanquetCoupons_{bqid}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            document.Save(tempPath);
            Process.Start(new ProcessStartInfo(tempPath) { UseShellExecute = true });
        }
        public class CouponData
        {
            public string SerialNum { get; set; }
            public string CouponNum { get; set; }
        }
        public struct AddCouponInput
        {
            public int Quantity;
            public string Reason;
        }
    }
}

