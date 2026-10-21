using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
  Task 1 - Trace   T = "JIM SAW ME IN A BARBERSHOP", P = "BARBER"
  Task 2 - Write   findAll, highlight, goodSuffixTable, boyerMoore + counters
                   (pa IndexOf, Contains, Replace, Split, StartsWith, EndsWith, Regex)
*/

namespace AlgoritmeTeAvancuara.java4
{
    internal class Detyra
    {
        public static void Run()
        {
            Console.OutputEncoding = Encoding.UTF8;

            const string T = "JIM SAW ME IN A BARBERSHOP";
            const string P = "BARBER";

            Console.WriteLine("\n--- java4: Task 1 Trace ---");

            var table = ShiftTable(P);
            Console.WriteLine("shiftTable(P): { " +
                string.Join(", ", table.Select(kv => $"'{kv.Key}' => {kv.Value}")) + " }");

            Console.WriteLine("bruteForce(T, P): " + BruteForce(T, P));

            Console.WriteLine("horspool(T, P) alignments:");
            Console.WriteLine("horspool(T, P): " + Horspool(T, P));

            Console.WriteLine("horspool2(T, P): " + Horspool2(T, P));

            Console.WriteLine("\n--- java4: Task 2 Write ---");

            const string kur = "Kur ngjite përpjetë, shiko dhe tatëpjetë.";

            ResetCounters();
            Console.WriteLine($"findAll(kur, 'pjetë') -> {Show(FindAll(kur, "pjetë"))}   counters -> {ShowCounters()}");
            ResetCounters();
            Console.WriteLine($"findAll('banana', 'ana') -> {Show(FindAll("banana", "ana"))}");

            Console.WriteLine($"highlight(kur, 'pjetë') -> '{Highlight(kur, "pjetë")}'");
            Console.WriteLine($"highlight('banana', 'ana') -> '{Highlight("banana", "ana")}'");

            Console.WriteLine($"goodSuffixTable('ABCBAB') -> {Show(GoodSuffixTable("ABCBAB"))}");
            Console.WriteLine($"goodSuffixTable('BAOBAB') -> {Show(GoodSuffixTable("BAOBAB"))}");
            Console.WriteLine($"goodSuffixTable('tatëpjetë') -> {Show(GoodSuffixTable("tatëpjetë"))}");

            ResetCounters();
            Console.WriteLine($"boyerMoore('BESS KNEW ABOUT BAOBABS', 'BAOBAB') -> " +
                $"{BoyerMoore("BESS KNEW ABOUT BAOBABS", "BAOBAB")}   counters -> {ShowCounters()}");
            ResetCounters();
            Console.WriteLine($"boyerMoore(\"Shqiptari me armë e shokë, s'i lutet kurkujt mbi tokë.\", 'tokë') -> " +
                $"{BoyerMoore("Shqiptari me armë e shokë, s'i lutet kurkujt mbi tokë.", "tokë")}");
        }

        // ---------- Task 1 ----------

        static int BruteForce(string T, string P)
        {
            int n = T.Length, m = P.Length;
            for (int i = 0; i <= n - m; i++)
            {
                int j = 0;
                while (j < m && P[j] == T[i + j]) j++;
                if (j == m) return i;
            }
            return -1;
        }

        static Dictionary<char, int> ShiftTable(string P)
        {
            int m = P.Length;
            var table = new Dictionary<char, int>();
            for (int j = 0; j < m - 1; j++)
                table[P[j]] = m - 1 - j;
            return table;
        }

        static int Shift(Dictionary<char, int> table, char c, int m)
        {
            return table.ContainsKey(c) ? table[c] : m;
        }

        static int Horspool(string T, string P)
        {
            int n = T.Length, m = P.Length;
            var table = ShiftTable(P);
            int i = m - 1;
            while (i <= n - 1)
            {
                Console.WriteLine(i - m + 1);
                int k = 0;
                while (k <= m - 1 && P[m - 1 - k] == T[i - k]) k++;
                if (k == m) return i - m + 1;
                i += Shift(table, T[i], m);
            }
            return -1;
        }

        static int Horspool2(string T, string P)
        {
            int n = T.Length, m = P.Length;
            var table = ShiftTable(P);
            int i = m - 1;
            while (i <= n - 1)
            {
                int k = 0;
                while (k <= m - 1 && P[m - 1 - k] == T[i - k]) k++;
                if (k == m) return i - m + 1;
                i += Shift(table, T[i - k], m);
            }
            return -1;
        }

        // ---------- Task 2 ----------

        static int comparisons = 0;
        static int alignments = 0;

        public static void ResetCounters()
        {
            comparisons = 0;
            alignments = 0;
        }

        public static (int comparisons, int alignments) GetCounters()
        {
            return (comparisons, alignments);
        }

        // +1 sa here krahasohet nje karakter i patternit me nje karakter te tekstit
        static bool Compare(char p, char t)
        {
            comparisons++;
            return p == t;
        }

        static List<int> FindAll(string T, string P)
        {
            int n = T.Length, m = P.Length;
            var result = new List<int>();
            var table = ShiftTable(P);
            int i = m - 1;
            while (i <= n - 1)
            {
                alignments++;
                int k = 0;
                while (k <= m - 1 && Compare(P[m - 1 - k], T[i - k])) k++;
                if (k == m) result.Add(i - m + 1);
                i += Shift(table, T[i], m);
            }
            return result;
        }

        static string Highlight(string T, string P)
        {
            var matches = FindAll(T, P);
            int m = P.Length;
            var sb = new StringBuilder();
            int next = 0;      // indeksi i match-it te radhes ne liste
            int end = -1;      // ku mbaron match-i i fundit i theksuar
            for (int i = 0; i < T.Length; i++)
            {
                // kapercej match-et qe fillojne brenda atij te theksuar me pare
                while (next < matches.Count && matches[next] < end) next++;
                if (next < matches.Count && matches[next] == i)
                {
                    sb.Append('[');
                    end = i + m;
                    next++;
                }
                sb.Append(T[i]);
                if (i == end - 1) sb.Append(']');
            }
            return sb.ToString();
        }

        // a eshte P[a .. a+len-1] i barabarte me P[b .. b+len-1]
        static bool SameSlice(string P, int a, int b, int len)
        {
            for (int x = 0; x < len; x++)
                if (P[a + x] != P[b + x]) return false;
            return true;
        }

        static int[] GoodSuffixTable(string P)
        {
            int m = P.Length;
            var d2 = new int[m];
            d2[0] = 0;
            for (int k = 1; k <= m - 1; k++)
            {
                int start = m - k;     // ku fillon sufiksi
                int found = -1;
                for (int s = start - 1; s >= 0; s--)
                {
                    if (SameSlice(P, s, start, k) && (s == 0 || P[s - 1] != P[start - 1]))
                    {
                        found = s;
                        break;
                    }
                }
                if (found != -1)
                {
                    d2[k] = start - found;
                }
                else
                {
                    int l = k - 1;
                    while (l > 0 && !SameSlice(P, 0, m - l, l)) l--;
                    d2[k] = m - l;     // l = 0 -> m
                }
            }
            return d2;
        }

        static int BoyerMoore(string T, string P)
        {
            int n = T.Length, m = P.Length;
            var t1 = ShiftTable(P);
            var d2 = GoodSuffixTable(P);
            int i = m - 1;
            while (i <= n - 1)
            {
                alignments++;
                int k = 0;
                while (k <= m - 1 && Compare(P[m - 1 - k], T[i - k])) k++;
                if (k == m) return i - m + 1;
                char c = T[i - k];
                int d1 = Math.Max(Shift(t1, c, m) - k, 1);
                i += k == 0 ? d1 : Math.Max(d1, d2[k]);
            }
            return -1;
        }

        static string Show(IEnumerable<int> xs) => "[" + string.Join(", ", xs) + "]";

        static string ShowCounters()
        {
            var c = GetCounters();
            return $"{c.comparisons} comparisons, {c.alignments} alignments";
        }
    }
}
