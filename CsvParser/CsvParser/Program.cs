using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;

namespace CsvParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string jsonString = ReadCSVFile("TestData.csv");
            // Convertendo de json para uma lista de objetos UserDetails
            List<UserDetails> person = (List<UserDetails>)JsonConvert.DeserializeObject(jsonString, (typeof(List<UserDetails>)));
            IEnumerable<UserDetails> people = from p in person
                          where p.City == "city1" || p.City == "City1"
                          select p;

            Console.WriteLine("Name " + "ID   " + "City " + "Country");
            foreach (UserDetails details in people)
            {
                Console.WriteLine($"{details.Name} {details.ID} {details.City} {details.Country}");
            }
        }

        private static string ReadCSVFile(string csv_file_path)
        {
            DataTable csvData = new DataTable();
            string jsonString = string.Empty;
            try
            {
                // TextFieldParser pertence ao visual basic
                using (TextFieldParser csvReader = new TextFieldParser(csv_file_path))
                {
                    //csvReader.SetDelimiters(new string[] { ","}) // outro jeito
                    csvReader.SetDelimiters(",");
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields;
                    bool tableCreated = false;
                    while (tableCreated == false)
                    {
                        // Lendo os primeiros campos e definindo como header
                        colFields = csvReader.ReadFields();
                        foreach (string column in colFields)
                        {
                            DataColumn datecolumn = new DataColumn(column);
                            datecolumn.AllowDBNull = true;
                            csvData.Columns.Add(datecolumn); // Construindo DataTable com os nomes corretos
                        }
                        tableCreated = true;
                    }
                    // Lendo os valores até o fim do arquivo
                    while (!csvReader.EndOfData)
                    {                        
                        csvData.Rows.Add(csvReader.ReadFields());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Error: Parsing CSV";
            }
            //Se tudo der certo, é convertido para json string
            jsonString = JsonConvert.SerializeObject(csvData);
            return jsonString;
        }         
    }
}
