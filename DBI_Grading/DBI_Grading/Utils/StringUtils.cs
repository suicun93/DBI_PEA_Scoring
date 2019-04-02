using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using DBI_Grading.Model.Teacher;

namespace DBI_Grading.Utils
{
    internal static class StringUtils
    {
        internal static string GetNumbers(this string input)
        {
            while (input.Length > 0 && !char.IsDigit(input[input.Length - 1]))
                input = input.RemoveAt(input.Length - 1);
            var position = input.Length - 1;
            if (position == -1)
                return input;
            while (position != -1)
            {
                position--;
                if (position == -1) break;
                if (!char.IsNumber(input[position]))
                    break;
            }
            return position == -1 ? input : input.Remove(0, position + 1);
        }

        internal static string RemoveAt(this string s, int index)
        {
            return s.Remove(index, 1);
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="oldValue">in lowercase</param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static string ReplaceByLine(string input, string oldValue, string newValue)
        {
            var list = input.Split('\n');
            var output = "";
            for (var i = 0; i < list.Length; i++)
            {
                var tmp = Regex.Replace(list[i], @"\s+", "");
                if (tmp.ToLower().Equals(oldValue))
                    list[i] = newValue;
                output = string.Concat(output, "\n", list[i]);
            }
            return output;
        }

        internal static int GetHammingDistance(string s, string t)
        {
            if (s.Length != t.Length)
                throw new Exception("Strings must be equal length");

            var distance =
                s.ToCharArray()
                    .Zip(t.ToCharArray(), (c1, c2) => new {c1, c2})
                    .Count(m => m.c1 != m.c2);

            return distance;
        }

        public static List<TestCase> GetTestCases(string input, Candidate candidate)
        {
            var matchPoint = Regex.Match(input, @"(/\*(.|[\r\n])*?\*/)|(--(.*|[\r\n]))",
                RegexOptions.Singleline);
            var matchQuery = Regex.Match(input + "/*", @"(\*/(.|[\r\n])*?/\*)|(--(.*|[\r\n]))",
                RegexOptions.Multiline);
            var queryList = new List<string>();
            while (matchQuery.Success)
            {
                queryList.Add(matchQuery.Value.Split('/')[1].Trim());
                matchQuery = matchQuery.NextMatch();
            }

            var tcpList = new List<TestCase>();
            var count = 0;
            var tcp = new TestCase();
            while (matchPoint.Success)
            {
                var matchFormatted = matchPoint.Value.Split('*')[1];
                if (count++ % 2 == 0)
                {
                    tcp.RatePoint = double.Parse(matchFormatted, CultureInfo.InvariantCulture);
                }
                else
                {
                    tcp.Description = matchFormatted;
                    tcp.TestQuery = queryList.ElementAt(count - 1);
                    tcpList.Add(tcp);
                    tcp = new TestCase();
                }
                matchPoint = matchPoint.NextMatch();
            }
            if (tcpList.Count == 0)
                tcpList.Add(new TestCase
                {
                    TestQuery = input,
                    Description = "",
                    RatePoint = candidate.Point
                });
            return tcpList;
        }

        public class TestCase
        {
            public double RatePoint { get; set; }
            public string Description { get; set; }
            public string TestQuery { get; set; }
        }
    }
}