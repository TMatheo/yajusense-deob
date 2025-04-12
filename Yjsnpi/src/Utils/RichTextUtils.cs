using System.Text.RegularExpressions;
using UnityEngine;
using Yjsnpi.Extensions;

namespace Yjsnpi.Utils;

public static class RichTextUtils
{
    private const string BoldFormat = "<b>{0}</b>";
    private const string ItalicFormat = "<i>{0}</i>";
    private const string ColorFormat = "<color={1}>{0}</color>";
    private const string SizeFormat = "<size={1}>{0}</size>";
    
    private static readonly Regex BoldRegex = new(@"<b>(.*?)<\/b>", RegexOptions.Singleline);
    private static readonly Regex ItalicRegex = new(@"<i>(.*?)<\/i>", RegexOptions.Singleline);
    private static readonly Regex ColorRegex = new(@"<color=[^>]+>(.*?)<\/color>", RegexOptions.Singleline);
    private static readonly Regex SizeRegex = new(@"<size=\d+>(.*?)<\/size>", RegexOptions.Singleline);

    public static string Bold(this string text)
    {
        string stripped = BoldRegex.Replace(text, "$1");
        return string.Format(BoldFormat, stripped);
    }
    
    public static string Italic(this string text)
    {
        string stripped = ItalicRegex.Replace(text, "$1");
        return string.Format(ItalicFormat, stripped);
    }
    
    public static string Color(this string text, Color color)
    {
        string stripped = ColorRegex.Replace(text, "$1");
        return string.Format(ColorFormat, stripped, color.ToHexRGBA());
    }
    
    public static string Size(this string text, int size)
    {
        string stripped = SizeRegex.Replace(text, "$1");
        return string.Format(SizeFormat, stripped, size);
    }
}