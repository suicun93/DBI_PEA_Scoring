﻿using System;
using System.Collections.Generic;

namespace DBI_Grading.Model.Teacher
{
    [Serializable]
    public class Paper
    {
        public string PaperNo { get; set; }
        public List<Candidate> CandidateSet { get; set; }
        public string ExamCode { get; set; }
    }
}