using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNC
{
    public class Program
    {
        public DateTime LastModified { get; set; }

        public int Number { get; set; }
        public string Name { get; set; }
     
        public FileInfo ProgramFile { get; set; }
        
        

        public Program(int number, string name)
        {
            Number = number;
            Name = name;

            LastModified = DateTime.Now;
        }
    }
}
