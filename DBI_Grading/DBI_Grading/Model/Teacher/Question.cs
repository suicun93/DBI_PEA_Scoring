using System;
using System.Collections.Generic;

namespace DBI_Grading.Model.Teacher
{
    [Serializable]
    public class Question
    {
        public string QuestionId { get; set; }
        public decimal Point { get; set; }
        public List<Candidate> Candidates { get; set; }
    }
}