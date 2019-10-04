using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNC.Models
{
    public class Program
    {
        public int Number { get; set; }
        public string Name { get; set; }
        
        public DateTime LastModified { get; set; }
        public string[] FileContent { get; set; }

        public Program(int number, string name)
        {
            Number = number;
            Name = name;

            LastModified = DateTime.Now;
        }
    }
}
