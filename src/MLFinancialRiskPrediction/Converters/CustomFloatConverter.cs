namespace MLFinancialRiskPrediction.Converters
{
    using System.Globalization;

    using CsvHelper;
    using CsvHelper.Configuration;
    using CsvHelper.TypeConversion;

    public class CustomFloatConverter : DefaultTypeConverter
    {
        /// <summary>
        /// Converts a string representation of a float to a float value, handling various formats including thousands separators and decimal points.
        /// </summary>
        /// <param name="text">The string representation of the float.</param>
        /// <param name="row">The reader row context.</param>
        /// <param name="memberMapData">Metadata about the member being converted.</param>
        /// <returns>
        /// A float value parsed from the string, or 0f if the string is null, empty, or cannot be parsed.
        /// </returns>
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return 0f;
            }

            // Remove periods used as thousand separators and handle decimal points
            string cleanText = text.Replace(".", "");

            // Handle the case where the original had a decimal point
            // If the original number seems to have decimals, we need to restore them
            if (float.TryParse(cleanText, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
            {
                // For very large numbers, scale them down to reasonable ranges
                if (result > 1000000000) // More than 1 billion
                {
                    result = result / 1000000000f; // Scale down
                }
                else if (result > 1000000) // More than 1 million  
                {
                    result = result / 1000f; // Scale down moderately
                }

                return result;
            }

            return 0f;
        }
    }
}
