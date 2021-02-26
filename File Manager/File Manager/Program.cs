using System;
using System.IO;

namespace File_Manager
{
    class Program
    {
        static void Main(string[] args)
        {
            // Exemplo #1: Escrevendo um array de strings para um arquivo.
            string[] lines = { "First line", "Second line", "Third line" };
            // WriteAllLines cria o arquivo, escreve a coleção de strings e fecha o arquivo.
            // Não é necessário nesse caso utilizar Flush() ou Close().
            File.WriteAllLines(@"D:\Arquivos Importantes\Desenvolvimento\CSHARP\Base\File Manager\File Manager\writeLines.txt", lines);

            // Exemplo #2: Escrevendo uma string em um arquivo de texto
            string text = "Classe é o tipo de dados mais poderoso do C#. Como uma estrutura," +
                "uma classe define o dado e o comportamento do tipo de dado. ";
            // WriteAllTExt cria o arquivo e escreve a string, por fim também fecha o arquivo
            File.WriteAllText(@"D:\Arquivos Importantes\Desenvolvimento\CSHARP\Base\File Manager\File Manager\writeText.txt", text);

            // Exemplo #3: Escrever apenas algumas strings de um array no arquivo
            // A sintaxe 'using' automaticamente fecha e limpa a stream e
            // chama IDisposable.DIsposable no stream object.
            // NOTE: Cuidado com o FileStream pois ele escreve em bytes, diferente do StreamWriter
            using (StreamWriter file =
                 new StreamWriter(@"D:\Arquivos Importantes\Desenvolvimento\CSHARP\Base\File Manager\File Manager\WriteLines2.txt"))
            {
                foreach (string line in lines)
                {
                    // Não escrever as linhas com a palavra "segundo"
                    if (!line.Contains("Second"))
                    {
                        file.WriteLine(line);
                    }
                }
            }

            // Exemplo #4: Acrescentar um texto a um arquivo existente
            using (StreamWriter file =
                new StreamWriter(@"D:\Arquivos Importantes\Desenvolvimento\CSHARP\Base\File Manager\File Manager\WriteLines2.txt", true))
            {
                file.WriteLine("Fourth line");
            }
        }
    }
}
