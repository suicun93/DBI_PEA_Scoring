using System;
using System.Collections.Generic;

namespace DBI_PEA_Scoring.Model
{
    [Serializable]
    public class Candidate
    {
        public enum QuestionTypes
        {
            Select = 1,
            Procedure = 2,
            Trigger = 3,
            Schema = 4,
            DML = 5
        }
        public string CandidateId { get; set; }
        public string QuestionId { get; set; }
        public string QuestionRequirement { get; set; }
        public QuestionTypes QuestionType { get; set; }

        public string Solution { get; set; }
        public string TestQuery { get; set; }

        public bool RequireSort { get; set; }
        public bool CheckColumnName { get; set; }
        public bool CheckDistinct { get; set; }
        public bool RelatedSchema { get; set; }

        public List<string> Illustration { get; set; }
        public double Point { get; set; }
    }
}