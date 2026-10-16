using System;
using System.Linq;

namespace AlgoritmeTeAvancuara.java3
{
    internal static class Task2
    {
        private static int comparisons = 0;
        private static int passes = 0;

        public static void ResetCounters()
        {
            comparisons = 0;
            passes = 0;
        }

        public static (int comparisons, int passes) GetCounters()
        {
            return (comparisons, passes);
        }

        public static T[] ComparisonCountingSort<T>(
            T[] A, Func<T, int> key)
        {
            int n = A.Length;
            int[] count = new int[n];

            for (int i = 0; i < n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    comparisons++;

                    if (key(A[i]) <= key(A[j]))
                    {
                        count[j]++;
                    }
                    else
                    {
                        count[i]++;
                    }
                }
            }

            T[] S = new T[n];

            for (int i = 0; i < n; i++)
            {
                S[count[i]] = A[i];
            }

            return S;
        }

        public static T[] DistributionCountingSort<T>(
            T[] A, Func<T, int> key, int lo, int hi)
        {
            passes++;

            int[] D = new int[hi - lo + 1];

            for (int i = 0; i < A.Length; i++)
            {
                D[key(A[i]) - lo]++;
            }

            for (int j = 1; j < D.Length; j++)
            {
                D[j] += D[j - 1];
            }

            T[] S = new T[A.Length];

            for (int i = A.Length - 1; i >= 0; i--)
            {
                int j = key(A[i]) - lo;
                S[D[j] - 1] = A[i];
                D[j]--;
            }

            return S;
        }

        public static DateRecord[] SortByDate(DateRecord[] records)
        {
            Func<DateRecord, int> year = r => int.Parse(r.Date.Substring(0, 4));
            Func<DateRecord, int> month = r => int.Parse(r.Date.Substring(5, 2));
            Func<DateRecord, int> day = r => int.Parse(r.Date.Substring(8, 2));

            if (records.Length == 0)
            {
                return new DateRecord[0];
            }

            int minYear = year(records[0]);
            int maxYear = minYear;

            for (int i = 1; i < records.Length; i++)
            {
                int y = year(records[i]);
                if (y < minYear) minYear = y;
                if (y > maxYear) maxYear = y;
            }

            DateRecord[] S = DistributionCountingSort(records, day, 1, 31);
            S = DistributionCountingSort(S, month, 1, 12);
            S = DistributionCountingSort(S, year, minYear, maxYear);

            return S;
        }

        private static readonly string[] Alphabet =
        {
            "a", "b", "c", "ç", "d", "dh", "e", "ë", "f", "g", "gj", "h",
            "i", "j", "k", "l", "ll", "m", "n", "nj", "o", "p", "q", "r",
            "rr", "s", "sh", "t", "th", "u", "v", "x", "xh", "y", "z", "zh"
        };

        private static int LetterNumber(string letter)
        {
            for (int k = 0; k < Alphabet.Length; k++)
            {
                if (Alphabet[k] == letter)
                {
                    return k + 1;
                }
            }

            return 0;
        }

        public static int[] Tokenize(string word)
        {
            var letters = new System.Collections.Generic.List<int>();
            int i = 0;

            while (i < word.Length)
            {
                if (i + 1 < word.Length)
                {
                    int two = LetterNumber(word.Substring(i, 2));
                    if (two != 0)
                    {
                        letters.Add(two);
                        i += 2;
                        continue;
                    }
                }

                letters.Add(LetterNumber(word.Substring(i, 1)));
                i++;
            }

            return letters.ToArray();
        }

        public static string[] SortAlbanian(string[] words)
        {
            var items = words
                .Select(w => (word: w, tokens: Tokenize(w)))
                .ToArray();

            int maxLength = 0;
            foreach (var item in items)
            {
                if (item.tokens.Length > maxLength)
                {
                    maxLength = item.tokens.Length;
                }
            }

            for (int pos = maxLength - 1; pos >= 0; pos--)
            {
                int p = pos;
                items = DistributionCountingSort(
                    items,
                    item => p < item.tokens.Length ? item.tokens[p] : 0,
                    0,
                    36);
            }

            return items.Select(item => item.word).ToArray();
        }

        public static void Run()
        {
            Student[] six =
            {
                new Student("Arta", 9),
                new Student("Blend", 6),
                new Student("Dita", 9),
                new Student("Edon", 9),
                new Student("Fjolla", 6),
                new Student("Gent", 5)
            };

            Func<Student, int> g = student => student.Grade;

            Console.WriteLine("\n=== Task 2 ===");

            ResetCounters();
            Student[] r1 = ComparisonCountingSort(six, g);
            Console.WriteLine("comparisonCountingSort(six, g): " +
                string.Join(" ", r1.Select(s => s.Name)));
            Console.WriteLine("comparisons: " + GetCounters().comparisons);

            Student[] r2 = DistributionCountingSort(six, g, 5, 10);
            Console.WriteLine("distributionCountingSort(six, g, 5, 10): " +
                string.Join(" ", r2.Select(s => s.Name)));

            int[] nums = { 1003, 1001, 1002, 1001 };
            int[] r3 = DistributionCountingSort(nums, x => x, 1001, 1003);
            Console.WriteLine("distributionCountingSort([1003,1001,1002,1001]): [" +
                string.Join(", ", r3) + "]");

            DateRecord[] recs =
            {
                new DateRecord(0, "2024-10-13"), new DateRecord(1, "2024-01-20"),
                new DateRecord(2, "2026-05-15"), new DateRecord(3, "2024-01-15"),
                new DateRecord(4, "2024-05-02"), new DateRecord(5, "2026-08-27"),
                new DateRecord(6, "2025-02-19"), new DateRecord(7, "2026-11-22"),
                new DateRecord(8, "2024-09-10"), new DateRecord(9, "2026-12-11")
            };

            ResetCounters();
            DateRecord[] r4 = SortByDate(recs);
            Console.WriteLine("sortByDate(recs): " +
                string.Join(" ", r4.Select(r => r.Id)));
            Console.WriteLine("passes: " + GetCounters().passes);

            Console.WriteLine("tokenize('llamba'): [" +
                string.Join(", ", Tokenize("llamba")) + "]");
            Console.WriteLine("tokenize('shtëpi'): [" +
                string.Join(", ", Tokenize("shtëpi")) + "]");

            string[] words =
            {
                "shi", "çaj", "llamba", "zemër", "dhe", "gur", "nënë", "tokë",
                "xhami", "emër", "rrugë", "cep", "ëndërr", "drita", "thikë",
                "lule", "njeri", "gjak", "zhurmë", "rosa", "dera", "sot",
                "xixa", "dhelpër"
            };

            ResetCounters();
            string[] r5 = SortAlbanian(words);
            Console.WriteLine("sortAlbanian(words): " + string.Join(" ", r5));
            Console.WriteLine("passes: " + GetCounters().passes);
        }
    }

    internal class DateRecord
    {
        public int Id { get; }
        public string Date { get; }

        public DateRecord(int id, string date)
        {
            Id = id;
            Date = date;
        }
    }
}
