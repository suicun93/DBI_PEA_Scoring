using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DBI_Grading.Common
{
    public static class FileExtension
    {
        internal static string[] GetAllSql(string rootPath) => Directory.GetFiles(rootPath, "*.sql", SearchOption.AllDirectories);
    }
}
