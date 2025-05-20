using System;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;

class FontInstaller
{
    // ฟังก์ชันจาก WinAPI สำหรับติดตั้งฟอนต์
    [DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
    private static extern int AddFontResource([In][MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

    [DllImport("gdi32.dll")]
    private static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    private const int WM_FONTCHANGE = 0x001D;
    private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);

    public static bool IsFontInstalled(string fontName)
    {
        using (InstalledFontCollection fontsCollection = new InstalledFontCollection())
        {
            foreach (var font in fontsCollection.Families)
            {
                if (font.Name.Equals(fontName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }
    }

    public static bool InstallFont(string fontFilePath)
    {
        if (!File.Exists(fontFilePath))
            throw new FileNotFoundException("ไฟล์ฟอนต์ไม่พบ: " + fontFilePath);

        // คัดลอกไฟล์ฟอนต์ไปไว้ในโฟลเดอร์ Fonts ของ Windows
        string fontsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts");
        string destFileName = Path.Combine(fontsFolder, Path.GetFileName(fontFilePath));

        try
        {
            if (!File.Exists(destFileName))
            {
                File.Copy(fontFilePath, destFileName);
            }

            // ลงทะเบียนฟอนต์กับระบบ
            int result = AddFontResource(destFileName);
            if (result == 0)
            {
                Console.WriteLine("ไม่สามารถเพิ่มฟอนต์ได้");
                return false;
            }

            // อัพเดต Registry เพื่อบอก Windows ว่ามีฟอนต์ใหม่
            string fontName = GetFontNameFromFile(fontFilePath);
            if (!string.IsNullOrEmpty(fontName))
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts", true);
                if (key != null)
                {
                    string regValue = Path.GetFileName(destFileName);
                    key.SetValue(fontName, regValue);
                    key.Close();
                }
            }

            // ส่งข้อความแจ้งให้ Windows รู้ว่ามีการเปลี่ยนแปลงฟอนต์
            SendMessage(HWND_BROADCAST, WM_FONTCHANGE, IntPtr.Zero, IntPtr.Zero);

            Console.WriteLine("ติดตั้งฟอนต์เรียบร้อย");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error ติดตั้งฟอนต์: " + ex.Message);
            return false;
        }
    }

    private static string GetFontNameFromFile(string fontFilePath)
    {
        // ดึงชื่อฟอนต์จากไฟล์ (ง่าย ๆ ใช้ PrivateFontCollection)
        try
        {
            using (PrivateFontCollection pfc = new PrivateFontCollection())
            {
                pfc.AddFontFile(fontFilePath);
                if (pfc.Families.Length > 0)
                {
                    return pfc.Families[0].Name + " (TrueType)";
                }
            }
        }
        catch { }
        return null;
    }
}
