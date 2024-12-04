using System;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using Microsoft.IdentityModel.Tokens;

namespace AbpInsight.VoloAbp.DependencyInjection;

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

        return str[..len];
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
}