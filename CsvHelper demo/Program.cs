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
            try
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
            catch (System.IO.IOException)
            {
                System.Console.WriteLine("Arquivo não encontrado");
            }
            catch (HeaderValidationException)
            {
                System.Console.WriteLine("Arquivo ou cdificação inválida");
            }
            catch (EncodingInvalidoException)
            {
                System.Console.WriteLine("Codificação inválida. Precisa ser UTF8");
            }
            catch (System.Exception)
            {
                System.Console.WriteLine("Exception");
            }

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
                using (var reader = new StreamReader(stream, Encoding.UTF8, true))
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
                        throw new IOException("Erro ao ler arquivo");
                    }
                    catch (HeaderValidationException ex)
                    {
                        Console.WriteLine($"{ex}, Erro no mapeamento dos cabeçalhos");
                        throw;
                    }
                }
            }
        }

        // Método não funciona
        private static void GetFileEncoding(string srcFile)
        {
            Encoding enc = Encoding.UTF8;
            byte[] buffer = new byte[5];
            FileStream file = new FileStream(srcFile, FileMode.Open);
            file.Read(buffer, 0, 5);

            // Checando BOM
            if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
                enc = Encoding.UTF8;
            else if (buffer[0] == 0xfe && buffer[1] == 0xff)
                enc = Encoding.Unicode;
            else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
                enc = Encoding.UTF32;
            else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
                enc = Encoding.UTF7;

            System.Console.WriteLine(enc.EncodingName);
            if (enc != Encoding.UTF8)
            {
                System.Console.WriteLine(enc);
                throw new EncodingInvalidoException("O arquivo precisa ser UTF8");
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
