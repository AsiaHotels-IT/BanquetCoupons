using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

public class FontManager
{
    private PrivateFontCollection privateFonts;
    private PrivateFontCollection barcodeFonts; // เพิ่ม collection สำหรับฟอนต์บาร์โค้ด

    public Font FontSmall { get; private set; }
    public Font FontRegular { get; private set; }
    public Font FontBold { get; private set; }
    public Font FontSmallBold { get; private set; }

    public Font FontBarcode { get; private set; }

    public FontManager()
    {
        privateFonts = new PrivateFontCollection();
        barcodeFonts = new PrivateFontCollection();

        // โหลดฟอนต์ไทย
        string fontPath = Path.Combine(Application.StartupPath, "fonts", "NotoSansThai-Regular.ttf");
        if (!File.Exists(fontPath))
        {
            MessageBox.Show("ไม่พบไฟล์ฟอนต์: " + fontPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        privateFonts.AddFontFile(fontPath);
        FontFamily fontFamily = privateFonts.Families[0];

        // โหลดฟอนต์บาร์โค้ด
        string barcodeFontPath = Path.Combine(Application.StartupPath, "fonts", "Free3of9.ttf");  // เปลี่ยนชื่อไฟล์ตามฟอนต์จริง
        if (!File.Exists(barcodeFontPath))
        {
            MessageBox.Show("ไม่พบไฟล์ฟอนต์บาร์โค้ด: " + barcodeFontPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        barcodeFonts.AddFontFile(barcodeFontPath);
        FontFamily barcodeFontFamily = barcodeFonts.Families[0];

        // สร้างฟอนต์ด้วยขนาดต่าง ๆ
        FontSmall = new Font(fontFamily, 10, FontStyle.Regular);
        FontRegular = new Font(fontFamily, 12, FontStyle.Regular);
        FontBold = new Font(fontFamily, 12, FontStyle.Bold);
        FontSmallBold = new Font(fontFamily, 9, FontStyle.Bold);

        // ฟอนต์บาร์โค้ด ขนาดตามต้องการ เช่น 48 pt
        FontBarcode = new Font(barcodeFontFamily, 9, FontStyle.Regular);
    }
}

