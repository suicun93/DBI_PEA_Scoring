using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DBI_PEA_Scoring.Model
{
    class BaiChungDeChamDiem
    {
        public string StudentID { get; set; }
        public string PaperNo { get; set; }
        public List<string> ListAnswers;
        public List<Candidate> ListCandidates { get; set; }
        public double[] Points { get; set; }

        public BaiChungDeChamDiem()
        {
            Points = new double[10];
            ListAnswers = new List<string>();
            ListCandidates = new List<Candidate>();
        }

        /// <summary>
        ///  Get Sum of point
        /// </summary>
        /// <returns></returns>
        public double Point()
        {
            double sum = 0;
            foreach (double point in Points)
            {
                sum += point;
            }
            return sum;
        }

        private bool Cham(Candidate candidate, string baiLam)
        {
            if (String.IsNullOrEmpty(baiLam))
                return false;
            // Cho nay moi xu ly Sql 
            return true;
        }

        /// <summary>
        /// Cham diem function
        /// </summary>
        public void ChamDiem()
        {
            Thread.Sleep(30);
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    // Cham diem o day
                    if (Cham(ListCandidates.ElementAt(i), ListAnswers.ElementAt(i)))
                    {
                        Points[i] += ListCandidates.ElementAt(i).Point;
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
