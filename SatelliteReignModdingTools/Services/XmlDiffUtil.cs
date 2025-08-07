using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SatelliteReignModdingTools.Services
{
    public static class XmlDiffUtil
    {
        public static string Diff(string oldText, string newText)
        {
            if (string.IsNullOrEmpty(oldText) && string.IsNullOrEmpty(newText)) return "(no changes)";
            if (oldText == newText) return "(no changes)";

            var oldLines = (oldText ?? string.Empty).Replace("\r\n", "\n").Split('\n');
            var newLines = (newText ?? string.Empty).Replace("\r\n", "\n").Split('\n');

            var sb = new StringBuilder();
            int i = 0, j = 0;
            while (i < oldLines.Length || j < newLines.Length)
            {
                var left = i < oldLines.Length ? oldLines[i] : null;
                var right = j < newLines.Length ? newLines[j] : null;

                if (left == right)
                {
                    if (left != null) sb.AppendLine("  " + left);
                    i++; j++;
                }
                else if (right != null && (i >= oldLines.Length || !Array.Exists(newLines, l => l == left)))
                {
                    sb.AppendLine("+ " + right);
                    j++;
                }
                else
                {
                    if (left != null) sb.AppendLine("- " + left);
                    i++;
                }
            }

            return sb.ToString();
        }

        public static string ReadAllTextOrEmpty(string path)
        {
            try { return File.Exists(path) ? File.ReadAllText(path) : string.Empty; }
            catch { return string.Empty; }
        }
    }
}
