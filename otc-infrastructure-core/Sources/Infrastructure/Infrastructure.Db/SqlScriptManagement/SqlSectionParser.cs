using System;
using System.Collections.Generic;

namespace Infrastructure.Db.SqlScriptManagement
{
    public static class SqlSectionParser
    {
        private const string StartSection = "/*Section_";
        private const string EndSection = "EndSection*/";
        private const string EndSection2 = "--EndSection*/";

        public static string ReadMainPart(string sql)
        {
            var sectionStart = GetStartSection(sql, 0, false);
            if (sectionStart.Index < 0)
            {
                return sql;
            }

            return sql.Substring(0, sectionStart.Index);
        }

        public static IEnumerable<KeyValuePair<string, string>> ReadSections(string sql)
        {
            int index = 0;
            var section = GetStartSection(sql, index);
            while (section.Index >= 0)
            {
                var start = section.Index + StartSection.Length + section.Name.Length;
                var endSection = GetEndSection(sql, start);
                var resString = sql.Substring(start, endSection.Start - start);
                yield return new KeyValuePair<string, string>(section.Name, resString);

                index = endSection.Start + endSection.Length;
                if (index >= sql.Length)
                {
                    break;
                }

                section = GetStartSection(sql, index);
            }
        }

        private static (int Index, string Name) GetStartSection(string sql, int start = 0, bool withName = true)
        {
            var index = sql.IndexOf(StartSection, start, StringComparison.InvariantCulture);
            if (index < 0)
            {
                return (index, string.Empty);
            }

            var name = string.Empty;
            if (withName)
            {
                var startName = index + StartSection.Length;
                var index2 = sql.IndexOf("\r", startName, StringComparison.InvariantCulture);
                var index3 = sql.IndexOf("\n", startName, StringComparison.InvariantCulture);
                var index4 = sql.IndexOf(" ", startName, StringComparison.InvariantCulture);

                index2 = MinCorrect(index2, index3);
                index2 = MinCorrect(index2, index4);
                if (index2 < 0)
                {
                    index2 = sql.Length;
                }

                name = sql.Substring(startName, index2 - startName);
            }

            return (index, name);
        }

        private static int MinCorrect(int a, int b)
        {
            if (a < 0)
            {
                a = b;
            }
            else if (b >= 0)
            {
                a = Math.Min(a, b);
            }
            return a;
        }

        private static (int Start, int Length) GetEndSection(string sql, int sectionStartIndex)
        {
            var index = sql.IndexOf(EndSection, sectionStartIndex, StringComparison.InvariantCulture);
            var index2 = sql.IndexOf(EndSection2, sectionStartIndex, StringComparison.InvariantCulture);

            if (index < 0)
            {
                return (sql.Length, 0);
            }

            if (index2 >= 0 && index2 < index)
            {
                return (index2, EndSection2.Length);
            }

            return (index, EndSection.Length);
        }
    }
}