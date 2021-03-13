using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

            await DapperAsyncEnvolvendoComTransaction(listaNumeros); // semelhante a outra forma
            await DapperAsync(listaNumeros); // Rápido
            DapperInsert(listaNumeros); // Mais lento
            DapperInsertParallel(listaNumeros); // Lento no SQlite


            var numeros = GetNumeros();
            numeros.ToList();

            foreach (Numero numero in numeros)
            {
                System.Console.WriteLine(numero);
            }

            stopwatch.Stop();
            System.Console.WriteLine($"Elapsed Time is {stopwatch.ElapsedMilliseconds}");
        }

        private static IEnumerable<Numero> GetNumeros()
        {

            using var conn = new SqliteConnection("Data Source=numeros.sqlite");
            try
            {
                string sql = "SELECT * FROM Numeros;";
                var numeros = conn.Query<Numero>(sql);
                return numeros;
            }
            catch (Exception)
            {

                throw;
            }

        }

        private static async Task DapperAsync(IList<Numero> listaNumeros)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            System.Console.WriteLine("Iniciando DapperAsync");

            // Inserir lista de objetos de forma assíncrona            
            using var conn = new SqliteConnection("Data Source=numeros.sqlite");

            await conn.OpenAsync();
            await using var transaction = await conn.BeginTransactionAsync();

            try
            {
                string sql = "INSERT INTO Numeros (Num1, Num2) VALUES (@Num1, @Num2);";
                //conn.Execute(sql, listaNumeros);            
                //TODO: Ver para que serve argumento transaction no dapper. Atualmente está opcional.
                // Pode servir para associar transaction no caso de não utilizar BeginTransactionAsync
                var affectedRows = await conn.ExecuteAsync(sql, listaNumeros);
                await transaction.CommitAsync();
            }
            catch (System.Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            stopwatch.Stop();
            System.Console.WriteLine($"Fim do DapperAsync com {stopwatch.ElapsedMilliseconds}");
        }

        // Insert de listas padrão do Dapper, sem o uso de transaction compartilhada
        private static void DapperInsert(IList<Numero> listaNumeros)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            System.Console.WriteLine("Iniciando DapperInsert");

            using var conn = new SqliteConnection("Data Source=numeros.sqlite");

            conn.Open();
            //using var transaction = conn.BeginTransaction();
            try
            {
                string sql = "INSERT INTO Numeros (Num1, Num2) VALUES (@Num1, @Num2)";
                var affectedRows = conn.Execute(sql, listaNumeros);
                //transaction.Commit();

            }
            catch (System.Exception)
            {
                //transaction.Rollback();                   
                throw;
            }
            stopwatch.Stop();
            System.Console.WriteLine($"Fim do DapperInsert com {stopwatch.ElapsedMilliseconds}");
        }

        // Insert em paralelo, criando uma transactioin e passando para o dapper.
        // No caso do SQLITE, ele não suporta o volume de conexões geradas, dando exception
        private static void DapperInsertParallel(IList<Numero> listaNumeros)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            System.Console.WriteLine("Iniciando DapperInsertParallel");

            
            Parallel.ForEach(listaNumeros, async (numero) =>
            {
                using var conn = new SqliteConnection("Data Source=numeros.sqlite");
                await conn.OpenAsync();
                using var transaction = await conn.BeginTransactionAsync();

                try
                {
                    string sql = "INSERT INTO Numeros (Num1, Num2) VALUES (@Num1, @Num2)";
                    await conn.ExecuteAsync(sql, numero, transaction);
                    await transaction.CommitAsync();

                }
                catch (System.Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });

            stopwatch.Stop();
            System.Console.WriteLine($"Fim do DapperInsertParallel com {stopwatch.ElapsedMilliseconds}");
        }

        // Insert paralelo sem async
        private static void DapperInsertParallel(IList<Numero> listaNumeros)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            System.Console.WriteLine("Iniciando DapperInsertParallel");

            
            Parallel.ForEach(listaNumeros, (numero) =>
            {
                using var conn = new SqliteConnection("Data Source=numeros.sqlite");
                conn.Open();
                using var transaction = conn.BeginTransaction();

                try
                {
                    string sql = "INSERT INTO Numeros (Num1, Num2) VALUES (@Num1, @Num2)";
                    conn.Execute(sql, numero, transaction);
                    transaction.Commit();

                }
                catch (System.Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            });

            stopwatch.Stop();
            System.Console.WriteLine($"Fim do DapperInsertParallel com {stopwatch.ElapsedMilliseconds}");
        }

        // Envolvendo o try-catch com uma transaction
        private static async Task DapperAsyncEnvolvendoComTransaction(IList<Numero> listaNumeros)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            System.Console.WriteLine("Iniciando DapperAsyncNovo");

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
                        await transaction.ExecuteAsync(sql, listaNumeros);
                        await transaction.CommitAsync();
                    }
                    catch (System.Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            // transactionScope.Complete();
            stopwatch.Stop();
            System.Console.WriteLine($"Fim do DapperAsyncNovo com {stopwatch.ElapsedMilliseconds}");
            //}
        }
    }
}
