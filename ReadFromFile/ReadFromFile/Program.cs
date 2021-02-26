using System;
using System.IO;

namespace File_Manager
{
    class ReadFromFile
    {
        static void Main(string[] args)
        {
            // Exemplo #1: Lendo como uma única string
            string text = File.ReadAllText(@"D:\Arquivos Importantes\Desenvolvimento\CSHARP\Base\File Manager\File Manager\WriteText.txt");
            Console.WriteLine($"Contents of WriteText.txt = {text}");

            // Exemplo #2: Lendo cada linha como um elemento de um array
            string[] lines = File.ReadAllLines(@"D:\Arquivos Importantes\Desenvolvimento\CSHARP\Base\File Manager\File Manager\WriteLines2.txt");
            Console.Write("Contents of WriteLines2.txt = ");
            foreach (string line in lines)
            {
                Console.WriteLine("\t" + line);
            }

            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
