using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace CsvHelper_demo
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Carro> carros = LerCsv<Carro>("carros_utf8.csv");
            var carrosFiltrado = filtrarFabricante(carros, "Volkswagen");           
            //carros.RemoveAt(1);

            foreach (Carro carro in carrosFiltrado)
            {
                Console.WriteLine(carro.ToString());
            }

            //EscreverCsv<Carro>(carros, "carros.csv");
        }

        private static List<Carro> filtrarFabricante(List<Carro> carros, string fabricante)
        {
            // Apenas carros de determinada marca
            IEnumerable<Carro> carrosFiltrado = from carro in carros
                                         where carro.Fabricante == fabricante
                                         select carro;

            return carrosFiltrado.ToList();
            // Alternativa
            //var carros3 = carros.Where(carro => carro.Fabricante == "Volkswagen");
        }

        private static List<T> LerCsv<T>(string caminho)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Caso as colunas sejam minúsculas no csv e não no código.
                PrepareHeaderForMatch = args => args.Header.ToLower(),
            };
            // Abrindo como apenas leitura
            // FileShare permite abrir arquivo enquanto outros processos o estão usando para algo
            using (FileStream stream = File.Open(caminho, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new StreamReader(stream))
                using (var csv = new CsvReader(reader, config))
                {
                    // Retorna um registro por vez
                    // Cuidado: Caso se utilize o .ToList() do LINQ, ele lê o arquivo inteiro na memória
                    try
                    {
                        IEnumerable<T> records = csv.GetRecords<T>();
                        return records.ToList();
                    }
                    catch (System.IO.IOException ex)
                    {
                        System.Console.WriteLine(ex + "Erro ao ler o arquivo");
                        throw;
                    }
                    catch (HeaderValidationException ex)
                    {
                        Console.WriteLine($"{ex}, Erro no mapeamento dos cabeçalhos");
                        throw;
                    }
                }
            }
        }

        // Escreve um novo documento com os dados informados
        private static void EscreverCsv<T>(IEnumerable<T> lista, string caminho)
        {
            try
            {
                // True para realizar append
                using (var writer = new StreamWriter(caminho, true, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(lista);
                }
            }
            catch (System.IO.IOException ex)
            {
                System.Console.WriteLine(ex + "Erro ao escrever o arquivo");
                throw;
            }

        }
    }
}
