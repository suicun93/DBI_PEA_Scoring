using System;
using System.Collections.Generic;

namespace DBI_PEA_Scoring.Model
{
    [Serializable]
    class Candidate
    {
        public Candidate(string questionId, string candidateId, string content, string imageData, List<Requirement> requirements)
        {
            QuestionId = questionId;
            CandidateId = candidateId;
            Content = content;
            ImageData = imageData;
            Requirements = requirements;
        }
        public String CandidateId { get; set; }
        public String QuestionId { get; set; }
        public String Content { get; set; }
        public String QuestionType { get; set; }
        public String ImageData { get; set; }
        public List<Requirement> Requirements { get; set; }



    }
}