﻿using System;
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
        

        internal static string GetNumbers(this string input) {
		while(!Char.IsDigit(input[input.Length-1])) {
			input = input.RemoveAt(input.Length-1);
		}
		int position = input.Length -1;
		while(Char.IsDigit(input[position])) {
			position--;
		}
		return input.Remove(0,position +1);
	}
	
	internal static string RemoveAt(this string s, int index)
        {
         return s.Remove(index, 1);
        }
        
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
                    tcp.ratePoint = Double.Parse(matchFormatted, CultureInfo.InvariantCulture);
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
                    ratePoint = candidate.Point
                });
            return tcpList;
        }

        public class TestCase
        {
            public double ratePoint { get; set; }
            public string Description { get; set; }
            public string TestQuery { get; set; }
        }
    }
}