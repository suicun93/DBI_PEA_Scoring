using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using DBI_Grading.Model.Teacher;

namespace DBI_Grading.Utils
{
    internal class StringUtils
    {
        

        internal static string GetNumbers(string input) => new string(input.Where(c => Char.IsDigit(c)).ToArray());

        internal static int GetHammingDistance(string s, string t)
        {
            if (s.Length != t.Length)
            {
                throw new Exception("Strings must be equal length");
            }

            int distance =
                s.ToCharArray()
                    .Zip(t.ToCharArray(), (c1, c2) => new { c1, c2 })
                    .Count(m => m.c1 != m.c2);

            return distance;
        }

        public static List<TestCase> GetTestCases(Candidate candidate)
        {
            var matchPoint = Regex.Match(candidate.TestQuery, @"(/\*(.|[\r\n])*?\*/)|(--(.*|[\r\n]))",
                RegexOptions.Singleline);
            var matchQuery = Regex.Match(candidate.TestQuery + "/*", @"(\*/(.|[\r\n])*?/\*)|(--(.*|[\r\n]))",
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
                    tcp.Point = Double.Parse(matchFormatted, CultureInfo.InvariantCulture);
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
                    TestQuery = candidate.TestQuery,
                    Description = "",
                    Point = candidate.Point
                });
            return tcpList;
        }

        public class TestCase
        {
            public double Point { get; set; }
            public string Description { get; set; }
            public string TestQuery { get; set; }
        }
    }
}