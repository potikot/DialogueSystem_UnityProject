using System.Text.RegularExpressions;

namespace PotikotTools.UniTalks
{
    public static class VariablesParser
    {
        public static string Parse(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";
            
            return Regex.Replace(text, @"\{([^\{\}]+)\}", match =>
            {
                string variableName = match.Groups[1].Value;
                object value = UniTalksAPI.GetRawVariable(variableName);
                return value != null ? value.ToString() : $"<variable '{variableName}' not found>";
            });
        }
    }
}
