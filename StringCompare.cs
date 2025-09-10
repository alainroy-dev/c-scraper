using System;

namespace BoxrecScraper.Utils
{
    public static class StringCompareFn
    {
        private static int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; i++) d[i, 0] = i;
            for (int j = 0; j <= m; j++) d[0, j] = j;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1,
                                 d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost
                    );
                }
            }

            return d[n, m];
        }

        public static double Compare(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2))
                return 100;
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
                return 0;

            int distance = LevenshteinDistance(s1, s2);
            int maxLen = Math.Max(s1.Length, s2.Length);

            double similarity = (1.0 - (double)distance / maxLen) * 100;

            if (similarity < 0) similarity = 0;
            if (similarity > 100) similarity = 100;

            return similarity;
        }

        // New function: compare a string against a string array
        public static (int index, double score) CompareWithArray(string input, string[] array)
        {
            int bestIndex = -1;
            double bestScore = -1;

            for (int i = 0; i < array.Length; i++)
            {
                double score = Compare(input, array[i]);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestIndex = i;
                }
            }

            return (bestIndex, bestScore);
        }
    }
}
