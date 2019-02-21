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
        public bool RequireSort { get; set; }

        public string EffectTable { get; set; }
        public string CheckEffectQuery { get; set; }
        public string TriggerTriggerQuery { get; set; }
    }
}
