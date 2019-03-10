using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using DBI_PEA_Scoring.Model;
using DBI_PEA_Scoring.Model.Teacher;

namespace DBI_PEA_Scoring.Utils
{
    class JsonUtils
    {
        //Object to JsonString
        public static string SerializeJson(object obj)
        {
            string jsonString = JsonConvert.SerializeObject(obj);
            return jsonString;
        }

        public static bool WriteJson(object obj, string path)
        {
            try
            {//path with name of file, remember
                File.WriteAllText(path, SerializeJson(obj));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static object LoadQuestion(string path)
        {
            string jsonQuestion = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<PaperSet>(jsonQuestion);
        }

        public static PaperSet DeserializeJson(string localPath)
        {
            // read file into a string and deserialize JSON to a type
            return JsonConvert.DeserializeObject<PaperSet>(File.ReadAllText(localPath));
        }
    }
}
