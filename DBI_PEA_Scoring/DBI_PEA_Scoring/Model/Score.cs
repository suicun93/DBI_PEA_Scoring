using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBI_PEA_Scoring.Model
{
    class Result
    {
        public string StudentID { get; set; }
        public string PaperNo { get; set; }
        public List<string> ListAnswers { get; set; }
        public List<Candidate> ListCandidates { get; set; }
        public double[] Points { get; set; }

        public Result()
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

        private  bool Cham(Candidate candidate, string answer)
        {
            // await TaskEx.Delay(100);
            if (String.IsNullOrEmpty(answer))
                return false;
            // Cho nay moi xu ly Sql 
            return true;
        }

        /// <summary>
        /// Get Point function
        /// </summary>
        public  void GetPoint(DataGridView dataGridView, int row)
        {
            dataGridView.Rows.Add(1);
            dataGridView.Rows[row].Cells[0].Value = StudentID;
            dataGridView.Rows[row].Cells[1].Value = PaperNo;
            dataGridView.FirstDisplayedScrollingRowIndex = dataGridView.RowCount - 1;
            for (int questionOrder = 0; questionOrder < 10; questionOrder++)
            {
                try
                {
                    // Cham diem o day
                    //bool correct = await Cham(ListCandidates.ElementAt(i), ListAnswers.ElementAt(i));
                    if (Cham(ListCandidates.ElementAt(questionOrder), ListAnswers.ElementAt(questionOrder)))
                        Points[questionOrder] = ListCandidates.ElementAt(questionOrder).Point;
                    else
                        Points[questionOrder] = 0;
                    dataGridView.Rows[row].Cells[2 + questionOrder].Value = Points[questionOrder].ToString();
                }
                catch (Exception e)
                {
                    // Thieu candidate hoac answer thi cho 0 luon
                    dataGridView.Rows[row].Cells[2 + questionOrder].Value = Points[questionOrder].ToString();
                    Console.WriteLine(e.Message);
                }
            }
            dataGridView.Refresh();
        }
    }
}
