namespace MLFinancialRiskPrediction.Services
{
    using CsvHelper.Configuration;
    using CsvHelper.TypeConversion;

    using MLFinancialRiskPrediction.Converters;
    using MLFinancialRiskPrediction.Models;

    public class LoanDataMap : ClassMap<LoanData>
    {
        public LoanDataMap()
        {
            this.Map(m => m.ApplicationDate).Index(0);
            this.Map(m => m.Age).Index(1).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.AnnualIncome).Index(2).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.CreditScore).Index(3).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.EmploymentStatus).Index(4);
            this.Map(m => m.EducationLevel).Index(5);
            this.Map(m => m.Experience).Index(6).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.LoanAmount).Index(7).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.LoanDuration).Index(8).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.MaritalStatus).Index(9);
            this.Map(m => m.NumberOfDependents).Index(10).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.HomeOwnershipStatus).Index(11);
            this.Map(m => m.MonthlyDebtPayments).Index(12).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.CreditCardUtilizationRate).Index(13).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.NumberOfOpenCreditLines).Index(14).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.NumberOfCreditInquiries).Index(15).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.DebtToIncomeRatio).Index(16).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.BankruptcyHistory).Index(17).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.LoanPurpose).Index(18);
            this.Map(m => m.PreviousLoanDefaults).Index(19).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.PaymentHistory).Index(20).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.LengthOfCreditHistory).Index(21).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.SavingsAccountBalance).Index(22).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.CheckingAccountBalance).Index(23).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.TotalAssets).Index(24).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.TotalLiabilities).Index(25).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.MonthlyIncome).Index(26).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.UtilityBillsPaymentHistory).Index(27).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.JobTenure).Index(28).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.NetWorth).Index(29).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.BaseInterestRate).Index(30).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.InterestRate).Index(31).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.MonthlyLoanPayment).Index(32).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.TotalDebtToIncomeRatio).Index(33).TypeConverter<CustomFloatConverter>();
            this.Map(m => m.LoanApproved).Index(34).TypeConverter<BooleanConverter>();
            this.Map(m => m.RiskScore).Index(35).TypeConverter<CustomFloatConverter>();
        }
    }
}
