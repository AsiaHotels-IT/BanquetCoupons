using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

public class FontManager
{
    private PrivateFontCollection privateFonts;
    private PrivateFontCollection barcodeFonts;

    public Font FontSmall { get; private set; }
    public Font FontTooltip { get; private set; }
    public Font FontRegular { get; private set; }
    public Font FontThaiBold { get; private set; }
    public Font FontBold { get; private set; }
    public Font FontSmallBold { get; private set; }
    public Font FontBarcode { get; private set; }
    public Font FontSerial { get; private set; }
    public Font FontTopic { get; private set; }
    public Font FontShowDate { get; private set; }
    public Font FontShowTopic { get; private set; }

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
        FontFamily thaiFontFamily = privateFonts.Families[0];

        // โหลดฟอนต์ไทยแบบหนา
        string fontPath1 = Path.Combine(Application.StartupPath, "fonts", "NotoSansThai-Bold.ttf");
        if (!File.Exists(fontPath))
        {
            MessageBox.Show("ไม่พบไฟล์ฟอนต์: " + fontPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        privateFonts.AddFontFile(fontPath);
        FontFamily thaiFontFamilyBold = privateFonts.Families[0];

        // โหลดฟอนต์อังกฤษ
        string fontSenumPath = Path.Combine(Application.StartupPath, "fonts", "AsiaHotelBeta-Regular.otf");
        if (!File.Exists(fontSenumPath))
        {
            MessageBox.Show("ไม่พบไฟล์ฟอนต์: " + fontSenumPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        privateFonts.AddFontFile(fontSenumPath); // เพิ่มเข้า collection เดิม
        FontFamily engFontFamily = privateFonts.Families[1]; // ต้องใช้ index 1 เพราะเป็นฟอนต์ที่สอง

        // โหลดฟอนต์บาร์โค้ด
        string barcodeFontPath = Path.Combine(Application.StartupPath, "fonts", "Free3of9.ttf");
        if (!File.Exists(barcodeFontPath))
        {
            MessageBox.Show("ไม่พบไฟล์ฟอนต์บาร์โค้ด: " + barcodeFontPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        barcodeFonts.AddFontFile(barcodeFontPath);
        FontFamily barcodeFontFamily = barcodeFonts.Families[0];

        // ฟอนต์ไทย
        FontTooltip = new Font(thaiFontFamily, 18, FontStyle.Regular);
        FontSmall = new Font(thaiFontFamily, 10, FontStyle.Regular);
        FontRegular = new Font(thaiFontFamily, 12, FontStyle.Regular);
        FontThaiBold = new Font(thaiFontFamilyBold, 32);
        FontBold = new Font(thaiFontFamily, 12, FontStyle.Bold);
        FontSmallBold = new Font(thaiFontFamily, 8, FontStyle.Bold);

        // ฟอนต์หัวข้อ
        FontTopic = new Font(thaiFontFamily, 12, FontStyle.Bold);
        FontShowDate = new Font(thaiFontFamily, 16, FontStyle.Regular);
        FontShowTopic = new Font(thaiFontFamily, 14, FontStyle.Bold);

        // ฟอนต์บาร์โค้ด
        FontBarcode = new Font(barcodeFontFamily, 25, FontStyle.Regular);

        // ฟอนต์ serial number
        FontSerial = new Font(engFontFamily, 9, FontStyle.Regular);
    }
}
