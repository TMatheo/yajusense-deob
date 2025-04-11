namespace Yjsnpi.Utilities;

public static class RichTextUtility
{
    private const string BoldFormat = "<b>{0}</b>";
    private const string ItalicFormat = "<i>{0}</i>";
    private const string ColorFormat = "<color={1}>{0}</color>";
    private const string SizeFormat = "<size={1}>{0}</size>";
    
    public static class Colors
    {
        public const string Red = "#FF0000";
        public const string Green = "#00FF00";
        public const string Orange = "#FFA500";
        public const string White = "#FFFFFF";
        public const string Gray = "#808080";
    }
    
    public static string Bold(this string text) => string.Format(BoldFormat, text);
    
    public static string Italic(this string text) => string.Format(ItalicFormat, text);
    
    public static string Color(this string text, string hexColor) => string.Format(ColorFormat, text, hexColor);
    
    public static string Size(this string text, int size) => string.Format(SizeFormat, text, size);
    
    public static string Format(this string text, RichTextOptions options)
    {
        var result = text;
        if (options.IsBold) result = result.Bold();
        if (options.IsItalic) result = result.Italic();
        if (options.Color != null) result = result.Color(options.Color);
        if (options.Size > 0) result = result.Size(options.Size);
        return result;
    }
    
    public struct RichTextOptions
    {
        public bool IsBold;
        public bool IsItalic;
        public string Color;
        public int Size;

        public RichTextOptions(
            bool bold = false, 
            bool italic = false, 
            string color = null, 
            int size = 0)
        {
            IsBold = bold;
            IsItalic = italic;
            Color = color;
            Size = size;
        }
    }
}