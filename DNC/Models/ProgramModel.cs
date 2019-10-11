using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNC
{

    [Serializable]
    public class Program
    {
        public DateTime LastModified { get; set; }
        public int Number { get; set; }
        public string FileName { get; set; }
        public string[] FileData { get; set; }
        public string MachineSafeData
        {
            get
            {
                string x = null;
                foreach (string line in FileData)
                {
                    x += line;
                    x += '\n';
                }
                return x;
            }
        }
        
        internal Program() { }

        // TODO add parse data
        public Program(string fileName, string[] fileData)
        {
            LastModified = DateTime.Now;

            FileName = fileName;
            FileData = fileData;
        }
    }
}
