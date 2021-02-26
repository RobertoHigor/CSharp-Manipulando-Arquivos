using System;
using System.Collections.Generic;
using System.Text;

namespace CsvParser
{
    class UserDetails
    {
        private string iD;
        private string name;
        private string city;
        private string country;

        public string ID { get => iD; set => iD = value; }
        public string Name { get => name; set => name = value; }
        public string City { get => city; set => city = value; }
        public string Country { get => country; set => country = value; }
    }
}
