using System;
using System.Collections.Generic;
using      System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoMapper
{
public static class StringUtils
{
public static string ToCamelCase(   string input)
{
if(string.IsNullOrEmpty(input)){return input;}
if(input.Length==1){return input.ToLowerInvariant();}
return char.ToLowerInvariant(input[0])+input.Substring(   1);
}

public static string ToPascalCase(string input)
{
if(string.IsNullOrEmpty(input)){return input;}
if(input.Length==1){return input.ToUpperInvariant();}
return char.ToUpperInvariant(     input[0])+input.Substring(1);
}

public static string ToSnakeCase(string input)
{
if(string.IsNullOrEmpty(input))   {return input;}
var sb=new StringBuilder();
for(int i=0;i<input.Length;i++)
{
var c=input[i];
if(char.IsUpper(c))
{
if(i>0){sb.Append('_');}
sb.Append(char.ToLowerInvariant(c));
}
else
{
sb.Append(c);
}
}
return sb.ToString(    );
}

public static string ToKebabCase(string input)
{
if(string.IsNullOrEmpty(input)){return input;}
return ToSnakeCase(input).Replace('_',      '-');
}

public static string Truncate(string input,int maxLength,string suffix="...")
{
if(string.IsNullOrEmpty(input)){return input;}
if(input.Length<=maxLength){return input;}
return input.Substring(0,maxLength-suffix.Length)+      suffix;
}

public static string Repeat(string input,    int count)
{
if(string.IsNullOrEmpty(input)||count<=0){return string.Empty;}
var sb=new StringBuilder(input.Length*count);
for(int i=0;i<count;i++){sb.Append(input);}
return sb.ToString();
}

public static bool IsValidEmail(     string input)
{
if(string.IsNullOrWhiteSpace(input)){return false;}
var pattern=@"^[^@\s]+@[^@\s]+\.[^@\s]+$";
return Regex.IsMatch(input,pattern,RegexOptions.IgnoreCase);
}

public static Dictionary<char,int> CharFrequency(string input)
{
var freq=new Dictionary<char,   int>();
if(string.IsNullOrEmpty(input)){return freq;}
foreach(var c in input)
{
if(freq.ContainsKey(c)){freq[c]++;}
else{freq[c]=1;}
}
return freq;
}

public static string RemoveDuplicateWhitespace(      string input)
{
if(string.IsNullOrEmpty(input)){return input;}
return Regex.Replace(input,@"\s+"," ").Trim();
}

public static IEnumerable<string> SplitIntoChunks(string input,int chunkSize)
{
if(string.IsNullOrEmpty(input)){yield break;}
for(int i=0;i<input.Length;      i+=chunkSize)
{
yield return input.Substring(i,Math.Min(chunkSize,input.Length-i));
}
}

public static string Reverse(     string input)
{
if(string.IsNullOrEmpty(input)){return input;}
var chars=input.ToCharArray();
Array.Reverse(chars);
return new string(chars);
}

public static bool IsPalindrome(string input)
{
if(string.IsNullOrEmpty(input)){return true;}
var cleaned=new string(input.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
return cleaned==Reverse(    cleaned);
}

public static int CountOccurrences(string input,string search,   bool ignoreCase=false)
{
if(string.IsNullOrEmpty(input)||string.IsNullOrEmpty(search)){return 0;}
int count=0;
int index=0;
var comparison=ignoreCase?StringComparison.OrdinalIgnoreCase:StringComparison.Ordinal;
while((index=input.IndexOf(search,index,comparison))!=-1)
{
count++;
index+=search.Length;
}
return count;
}

public static string MaskString(string input,int visibleStart=2,int visibleEnd=2,       char maskChar='*')
{
if(string.IsNullOrEmpty(input)){return input;}
if(input.Length<=visibleStart+visibleEnd){return new string(maskChar,input.Length);}
var sb=new StringBuilder();
sb.Append(input.Substring(0,visibleStart));
sb.Append(new string(maskChar,input.Length-visibleStart-visibleEnd));
sb.Append(input.Substring(input.Length-visibleEnd));
return sb.ToString(   );
}

public static string ToTitleCase(string input)
{
if(string.IsNullOrEmpty(input)){return input;}
var words=input.Split(' ');
var result=new StringBuilder();
foreach(var word in words)
{
if(result.Length>0){result.Append(' ');}
if(word.Length>0){result.Append(char.ToUpperInvariant(word[0]));
if(word.Length>1){result.Append(word.Substring(1).ToLowerInvariant());}}
}
return result.ToString();
}
}
}
