namespace DBI_PEA_Scoring.Model
{
    [System.Serializable]
    public class Requirement
    {
        public enum RequirementTypes
        {
            ResultSet = 1,
            Effect = 2,
            Parameter = 3
        }

        public string RequirementId { get; set; }
        public string CandidateId { get; set; }
        public RequirementTypes Type { get; set; }
        public bool RequireSort { get; set; }
        public string CheckEffectQuery { get; set; }
    }
}
