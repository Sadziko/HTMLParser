using System;
using System.Collections.Generic;
using System.Text;

namespace Model.HtmlScratchDataModel
{
    public static class StringExtention
    {
        public static IEnumerable<string> Slice2(this string input)
        {
            var table = new string[input.Length - 1];
            for (var i = 0; i < input.Length - 1; i++)
                table[i] = $"{input[i]}{(input.Length > i + 1 ? input[i + 1].ToString() : "")}";

            return table;
        }
    }
}
