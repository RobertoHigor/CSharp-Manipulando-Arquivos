using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsvHelper_demo
{
    public class Carro
    {
        /*
         * É possível mapear um atributo pelo índice de coluna no csv. Para pegar a primeira coluna por ex
         * [Index(0)]
         * Também é possível por nome com [Name("nomeColuna")]
         */
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Fabricante { get; set; }
        [Name("AnoFabricação")]
        public int AnoFabricacao { get; set; }

        public override string ToString()
        {
            return $"ID: {Id} |Nome: {Nome} |Fabricante: {Fabricante} |Ano de fabricação: {AnoFabricacao}";
        }
    }
}
