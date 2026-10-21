using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmeTeAvancuara.java1
{
    public class Funksione
    {
        static int calls = 0;

        // m1: ndan vargun në gjysma (merge-sort style recursion)
        static int M1(int length)
        {
            calls++;
            if (length <= 1) return length;
            int mid = length >> 1;
            return M1(mid) + M1(length - mid) + 1;
        }

        // m2 dhe mm janë rekursion i ndërsjellë (Hofstadter Female/Male)
        static int M2(int n)
        {
            calls++;
            if (n == 0) return 1;
            return n - Mm(M2(n - 1));
        }

        static int Mm(int n)
        {
            if (n == 0) return 0;
            return n - M2(Mm(n - 1));
        }

        static int M3(int n)
        {
            calls++;
            if (n <= 1) return 1;
            return M3(n - 1) + M3(n - 2);
        }

        static void Run(string label, Func<int> fn)
        {
            calls = 0;
            int value = fn();
            Console.WriteLine(label);
            Console.WriteLine($"Returns: {value}");
            Console.WriteLine($"Calls: {calls}");
        }

        static void Main()
        {
            Run("m1, length 8", () => M1(8));
            Run("m2(20)", () => M2(20));
            Run("m3(20)", () => M3(20));
        }
    }
}
