using System;
using System.Linq;

namespace AlgoritmeTeAvancuara.java3
{
    internal class Program
    {
        public static void Run(string[] args)
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

            Console.WriteLine(
                "Shkruaj rendin e emrave qe parashikon.");

            Console.Write("c1: ");
            string predictedC1 = Console.ReadLine() ?? "";

            Console.Write("c2: ");
            string predictedC2 = Console.ReadLine() ?? "";

            Console.Write("c3: ");
            string predictedC3 = Console.ReadLine() ?? "";


            Student[] resultC1 = c1(six, g);
            Student[] resultC2 = c2(six, g, 5, 10);
            Student[] resultC3 = c3(six, g, 5, 10);

            ShowResult("c1", predictedC1, resultC1);
            ShowResult("c2", predictedC2, resultC2);
            ShowResult("c3", predictedC3, resultC3);
        }

        static Student[] c1(
            Student[] A, Func<Student, int> key)
        {
            int n = A.Length;
            int[] count = new int[n];

            for (int i = 0; i < n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (key(A[i]) < key(A[j]))
                    {
                        count[j]++;
                    }
                    else
                    {
                        count[i]++;
                    }
                }
            }

            Student[] S = new Student[n];

            for (int i = 0; i < n; i++)
            {
                S[count[i]] = A[i];
            }

            return S;
        }

        static Student[] c2(
            Student[] A,
            Func<Student, int> key,
            int lo,
            int hi)
        {
            int[] D = new int[hi - lo + 1];

            for (int i = 0; i < A.Length; i++)
            {
                D[key(A[i]) - lo]++;
            }

            for (int j = 1; j < D.Length; j++)
            {
                D[j] += D[j - 1];
            }

            Student[] S = new Student[A.Length];

            // Vendosja nga fundi ne fillim.
            for (int i = A.Length - 1; i >= 0; i--)
            {
                int j = key(A[i]) - lo;
                S[D[j] - 1] = A[i];
                D[j]--;
            }

            return S;
        }

        static Student[] c3(
            Student[] A,
            Func<Student, int> key,
            int lo,
            int hi)
        {
            int[] D = new int[hi - lo + 1];

            for (int i = 0; i < A.Length; i++)
            {
                D[key(A[i]) - lo]++;
            }

            for (int j = 1; j < D.Length; j++)
            {
                D[j] += D[j - 1];
            }

            Student[] S = new Student[A.Length];

            // Vendosja nga fillimi ne fund.
            for (int i = 0; i < A.Length; i++)
            {
                int j = key(A[i]) - lo;
                S[D[j] - 1] = A[i];
                D[j]--;
            }

            return S;
        }

        static void ShowResult(
            string function,
            string prediction,
            Student[] result)
        {
            Console.WriteLine($"\n{function}:");
            Console.WriteLine($"Parashikimi: {prediction}");
            Console.WriteLine(
                "Rendi real: " +
                string.Join(", ", result.Select(s => s.Name)));
        }
    }

    internal class Student
    {
        public string Name { get; }
        public int Grade { get; }

        public Student(string name, int grade)
        {
            Name = name;
            Grade = grade;
        }
    }
}