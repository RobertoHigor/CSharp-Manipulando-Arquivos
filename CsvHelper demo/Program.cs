using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CsvHelper_demo
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Carro> carros = LerCsv("carros.csv");
            carros.RemoveAt(1);

            foreach (Carro carro in carros)
            {
                Console.WriteLine(carro.ToString());
            }

            EscreverCsv(carros, "carros.csv");
        }

        private static List<T> LerCsv<T>(string caminho)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Caso as colunas sejam minúsculas no csv e não no código.
                PrepareHeaderForMatch = args => args.Header.ToLower(),
            };

            using (var reader = new StreamReader(caminho))
            using (var csv = new CsvReader(reader, config))
            {
                // Retorna um registro por vez
                // Cuidado: Caso se utilize o .ToList() do LINQ, ele lê o arquivo inteiro na memória
                try
                {    
                    
                    IEnumerable<T> records = csv.GetRecords<T>();
                    return records.ToList();
                }
                catch (HeaderValidationException ex)
                {
                    Console.WriteLine($"{ex}, Erro no mapeamento dos cabeçalhos");
                    throw;
                }
            }
        }

        // Escreve um novo documento com os dados informados
        private static void EscreverCsv(IEnumerable<Carro> carros, string caminho)
        {
            using (var writer = new StreamWriter(caminho))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(carros);
            }
        }
    }
}
