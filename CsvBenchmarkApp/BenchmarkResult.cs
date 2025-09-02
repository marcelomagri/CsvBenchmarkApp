using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class BenchmarkResult
    {
        public string Nome { get; set; }
        public double TempoSegundos { get; set; }
        public double MemoriaMB { get; set; }
        public int Linhas { get; set; }
        public int Colunas { get; set; }

        public static BenchmarkResult ExecutarBenchmark(string nome, Func<DataTable> metodo)
        {
            var stopwatch = Stopwatch.StartNew();
            long memoriaAntes = GC.GetTotalMemory(true);

            var dt = metodo();

            stopwatch.Stop();
            long memoriaDepois = GC.GetTotalMemory(false);

            return new BenchmarkResult
            {
                Nome = nome,
                TempoSegundos = stopwatch.Elapsed.TotalSeconds,
                MemoriaMB = (memoriaDepois - memoriaAntes) / 1024.0 / 1024.0,
                Linhas = dt.Rows.Count,
                Colunas = dt.Columns.Count
            };
        }

        public static void ExibirTabelaResultados(List<BenchmarkResult> resultados)
        {
            Console.WriteLine("\n📊 Resultados dos Benchmarks:");
            Console.WriteLine("┌────────────────────────────┬────────────┬────────────┬────────────┬────────────┐");
            Console.WriteLine("│ Abordagem                  │ Tempo (s)  │ Memória MB │ Linhas     │ Colunas    │");
            Console.WriteLine("├────────────────────────────┼────────────┼────────────┼────────────┼────────────┤");

            foreach (var r in resultados)
            {
                Console.WriteLine($"│ {r.Nome,-26} │ {r.TempoSegundos,10:F2} │ {r.MemoriaMB,10:F2} │ {r.Linhas,10} │ {r.Colunas,10} │");
            }

            Console.WriteLine("└────────────────────────────┴────────────┴────────────┴────────────┴────────────┘");
        }


    }
