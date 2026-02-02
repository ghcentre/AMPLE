namespace Ample.Core.Strings;

public static class StringExtensions
{
    extension(IEnumerable<string> strings)
    {
        /// <summary>
        /// Finds the length of the longest common prefix among the provided strings.
        /// </summary>
        /// <returns>The <see cref="Index"/> which is the length of the longest common prefix.</returns>
        /// <exception cref="ArgumentNullException">Input sequence is <see langword="null"/>, or,
        /// one of the strings in the input sequence is <see langword="null"/>.</exception>
        public Index LongestCommonPrefixIndex()
        {
            ArgumentNullException.ThrowIfNull(strings);

            int maxLength = 0;
            string? first = null;

            foreach (string s in strings)
            {
                if (s == null)
                {
                    throw new ArgumentNullException(nameof(strings), SR.Sequence_NullString);
                }

                if (first == null)
                {
                    first = s;
                    maxLength = s.Length;
                    continue;
                }

                maxLength = Math.Min(maxLength, s.Length);

                int index = 0;
                for (; index < maxLength; index++)
                {
                    if (first[index] != s[index])
                    {
                        maxLength = index;
                        break;
                    }
                }

                if (index == 0)
                {
                    break;
                }
            }

            return maxLength;
        }
    }
}
