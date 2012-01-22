namespace AWC.Domain.Entities
{
    public class ResidentIncome
    {
        public int ResidentIncomeId { get; set; }

        public int ClientId { get; set; }

        public bool IsHeadOfHousehold { get; set; }

        public string ResidentName { get; set; }

        public string Relationship { get; set; }

        public double MonthlyIncome { get; set; }

        public string SourceOfIncome { get; set; }

        public virtual Client Client { get; set; }
    }
}