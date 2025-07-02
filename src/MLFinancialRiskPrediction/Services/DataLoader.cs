namespace MLFinancialRiskPrediction.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using CsvHelper;
    using CsvHelper.Configuration;

    using MLFinancialRiskPrediction.Core;
    using MLFinancialRiskPrediction.Models;

    public class DataLoader : IDataLoader
    {
        /// <summary>
        /// Loads loan data records from a CSV file at the specified file path.
        /// </summary>
        /// <param name="filePath">The path to the CSV file containing loan data.</param>
        /// <returns>An <see cref="IEnumerable{LoanData}"/> containing the parsed loan data records.</returns>
        /// <remarks>
        /// This method reads the entire contents of the specified CSV file, configures the CSV reader to expect a header row and trim whitespace,
        /// and uses a custom class map (<see cref="LoanDataMap"/>) to map CSV columns to <see cref="LoanData"/> properties.
        /// The records are parsed and returned as a list.
        /// </remarks>
        public IEnumerable<LoanData> LoadData(string filePath)
        {
            CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim
            };

            using StringReader reader = new StringReader(File.ReadAllText(filePath));
            using CsvReader csv = new CsvReader(reader, config);

            csv.Context.RegisterClassMap<LoanDataMap>();
            return csv.GetRecords<LoanData>().ToList();
        }

        /// <summary>
        /// Splits the provided loan data into training and testing datasets.
        /// </summary>
        /// <param name="data">The collection of loan data to be split.</param>
        /// <param name="testFraction">The fraction of data to be used for testing (default is 0.3).</param>
        /// <returns>
        /// A tuple containing two <see cref="IEnumerable{LoanData}"/>:
        /// - The first element is the training dataset.
        /// - The second element is the testing dataset.
        /// </returns>
        public (IEnumerable<LoanData> Train, IEnumerable<LoanData> Test) SplitData(IEnumerable<LoanData> data, double testFraction = 0.3)
        {
            List<LoanData> dataList = data.ToList();
            Random random = new Random(42);
            List<LoanData> shuffled = dataList.OrderBy(x => random.Next()).ToList();

            int testSize = (int)(shuffled.Count * testFraction);
            IEnumerable<LoanData> testData = shuffled.Take(testSize);
            IEnumerable<LoanData> trainData = shuffled.Skip(testSize);

            return (trainData, testData);
        }
    }
}
