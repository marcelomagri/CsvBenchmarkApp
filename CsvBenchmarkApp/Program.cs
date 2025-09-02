using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Data;

class CsvBenchmarkApp
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Benchmark de Leitura de CSV ===");

        Console.Write("Informe o caminho do arquivo CSV: ");
        string path = Console.ReadLine();

        Console.Write("Separador de linha (padrão \\n): ");
        string lineSep = Console.ReadLine();
        if (string.IsNullOrEmpty(lineSep)) lineSep = "\n";

        Console.Write("Separador de coluna (padrão ,): ");
        string colSep = Console.ReadLine();
        if (string.IsNullOrEmpty(colSep)) colSep = ",";

        Console.WriteLine("\nEscolha a abordagem:");
        Console.WriteLine("1 - StreamReader linha a linha");
        Console.WriteLine("2 - ReadAllText + Split");
        Console.WriteLine("3 - ReadLines + Parallel LINQ");
        Console.WriteLine("4 - MemoryMappedFile");
        Console.WriteLine("5 - Span<T> + ReadAllBytes");

        Console.Write("Opção: ");
        string option = Console.ReadLine();

        var stopwatch = Stopwatch.StartNew();
        long memoryBefore = GC.GetTotalMemory(true);

        DataTable result = option switch
        {
            "1" => CsvStrategies.ReadWithStreamReader(path, colSep),
            "2" => CsvStrategies.ReadWithAllText(path, lineSep, colSep),
            "3" => CsvStrategies.ReadWithParallel(path, colSep),
            "4" => CsvStrategies.ReadWithMemoryMapped(path, colSep),
            "5" => CsvStrategies.ReadWithSpan(path, lineSep, colSep),
            _ => throw new ArgumentException("Opção inválida")
        };

        stopwatch.Stop();
        long memoryAfter = GC.GetTotalMemory(false);

        Console.WriteLine($"\n✅ Benchmark concluído!");
        Console.WriteLine($"⏱️ Tempo: {stopwatch.Elapsed.TotalSeconds:F2} segundos");
        Console.WriteLine($"📊 Memória usada: {(memoryAfter - memoryBefore) / 1024 / 1024} MB");
        Console.WriteLine($"📄 Linhas lidas: {result.Rows.Count}");
        Console.WriteLine($"📁 Colunas detectadas: {result.Columns.Count}");
    }
}
