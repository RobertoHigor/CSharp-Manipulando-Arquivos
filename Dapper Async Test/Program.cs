using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Dapper_Async_Test
{
    class Numero
    {
        public int Num1 { get; set; }
        public int Num2 { get; set; }
    }
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            IList<Numero> listaNumeros = new List<Numero>();

            for (int i = 0; i < 1000; i++)
            {
                listaNumeros.Add(new Numero { Num1 = i, Num2 = i + 1 });
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            System.Console.WriteLine("Começando a contagem");
            
            // Inserir lista de objetos de forma assíncrona            
            using (var conn = new SqliteConnection("Data Source=numeros.sqlite")){
                await conn.OpenAsync().ConfigureAwait(false);
                await using var transaction = await conn.BeginTransactionAsync();

                try
                {
                    string sql = "INSERT INTO Numeros (Num1, Num2) VALUES (@Num1, @Num2)";
                    //conn.Execute(sql, listaNumeros);            
                    //TODO: Ver para que serve argumento transaction no dapper. Atualmente está opcional.
                    await conn.ExecuteAsync(sql, listaNumeros, transaction: transaction);

                    await transaction.CommitAsync().ConfigureAwait(false);
                }
                catch (System.Exception)
                {
                    await transaction.RollbackAsync().ConfigureAwait(false);
                    throw;
                }
        }

        stopwatch.Stop();
            System.Console.WriteLine($"Elapsed Time is {stopwatch.ElapsedMilliseconds}");
        }
}
}
