using HCM.CodeFormatter;

namespace Application.Helper
{
    public static class KeyGenerator
    {
        private static readonly Random Random = new();

        public static string GenerateKey(string prefix, int nextIndex, int anchorLength = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var anchor = new string(Enumerable.Range(0, anchorLength)
                .Select(_ => chars[Random.Next(chars.Length)]).ToArray());

            return CodeFormatter.Format(prefix, anchor, DateTime.UtcNow, nextIndex);
        }
    }

}
