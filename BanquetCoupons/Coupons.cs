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
        public Coupons()
        {
            InitializeComponent();
        }

        private bool isDatePicked = false;
        private void Coupons_Load(object sender, EventArgs e)
        {
            loadData();

            // สร้างตัวจัดการฟอนต์แบบส่วนตัว (ไม่ต้องติดตั้งใน Windows)
            PrivateFontCollection pfc = new PrivateFontCollection();

            // โหลดฟอนต์จากไฟล์ในโฟลเดอร์ fonts (ในโฟลเดอร์โปรแกรม)
            string fontPath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "fonts", "NotoSansThai-Regular.ttf");
            pfc.AddFontFile(fontPath);

            // สร้างฟอนต์จากฟอนต์ที่โหลดมา (ขนาด 12)
            FontFamily fontFamily = pfc.Families[0];

            // กำหนดขนาดและน้ำหนักแบบต่างๆ
            Font fontSmall = new Font(fontFamily, 10, FontStyle.Regular);
            Font fontRegular = new Font(fontFamily, 12, FontStyle.Regular);
            Font fontBold = new Font(fontFamily, 12, FontStyle.Bold);
            Font fontSmallBold = new Font(fontFamily, 9, FontStyle.Bold);

            // สมมติอยากใช้ฟอนต์กับ panel1 ที่มี Label อยู่ข้างใน
            foreach (Control ctl in panel1.Controls)
            {
                if (ctl is Label label)
                {
                    // ตั้งฟอนต์ตามชื่อ
                    if (label.Name == "label6" || label.Name == "label7" || label.Name == "label8")
                        label.Font = fontBold;
                    else if (label.Name == "lblPreview" || label.Name == "lblSerialNumber")
                        label.Font = fontSmall;
                    else if (label.Name == "label9")
                        label.Font = fontSmallBold;
                    else
                        label.Font = fontRegular;
                }
            }

            panel1.PerformLayout();
            panel1.Refresh();
            System.Windows.Forms.Application.DoEvents();

            // ค่อย draw
            Bitmap panelImage = new Bitmap(panel1.Width, panel1.Height);
            panel1.DrawToBitmap(panelImage, new Rectangle(0, 0, panel1.Width, panel1.Height));

            foreach (Control ctl in panelTopic.Controls)
            {
                if (ctl is Label label)
                {
                    if (label.Name == "label6")
                    {
                        label.Font = fontBold;
                    }
                    else if (label.Name == "label7")
                    {
                        label.Font = fontBold;
                    }
                    else if (label.Name == "label8")
                    {
                        label.Font = fontBold;
                    }                    
                    else
                    {
                        label.Font = fontRegular;        // ตัวอื่นขนาดเล็กปกติ
                    }
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
                            CAST(serialNum AS INT)";

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
            agencyPreview.Text = Agency;
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

        private void SaveCouponAsPDF()
        {
            int count = int.Parse(quantity.Text);
            string selectedPaper = comboBoxPaperSize.SelectedItem.ToString();
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

                        // 👇 ตั้งหมายเลขคูปองบน panel1
                        UpdatePanelWithSerialNumber($"00{index + 1}");

                        // 💥 สำคัญมาก: บังคับให้ panel render ให้เสร็จก่อน
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



        private void UpdatePanelWithSerialNumber(string serial)
        {
            lblSerialNumber.Text = serial; // หรืออะไรก็ตามที่คุณใช้ใน panel1 เพื่อแสดงหมายเลขคูปอง
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
    }
}
