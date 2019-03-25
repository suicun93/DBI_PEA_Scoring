using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBI_PEA_Scoring.Model.Teacher
{
    [Serializable]
    public class Question
    {
        public string QuestionId { get; set; }
        public double Point { get; set; }
        public List<Candidate> Candidates { get; set; }
    }
}
