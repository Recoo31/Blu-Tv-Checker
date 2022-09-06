namespace Parsing
{
    public static class LRParser
    {

        public static IEnumerable<string> ParseBetween(string input, string leftDelim, string rightDelim, bool caseSensitive = true)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (leftDelim == null)
                throw new ArgumentNullException(nameof(leftDelim));

            if (rightDelim == null)
                throw new ArgumentNullException(nameof(rightDelim));
            if (leftDelim == string.Empty && rightDelim == string.Empty)
            {
                yield return input;
                yield break;
            }

            var comp = caseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
            if (((leftDelim != string.Empty && !input.Contains(leftDelim, comp)) || (rightDelim != string.Empty && !input.Contains(rightDelim, comp))))
                yield break;

            while ((leftDelim == string.Empty || (input.Contains(leftDelim, comp))) && (rightDelim == string.Empty || input.Contains(rightDelim, comp)))
            {
                var pFrom = leftDelim == string.Empty ? 0 : input.IndexOf(leftDelim, comp) + leftDelim.Length;
                input = input.Substring(pFrom);
                var pTo = rightDelim == string.Empty ? input.Length : input.IndexOf(rightDelim, comp);

                yield return (input.Substring(0, pTo));
                input = input.Substring(pTo + rightDelim.Length);
            }
        }
    }

}