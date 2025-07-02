namespace MLFinancialRiskPrediction.Core
{
    using System.Collections.Generic;

    using MLFinancialRiskPrediction.Models;

    public interface IDataLoader
    {
        IEnumerable<LoanData> LoadData(string filePath);
        (IEnumerable<LoanData> Train, IEnumerable<LoanData> Test) SplitData(IEnumerable<LoanData> data, double testFraction = 0.3);
    }
}
