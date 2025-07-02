namespace MLFinancialRiskPrediction.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.ML;

    using MLFinancialRiskPrediction.Models;

    public static class MLContextExtensions
    {
        public static IDataView LoadFromLoanData(this MLContext context, IEnumerable<LoanData> data)
        {
            return context.Data.LoadFromEnumerable(data);
        }

        public static void SaveModel(this MLContext context, ITransformer model, IDataView schema, string filePath)
        {
            context.Model.Save(model, schema.Schema, filePath);
            Console.WriteLine($"Model saved to: {filePath}");
        }

        public static ITransformer LoadModel(this MLContext context, string filePath)
        {
            ITransformer model = context.Model.Load(filePath, out DataViewSchema _);
            Console.WriteLine($"Model loaded from: {filePath}");
            return model;
        }

        public static void PrintDataViewSample(this MLContext context, IDataView data, int maxRows = 5)
        {
            Microsoft.ML.Data.DataDebuggerPreview preview = data.Preview(maxRows);

            Console.WriteLine("Data Preview:");
            foreach (Microsoft.ML.Data.DataDebuggerPreview.RowInfo? row in preview.RowView)
            {
                string values = string.Join(", ", row.Values.Select(v => $"{v.Key}={v.Value}"));
                Console.WriteLine($"Row: {values}");
            }
            Console.WriteLine();
        }

        public static IEstimator<ITransformer> CreateFeaturePipeline(this MLContext context)
        {
            return context.Transforms.Categorical.OneHotEncoding(
                    new[]
                    {
                    new InputOutputColumnPair("EmploymentStatusEncoded", "EmploymentStatus"),
                    new InputOutputColumnPair("EducationLevelEncoded", "EducationLevel"),
                    new InputOutputColumnPair("MaritalStatusEncoded", "MaritalStatus"),
                    new InputOutputColumnPair("HomeOwnershipStatusEncoded", "HomeOwnershipStatus"),
                    new InputOutputColumnPair("LoanPurposeEncoded", "LoanPurpose")
                    })
                .Append(context.Transforms.NormalizeMeanVariance(
                    new[]
                    {
                    new InputOutputColumnPair("AgeNormalized", "Age"),
                    new InputOutputColumnPair("AnnualIncomeNormalized", "AnnualIncome"),
                    new InputOutputColumnPair("CreditScoreNormalized", "CreditScore"),
                    new InputOutputColumnPair("LoanAmountNormalized", "LoanAmount"),
                    new InputOutputColumnPair("MonthlyIncomeNormalized", "MonthlyIncome")
                    }))
                .Append(context.Transforms.Concatenate("Features",
                    "AgeNormalized", "AnnualIncomeNormalized", "CreditScoreNormalized", "EmploymentStatusEncoded",
                    "EducationLevelEncoded", "Experience", "LoanAmountNormalized", "LoanDuration",
                    "MaritalStatusEncoded", "NumberOfDependents", "HomeOwnershipStatusEncoded",
                    "MonthlyDebtPayments", "CreditCardUtilizationRate", "NumberOfOpenCreditLines",
                    "NumberOfCreditInquiries", "DebtToIncomeRatio", "BankruptcyHistory", "LoanPurposeEncoded",
                    "PreviousLoanDefaults", "PaymentHistory", "LengthOfCreditHistory", "SavingsAccountBalance",
                    "CheckingAccountBalance", "TotalAssets", "TotalLiabilities", "MonthlyIncomeNormalized",
                    "UtilityBillsPaymentHistory", "JobTenure", "NetWorth", "BaseInterestRate",
                    "InterestRate", "MonthlyLoanPayment", "TotalDebtToIncomeRatio"));
        }
    }
}
