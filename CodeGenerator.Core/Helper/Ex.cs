﻿using CodeGenerator.Core.Models;
using CodeGenerator.Core.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Helper
{
    public static class Ex
    {
        public static async Task SaveFile(this ClassTemplate self, string filename, string content)
        {
            var path = self.SavePath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using var fs = File.Open(Path.Combine(path, filename), FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            byte[] data = Encoding.UTF8.GetBytes(content);
            await fs.WriteAsync(data, 0, data.Length);
            await fs.FlushAsync();
        }

        public static string PascalName(this DatabaseTable self, string prefix, string separator)
        {
            return Parse(self.TableName, prefix, separator);
        }

        public static string BuildContent(this DatabaseTable self, string prefix, string separator)
        {
            var columns = self.Columns;
            var content = new StringBuilder();
            foreach (var item in columns)
            {
                content.Append($@"
        /// <summary>
        /// {item.Comments}
        /// </summary>
        [ColumnName(""{item.ColumnName}"")]
        public {item.ParseDataType()} {item.PascalName(prefix, separator)} {{ get; set; }}
");
            }
            return content.ToString();
        }

        public static string PascalName(this TableColumn self, string prefix, string separator)
        {
            return Parse(self.ColumnName, prefix, separator);
        }

        public static string ParseDataType(this TableColumn self)
        {
            return TypeMap.Map(self.DataType, self.Nullable);
        }

        private static string Parse(string text, string prefix, string separator)
        {
            var removePrefix = Regex.Replace(text.ToLower(), $"^{prefix}", m => "");
            var fix = Regex.Replace(removePrefix, "^.", m =>
            {
                if (m.Value == separator) return separator;
                else return $"{separator}{m.Value}";
            });
            return Regex.Replace(fix, $"({separator})(?<={separator})(\\w)", m =>
            {
                return m.Groups[2].Value.ToUpper();
            });
        }
    }
}
