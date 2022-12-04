namespace LawyerAPI.Models
{
    public class AssignedLawyer
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public int CourtCaseId { get; set; }
        public int LawyerId { get; set; }
    }
}
