// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
//mire eshte


using AlgoritmeTeAvancuara.java1;

List<int>[] graf =
{
    new List<int> { 1, 2 },    // Nyja 0
    new List<int> { 0, 3, 4 }, // Nyja 1
    new List<int> { 0, 5 },    // Nyja 2
    new List<int> { 1 },       // Nyja 3
    new List<int> { 1 },       // Nyja 4
    new List<int> { 2 }        // Nyja 5
};

Console.WriteLine("DFS iterativ:");
Rekurzioni.DfsIterativ(graf, 0);

Console.WriteLine("\nDFS rekursiv:");
Rekurzioni.DfsRekurziv(graf, 0);
Console.WriteLine();


AlgoritmeTeAvancuara.java2.Program.Run();
AlgoritmeTeAvancuara.java3.Program.Run(new string[] {});
AlgoritmeTeAvancuara.java3.Task2.Run();
