using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using DBI_Grading.Model.Teacher;

namespace DBI_Grading.Utils
{
    internal class Utilities
    {
        public static T WithTimeout<T>(Func<T> proc, int duration)
        {
            var reset = new AutoResetEvent(false);
            var r = default(T);
            Exception ex = null;

            var t = new Thread(() =>
            {
                try
                {
                    r = proc();
                }
                catch (Exception e)
                {
                    ex = e;
                }
                reset.Set();
            });

            t.Start();

            // not sure if this is really needed in general
            while (t.ThreadState != ThreadState.Running)
                Thread.Sleep(0);

            if (!reset.WaitOne(duration * 1000))
            {
                t.Abort();
                throw new TimeoutException();
            }

            if (ex != null)
                throw ex;
            return r;
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
                    tcp.Point = double.Parse(matchFormatted, CultureInfo.InvariantCulture);
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