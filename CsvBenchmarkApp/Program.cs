using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;


class CsvBenchmarkApp
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Benchmark de Leitura de CSV ===");

        Console.Write("Informe o caminho do arquivo CSV: ");
        string? path = Console.ReadLine();

        Console.Write("Separador de linha (padrão \\n): ");
        string? lineSep = Console.ReadLine();
        if (string.IsNullOrEmpty(lineSep)) lineSep = "\n";

        Console.Write("Separador de coluna (padrão ,): ");
        string? colSep = Console.ReadLine();
        if (string.IsNullOrEmpty(colSep)) colSep = ",";

        Console.WriteLine("\nEscolha a abordagem:");
        Console.WriteLine("1 - StreamReader linha a linha");
        Console.WriteLine("2 - ReadAllText + Split");
        Console.WriteLine("3 - ReadLines + Parallel LINQ");
        Console.WriteLine("4 - MemoryMappedFile");
        Console.WriteLine("5 - Span<T> + ReadAllBytes");
        Console.WriteLine("6 - Executar todas as abordagens");

        Console.Write("Opção: ");
        string? option = Console.ReadLine();

        var stopwatch = Stopwatch.StartNew();
        long memoryBefore = GC.GetTotalMemory(true);

        Func<DataTable> metodo = option switch
        {

            "1" => () => {
                var resultados = new List<BenchmarkResult>
                {
                    BenchmarkResult.ExecutarBenchmark("StreamReader", () => CsvStrategies.ReadWithStreamReader(path, colSep))
                };
                BenchmarkResult.ExibirTabelaResultados(resultados);
                return resultados.OrderBy(r => r.TempoSegundos).First().Nome switch
                {
                    "StreamReader" => CsvStrategies.ReadWithStreamReader(path, colSep),
                    _ => new DataTable()
                };
            }
            ,
            "2" => () =>
                {
                var resultados = new List<BenchmarkResult> {
                    BenchmarkResult.ExecutarBenchmark("ReadAllText", () => CsvStrategies.ReadWithAllText(path, lineSep, colSep))
                };
                BenchmarkResult.ExibirTabelaResultados(resultados);
                    return resultados.OrderBy(r => r.TempoSegundos).First().Nome switch
                {
                    "ReadAllText" => CsvStrategies.ReadWithAllText(path, lineSep, colSep),
                    _ => new DataTable()
                };
                }
        ,
            "3" => () =>
            {
                var resultados = new List<BenchmarkResult> {
                    BenchmarkResult.ExecutarBenchmark("Parallel LINQ", () => CsvStrategies.ReadWithParallel(path, colSep))
                };
                BenchmarkResult.ExibirTabelaResultados(resultados);
                return resultados.OrderBy(r => r.TempoSegundos).First().Nome switch
                {
                    "Parallel LINQ" => CsvStrategies.ReadWithParallel(path, colSep),
                    _ => new DataTable()
                };
            }
            ,
            "4" => () =>
            {
                var resultados = new List<BenchmarkResult> {
                    BenchmarkResult.ExecutarBenchmark("MemoryMappedFile", () => CsvStrategies.ReadWithMemoryMapped(path, colSep))
                };
                BenchmarkResult.ExibirTabelaResultados(resultados);
                return resultados.OrderBy(r => r.TempoSegundos).First().Nome switch
                {
                    "MemoryMappedFile" => CsvStrategies.ReadWithMemoryMapped(path, colSep),
                    _ => new DataTable()
                };
            }
        ,
            "5" => () =>
            {
                var resultados = new List<BenchmarkResult> {
                    BenchmarkResult.ExecutarBenchmark("Span<T>", () => CsvStrategies.ReadWithSpan(path, lineSep, colSep))
                };
                BenchmarkResult.ExibirTabelaResultados(resultados);
                return resultados.OrderBy(r => r.TempoSegundos).First().Nome switch
                {
                    "Span<T>" => CsvStrategies.ReadWithSpan(path, lineSep, colSep),
                    _ => new DataTable()
                };
            }
            ,
            "6" => () =>
            {
                var resultados = new List<BenchmarkResult>
                    {
                        BenchmarkResult.ExecutarBenchmark("StreamReader", () => CsvStrategies.ReadWithStreamReader(path, colSep)),
                        BenchmarkResult.ExecutarBenchmark("ReadAllText", () => CsvStrategies.ReadWithAllText(path, lineSep, colSep)),
                        BenchmarkResult.ExecutarBenchmark("Parallel LINQ", () => CsvStrategies.ReadWithParallel(path, colSep)),
                        BenchmarkResult.ExecutarBenchmark("MemoryMappedFile", () => CsvStrategies.ReadWithMemoryMapped(path, colSep)),
                        BenchmarkResult.ExecutarBenchmark("Span<T>", () => CsvStrategies.ReadWithSpan(path, lineSep, colSep))
                    };

                BenchmarkResult.ExibirTabelaResultados(resultados);

                // Retorna o DataTable da abordagem mais rápida (opcional)
                return resultados.OrderBy(r => r.TempoSegundos).First().Nome switch
                {
                    "StreamReader" => CsvStrategies.ReadWithStreamReader(path, colSep),
                    "ReadAllText" => CsvStrategies.ReadWithAllText(path, lineSep, colSep),
                    "Parallel LINQ" => CsvStrategies.ReadWithParallel(path, colSep),
                    "MemoryMappedFile" => CsvStrategies.ReadWithMemoryMapped(path, colSep),
                    "Span<T>" => CsvStrategies.ReadWithSpan(path, lineSep, colSep),
                    _ => new DataTable()
                };
            }
            ,
            _ => throw new ArgumentException("Opção inválida")
        }; 

        var x = metodo();

        stopwatch.Stop();
        long memoryAfter = GC.GetTotalMemory(false);

    }
}
