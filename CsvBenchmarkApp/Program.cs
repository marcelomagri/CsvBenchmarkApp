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
        string[] opcoes = {
            "Executar benchmark com método específico",
            "Executar todas as abordagens",
            "Sair"
        };

        int indiceSelecionado = 0;
        bool executando = true;

        while (executando)
        {
            Console.Clear();
            Console.WriteLine("=== MENU DE BENCHMARK CSV ===\n");

            for (int i = 0; i < opcoes.Length; i++)
            {
                if (i == indiceSelecionado)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"> {opcoes[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {opcoes[i]}");
                }
            }

            var tecla = Console.ReadKey(true);

            switch (tecla.Key)
            {
                case ConsoleKey.UpArrow:
                    indiceSelecionado = (indiceSelecionado - 1 + opcoes.Length) % opcoes.Length;
                    break;
                case ConsoleKey.DownArrow:
                    indiceSelecionado = (indiceSelecionado + 1) % opcoes.Length;
                    break;
                case ConsoleKey.Enter:
                    switch (indiceSelecionado)
                    {
                        case 0: ExecutarMetodoEspecifico(); break;
                        case 1: ExecutarTodosBenchmarks(); break;
                        case 2: executando = false; break;
                    }
                    break;
            }
        }
    }

    static void ExecutarMetodoEspecifico()
    {
        Console.Clear();
        Console.WriteLine("=== Benchmark com Método Específico ===");

        string path = Solicitar("Informe o caminho do arquivo CSV");
        string lineSep = Solicitar("Separador de linha (padrão \\n)", "\n");
        string colSep = Solicitar("Separador de coluna (padrão ,)", ",");

        string option = MenuSelecionarMetodo();

        Func<DataTable> metodo = option switch
        {
            "1" => () => CsvStrategies.ReadWithStreamReader(path, colSep),
            "2" => () => CsvStrategies.ReadWithAllText(path, lineSep, colSep),
            "3" => () => CsvStrategies.ReadWithParallel(path, colSep),
            "4" => () => CsvStrategies.ReadWithMemoryMapped(path, colSep),
            "5" => () => CsvStrategies.ReadWithSpan(path, lineSep, colSep),
            _ => throw new ArgumentException("Opção inválida")
        };

        var resultados = new List<BenchmarkResult>
        {
            BenchmarkResult.ExecutarBenchmark($"Método {option}", metodo)
        };

        BenchmarkResult.ExibirTabelaResultados(resultados);
        Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
        Console.ReadKey();
    }

    static void ExecutarTodosBenchmarks()
    {
        Console.Clear();
        Console.WriteLine("=== Benchmark com Todas as Abordagens ===");

        string path = Solicitar("Informe o caminho do arquivo CSV");
        string lineSep = Solicitar("Separador de linha (padrão \\n)", "\n");
        string colSep = Solicitar("Separador de coluna (padrão ,)", ",");

        var resultados = new List<BenchmarkResult>
        {
            BenchmarkResult.ExecutarBenchmark("StreamReader", () => CsvStrategies.ReadWithStreamReader(path, colSep)),
            BenchmarkResult.ExecutarBenchmark("ReadAllText", () => CsvStrategies.ReadWithAllText(path, lineSep, colSep)),
            BenchmarkResult.ExecutarBenchmark("Parallel LINQ", () => CsvStrategies.ReadWithParallel(path, colSep)),
            BenchmarkResult.ExecutarBenchmark("MemoryMappedFile", () => CsvStrategies.ReadWithMemoryMapped(path, colSep)),
            BenchmarkResult.ExecutarBenchmark("Span<T>", () => CsvStrategies.ReadWithSpan(path, lineSep, colSep))
        };

        BenchmarkResult.ExibirTabelaResultados(resultados);
        Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
        Console.ReadKey();
    }

    static string MenuSelecionarMetodo()
    {
        string[] metodos = {
            "StreamReader linha a linha",
            "ReadAllText + Split",
            "ReadLines + Parallel LINQ",
            "MemoryMappedFile",
            "Span<T> + ReadAllBytes"
        };

        int indice = 0;
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Selecione o Método de Leitura CSV ===\n");

            for (int i = 0; i < metodos.Length; i++)
            {
                if (i == indice)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"> {metodos[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {metodos[i]}");
                }
            }

            var tecla = Console.ReadKey(true);
            switch (tecla.Key)
            {
                case ConsoleKey.UpArrow:
                    indice = (indice - 1 + metodos.Length) % metodos.Length;
                    break;
                case ConsoleKey.DownArrow:
                    indice = (indice + 1) % metodos.Length;
                    break;
                case ConsoleKey.Enter:
                    return (indice + 1).ToString(); // retorna "1" a "5"
            }
        }
    }

    static string Solicitar(string mensagem, string padrao = "")
    {
        Console.Write($"{mensagem}: ");
        string? entrada = Console.ReadLine();
        return string.IsNullOrEmpty(entrada) ? padrao : entrada;
    }
}
