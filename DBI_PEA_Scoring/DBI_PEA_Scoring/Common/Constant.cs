using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBI_PEA_Scoring.Common
{
    class Constant
    {
        DB[] listDB = {(new DB("A","C:\\duc")),
        (new DB("B","D:\\Duc"))};
    }
    public class DB
    {
        public DB(string name, string path)
        {
            Name = name;
            Path = path;
        }
        public string Name { get; set; }
        public string Path { get; set; }
        
        
    }
}
