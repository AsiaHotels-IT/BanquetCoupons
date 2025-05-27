using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

public class FontManager
{
    private PrivateFontCollection privateFonts;
    private PrivateFontCollection barcodeFonts; // เพิ่ม collection สำหรับฟอนต์บาร์โค้ด

    public Font FontSmall { get; private set; }

    public Font FontTooltip { get; private set; }
    public Font FontRegular { get; private set; }
    public Font FontBold { get; private set; }
    public Font FontSmallBold { get; private set; }

    public Font FontBarcode { get; private set; }
    public Font FontSerial { get; private set; }

    public Font FontTopic { get; private set; }
    public Font FontShowDate { get; }
    public Font FontShowTopic { get; }

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

        // โหลดฟอนต์อังกฤษ
        string fontSenum = Path.Combine(Application.StartupPath, "fonts", "AsiaHotelBeta-Regular.otf");
        if (!File.Exists(fontPath))
        {
            MessageBox.Show("ไม่พบไฟล์ฟอนต์: " + fontPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        privateFonts.AddFontFile(fontSenum);
        FontFamily fontSerialNum = privateFonts.Families[0];

        // โหลดฟอนต์บาร์โค้ด
        string barcodeFontPath = Path.Combine(Application.StartupPath, "fonts", "Free3of9.ttf");  // เปลี่ยนชื่อไฟล์ตามฟอนต์จริง
        if (!File.Exists(barcodeFontPath))
        {
            MessageBox.Show("ไม่พบไฟล์ฟอนต์บาร์โค้ด: " + barcodeFontPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        barcodeFonts.AddFontFile(barcodeFontPath);
        FontFamily barcodeFontFamily = barcodeFonts.Families[0];

        //NotoSansThai-Bold.ttf
        // สร้างฟอนต์ด้วยขนาดต่าง ๆ
        FontTooltip = new Font(fontFamily,9,FontStyle.Regular);
        FontSmall = new Font(fontFamily, 10, FontStyle.Regular);
        FontRegular = new Font(fontFamily, 12, FontStyle.Regular);
        FontBold = new Font(fontFamily, 12, FontStyle.Bold);
        FontSmallBold = new Font(fontFamily, 8, FontStyle.Bold);
        FontTopic = new Font(fontSenum, 16, FontStyle.Regular);
        FontShowDate = new Font(fontFamily, 16, FontStyle.Regular);


        // ฟอนต์บาร์โค้ด ขนาดตามต้องการ 
        FontBarcode = new Font(barcodeFontFamily, 11, FontStyle.Regular);
        //ฟอนต์เลข serialNum
        FontSerial = new Font(fontSenum, 9, FontStyle.Regular);
    }
}

