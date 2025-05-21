using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

public class FontManager
{
    private PrivateFontCollection privateFonts;
    public Font FontSmall { get; private set; }
    public Font FontRegular { get; private set; }
    public Font FontBold { get; private set; }
    public Font FontSmallBold { get; private set; }

    public FontManager()
    {
        privateFonts = new PrivateFontCollection();

        string fontPath = Path.Combine(Application.StartupPath, "fonts", "NotoSansThai-Regular.ttf");
        if (!File.Exists(fontPath))
        {
            MessageBox.Show("ไม่พบไฟล์ฟอนต์: " + fontPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        privateFonts.AddFontFile(fontPath);
        FontFamily fontFamily = privateFonts.Families[0];

        // สร้างฟอนต์ด้วยขนาดต่าง ๆ
        FontSmall = new Font(fontFamily, 10, FontStyle.Regular);
        FontRegular = new Font(fontFamily, 12, FontStyle.Regular);
        FontBold = new Font(fontFamily, 12, FontStyle.Bold);
        FontSmallBold = new Font(fontFamily, 9, FontStyle.Bold);
    }
}
