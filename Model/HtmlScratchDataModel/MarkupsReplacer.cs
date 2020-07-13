using System;
using System.Linq;

namespace Model.HtmlScratchDataModel
{
    public static class MarkupsReplacer
    {
        private static int hIndex = 1;
        public static string ReplaceMarkups(this string input)
            => input
                .NewLineMarkup()
                .TwoPatternsReplacer(">>", "<<", "<q>", "</q>")
                .TwoPatternsReplacer("_!", "!_", "<ins>", "</ins>")
                .TwoPatternsReplacer("-!", "!-", "<del>", "</del>")
                .SamePatternReplacement("**", "<strong>", "</strong>")
                .SamePatternReplacement('*', "<em>", "</em>")
                .HashMarkup()
                .WholeLineMarkup();

        public static string NewLineMarkup(this string input)
        {
            return input.Contains(">>")
                   || input.Contains("<<")
                   || input.Contains("_!")
                   || input.Contains("!_")
                   || input.Contains("-!")
                   || input.Contains("!-")
                   || input.Contains("#")
                   || input.Contains("{")
                   || input.Contains("|")
                   || input.Contains("}")
                   || input.Contains("[")
                   || input.Contains("]")
                   || input.Contains("*")
                ? input
                : $"<p>{input}</p>";
        }
        public static string TwoPatternsReplacer(this string input, string leftPattern, string rightPattern,
            string leftMarkup, string rightMarkup)
        {
            //markup correction check
            if (leftPattern == rightPattern) throw new ArgumentException("Patterns can't be the same!");
            if (leftMarkup == rightPattern) throw new ArgumentException("Left markup and right pattern can't be the same!");

            var leftMarkupElementsCheck = input.Slice2().Where(x => x == leftPattern).ToArray();
            var rightMarkupElementsCheck = input.Slice2().Where(x => x == rightPattern).ToArray();
            if (leftMarkupElementsCheck.Length != rightMarkupElementsCheck.Length) throw new WrongMarkupException();

            //expected markup replace
            return input
                .Replace(leftPattern, leftMarkup)
                .Replace(rightPattern, rightMarkup);
        }
        public static string SamePatternReplacement(this string input, string pattern, string markupLeft,
            string markupRight)
        {
            if (!input.Contains(pattern)) return input;
            var slice2 = input.Slice2();
            var pairsWhere = slice2.Where(x => x == pattern);
            var indexes = pairsWhere.Select(x => input.IndexOf(x));
            var tableOfIndexes = indexes.ToArray();
            if (tableOfIndexes.Length % 2 != 0) throw new WrongMarkupException();

            var leftMarkupsString = input.Substring(0, tableOfIndexes[tableOfIndexes.Length / 2]);
            var rightMarkupsString = input.Substring(tableOfIndexes[tableOfIndexes.Length / 2]);

            leftMarkupsString = leftMarkupsString.Replace(pattern, markupLeft);
            rightMarkupsString = rightMarkupsString.Replace(pattern, markupRight);


            return leftMarkupsString + rightMarkupsString;
        }
        public static string SamePatternReplacement(this string input, char pattern, string markupLeft,
            string markupRight)
        {
            if (!input.Contains(pattern)) return input;
            var pairsWhere = input.Where(x => x == pattern);
            var indexes = input.Select((c, index) => new { Item = c, Index = index }).Where(x => x.Item == pattern).Select(x => x.Index).ToArray();
            var tableOfIndexes = indexes.ToArray();
            if (tableOfIndexes.Length % 2 != 0) throw new WrongMarkupException();

            var leftMarkupsString = input.Substring(0, tableOfIndexes[tableOfIndexes.Length / 2]);
            var rightMarkupsString = input.Substring(tableOfIndexes[tableOfIndexes.Length / 2]);

            leftMarkupsString = leftMarkupsString.Replace(pattern.ToString(), markupLeft);
            rightMarkupsString = rightMarkupsString.Replace(pattern.ToString(), markupRight);


            return leftMarkupsString + rightMarkupsString;
        }

        public static string HashMarkup(this string input)
            => input.First() == '#' ? $"<h{hIndex} id=”nX”>{input.Remove(0, 1)}</h{hIndex++}>" : input;

        public static string WholeLineMarkup(this string input)
        {
            if (input.First() != '{') return input;
            var LBCount = input.Count(x => x == '{');
            var RBCout = input.Count(x => x == '}');
            var VBCount = input.Count(x => x == '|');

            if (LBCount != 1 || RBCout != 1 || VBCount != 1) throw new WrongMarkupException();

            var firstSplit = input.Split('|');
            var type = firstSplit[0].Replace("{", "<aside cat=”");
            var secondSplit = firstSplit[1].Split('}');
            var title = secondSplit[0];
            var text = secondSplit[1];

            return $"{type}”>\n\t<header>{title}</header>\n\t<main>{text}</main>\n</aside>";
        }

        public static void Reset()
        {
            hIndex = 1;
        }
    }
}