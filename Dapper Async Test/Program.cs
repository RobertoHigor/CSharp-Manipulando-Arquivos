using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using Dapper.Transaction;
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

            await DapperAsyncNovo(listaNumeros).ConfigureAwait(false);
            //await DapperAsync(listaNumeros).ConfigureAwait(false);

            stopwatch.Stop();
            System.Console.WriteLine($"Elapsed Time is {stopwatch.ElapsedMilliseconds}");
        }

        private static async Task DapperAsync(IList<Numero> listaNumeros)
        {
            // Inserir lista de objetos de forma assíncrona            
            using (var conn = new SqliteConnection("Data Source=numeros.sqlite"))
            {
                await conn.OpenAsync().ConfigureAwait(false);
                await using var transaction = await conn.BeginTransactionAsync();

                try
                {
                    string sql = "INSERT INTO Numeros (Num1, Num2) VALUES (@Num1, @Num2)";
                    //conn.Execute(sql, listaNumeros);            
                    //TODO: Ver para que serve argumento transaction no dapper. Atualmente está opcional.
                    // Pode servir para associar transaction no caso de não utilizar BeginTransactionAsync
                    await conn.ExecuteAsync(sql, listaNumeros, transaction: transaction);

                    await transaction.CommitAsync().ConfigureAwait(false);
                }
                catch (System.Exception)
                {
                    await transaction.RollbackAsync().ConfigureAwait(false);
                    throw;
                }
            }
        }

        private static async Task DapperAsyncNovo(IList<Numero> listaNumeros)
        {
            // Inserir lista de objetos de forma assíncrona        
            //using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            //{
                using (var conn = new SqliteConnection("Data Source=numeros.sqlite"))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {

                        try
                        {
                            string sql = "INSERT INTO Numeros (Num1, Num2) VALUES (@Num1, @Num2)";
                            //await conn.ExecuteAsync(sql, listaNumeros).ConfigureAwait(false);
                            await transaction.ExecuteAsync(sql, listaNumeros);
                            await transaction.CommitAsync();
                        }
                        catch (System.Exception)
                        {
                            throw;
                        }
                    }
                }
               // transactionScope.Complete();
            //}
        }
    }
}
