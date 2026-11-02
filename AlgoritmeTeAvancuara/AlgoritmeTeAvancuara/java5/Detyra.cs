using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
  Task 1 - Trace   W = A FOOL AND HIS MONEY ARE SOON PARTED, m = 13
                   num, h, chainedTable, probeInsert, probeFind, probeRemove
  Task 2 - Write   DELETED, remove, doubleHashInsert, doubleHashFind, maxComparisons,
                   nextPrime, insertGrow
                   (tabelat jane array, qeliza bosh = null, e fshire = DELETED;
                    find/remove shikojne vetem qelizat ne rrugen e celesit)
*/

namespace AlgoritmeTeAvancuara.java5
{
    public class Detyra
    {
        // ne JS: const DELETED = Symbol('deleted');  ne C#: nje objekt unik
        public static readonly object DELETED = new object();

        static readonly string[] W = { "A", "FOOL", "AND", "HIS", "MONEY",
                                       "ARE", "SOON", "PARTED" };

        // tabela si ne slajdin 1
        static object?[] BuildT()
        {
            object?[] T = new object?[13]; // te gjitha null
            foreach (var w in W) ProbeInsert(T, w);
            return T;
        }

        static string Show(object?[] table)
        {
            return "[ " + string.Join(", ", table.Select(x =>
                x == null ? "null" : x == DELETED ? "DELETED" : x.ToString())) + " ]";
        }

        public static void Run()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("\n--- java5: Task 1 Trace ---");

            var T = BuildT();
            Console.WriteLine("T = " + Show(T));

            // Predicted:
            // h: A=1, FOOL=48%13=9, AND=19%13=6, HIS=36%13=10, MONEY=72%13=7,
            //    ARE=24%13=11, SOON=63%13=11, PARTED=64%13=12, KID=24%13=11
            // T: [0]=PARTED [1]=A [6]=AND [7]=MONEY [9]=FOOL [10]=HIS [11]=ARE [12]=SOON
            //
            // chainedTable(W, 13)[11] -> [ARE, SOON]
            // probeFind(T, "PARTED")  -> 0   (12 i zene nga SOON, kalon ne 0)
            // probeFind(T, "KID")     -> -1  (11, 12, 0, 1 -> 2 eshte null)
            // probeRemove(T, "ARE")   -> 11  (T[11] behet null)
            // probeFind(T, "SOON")    -> -1  (fillon ne 11 qe tani eshte null, ndalon -
            //                                 edhe pse SOON eshte ne 12!)

            Console.WriteLine("chainedTable(W, 13)[11] = [ " + string.Join(", ", ChainedTable(W, 13)[11]) + " ]");
            Console.WriteLine("probeFind(T, \"PARTED\")  = " + ProbeFind(T, "PARTED"));
            Console.WriteLine("probeFind(T, \"KID\")     = " + ProbeFind(T, "KID"));
            Console.WriteLine("probeRemove(T, \"ARE\")   = " + ProbeRemove(T, "ARE"));
            Console.WriteLine("probeFind(T, \"SOON\")    = " + ProbeFind(T, "SOON"));

            Console.WriteLine("\n--- java5: Task 2 Write ---");

            // remove - tabela ndertohet perseri si ne slajdin 1
            // pritet: 11, 12, -1, -1
            T = BuildT();
            Console.WriteLine("remove(T, \"ARE\")      = " + Remove(T, "ARE"));
            Console.WriteLine("probeFind(T, \"SOON\")  = " + ProbeFind(T, "SOON"));
            Console.WriteLine("probeFind(T, \"ARE\")   = " + ProbeFind(T, "ARE"));
            Console.WriteLine("remove(T, \"KID\")      = " + Remove(T, "KID"));
            Console.WriteLine("T = " + Show(T) + "  (length " + T.Length + ")");

            // doubleHashInsert · doubleHashFind - numrat e ligjerates
            // pritet: D = [null, 56, null, 31, 75, 19, null, null, 30, 20, null], 5, -1
            object[] N = { 30, 20, 56, 75, 31, 19 };
            object?[] D = new object?[11];
            foreach (var k in N) DoubleHashInsert(D, k);
            Console.WriteLine("D = " + Show(D));
            Console.WriteLine("doubleHashFind(D, 19) = " + DoubleHashFind(D, 19));
            Console.WriteLine("doubleHashFind(D, 42) = " + DoubleHashFind(D, 42));

            // maxComparisons
            // pritet: { chained: 3, linear: 6, double: 2 }, { chained: 2, linear: 2, double: 3 }
            Console.WriteLine("maxComparisons(N, 11) = " + ShowMax(MaxComparisons(N, 11)));
            Console.WriteLine("maxComparisons(W, 13) = " + ShowMax(MaxComparisons(W, 13)));

            // insertGrow
            // pritet: sizes 5 5 11 11 11 23 23 23, G.length = 23, probeFind(G, "ARE") = 4
            object?[] G = new object?[5];
            var sizes = new List<int>();
            foreach (var w in W) { G = InsertGrow(G, w); sizes.Add(G.Length); }
            Console.WriteLine("sizes: " + string.Join(" ", sizes));
            Console.WriteLine("G.length = " + G.Length);
            Console.WriteLine("probeFind(G, \"ARE\") = " + ProbeFind(G, "ARE"));
        }

        static string ShowMax((int chained, int linear, int @double) r)
        {
            return $"{{ chained: {r.chained}, linear: {r.linear}, double: {r.@double} }}";
        }

        // ---------- Task 1 ----------

        // num · h · chainedTable
        public static int Num(object key)
        {
            if (key is int n) return n;
            int s = 0;
            foreach (char c in (string)key) s += c - 64;
            return s;
        }

        public static int H(object key, int m)
        {
            return Num(key) % m;
        }

        public static List<object>[] ChainedTable(IEnumerable<object> keys, int m)
        {
            var table = new List<object>[m];
            for (int i = 0; i < m; i++) table[i] = new List<object>();
            foreach (var k in keys) table[H(k, m)].Add(k);
            return table;
        }

        // probeInsert · probeFind
        public static int ProbeInsert(object?[] table, object key)
        {
            int m = table.Length;
            int i = H(key, m);
            while (table[i] != null) i = (i + 1) % m;
            table[i] = key;
            return i;
        }

        public static int ProbeFind(object?[] table, object key)
        {
            int m = table.Length;
            int i = H(key, m);
            while (table[i] != null)
            {
                if (Equals(table[i], key)) return i;
                i = (i + 1) % m;
            }
            return -1;
        }

        // probeRemove - njejte si probeFind, vetem kur gjendet celesi, celula zbrazet
        public static int ProbeRemove(object?[] table, object key)
        {
            int m = table.Length;
            int i = H(key, m);
            while (table[i] != null)
            {
                if (Equals(table[i], key)) { table[i] = null; return i; }
                i = (i + 1) % m;
            }
            return -1;
        }

        // ---------- Task 2 ----------

        // remove - si probeRemove, por ne vend te null shkruan DELETED,
        // keshtu probeFind nuk ndalon aty dhe i gjen celesat qe jane pas saj
        public static int Remove(object?[] table, object key)
        {
            int m = table.Length;
            int i = H(key, m);
            while (table[i] != null)
            {
                if (Equals(table[i], key)) { table[i] = DELETED; return i; }
                i = (i + 1) % m;
            }
            return -1;
        }

        // doubleHashInsert · doubleHashFind
        // hapi vjen nga hash-i i dyte: s = m - 2 - num(key) % (m - 2)
        // (m eshte numer prim, keshtu ecja mund te arrije cdo qelize;
        //  DELETED nuk eshte bosh, prandaj kalohet)
        public static int DoubleHashInsert(object?[] table, object key)
        {
            int m = table.Length;
            int i = H(key, m);
            int s = m - 2 - Num(key) % (m - 2);
            while (table[i] != null) i = (i + s) % m;
            table[i] = key;
            return i;
        }

        public static int DoubleHashFind(object?[] table, object key)
        {
            int m = table.Length;
            int i = H(key, m);
            int s = m - 2 - Num(key) % (m - 2);
            while (table[i] != null)
            {
                if (Equals(table[i], key)) return i;
                i = (i + s) % m;
            }
            return -1;
        }

        // maxComparisons - numri me i madh i krahasimeve ne nje kerkim te suksesshem
        // per secilen metode; keys nuk ndryshohet
        public static (int chained, int linear, int @double) MaxComparisons(object[] keys, int m)
        {
            var chainedT = ChainedTable(keys, m);
            object?[] linearT = new object?[m];
            object?[] doubleT = new object?[m];
            foreach (var k in keys)
            {
                ProbeInsert(linearT, k);
                DoubleHashInsert(doubleT, k);
            }

            // chained: gjatesia e listes me te gjate
            int chained = chainedT.Max(list => list.Count);

            // linear dhe double: ecim nga h(key, m) dhe numerojme qelizat
            // deri te qeliza e vet celesit (perfshire ate)
            int linear = 0, dbl = 0;
            foreach (var k in keys)
            {
                int i = H(k, m), count = 1;
                while (!Equals(linearT[i], k)) { i = (i + 1) % m; count++; }
                linear = Math.Max(linear, count);

                int s = m - 2 - Num(k) % (m - 2);
                i = H(k, m); count = 1;
                while (!Equals(doubleT[i], k)) { i = (i + s) % m; count++; }
                dbl = Math.Max(dbl, count);
            }

            return (chained, linear, dbl);
        }

        // nextPrime - numri prim me i vogel >= x
        public static int NextPrime(int x)
        {
            if (x < 2) return 2;
            while (true)
            {
                bool prim = true;
                for (int d = 2; d * d <= x; d++)
                    if (x % d == 0) { prim = false; break; }
                if (prim) return x;
                x++;
            }
        }

        // insertGrow - mban faktorin e ngarkeses alfa = n / m <= 0.5
        // kthen te njejten tabele nese nuk u rrit, tabelen e re nese u rrit
        public static object?[] InsertGrow(object?[] table, object key)
        {
            int m = table.Length;

            // n = numri i celesave (null dhe DELETED nuk jane celesa)
            int n = 0;
            foreach (var x in table)
                if (x != null && x != DELETED) n++;

            // (n + 1) / m > 0.5  <=>  2 * (n + 1) > m
            if (2 * (n + 1) > m)
            {
                object?[] bigger = new object?[NextPrime(2 * m)]; // pa DELETED
                for (int i = 0; i < m; i++)
                    if (table[i] != null && table[i] != DELETED)
                        ProbeInsert(bigger, table[i]!);
                table = bigger;
            }

            ProbeInsert(table, key);
            return table;
        }
    }
}
