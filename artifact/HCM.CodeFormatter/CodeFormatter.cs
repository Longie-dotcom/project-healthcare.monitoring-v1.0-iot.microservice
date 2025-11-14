namespace HCM.CodeFormatter
{
    public static class CodeFormatter
    {
        /// <summary>
        /// Format a code with optional key and index.
        /// </summary>
        /// <param name="prefix">Code prefix, e.g. PAT, STAF, DEV</param>
        /// <param name="key">Optional key (identity, etc.)</param>
        /// <param name="date">Optional date to include (default: today)</param>
        /// <param name="index">Incremental index provided by the microservice</param>
        public static string Format(string prefix, string? key = null, DateTime? date = null, int index = 1)
        {
            var datePart = (date ?? DateTime.UtcNow).ToString("yyyyMMdd");
            var indexPart = index.ToString("D3");

            return $"{prefix}-{key}-{datePart}-{indexPart}";
        }
    }
}
