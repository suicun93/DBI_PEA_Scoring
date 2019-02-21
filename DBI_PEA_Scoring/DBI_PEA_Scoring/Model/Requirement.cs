using System;

namespace DBI_PEA_Scoring.Model
{
    [Serializable]
    public class Requirement
    {
        public int RequirementId { get; set; }
        public int CandidateId { get; set; }
        public string Type { get; set; }

        public string ResultQuery { get; set; }

        public string EffectTable { get; set; }
        public string CheckEffectQuery { get; set; }

        public Requirement()
        {
        }

        public Requirement(int requirementId, int candidateId, string type, string resultQuery, string effectTable, string checkEffectQuery)
        {
            RequirementId = requirementId;
            CandidateId = candidateId;
            Type = type;
            ResultQuery = resultQuery;
            EffectTable = effectTable;
            CheckEffectQuery = checkEffectQuery;
        }
    }
}
