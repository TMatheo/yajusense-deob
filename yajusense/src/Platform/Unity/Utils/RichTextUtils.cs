using System;
using System.Text.RegularExpressions;
using UnityEngine;
using yajusense.Extensions;

namespace yajusense.Platform.Unity.Utils;

public static class RichTextUtils
{
	private const string BoldFormat = "<b>{0}</b>";
	private const string ItalicFormat = "<i>{0}</i>";
	private const string ColorFormat = "<color={1}>{0}</color>";
	private const string SizeFormat = "<size={1}>{0}</size>";
	private static readonly Lazy<Regex> BoldRegex = new(() => new Regex(@"<b>(.*?)<\/b>", RegexOptions.Singleline));
	private static readonly Lazy<Regex> ItalicRegex = new(() => new Regex(@"<i>(.*?)<\/i>", RegexOptions.Singleline));

	private static readonly Lazy<Regex> ColorRegex = new(() => new Regex(@"<color=[^>]+>(.*?)<\/color>", RegexOptions.Singleline));

	private static readonly Lazy<Regex> SizeRegex = new(() => new Regex(@"<size=\d+>(.*?)<\/size>", RegexOptions.Singleline));

	public static string Bold(this string text)
	{
		string stripped = StripTags(text, BoldRegex.Value);
		return string.Format(BoldFormat, stripped);
	}

	public static string Italic(this string text)
	{
		string stripped = StripTags(text, ItalicRegex.Value);
		return string.Format(ItalicFormat, stripped);
	}

	public static string Color(this string text, Color color)
	{
		string stripped = StripTags(text, ColorRegex.Value);
		return string.Format(ColorFormat, stripped, color.ToHexRGBA());
	}

	public static string Size(this string text, int size)
	{
		string stripped = StripTags(text, SizeRegex.Value);
		return string.Format(SizeFormat, stripped, size);
	}

	private static string StripTags(string text, Regex regex)
	{
		return string.IsNullOrEmpty(text) ? string.Empty : regex.Replace(text, "$1");
	}
}