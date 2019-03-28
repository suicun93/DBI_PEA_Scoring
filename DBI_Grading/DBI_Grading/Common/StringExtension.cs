using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBI_Grading.Common
{
    public static class StringExtension
    {
        internal static string GetNumbers(string input) => new string(input.Where(c => char.IsDigit(c)).ToArray());
    }
}
