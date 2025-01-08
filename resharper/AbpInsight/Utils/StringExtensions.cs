using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using Microsoft.IdentityModel.Tokens;

namespace AbpInsight.Utils;

public static class StringExtensions
{
    [ContractAnnotation("null <= str:null")]
    public static string RemovePostfix(this string str, params string[] postFixes)
    {
        return str.RemovePostfix(StringComparison.Ordinal, postFixes);
    }


    [ContractAnnotation("null <= str:null")]
    public static string RemovePostfix(this string str, StringComparison comparisonType, params string[] postFixes)
    {
        if (str.IsNullOrEmpty())
        {
            return str;
        }

        if (postFixes.IsNullOrEmpty())
        {
            return str;
        }

        foreach (var postFix in postFixes)
        {
            if (str.EndsWith(postFix, comparisonType))
            {
                return str.Left(str.Length - postFix.Length);
            }
        }

        return str;
    }

    public static string Left(this string str, int len)
    {
        str.NotNull();

        if (str.Length < len)
        {
            throw new ArgumentException("len argument can not be bigger than given string's length!");
        }

        return str.Substring(0, len);
    }

    [ContractAnnotation("null <= str:null")]
    public static string RemovePrefix(this string str, params string[] preFixes)
    {
        return str.RemovePrefix(StringComparison.Ordinal, preFixes);
    }

    [ContractAnnotation("null <= str:null")]
    public static string RemovePrefix(this string str, StringComparison comparisonType, params string[] preFixes)
    {
        if (str.IsNullOrEmpty())
        {
            return str;
        }

        if (preFixes.IsNullOrEmpty())
        {
            return str;
        }

        foreach (var preFix in preFixes)
        {
            if (str.StartsWith(preFix, comparisonType))
            {
                return str.Right(str.Length - preFix.Length);
            }
        }

        return str;
    }

    public static string Right(this string str, int len)
    {
        str.NotNull();

        if (str.Length < len)
        {
            throw new ArgumentException("len argument can not be bigger than given string's length!");
        }

        return str.Substring(str.Length - len, len);
    }

    [ContractAnnotation("null <= str:null")]
    public static string ToCamelCase(this string str, bool useCurrentCulture = false, bool handleAbbreviations = false)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        if (str.Length == 1)
        {
            return useCurrentCulture ? str.ToLower() : str.ToLowerInvariant();
        }

        if (handleAbbreviations && IsAllUpperCase(str))
        {
            return useCurrentCulture ? str.ToLower() : str.ToLowerInvariant();
        }

        return (useCurrentCulture ? char.ToLower(str[0]) : char.ToLowerInvariant(str[0])) + str.Substring(1);
    }

    [ContractAnnotation("null <= str:null")]
    public static string ToKebabCase(this string str, bool useCurrentCulture = false)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        str = str.ToCamelCase();

        return useCurrentCulture
            ? Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + "-" + char.ToLower(m.Value[1]))
            : Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + "-" + char.ToLowerInvariant(m.Value[1]));
    }

    private static bool IsAllUpperCase(string input)
    {
        for (var i = 0; i < input.Length; i++)
        {
            if (char.IsLetter(input[i]) && !char.IsUpper(input[i]))
            {
                return false;
            }
        }

        return true;
    }
}