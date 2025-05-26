using PdfSharp.Fonts;
using System.IO;

public class CustomFontResolver : IFontResolver
{
    private const string fontFamilyName = "NotoSansThai-Regular";
    private readonly byte[] fontData;

    public CustomFontResolver(string fontPath)
    {
        fontData = File.ReadAllBytes(fontPath);
    }

    public byte[] GetFont(string faceName)
    {
        if (faceName == fontFamilyName)
            return fontData;
        return null;
    }

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        if (familyName.Equals("NotoSansThai-Regular", System.StringComparison.OrdinalIgnoreCase) ||
            familyName.Equals("NotoSansThai", System.StringComparison.OrdinalIgnoreCase))
        {
            return new FontResolverInfo(fontFamilyName);
        }
        // fallback ฟอนต์อื่น ๆ ตามระบบ PdfSharp
        return PlatformFontResolver.ResolveTypeface(familyName, isBold, isItalic);
    }
}
