namespace AlgoritmeTeAvancuara.java1
{
    public class Rekurzioni
    {
        public static void DfsIterativ(List<int>[] graf, int fillimi)
        {
            bool[] vizituar = new bool[graf.Length];
            Stack<int> stack = new Stack<int>();

            stack.Push(fillimi);
            vizituar[fillimi] = true;

            while (stack.Count > 0)
            {
                int nyja = stack.Pop();
                Console.Write($"{nyja}");

                foreach (int fqinji in graf[nyja])
                {
                    if (!vizituar[fqinji])
                    {
                        vizituar[fqinji] = true;
                        stack.Push(fqinji);
                    }
                }
            }
        }

        public static void DfsRekurziv(List<int>[] graf, int fillimi)
        {
            bool[] vizituar = new bool[graf.Length];
            Vizito(graf, fillimi, vizituar);
        }

        public static void Vizito(List<int>[] graf, int nyja, bool[] vizituar)
        {
            vizituar[nyja] = true;
            Console.Write($"{nyja}");

            foreach (int fqinji in graf[nyja])
            {
                if (!vizituar[fqinji])
                {
                    Vizito(graf, fqinji, vizituar);
                }
            }
        }
        

        //Funksioni i dyte per fuqine

        public static double FuqiaIterative(double x, int n)
        {
            if (n < 0)
                throw new ArgumentException("n duhet të jetë jo negativ.");

            double rezultati = 1;

            while (n > 0)
            {
                if (n % 2 != 0)
                    rezultati *= x;

                x *= x;
                n /= 2;
            }

            return rezultati;
        }

        public static double FuqiaRekursive(double x, int n)
        {
            if (n < 0)
                throw new ArgumentException("n duhet të jetë jo negativ.");

            if (n == 0)
                return 1;

            double majtas = FuqiaRekursive(x, n / 2);
            double djathtas = FuqiaRekursive(x, n / 2);

            if (n % 2 == 0)
                return majtas * djathtas;

            return x * majtas * djathtas;
        }

        
    }
}

//nje funksion qe kur e bajna me rekurzion time dhe space complexity te jene te njejte kurse nje funksion tjeter qe kur e bajna me rekurzion
// e degradon se paku njeren 